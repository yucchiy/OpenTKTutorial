using ImGuiNET;
using System;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace OpenTKTutorial
{
    public class ModelLoadingScene : IScene, IInitializable, IRenderable, IUpdatable, IResizable, IDisposable
    {
        private Model Model { get; set; }
        private Camera Camera { get; set; }
        private SphericalCoordinateSystem CameraCoordinate;
        private Light Light { get; set; }

        public void Initialize(InitializeContext context)
        {
            Model = ModelLoader.LoadModel("Scene/Resource/ModelLoadingScene/Alicia/FBX/Alicia_solid_Unity.FBX");
            Model.Rotation = new Vector3(-90f, 0f, 0f);
            Camera = new Camera(
                new Vector3(0f, 0f, 3f),
                new Vector3(0f, 1.6f, 0f),
                MathHelper.DegreesToRadians(60f),
                0.1f,
                1000f,
                (int)context.Manager.DisplaySize.X,
                (int)context.Manager.DisplaySize.Y
            );
            CameraCoordinate = new SphericalCoordinateSystem(new Vector3(0f, 1.6f, 0f), 5f, 60f, 90f);
            Light = new Light(
                new Vector3(0f, 5f, 5f),
                new Vector3(1f, 1f, 1f)
            );
        }

        public void Render(double deltaTime)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);

            Utility.CheckError();

            Model.UpdateCamera(Camera);
            Model.UpdateLight(Light);

            Model.Render(deltaTime);
        }

        public void Update(double deltaTime)
        {
            ImGui.Begin("Camera");

            ImGui.SliderFloat("Distance", ref CameraCoordinate.Radius, 0f, 150f);
            ImGui.SliderFloat("Theta", ref CameraCoordinate.Theta, 0f, 180f);
            ImGui.SliderFloat("Phi", ref CameraCoordinate.Phi, 0f, 360f);

            ImGui.End();

            Camera.Position = CameraCoordinate.Position;
        }

        public void Resize(int width, int height)
        {
            Camera.Resize(width, height);
        }

        public void Dispose()
        {
            Model?.Dispose();
        }

        private struct SphericalCoordinateSystem
        {
            public SphericalCoordinateSystem(Vector3 offset, float radius, float theta, float phi)
            {
                Offset = offset;
                Radius = radius;
                Theta = theta;
                Phi = phi;
            }

            public Vector3 Offset;
            public float Radius;
            public float Theta;
            public float Phi;

            public Vector3 Position
            {
                get
                {
                    var theta = MathHelper.DegreesToRadians(Theta);
                    var phi = MathHelper.DegreesToRadians(Phi);

                    return new Vector3(
                        Radius * MathF.Sin(theta) * MathF.Cos(phi) + Offset.X,
                        Radius * MathF.Cos(theta)                  + Offset.Y,
                        Radius * MathF.Sin(theta) * MathF.Sin(phi) + Offset.Z
                    );
                }
            }
        }
    }
}