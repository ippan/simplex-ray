using System;
using System.Numerics;

namespace SimplexRay
{
    public struct Ray
    {
        public Vector3 Origin;

        public Vector3 Direction;        

        public Ray(Vector3 origin, Vector3 direction)
        {
            Origin = origin;
            Direction = Vector3.Normalize(direction);
            
        }

        public Vector3 PointAt(float distance) { return Origin + distance * Direction; }

    }

    public struct HitData
    {
        public float Distance;
        
        public Vector3 Point;

        public Vector3 Normal;

        public IMaterial Material;
    }

    public static class MathHelper
    {
        private static Random random = new Random();

        public static Vector3 RandomInUnitSPhere()
        {
            Vector3 vector;
            do 
            {
                vector = 2.0f * new Vector3(RandomFloat(), RandomFloat(), RandomFloat()) - Vector3.One;
            } while (vector.LengthSquared() >= 1.0f);

            return vector;
        }

        public static float RandomFloat()
        {
            return (float)random.NextDouble();
        }

        public static bool Refract(Vector3 vector, Vector3 normal, float n, ref Vector3 refracted)
        {
            Vector3 unit_vector = Vector3.Normalize(vector);
            float dt = Vector3.Dot(unit_vector, normal);
            float discriminant = 1.0f - n * n * (1 - dt * dt);

            if (discriminant > 0)
            {
                refracted = n * (unit_vector - normal * dt) - normal * (float)Math.Sqrt(discriminant);
                return true;
            }

            return false;
        }

        public static Quaternion LookAt(Vector3 form, Vector3 to, Vector3 up)
        {
            Vector3 forward = Vector3.Normalize(to - form);
            /*/
            forward.Z = -forward.Z;

            Vector3 right = Vector3.Cross(Vector3.Normalize(up), forward);
            up = Vector3.Cross(forward, right);
            
            Quaternion rotation = new Quaternion();
            rotation.W = (float)Math.Sqrt(1.0f + right.X + up.Y + forward.Z) * 0.5f;
            float invert_w4 = 1.0f / (4.0f * rotation.W);
            rotation.X = (forward.Y - up.Z) * invert_w4;
            rotation.Y = (right.Z - forward.X) * invert_w4;
            rotation.Z = (up.X - right.Y) * invert_w4;

            return rotation;
            */

            float yaw = -(float)Math.Atan2(forward.X, -forward.Z);
            float pitch = (float)Math.Asin(forward.Y);

            return Quaternion.CreateFromYawPitchRoll(yaw, pitch, 0.0f);            
        }

    }
}