using ImGuiNET;
using System;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace OpenTKTutorial
{
    public class MultipleLightScene : IScene, IInitializable, IRenderable, IUpdatable, IResizable, IDisposable
    {
        private MultipleLight.Box Box { get; set; }
        private MultipleLight.Plane Plane { get; set; }
        private Camera Camera { get; set; }
        private Light DirectionalLight { get; set; }
        private Light[] PointLights { get; set; }

        private Light[] AllLight { get; set; }

        public void Initialize(InitializeContext context)
        {
            Box = new MultipleLight.Box();
            Box.Model.Rotation = new Vector3(0f, 0f, 0f);
            Plane = new MultipleLight.Plane();
            Camera = new Camera(
                new Vector3(0f, 3f, 3f),
                new Vector3(0f, 0f, 0f),
                MathHelper.DegreesToRadians(60f),
                0.1f,
                1000f,
                (int)context.Manager.DisplaySize.X,
                (int)context.Manager.DisplaySize.Y
            );

            DirectionalLight = new Light(
                new Vector3(0f, 1f, 0f),
                new Vector3(0.5f, 0.5f, 0.5f)
            );
            DirectionalLight.Direction = (new Vector3(-1f, -1f, -1f)).Normalized();

            PointLights = new Light[3];
            PointLights[0] = new Light(
                new Vector3(0f, 0.2f, -2f),
                new Vector3(1f, 0f, 0f)
            );
            PointLights[0].Constant = 1.0f;
            PointLights[0].Linear = 0.09f;
            PointLights[0].Quadratic = 0.032f;

            PointLights[1] = new Light(
                new Vector3(2f, 0.2f, 2f),
                new Vector3(0f, 1f, 0f)
            );
            PointLights[1].Constant = 1.0f;
            PointLights[1].Linear = 0.09f;
            PointLights[1].Quadratic = 0.032f;

            PointLights[2] = new Light(
                new Vector3(-2f, 0.2f, 2f),
                new Vector3(0f, 0f, 1f)
            );
            PointLights[2].Constant = 1.0f;
            PointLights[2].Linear = 0.09f;
            PointLights[2].Quadratic = 0.032f;

            AllLight = new Light[]{
                DirectionalLight,
                PointLights[0],
                PointLights[1],
                PointLights[2],
            };
        }

        public void Render(double deltaTime)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);

            Utility.CheckError();

            DirectionalLight.UpdateCamera(Camera);
            PointLights[0].UpdateCamera(Camera);
            PointLights[1].UpdateCamera(Camera);
            PointLights[2].UpdateCamera(Camera);

            Plane.Model.UpdateCamera(Camera);
            Plane.UpdateDirectionalLight(DirectionalLight);
            Plane.UpdateSpotLight(0, PointLights[0]);
            Plane.UpdateSpotLight(1, PointLights[1]);
            Plane.UpdateSpotLight(2, PointLights[2]);

            DirectionalLight.Render(deltaTime);
            PointLights[0].Render(deltaTime);
            PointLights[1].Render(deltaTime);
            PointLights[2].Render(deltaTime);

            Plane.Render(deltaTime);
        }

        public void Update(double deltaTime)
        {
            ImGui.Begin("MultipleLightScene");

            for (var i = 0; i < AllLight.Length; ++i)
            {
                ImGui.Text($"Light {i}: ");

                var light = AllLight[i];

                var lightPosition = new System.Numerics.Vector3(
                    light.Position.X,
                    light.Position.Y,
                    light.Position.Z
                );
                ImGui.SliderFloat3($"Light Position {i}", ref lightPosition, -10f, 10f);
                light.Position = new Vector3(
                    lightPosition.X,
                    lightPosition.Y,
                    lightPosition.Z
                );

                var lightColor = new System.Numerics.Vector3(
                    light.Color.X,
                    light.Color.Y,
                    light.Color.Z
                );
                ImGui.ColorEdit3($"Light Color {i}", ref lightColor);
                light.Color = new Vector3(
                    lightColor.X,
                    lightColor.Y,
                    lightColor.Z
                );
            }

            ImGui.End();
        }

        private static readonly string[] LightTypes = new string[]{"Directional", "Point", "Spot"};

        public void Resize(int width, int height)
        {
            Camera.Resize(width, height);
        }

        public void Dispose()
        {
            Plane = null;
            Box = null;
        }
    }
}