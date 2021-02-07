using OpenTK.Mathematics;

namespace OpenTKTutorial
{
    public class Light : IRenderable
    {
        public LightType Type { get; set; } = LightType.Unknown;
        public Vector3 Position { get; set; }
        public Vector3 Color { get; set; }

        // for directional light
        public Vector3 Direction { get; set; }
        
        // for point
        public float Constant { get; set; }
        public float Linear { get; set; }
        public float Quadratic { get; set; }

        // for spot

        public float CutOff { get; set; }
        public float OuterCutOff { get; set; }

        public Model Model { get; }

        public Light(Vector3 position, Vector3 color)
        {
            Position = position;
            Color = color;

            PrimitiveFactory.CreateCubeIndexed(1f, out var vertices, out var indices, out var normals, out var __);
            var mesh = new Mesh(
                new Mesh.Descriptor()
                {
                    Name = "LightCube",
                    Positions = vertices,
                    Normals = normals,
                    Indices = indices,
                }
            );

            var material = new Material(
                "Custom",
                (new System.IO.StreamReader(Utility.GetEmbeddedResourceStream("Resource/Shader/light_vertex_shader.glsl"))).ReadToEnd(),
                (new System.IO.StreamReader(Utility.GetEmbeddedResourceStream("Resource/Shader/light_fragment_shader.glsl"))).ReadToEnd()
            );

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
            Model.Scale = Vector3.One * 0.1f;
        }

        public void Render(double deltaTime)
        {
            foreach (var meshRenderer in Model.MeshRenderers)
            {
                meshRenderer.Material.SetVec3(OpenTKTutorialConstant.Material.AmbientColorName, Color);
            }
            Model.Position = Position;

            Model.Render(deltaTime);
        }

        public void UpdateCamera(Camera camera)
        {
            foreach (var meshRenderer in Model.MeshRenderers)
            {
                var viewMatrix = camera.GetViewMatrix();
                meshRenderer.Material.SetMat4(
                    OpenTKTutorialConstant.Material.MVPViewMatrixName,
                    false,
                    ref viewMatrix
                );

                var projectionMatrix = camera.GetProjectionMatrix();
                meshRenderer.Material.SetMat4(
                    OpenTKTutorialConstant.Material.MVPProjectionMatrixName,
                    false,
                    ref projectionMatrix
                );
            }
        }
    }

    public enum LightType : int
    {
        Unknown     = 0,
        Directional = 1,
        Point       = 2,
        Spot        = 3,
    }
}