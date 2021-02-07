using OpenTK.Graphics.OpenGL4;

namespace OpenTKTutorial.MultipleLight
{
    public class Box : IRenderable
    {
        public Model Model { get; }

        public Box()
        {
            PrimitiveFactory.CreateCubeIndexed(0.5f, out var vertices, out var indices, out var normals, out var textureCoordinates);
            var mesh = new Mesh(
                new Mesh.Descriptor()
                {
                    Name = "Box",
                    Positions = vertices,
                    Normals = normals,
                    Indices = indices,
                    TextureCoordinates1 = textureCoordinates,
                }
            );

            var material = new Material(
                "Custom",
                (new System.IO.StreamReader(Utility.GetEmbeddedResourceStream("Scene/Resource/MultipleLight/box_vertex_shader.glsl"))).ReadToEnd(),
                (new System.IO.StreamReader(Utility.GetEmbeddedResourceStream("Scene/Resource/MultipleLight/box_fragment_shader.glsl"))).ReadToEnd()
            );

            material.SetTexture(TextureUnit.Texture0, OpenTKTutorialConstant.Material.DiffuseTextureName, new Texture2D("Scene/Resource/MultipleLight/crate_diffuse.png"));
            material.SetTexture(TextureUnit.Texture1, OpenTKTutorialConstant.Material.SpecularTextureName, new Texture2D("Scene/Resource/LightCaster/crete_specular.png"));
            material.SetVec3(OpenTKTutorialConstant.Material.AmbientColorName,  new OpenTK.Mathematics.Vector3(0.2f, 0.2f, 0.2f));
            material.SetVec3(OpenTKTutorialConstant.Material.DiffuseColorName,  new OpenTK.Mathematics.Vector3(0.5f, 0.5f, 0.5f));
            material.SetVec3(OpenTKTutorialConstant.Material.SpecularColorName, new OpenTK.Mathematics.Vector3(1.0f, 1.0f, 1.0f));

            Model = new Model(
                new Model.Descriptor()
                {
                    Meshes = new Mesh[]
                    {
                        mesh
                    },
                    Materials = new Material[]
                    {
                        material
                    },
                    MaterialIndices = new int[]
                    {
                        0
                    }
                }
            );
        }

        public void Render(double deltaTime)
        {
            Model.Render(deltaTime);
        }
    }
}