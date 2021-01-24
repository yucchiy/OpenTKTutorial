using OpenTK.Graphics.OpenGL4;

namespace OpenTKTutorial
{
    public class MeshRenderer : IRenderable
    {
        public Mesh Mesh { get; }
        public Material Material { get; }

        public MeshRenderer(Mesh mesh, Material material)
        {
            Mesh = mesh;
            Material = material;
        }

        public void Render(double deltaTime)
        {
            Material.Use();
            Mesh.Bind();

            GL.DrawElements(PrimitiveType.Triangles, Mesh.Indices.Length, DrawElementsType.UnsignedInt, 0);
            Utility.CheckError();

            Mesh.Unbind();
            Material.Unuse();
        }
    }
}