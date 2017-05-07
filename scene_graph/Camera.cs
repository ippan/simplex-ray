using System;
using System.Numerics;

namespace SimplexRay
{
    internal class View : IView
    {
        public View()
        {
            Width = 200;
            Height = 100;
        }

        public int Width { get; set; }

        public int Height { get; set; }
    }

    internal class Camera : SceneNode, ICameraNode
    {
        protected View view;

        protected Vector3 center;
        protected Vector3 horizontal;
        protected Vector3 vertical;

        public Camera()
        {
            view = new View();
            SetProjection(60.0f, 0.0f);
        }

        public void SetProjection(float vertical_fov, float aspect)
        {
            if (aspect == 0.0f)
                aspect = (float)view.Width / (float)view.Height;
            
            float theta = vertical_fov * (float)Math.PI / 180.0f;
            float half_height = (float)Math.Tan(theta / 2.0f);
            float half_width = aspect * half_height;
            center = new Vector3(0.0f, 0.0f, -1.0f);
            horizontal = new Vector3(half_width, 0.0f, 0.0f);
            vertical = new Vector3(0.0f, half_height, 0.0f);
            
        }

        public Ray GetRay(float u, float v)
        {
            return new Ray(Translation, Vector3.Transform(center + u * horizontal + v * vertical, Rotation));
        }

        public IView View { get { return view; } }
    }

}