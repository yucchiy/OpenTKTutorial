using OpenTK.Graphics.OpenGL4;

namespace OpenTKTutorial.MultipleLight
{
    public class Plane : IRenderable
    {
        public Model Model { get; }

        public Plane()
        {
            PrimitiveFactory.CreatePlaneIndexed(10, out var vertices, out var indices, out var normals, out var textureCoordinates);
            var mesh = new Mesh(
                new Mesh.Descriptor()
                {
                    Name = "Plane",
                    Positions = vertices,
                    Normals = normals,
                    Indices = indices,
                    TextureCoordinates1 = textureCoordinates,
                }
            );

            var material = new Material(
                "Custom",
                (new System.IO.StreamReader(Utility.GetEmbeddedResourceStream("Scene/Resource/MultipleLight/plane_vertex_shader.glsl"))).ReadToEnd(),
                (new System.IO.StreamReader(Utility.GetEmbeddedResourceStream("Scene/Resource/MultipleLight/plane_fragment_shader.glsl"))).ReadToEnd()
            );

            material.SetTexture(TextureUnit.Texture0, OpenTKTutorialConstant.Material.DiffuseTextureName, new Texture2D("Scene/Resource/MultipleLight/plane_diffuse.jpg"));
            material.SetVec3(OpenTKTutorialConstant.Material.AmbientColorName,  new OpenTK.Mathematics.Vector3(0.2f, 0.2f, 0.2f));
            material.SetVec3(OpenTKTutorialConstant.Material.DiffuseColorName,  new OpenTK.Mathematics.Vector3(0.5f, 0.5f, 0.5f));
            material.SetVec3(OpenTKTutorialConstant.Material.SpecularColorName, new OpenTK.Mathematics.Vector3(0.1f, 0.1f, 0.1f));

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

        public void UpdateDirectionalLight(Light light)
        {
            foreach (var meshRenderer in Model.MeshRenderers)
            {
                meshRenderer.Material.SetVec3(OpenTKTutorialConstant.Material.LightDirectionalLightColorName, light.Color);
                meshRenderer.Material.SetVec3(OpenTKTutorialConstant.Material.LightDirectionalLightDirectionWorldName, light.Direction);
            }
        }

        public void UpdateSpotLight(int index, Light light)
        {
            foreach (var meshRenderer in Model.MeshRenderers)
            {
                meshRenderer.Material.SetVec3(OpenTKTutorialConstant.Material.LightPointLightNPositionWorld(index), light.Position);
                meshRenderer.Material.SetVec3(OpenTKTutorialConstant.Material.LightPointLightNColorName(index), light.Color);
                meshRenderer.Material.SetFloat(OpenTKTutorialConstant.Material.LightPointLightNConstantName(index), light.Constant);
                meshRenderer.Material.SetFloat(OpenTKTutorialConstant.Material.LightPointLightNLinearName(index), light.Linear);
                meshRenderer.Material.SetFloat(OpenTKTutorialConstant.Material.LightPointLightNQuadratic(index), light.Quadratic);
            }
        }

        public void Render(double deltaTime)
        {
            Model.Render(deltaTime);
        }
    }
}