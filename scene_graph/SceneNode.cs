using System.Numerics;

namespace SimplexRay
{
    public class SceneNode : ISceneNode
    {
        public Vector3 Translation { get; set; } = Vector3.Zero;

        public Quaternion Rotation { get; set; } = Quaternion.Identity;
    }

}