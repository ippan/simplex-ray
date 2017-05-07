using System;
using System.Numerics;

namespace SimplexRay
{
    public class LambertianMaterial : IMaterial
    {
        public ITexture Albedo;

        public LambertianMaterial(ITexture albedo)
        {
            Albedo = albedo;
        }

        public string TypeName { get { return "Lambertian"; } }
    }

    public class MetalMaterial : IMaterial
    {
        public ITexture Albedo;
        public float Fuzz;

        public MetalMaterial(ITexture albedo, float fuzz = 0.3f)
        {
            Albedo = albedo;
            Fuzz = fuzz;
        }

        public string TypeName { get { return "Metal"; } }
    }

    public class DielectricMaterial : IMaterial
    {
        public float RefractiveIndices;

        public DielectricMaterial(float refractive_indices)
        {
            RefractiveIndices = refractive_indices;
        }

        public string TypeName { get { return "Dielectric"; } }
    }
}