using ImGuiNET;
using System;
using System.Numerics;
using OpenTK.Graphics.OpenGL4;

namespace OpenTKTutorial
{
    public class TransformTest : IScene, IInitializable, IRenderable, IUpdatable, IResizable, IDisposable
    {
        private readonly float[] Vertices =
        {
            -0.5f, -0.5f, 0.0f, 1.0f, 0.0f, 0.0f, // left-bottom
             0.5f, -0.5f, 0.0f, 0.0f, 1.0f, 0.0f, // right-bottom
             0.0f,  0.5f, 0.0f, 0.0f, 0.0f, 1.0f, // top
        };

        private float ScreenWidth { get; set; }
        private float ScreenHeight { get; set; }

        private OpenGLBuffer VertexBuffer { get; set; }
        private OpenGLVertexArray VertexArray { get; set; }

        private readonly string VertexShaderSourceCode = 
@"
#version 330 core

layout(location = 0) in vec3 InPosition;
layout(location = 1) in vec3 InColor;

uniform mat4 ModelMatrix;
uniform mat4 ViewMatrix;
uniform mat4 ProjectionMatrix;

out vec3 OutColor;

void main()
{
    gl_Position = ProjectionMatrix * ViewMatrix * ModelMatrix * vec4(InPosition, 1.0);
    OutColor = InColor;
}
";
        private readonly string FragmentShaderSourceCode = 
@"
#version 330 core

in vec3 OutColor;

out vec4 FragColor;

void main()
{
    FragColor = vec4(OutColor, 1.0);
}
";
        private OpenGLShader VertexShader { get; set; }
        private OpenGLShader FragmentShader { get; set; }
        private OpenGLProgram Program { get; set; }

        private OpenGLUniform ModelMatrixUniform { get; set; }
        private OpenGLUniform ViewMatrixUniform { get; set; }
        private OpenGLUniform ProjectionMatrixUniform { get; set; }

        private Vector3 TrianglePosition { get; set; } = Vector3.Zero;
        private Vector3 TriangleScale { get; set; } = Vector3.One;
        private Vector3 TriangleRotation { get; set; } = Vector3.Zero;

        private Vector3 CameraPosition { get; set; } = new Vector3(0f, 2f, 5f);
        private Vector3 CameraTarget { get; set; } = new Vector3(0f, 0f, 0f);
        private float CameraFovDegree { get; set; } = 60f;
        private float CameraNear { get; set; } = 0.1f;
        private float CameraFar { get; set; } = 100f;

        public void Initialize(InitializeContext context)
        {
            InitializeBuffer();
            InitializeShader();
            InitializeProgram();
            InitializeUniform();
            InitializeVertexArrayObject();
        }

        private void InitializeBuffer()
        {
            VertexBuffer = new OpenGLBuffer(1, BufferTarget.ArrayBuffer);
            VertexBuffer.SetData(0, 4 * 18, Vertices, BufferUsageHint.StaticDraw);
        }

        private void InitializeProgram()
        {
            Program = new OpenGLProgram(VertexShader, FragmentShader);
        }

        private void InitializeUniform()
        {
            ModelMatrixUniform = new OpenGLUniform(Program, "ModelMatrix");
            ViewMatrixUniform = new OpenGLUniform(Program, "ViewMatrix");
            ProjectionMatrixUniform = new OpenGLUniform(Program, "ProjectionMatrix");
        }

        private void InitializeShader()
        {
            VertexShader = new OpenGLShader(ShaderType.VertexShader, VertexShaderSourceCode);
            FragmentShader  = new OpenGLShader(ShaderType.FragmentShader, FragmentShaderSourceCode);
        }

        private void InitializeVertexArrayObject()
        {
            VertexArray = new OpenGLVertexArray(1);

            Program.Use();
            VertexBuffer.Bind(0);
            VertexArray.Bind(0);

            VertexArray.EnableAttribute(0, 0, 3, VertexAttribPointerType.Float, false, 24, 0);
            VertexArray.EnableAttribute(0, 1, 3, VertexAttribPointerType.Float, false, 24, 12);
            
            VertexArray.Unbind();
            VertexBuffer.Unbind();
            Program.Unuse();
        }

