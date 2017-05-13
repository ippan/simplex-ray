using System.Numerics;

namespace SimplexRay
{
    internal class SceneNode : ISceneNode
    {
        public Vector3 Translation { get; set; } = Vector3.Zero;

        public Quaternion Rotation { get; set; } = Quaternion.Identity;

        public Vector3 Scale { get; set; } = Vector3.One;
    }

}