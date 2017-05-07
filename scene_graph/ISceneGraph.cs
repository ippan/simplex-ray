using System.Numerics;

namespace SimplexRay
{
    public enum Shape
    {
        Sphere
    }

    public interface IMaterial
    {
        string TypeName { get; }
    }

    public interface ITexture
    {
        Vector3 Value(float u, float v, Vector3 point);
    }

    public interface ISceneNode
    {
        Vector3 Translation { get; set; }

        Quaternion Rotation { get; set; }

        Vector3 Scale { get; set; }
    }

    public interface IHitableNode
    {
        bool Hit(Ray ray, float min_distance, float max_distance, ref HitData hit_data);
        AABB BoundingBox { get; }        
    }

    public interface IVisualNode : IHitableNode
    {
    }

    public interface IShapeNode : IVisualNode, ISceneNode
    {
        Shape Shape { get; }

        IMaterial Material { get; set; }
    }

    public interface ICameraNode : ISceneNode
    {
        IView View { get; }

        Ray GetRay(float u, float v);
        
        void SetProjection(float vertical_fov = 60.0f, float aspect = 0.0f);
    }

    public interface IView
    {
        int Width { get; set; }
        int Height { get; set; }
    }

    public interface ISceneGraph
    {
        IShapeNode AddShape(Shape shape);

        ICameraNode AddCamera();

        bool Hit(Ray ray, float min_distance, float max_distance, ref HitData hit_data);

        Image Render(ICameraNode camera, int sample = 1, int max_depth = 50);
    }
}