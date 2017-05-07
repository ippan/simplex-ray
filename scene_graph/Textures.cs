using System;
using System.Numerics;

namespace SimplexRay
{
    public class ConstantTexture : ITexture
    {
        public Vector3 Color;

        public ConstantTexture(Vector3 color)
        {
            Color = color;
        }

        public Vector3 Value(float u, float v, Vector3 point)
        {
            return Color;
        }
    }

    public class CheckerTexture : ITexture
    {
        public ITexture OddTexture = null;
        public ITexture EvenTexture = null;

        public CheckerTexture(ITexture odd_texture, ITexture even_texture)
        {
            OddTexture = odd_texture;
            EvenTexture = even_texture;
        }

        public Vector3 Value(float u, float v, Vector3 point)
        {
            float sines = (float)(Math.Sin(10.0f * point.X) * Math.Sin(10.0f * point.Y) * Math.Sin(10.0f * point.Z));

            return sines < 0.0f ? OddTexture.Value(u, v, point) : EvenTexture.Value(u, v, point);
        }
    }
}