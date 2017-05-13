using System;
using System.Numerics;

namespace SimplexRay
{

    internal class SphereNode : SceneNode, IShapeNode
    {
        public Shape Shape { get { return Shape.Sphere; } }

        public IMaterial Material { get; set; }

        // only support uniform scale
        protected float Radius { get { return Scale.X / 2.0f; } }

        protected bool FillHitData(float root, Ray ray, float min_distance, float max_distance, ref HitData hit_data)
        {
            if (root < min_distance || root > max_distance)
                return false;

            hit_data.Distance = root;
            hit_data.Point = ray.PointAt(root);
            hit_data.Normal = (hit_data.Point - Translation) / Radius;
            hit_data.Material = Material;

            float phi = (float)Math.Atan2(hit_data.Point.Z, hit_data.Point.X);
            float theta = (float)Math.Asin(hit_data.Point.Y);
            hit_data.UV.X = 1 - (phi + (float)Math.PI) / (float)(2.0f * Math.PI);
            hit_data.UV.Y = (theta + (float)Math.PI / 2.0f) / (float)Math.PI;

            return true;
        }

        public virtual bool Hit(Ray ray, float min_distance, float max_distance, ref HitData hit_data) 
        { 
            Vector3 v = ray.Origin - Translation;
            float a = Vector3.Dot(ray.Direction, ray.Direction);
            float b = Vector3.Dot(v, ray.Direction);
            float c = Vector3.Dot(v, v) - Radius * Radius;
            
            if (b * b - a * c > 0)
            {
                if (FillHitData((-b - (float)Math.Sqrt(b * b - a * c)) / a, ray, min_distance, max_distance, ref hit_data))
                    return true;

                if (FillHitData((-b + (float)Math.Sqrt(b * b - a * c)) / a, ray, min_distance, max_distance, ref hit_data))
                    return true;                    
            }

            return false;
        }

        public AABB BoundingBox 
        { 
            get 
            {
                return new AABB(Translation - new Vector3(Radius), Translation + new Vector3(Radius));
            } 
        }
    }

}