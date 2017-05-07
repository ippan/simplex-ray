using System;
using System.Collections.Generic;
using System.Numerics;

namespace SimplexRay
{
    internal class SceneGraph : ISceneGraph
    {
        const float MAX_HIT_DISTANCE = 1000000.0f;

        protected List<IVisualNode> visual_nodes = new List<IVisualNode>();

        Random random = new Random();

        public IShapeNode AddShape(Shape shape)
        {
            IShapeNode node = null;

            switch (shape)
            {
                case Shape.Sphere:
                    node = new SphereNode();
                break;
            }

            if (node != null)
                visual_nodes.Add(node);

            return node;
        }

        public ICameraNode AddCamera()
        {
            return new Camera();
        }

        protected Vector3 Color(Ray ray, int depth, int max_depth)
        {
            HitData hit_data = new HitData();
            
            if (Hit(ray, 0.01f, MAX_HIT_DISTANCE, ref hit_data))
            {
                Ray scattered = new Ray();
                Vector3 attenuation = Vector3.Zero;

                IMaterialScatter material_scatter = MaterialScatters.GetMaterialScatter(hit_data.Material);

                if (depth < max_depth && material_scatter.Scatter(hit_data.Material, ray, hit_data, ref attenuation, ref scattered))
                    return attenuation * Color(scattered, depth + 1, max_depth);
                else
                    return Vector3.Zero;
            }

            float t = 0.5f * (ray.Direction.Y + 1.0f);
            return (1.0f - t) * Vector3.One + t * new Vector3(0.5f, 0.7f, 1.0f);
        }

        protected Vector3 RandomInUnitSPhere()
        {
            Vector3 vector;
            do 
            {
                vector = 2.0f * new Vector3(MathHelper.RandomFloat(), MathHelper.RandomFloat(), MathHelper.RandomFloat()) - Vector3.One;
            } while (vector.LengthSquared() >= 1.0f);

            return vector;
        }

        public bool Hit(Ray ray, float min_distance, float max_distance, ref HitData hit_data)
        {
            HitData temp_hit_data = new HitData();
            float closest_hit_distance = max_distance;

            bool hit = false;

            foreach (IVisualNode visual_node in visual_nodes)
            {
                if (!visual_node.Hit(ray, min_distance, max_distance, ref temp_hit_data))
                    continue;

                if (temp_hit_data.Distance > closest_hit_distance)
                    continue;

                hit = true;
                closest_hit_distance = temp_hit_data.Distance;

                hit_data.Distance = temp_hit_data.Distance;
                hit_data.Normal = temp_hit_data.Normal;
                hit_data.Point = temp_hit_data.Point;
                hit_data.Material = temp_hit_data.Material;
            }

            return hit;
        }        

        public void Render(ICameraNode camera, int sample, int max_depth)
        {            
            if (sample < 1)
                sample = 1;

            Console.WriteLine("P3");
            Console.WriteLine("{0} {1}", camera.View.Width, camera.View.Height);
            Console.WriteLine("255");

            Vector2 half_size = new Vector2(camera.View.Width / 2.0f, camera.View.Height / 2.0f);

            for (int y = 0; y < camera.View.Height ; ++y)
            {
                for (int x = 0; x < camera.View.Width; ++x)
                {
                    Vector2 uv = new Vector2((x - half_size.X) / half_size.X, (half_size.Y - y) / half_size.Y);
                    Vector3 color = Vector3.Zero;

                    for (int n = 0; n < sample; ++n)
                    {
                        Vector2 delta = new Vector2(MathHelper.RandomFloat() / camera.View.Width, MathHelper.RandomFloat() / camera.View.Height);
                        Vector2 new_uv = uv + delta;

                        Ray ray = camera.GetRay(new_uv.X, new_uv.Y);
                        color += Color(ray, 0, max_depth);
                    }

                    color /= sample;

                    Console.WriteLine("{0} {1} {2}", (int)(255.99f * color.X), (int)(255.99f * color.Y), (int)(255.99f * color.Z));
                }
            }

        }
    }

    public static class SceneGraphFactory
    {
        public static ISceneGraph Create()
        {
            return new SceneGraph();
        }
    }

}