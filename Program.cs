using System;
using System.Numerics;
using System.IO;

namespace SimplexRay
{
    public class PPMImageSaver
    {
        public void Save(Image image, string path)
        {
            using (FileStream steam = File.OpenWrite(path))
            {
                using (StreamWriter writer = new StreamWriter(steam))
                {
                    writer.WriteLine("P3");
                    writer.WriteLine("{0} {1}", image.Width, image.Height);
                    writer.WriteLine("255");

                    foreach (Vector3 pixel in image.Pixels)
                        writer.WriteLine("{0} {1} {2}", (int)(255.99f * pixel.X), (int)(255.99f * pixel.Y), (int)(255.99f * pixel.Z));
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {            
            ISceneGraph scene_graph = SceneGraphFactory.Create();

            IShapeNode sphere = scene_graph.AddShape(Shape.Sphere);
            sphere.Translation = new Vector3(0.0f, -1000.0f, 0.0f);
            sphere.Material = new LambertianMaterial(new Vector3(0.5f, 0.5f, 0.5f));
            sphere.Scale = new Vector3(2000.0f);

            sphere = scene_graph.AddShape(Shape.Sphere);
            sphere.Translation = new Vector3(0.0f, 1.0f, 0.0f);
            sphere.Material = new DielectricMaterial(1.5f);
            sphere.Scale = new Vector3(2.0f);
            
            sphere = scene_graph.AddShape(Shape.Sphere);
            sphere.Translation = new Vector3(-4.0f, 1.0f, 0.0f);
            sphere.Material = new LambertianMaterial(new Vector3(0.4f, 0.2f, 0.1f));
            sphere.Scale = new Vector3(2.0f);

            sphere = scene_graph.AddShape(Shape.Sphere);
            sphere.Translation = new Vector3(4.0f, 1.0f, 0.0f);
            sphere.Material = new MetalMaterial(new Vector3(0.7f, 0.6f, 0.5f), 0.0f);
            sphere.Scale = new Vector3(2.0f);

            for (int z = -10; z <= 10; ++z)
            {
                for (int x = -10; x <= 10; ++x)
                {
                    float choose_material = MathHelper.RandomFloat();
                    Vector3 translation = new Vector3(x + 0.9f * MathHelper.RandomFloat(), 0.2f, z + 0.9f * MathHelper.RandomFloat());
                    
                    if (Vector3.Distance(translation, new Vector3(4.0f, 0.2f, 0.0f)) > 0.9f)
                    {
                        IShapeNode ball = scene_graph.AddShape(Shape.Sphere);
                        ball.Translation = translation;
                        ball.Scale = new Vector3(0.4f);

                        if (choose_material < 0.8f)
                        {
                            ball.Material = new LambertianMaterial(new Vector3(MathHelper.RandomFloat() * MathHelper.RandomFloat(), MathHelper.RandomFloat() * MathHelper.RandomFloat(), MathHelper.RandomFloat() * MathHelper.RandomFloat()));
                        }
                        else if (choose_material < 0.95f)
                        {
                            ball.Material = new MetalMaterial(new Vector3(1.0f + MathHelper.RandomFloat(), 1.0f + MathHelper.RandomFloat(), 1.0f + MathHelper.RandomFloat()) * 0.5f, 0.5f * MathHelper.RandomFloat());
                        }
                        else
                        {
                            ball.Material = new DielectricMaterial(1.5f);
                        }
                    }
                }
            }

            ICameraNode camera = scene_graph.AddCamera();
            camera.View.Width = 1200;
            camera.View.Height = 800;
            camera.SetProjection(20.0f);
            camera.Translation = new Vector3(13.0f, 2.0f, 3.0f);
            camera.Rotation = MathHelper.LookAt(camera.Translation, new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f));
            Image image = scene_graph.Render(camera, 50, 50);
            PPMImageSaver saver = new PPMImageSaver();

            string path = args.Length > 0 ? args[0] : "output.ppm";
            saver.Save(image, path);
        }
    }
}
