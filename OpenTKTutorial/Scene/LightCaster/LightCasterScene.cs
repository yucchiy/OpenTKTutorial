using ImGuiNET;
using System;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace OpenTKTutorial
{
    public class LightCasterScene : IScene, IInitializable, IRenderable, IUpdatable, IResizable, IDisposable
    {
        private LightCaster.Box Box { get; set; }
        private LightCaster.Plane Plane { get; set; }
        private Camera Camera { get; set; }
        private Light Light { get; set; }

        public void Initialize(InitializeContext context)
        {
            Box = new LightCaster.Box();
            Box.Model.Rotation = new Vector3(0f, 0f, 0f);
            Plane = new LightCaster.Plane();
            Camera = new Camera(
                new Vector3(0f, 3f, 3f),
                new Vector3(0f, 0f, 0f),
                MathHelper.DegreesToRadians(60f),
                0.1f,
                1000f,
                (int)context.Manager.DisplaySize.X,
                (int)context.Manager.DisplaySize.Y
            );
            Light = new Light(
                new Vector3(0f, 0.2f, -2f),
                new Vector3(1f, 1f, 1f)
            );
            Light.Direction = (new Vector3(-1f, -1f, -1f)).Normalized();

            Light.Constant = 1.0f;
            Light.Linear = 0.09f;
            Light.Quadratic = 0.032f;

            Light.Direction = new Vector3(0f, -1f, 0f);
            Light.CutOff = MathF.Cos(MathHelper.DegreesToRadians(12.5f));
            Light.OuterCutOff = MathF.Cos(MathHelper.DegreesToRadians(14.5f));

            Light.Type = LightType.Point;
        }

        public void Render(double deltaTime)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);

            Utility.CheckError();

            Plane.Model.UpdateCamera(Camera);
            Plane.Model.UpdateLight(Light);
            Plane.Render(deltaTime);
        }

        public void Update(double deltaTime)
        {
            ImGui.Begin("Properties");

            var lightPosition = new System.Numerics.Vector3(
                Light.Position.X,
                Light.Position.Y,
                Light.Position.Z
            );
            ImGui.SliderFloat3("Light Position", ref lightPosition, -10f, 10f);
            Light.Position = new Vector3(
                lightPosition.X,
                lightPosition.Y,
                lightPosition.Z
            );

            var lightType = (int)Light.Type - 1;
            ImGui.Combo("LightType", ref lightType, LightTypes, LightTypes.Length);
            Light.Type = (LightType)(lightType + 1);

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