        public void Update(double deltaTime)
        {
            ImGui.Begin("Properties");

            ImGui.Text("Triangle");

            var trianglePosition = TrianglePosition;
            ImGui.InputFloat3("Object Position", ref trianglePosition);
            TrianglePosition = trianglePosition;

            var triangleRotation = TriangleRotation;
            ImGui.InputFloat3("Object Rotation", ref triangleRotation);
            TriangleRotation = triangleRotation;

            var triangleScale = TriangleScale;
            ImGui.InputFloat3("Object Scale", ref triangleScale);
            TriangleScale = triangleScale;

            ImGui.Text("Camera");

            var cameraPosition = CameraPosition;
            ImGui.InputFloat3("Camera Position", ref cameraPosition);
            CameraPosition = cameraPosition;

            var cameraTarget = CameraTarget;
            ImGui.InputFloat3("Camera Target", ref cameraTarget);
            CameraTarget = cameraTarget;

            var near = CameraNear;
            ImGui.SliderFloat("Camera Near", ref near, 0.01f, 1f);
            CameraNear = near;

            var far = CameraFar;
            ImGui.SliderFloat("Camera Far", ref far, 10f, 100f);
            CameraFar = far;

            var fov = CameraFovDegree;
            ImGui.SliderFloat("Camera Fov", ref fov, 20f, 80f);
            CameraFovDegree = fov;

            ImGui.End();
        }

        public void Render(double deltaTime)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            Utility.CheckError();

            VertexBuffer.Bind(0);
            VertexArray.Bind(0);
            Program.Use();

            var modelTranslationMatrix = OpenTK.Mathematics.Matrix4.CreateTranslation(TrianglePosition.X, TrianglePosition.Y, TrianglePosition.Z);
            var modelRotationMatrix = OpenTK.Mathematics.Matrix4.CreateFromQuaternion(
                OpenTK.Mathematics.Quaternion.FromEulerAngles(
                    OpenTK.Mathematics.MathHelper.DegreesToRadians(TriangleRotation.X),
                    OpenTK.Mathematics.MathHelper.DegreesToRadians(TriangleRotation.Y),
                    OpenTK.Mathematics.MathHelper.DegreesToRadians(TriangleRotation.Z)
                )
            );
            var modelScaleMatrix = OpenTK.Mathematics.Matrix4.CreateScale(TriangleScale.X, TriangleScale.Y, TriangleScale.Z);
            var modelMatrix = modelTranslationMatrix * modelRotationMatrix * modelScaleMatrix;
            ModelMatrixUniform.Matrix4(false, ref modelMatrix);

            var viewMatrix = OpenTK.Mathematics.Matrix4.LookAt(
                new OpenTK.Mathematics.Vector3(CameraPosition.X, CameraPosition.Y, CameraPosition.Z),
                new OpenTK.Mathematics.Vector3(CameraTarget.X, CameraTarget.Y, CameraTarget.Z),
                OpenTK.Mathematics.Vector3.UnitY
            );
            ViewMatrixUniform.Matrix4(false, ref viewMatrix);

            var projectionMatrix = OpenTK.Mathematics.Matrix4.CreatePerspectiveFieldOfView(
                OpenTK.Mathematics.MathHelper.DegreesToRadians(CameraFovDegree),
                (float)ScreenWidth / (float)ScreenHeight,
                CameraNear,
                CameraFar
            );
            ProjectionMatrixUniform.Matrix4(false, ref projectionMatrix);

            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
            Utility.CheckError();

            Program.Unuse();
            VertexArray.Unbind();
            VertexBuffer.Unbind();
        }

        public void Resize(int width, int height)
        {
            GL.Viewport(0, 0, width, height);
            ScreenWidth = width;
            ScreenHeight = height;
        }

        public void Dispose()
        {
            Program?.Dispose();
            FragmentShader?.Dispose();
            VertexShader?.Dispose();
            VertexArray?.Dispose();
            VertexBuffer?.Dispose();
        }
    }
}