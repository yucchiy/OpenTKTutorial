using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace OpenTKTutorial
{
    public class Model : IRenderable, System.IDisposable
    {
        public Vector3 Position { get; }
        public Vector3 Rotation { get; }
        public Vector3 Scale { get; }

        public MeshRenderer[] MeshRenderers { get; }

        public Model(Descriptor descriptor)
        {
            Utility.Assert(
                descriptor.Meshes != null &&
                descriptor.Meshes.Length > 0,
                "Model should have at least one mesh."
            );
            Utility.Assert(
                descriptor.MaterialIndices != null &&
                descriptor.MaterialIndices.Length == descriptor.Meshes.Length,
                "Model should have at least one mesh."
            );

            MeshRenderers = new MeshRenderer[descriptor.Meshes.Length];
            for (var rendererIndex = 0; rendererIndex < MeshRenderers.Length; ++rendererIndex)
            {
                var materialIndex = descriptor.MaterialIndices[rendererIndex];
                Utility.AssertRange(materialIndex, 0, descriptor.Materials.Length, "Invalid material index.");
                MeshRenderers[rendererIndex] = new MeshRenderer(descriptor.Meshes[rendererIndex], descriptor.Materials[rendererIndex]);
            }

            Position = Vector3.Zero;
            Rotation = Vector3.Zero;
            Scale = Vector3.One;
        }

        public void Render(double deltaTime)
        {
            foreach (var meshRenderer in MeshRenderers)
            {
                var modelTranslationMatrix = Matrix4.CreateTranslation(Position);
                var modelRotationMatrix = OpenTK.Mathematics.Matrix4.CreateFromQuaternion(
                    OpenTK.Mathematics.Quaternion.FromEulerAngles(Rotation)
                );
                var modelScaleMatrix = OpenTK.Mathematics.Matrix4.CreateScale(Scale);
                var modelMatrix = modelScaleMatrix * modelRotationMatrix * modelTranslationMatrix;
                meshRenderer.Material.SetMat4(
                    OpenTKTutorialConstant.Material.MVPModelMatrixName,
                    false,
                    ref modelMatrix
                );

                meshRenderer.Render(deltaTime);
            }
        }

        public void UpdateCamera(Camera camera)
        {
            foreach (var meshRenderer in MeshRenderers)
            {
                meshRenderer.Material.SetVec3(
                    OpenTKTutorialConstant.Material.CameraPositionName,
                    camera.Position
                );

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

        public void UpdateLight(Light light)
        {
            foreach (var meshRenderer in MeshRenderers)
            {
                meshRenderer.Material.SetVec3(
                    OpenTKTutorialConstant.Material.LightPositionName,
                    light.Position
                );

                meshRenderer.Material.SetVec3(
                    OpenTKTutorialConstant.Material.LightColorName,
                    light.Color
                );

                meshRenderer.Material.SetVec3(
                    OpenTKTutorialConstant.Material.AmbientColorName,
                    new Vector3(0.25f, 0.25f, 0.25f)
                );

                meshRenderer.Material.SetVec3(
                    OpenTKTutorialConstant.Material.DiffuseColorName,
                    new Vector3(0.5f, 0.5f, 0.5f)
                );

                meshRenderer.Material.SetVec3(
                    OpenTKTutorialConstant.Material.SpecularColorName,
                    new Vector3(0.25f, 0.25f, 0.25f)
                    // new Vector3(0.0f, 0.0f, 0.0f)
                );

                meshRenderer.Material.SetFloat(
                    OpenTKTutorialConstant.Material.SpecularShininessName,
                    32f
                );
            }
        }

        public void Dispose()
        {
        }

        public class Descriptor
        {
            public Mesh[] Meshes;
            public int[] MaterialIndices;
            public Material[] Materials;
        }
    }
}