using System;
using System.Numerics;
using System.Collections.Generic;

namespace SimplexRay
{
    internal interface IMaterialScatter
    {
        bool Scatter(IMaterial material, Ray ray, HitData hit_data, ref Vector3 attenuation, ref Ray scattered);
        Vector3 Emit(IMaterial material, Ray ray, HitData hit_data);
    }

    internal static class MaterialScatters
    {
        private static Dictionary<string, IMaterialScatter> material_scatters = null;

        private static void Init()
        {
            material_scatters = new Dictionary<string, IMaterialScatter>();
            AddScatter(new LambertianMaterial(null), new LambertianScatter());
            AddScatter(new MetalMaterial(null, 0.0f), new MetalScatter());
            AddScatter(new DielectricMaterial(0.0f), new DielectricScatter());
            AddScatter(new DiffuseLightMaterial(null), new DiffuseLightScatter());
        }

        private static void AddScatter(IMaterial material, IMaterialScatter scatter)
        {
            material_scatters.Add(material.TypeName, scatter);
        }

        public static IMaterialScatter GetMaterialScatter(string type_name)
        {
            if (material_scatters == null)
                Init();

            return material_scatters[type_name];
        }

        public static IMaterialScatter GetMaterialScatter(IMaterial material)
        {
            return GetMaterialScatter(material.TypeName);
        }
    }

    internal class LambertianScatter : IMaterialScatter
    {
        public bool Scatter(IMaterial material, Ray ray, HitData hit_data, ref Vector3 attenuation, ref Ray scattered)
        {
            LambertianMaterial lambertian_material = material as LambertianMaterial;

            Vector3 direction = hit_data.Normal + MathHelper.RandomInUnitSPhere();
            scattered = new Ray(hit_data.Point, direction);
            attenuation = lambertian_material.Albedo.Value(hit_data.UV.X, hit_data.UV.Y, hit_data.Point);

            return true;
        }

        public Vector3 Emit(IMaterial material, Ray ray, HitData hit_data) { return Vector3.Zero; }
    }

    internal class MetalScatter : IMaterialScatter
    {
        public bool Scatter(IMaterial material, Ray ray, HitData hit_data, ref Vector3 attenuation, ref Ray scattered)
        {
            MetalMaterial metal_material = material as MetalMaterial;

            Vector3 reflected = Vector3.Reflect(ray.Direction, hit_data.Normal);
            scattered = new Ray(hit_data.Point, reflected + metal_material.Fuzz * MathHelper.RandomInUnitSPhere());
            attenuation = metal_material.Albedo.Value(hit_data.UV.X, hit_data.UV.Y, hit_data.Point);

            return Vector3.Dot(scattered.Direction, hit_data.Normal) > 0;
        }

        public Vector3 Emit(IMaterial material, Ray ray, HitData hit_data) { return Vector3.Zero; }
    }

    internal class DielectricScatter : IMaterialScatter
    {
        private float Schlick(float cosine, float refractive_indices)
        {
            float r = (1.0f - refractive_indices) / (1.0f + refractive_indices);
            r *= r;
            return r + (1.0f - r) * (float)Math.Pow(1.0f - cosine, 5.0f);
        }

        public bool Scatter(IMaterial material, Ray ray, HitData hit_data, ref Vector3 attenuation, ref Ray scattered)
        {
            DielectricMaterial dielectric_material = material as DielectricMaterial;

            Vector3 outward_normal;
            Vector3 reflected = Vector3.Reflect(ray.Direction, hit_data.Normal);

            float n;
            attenuation = new Vector3(1.0f, 1.0f, 1.0f);

            Vector3 refracted = Vector3.Zero;

            float reflect_prob;
            float cosine;

            if (Vector3.Dot(ray.Direction, hit_data.Normal) > 0)
            {
                outward_normal = -hit_data.Normal;
                n = dielectric_material.RefractiveIndices;
                cosine = dielectric_material.RefractiveIndices * Vector3.Dot(ray.Direction, hit_data.Normal);
            }
            else
            {
                outward_normal = hit_data.Normal;
                n = 1.0f / dielectric_material.RefractiveIndices;
                cosine = -Vector3.Dot(ray.Direction, hit_data.Normal);
            }

            if (MathHelper.Refract(ray.Direction, outward_normal, n, ref refracted))
                reflect_prob = Schlick(cosine, dielectric_material.RefractiveIndices);
            else
                reflect_prob = 1.0f;

            if (MathHelper.RandomFloat() < reflect_prob)
                scattered = new Ray(hit_data.Point, reflected);
            else
                scattered = new Ray(hit_data.Point, refracted);

            return true;
        }

        public Vector3 Emit(IMaterial material, Ray ray, HitData hit_data) { return Vector3.Zero; }
    }


    internal class DiffuseLightScatter : IMaterialScatter
    {
        public bool Scatter(IMaterial material, Ray ray, HitData hit_data, ref Vector3 attenuation, ref Ray scattered)
        {
            return false;
        }

        public Vector3 Emit(IMaterial material, Ray ray, HitData hit_data) 
        { 
            DiffuseLightMaterial diffuse_light_material = material as DiffuseLightMaterial;
            return diffuse_light_material.Emit.Value(hit_data.UV.X, hit_data.UV.Y, hit_data.Point);
        }
    }


}