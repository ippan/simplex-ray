using System;
using System.Collections.Generic;
using System.Numerics;

namespace SimplexRay
{
    internal class SceneGraph : ISceneGraph
    {
        internal class BVH : IVisualNode
        {
            private AABB bounding_box;

            private IVisualNode left_node = null;
            public IVisualNode right_node = null;

            public BVH(List<IVisualNode> nodes)
            {
                if (nodes.Count == 1)
                {
                    left_node = nodes[0];
                    right_node = nodes[0];
                }
                else if (nodes.Count == 2)
                {
                    left_node = nodes[0];
                    right_node = nodes[1];
                }
                else
                {
                    int axis = (int)(MathHelper.RandomFloat() * 3);
                    if (axis == 0)
                        nodes.Sort(XCompare);
                    else if (axis == 1)
                        nodes.Sort(YCompare);
                    else 
                        nodes.Sort(ZCompare);
                    
                    List<IVisualNode> left_nodes = new List<IVisualNode>();
                    for (int i = 0; i < nodes.Count / 2; ++i)
                        left_nodes.Add(nodes[i]);
                    
                    left_node = new BVH(left_nodes);

                    List<IVisualNode> right_nodes = new List<IVisualNode>();
                    for (int i = nodes.Count / 2; i < nodes.Count; ++i)
                        right_nodes.Add(nodes[i]);

                    right_node = new BVH(right_nodes);
                }

                bounding_box = new AABB(left_node.BoundingBox, right_node.BoundingBox);
            }

            private int XCompare(IVisualNode a, IVisualNode b)
            {
                return a.BoundingBox.Min.X.CompareTo(b.BoundingBox.Min.X);
            }

            private int YCompare(IVisualNode a, IVisualNode b)
            {
                return a.BoundingBox.Min.Y.CompareTo(b.BoundingBox.Min.Y);
            }

            private int ZCompare(IVisualNode a, IVisualNode b)
            {
                return a.BoundingBox.Min.Z.CompareTo(b.BoundingBox.Min.Z);
            }            

            public bool Hit(Ray ray, float min_distance, float max_distance, ref HitData hit_data)
            {
                if (!bounding_box.Hit(ray, min_distance, max_distance))
                    return false;
                
                HitData left_hit_data = new HitData();
                HitData right_hit_data = new HitData();

                bool left_hit = left_node.Hit(ray, min_distance, max_distance, ref left_hit_data);
                bool right_hit = right_node.Hit(ray, min_distance, max_distance, ref right_hit_data);
                
                if (left_hit && right_hit)
                {
                    hit_data = left_hit_data.Distance < right_hit_data.Distance ? left_hit_data : right_hit_data;
                    return true;
                }

                if (left_hit)
                {
                    hit_data = left_hit_data;
                    return true;
                }

                if (right_hit)
                {
                    hit_data = right_hit_data;
                    return true;
                }

                return false;
            }

            public AABB BoundingBox { get { return bounding_box; } }

        }

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

                Vector3 emit = material_scatter.Emit(hit_data.Material, ray, hit_data);

                if (depth < max_depth && material_scatter.Scatter(hit_data.Material, ray, hit_data, ref attenuation, ref scattered))
                    return emit + attenuation * Color(scattered, depth + 1, max_depth);
                else
                    return emit;
            }

            return Vector3.Zero;
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
            return bvh.Hit(ray, min_distance, max_distance, ref hit_data);
            /*
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

                hit_data = temp_hit_data;
            }

            return hit;
            */
        }        

        private BVH bvh = null;

        public Image Render(ICameraNode camera, int sample, int max_depth)
        {            
            bvh = new BVH(new List<IVisualNode>(visual_nodes));

            if (sample < 1)
                sample = 1;

            Image image = new Image(camera.View.Width, camera.View.Height);            

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

                    image[x, y] = color;
                }
            }

            return image;
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