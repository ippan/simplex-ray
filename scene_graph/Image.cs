using System;
using System.Numerics;

namespace SimplexRay
{
    public class Image
    {
        protected Vector3[] pixels;

        private int width;
        private int height;

        public Image(int width, int height)
        {
            pixels = new Vector3[width * height];
            this.width = width;
            this.height = height;
        }

        public Vector3 this[int x, int y]
        {
            get { return pixels[y * width + x]; }
            set { pixels[y * width + x] = value; }
        }

        public Vector3[] Pixels { get { return pixels; } }

        public int Width { get { return width; } }
        public int Height { get { return height; } }

    }
}