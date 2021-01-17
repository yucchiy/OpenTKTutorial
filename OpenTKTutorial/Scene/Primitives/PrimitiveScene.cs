using ImGuiNET;
using System;
using System.Linq;
using System.Numerics;
using OpenTK.Graphics.OpenGL4;

namespace OpenTKTutorial
{
    public class PrimitiveScene : IScene, IInitializable, IRenderable, IUpdatable, IResizable, IDisposable
    {
        private float ScreenWidth { get; set; }
        private float ScreenHeight { get; set; }

        private Shape[] Shapes { get; set; }

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

        private Vector3 CameraPosition { get; set; } = new Vector3(0f, 5f, 5f);
        private Vector3 CameraTarget { get; set; } = new Vector3(0f, 0f, 0f);
        private float CameraFovDegree { get; set; } = 60f;
        private float CameraNear { get; set; } = 0.1f;
        private float CameraFar { get; set; } = 100f;

        public void Initialize(InitializeContext context)
        {
            InitializeShapes();
            InitializeShader();
            InitializeProgram();
            InitializeUniform();
        }

        private void InitializeShapes()
        {
            Shapes = new Shape[]
            {
                new Cube(new Vector3(-1f, 0f, 0f)),
                new Cube(new Vector3(1f, 0f, 0f)),
            };
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

        public void Update(double deltaTime)
        {
            ImGui.Begin("Properties");


            foreach (var shape in Shapes)
            {
                ImGui.Text($"Shape #{shape.GetHashCode()}");
                var position = shape.Position;
                ImGui.InputFloat3($"#{shape.GetHashCode()} Position", ref position);
                shape.Position = position;

                var rotation = shape.Rotation;
                ImGui.SliderFloat3($"#{shape.GetHashCode()} Rotation", ref rotation, -180f, 180f);
                shape.Rotation = rotation;

                var scale = shape.Scale;
                ImGui.InputFloat3($"#{shape.GetHashCode()} Scale", ref scale);
                shape.Scale = scale;
            }

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
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);
            Utility.CheckError();

            Program.Use();

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

            foreach (var shape in Shapes)
            {
                var modelTranslationMatrix = OpenTK.Mathematics.Matrix4.CreateTranslation(shape.Position.X, shape.Position.Y, shape.Position.Z);
                var modelRotationMatrix = OpenTK.Mathematics.Matrix4.CreateFromQuaternion(
                    OpenTK.Mathematics.Quaternion.FromEulerAngles(
                        OpenTK.Mathematics.MathHelper.DegreesToRadians(shape.Rotation.X),
                        OpenTK.Mathematics.MathHelper.DegreesToRadians(shape.Rotation.Y),
                        OpenTK.Mathematics.MathHelper.DegreesToRadians(shape.Rotation.Z)
                    )
                );
                var modelScaleMatrix = OpenTK.Mathematics.Matrix4.CreateScale(shape.Scale.X, shape.Scale.Y, shape.Scale.Z);
                var modelMatrix = modelScaleMatrix * modelRotationMatrix * modelTranslationMatrix;
                ModelMatrixUniform.Matrix4(false, ref modelMatrix);

                shape.VertexArray.Bind(0);
                shape.IndexBuffer.Bind(0);

                GL.DrawElements(PrimitiveType.Triangles, shape.GetIndexCount(), DrawElementsType.UnsignedInt, 0);
                Utility.CheckError();

                shape.IndexBuffer.Unbind();
                shape.VertexArray.Unbind();
            }

            Program.Unuse();
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

            foreach (var shape in Shapes)
            {
                shape.Dispose();
            }
        }

        private abstract class Shape : IDisposable
        {
            public Shape(Vector3 position)
            {
                Position = position;
                Rotation = Vector3.Zero;
                Scale = Vector3.One;

                VertexBuffer = new OpenGLBuffer(2, BufferTarget.ArrayBuffer);
                IndexBuffer = new OpenGLBuffer(1, BufferTarget.ElementArrayBuffer);
                VertexArray = new OpenGLVertexArray(1);

                InitializeShape();
            }

            public void Dispose()
            {
                VertexArray.Dispose();
                IndexBuffer.Dispose();
                VertexBuffer.Dispose();
            }

            public Vector3 Position { get; set; }
            public Vector3 Rotation { get; set; }
            public Vector3 Scale { get; set; }
            public OpenGLBuffer VertexBuffer { get; }
            public OpenGLBuffer IndexBuffer { get; }
            public OpenGLVertexArray VertexArray { get; }

            protected abstract void InitializeShape();
            public abstract int GetIndexCount();
        }

        private class Cube : Shape
        {
            public Cube(Vector3 position) : base(position) {}

            protected override void InitializeShape()
            {
                PrimitiveFactory.CreateCubeIndexed(1f, out var vertices, out var indices, out var normals, out var _);
                normals = normals.Select(v => MathF.Abs(v)).ToArray();

                VertexBuffer.SetData(0, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
                VertexBuffer.SetData(1, normals.Length * sizeof(float), normals, BufferUsageHint.StaticDraw);

                IndexBuffer.SetData(0, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
                _indexCount = indices.Length;

                IndexBuffer.Bind(0);
                VertexArray.Bind(0);

                VertexBuffer.Bind(0);
                VertexArray.EnableAttribute(0, 0, 3, VertexAttribPointerType.Float, false, 12, 0);

                VertexBuffer.Bind(1);
                VertexArray.EnableAttribute(0, 1, 3, VertexAttribPointerType.Float, false, 12, 0);
                
                VertexArray.Unbind();
                IndexBuffer.Unbind();
                VertexBuffer.Unbind();
            }

            public override int GetIndexCount() => _indexCount;

            private int _indexCount;
        }
    }
}