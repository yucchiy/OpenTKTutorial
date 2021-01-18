using ImGuiNET;
using System;
using System.Linq;
using System.Numerics;
using OpenTK.Graphics.OpenGL4;

namespace OpenTKTutorial
{
    public class BasicLightTestScene : IScene, IInitializable, IRenderable, IUpdatable, IResizable, IDisposable
    {
        private float ScreenWidth { get; set; }
        private float ScreenHeight { get; set; }

        private Shape[] Shapes { get; set; }
        private LightProgram[] Programs { get; set; }

        private Vector3 CameraPosition { get; set; } = new Vector3(0f, 1.5f, 5f);
        private Vector3 CameraTarget { get; set; } = new Vector3(0f, 0f, 0f);
        private float CameraFovDegree { get; set; } = 60f;
        private float CameraNear { get; set; } = 0.1f;
        private float CameraFar { get; set; } = 100f;

        private LightCube Light { get => Shapes[Shapes.Length - 1] as LightCube; }

        public void Initialize(InitializeContext context)
        {
            InitializeShapes();
            InitializePrograms();
        }

        private void InitializeShapes()
        {
            Shapes = new Shape[]
            {
                new Cube(new Vector3(0f, 0f, 0f), new Vector3(0f, 30f, 0f), new Vector3(0.5f, 0.5f, 0.5f)),
                new LightCube(new Vector3(1.5f, 1.5f, 1.5f), new Vector3(1f, 1f, 1f)),
            };
        }

        private void InitializePrograms()
        {
            Programs = new LightProgram[]
            {
                new LightProgram(new Vector3(1f, 1f, 1f), 0.1f, 32f, 0.5f),
                new LightProgram(new Vector3(1f, 1f, 1f), 1f, 1f, 1f)
            };
        }

        public void Update(double deltaTime)
        {
            ImGui.Begin("Properties");

            for (var i = 0; i < Shapes.Length; ++i)
            {
                var shape = Shapes[i];

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

                if (shape is Cube cube)
                {
                    var color = cube.Color;
                    ImGui.SliderFloat3($"#{shape.GetHashCode()} Color", ref color, 0f, 1f);
                    cube.Color = color;
                }

                var program = Programs[i];
                program.UpdateGUI(deltaTime);
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

            var viewMatrix = OpenTK.Mathematics.Matrix4.LookAt(
                new OpenTK.Mathematics.Vector3(CameraPosition.X, CameraPosition.Y, CameraPosition.Z),
                new OpenTK.Mathematics.Vector3(CameraTarget.X, CameraTarget.Y, CameraTarget.Z),
                OpenTK.Mathematics.Vector3.UnitY
            );

            var projectionMatrix = OpenTK.Mathematics.Matrix4.CreatePerspectiveFieldOfView(
                OpenTK.Mathematics.MathHelper.DegreesToRadians(CameraFovDegree),
                (float)ScreenWidth / (float)ScreenHeight,
                CameraNear,
                CameraFar
            );

            for (var i = 0; i < Shapes.Length; ++i)
            {
                var shape = Shapes[i];
                var program = Programs[i];
                program.Program.Use();

                program.ViewMatrix.Matrix4(false, ref viewMatrix);
                program.ProjectionMatrix.Matrix4(false, ref projectionMatrix);

                program.LightPositionWorldUniform.Uniform3(new OpenTK.Mathematics.Vector3(Light.Position.X, Light.Position.Y, Light.Position.Z));
                program.LightColorUniform.Uniform3(new OpenTK.Mathematics.Vector3(Light.Color.X, Light.Color.Y, Light.Color.Z));
                program.EyePositionWorldUniform.Uniform3(new OpenTK.Mathematics.Vector3(CameraPosition.X, CameraPosition.Y, CameraPosition.Z));

                if (shape is Cube cube)
                {
                    program.ObjectColorUniform.Uniform3(new OpenTK.Mathematics.Vector3(cube.Color.X, cube.Color.Y, cube.Color.Z));
                }

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
                program.ModelMatrix.Matrix4(false, ref modelMatrix);

                shape.VertexArray.Bind(0);
                shape.IndexBuffer.Bind(0);

                GL.DrawElements(PrimitiveType.Triangles, shape.GetIndexCount(), DrawElementsType.UnsignedInt, 0);
                Utility.CheckError();

                shape.IndexBuffer.Unbind();
                shape.VertexArray.Unbind();

                program.Program.Unuse();
            }
        }

        public void Resize(int width, int height)
        {
            GL.Viewport(0, 0, width, height);
            ScreenWidth = width;
            ScreenHeight = height;
        }

        public void Dispose()
        {
            foreach (var shape in Shapes)
            {
                shape.Dispose();
            }

            foreach (var program in Programs)
            {
                program.Dispose();
            }
        }

        private class LightProgram : IDisposable, IGUIUpdatable
        {
            public OpenGLProgram Program { get; }
            public OpenGLUniform ModelMatrix { get; }
            public OpenGLUniform ViewMatrix { get; }
            public OpenGLUniform ProjectionMatrix { get; }
            public OpenGLUniform ObjectColorUniform { get; }

            private OpenGLUniform AmbientColorUniform { get; }
            private OpenTK.Mathematics.Vector3 AmbientColor { get; set; }
            private OpenGLUniform AmbientStrengthUniform { get; }
            private float AmbientStrength { get; set; }

            private OpenGLUniform SpecularShininessUniform { get; }
            private float SpecularShininess { get; set; }
            private OpenGLUniform SpecularStrengthUniform { get; }
            private float SpecularStrength { get; set; }

            public OpenGLUniform LightPositionWorldUniform { get; }
            public OpenGLUniform LightColorUniform { get; }
            public OpenGLUniform EyePositionWorldUniform { get; }

            private static readonly string VertexShaderFilePath = "Scene/Resource/BasicLightTestScene/light_vertex_shader.glsl";
            private static readonly string FragmentShaderFilePath = "Scene/Resource/BasicLightTestScene/light_fragment_shader.glsl";

            public LightProgram(Vector3 ambientColor, float ambientStrength, float specularShininess, float specularStrength)
            {
                var vertexShaderSourceCode = (new System.IO.StreamReader(Utility.GetEmbeddedResourceStream(VertexShaderFilePath))).ReadToEnd();
                var fragmentShaderSourceCode = (new System.IO.StreamReader(Utility.GetEmbeddedResourceStream(FragmentShaderFilePath))).ReadToEnd();
                using (var vertexShader = new OpenGLShader(ShaderType.VertexShader, vertexShaderSourceCode))
                {
                    using (var fragmentShader = new OpenGLShader(ShaderType.FragmentShader, fragmentShaderSourceCode))
                    {
                        Program = new OpenGLProgram(vertexShader, fragmentShader);
                    }
                }

                ModelMatrix = new OpenGLUniform(Program, "ModelMatrix");
                ViewMatrix = new OpenGLUniform(Program, "ViewMatrix");
                ProjectionMatrix = new OpenGLUniform(Program, "ProjectionMatrix");

                ObjectColorUniform = new OpenGLUniform(Program, "ObjectColor");

                AmbientColorUniform = new OpenGLUniform(Program, "AmbientColor");
                AmbientColor = new OpenTK.Mathematics.Vector3(ambientColor.X, ambientColor.Y, ambientColor.Z);
                AmbientStrengthUniform = new OpenGLUniform(Program, "AmbientStrength");
                AmbientStrength = ambientStrength;

                SpecularShininessUniform = new OpenGLUniform(Program, "SpecularShininess");
                SpecularShininess = specularShininess;
                SpecularStrengthUniform = new OpenGLUniform(Program, "SpecularStrength");
                SpecularStrength = specularStrength;

                LightPositionWorldUniform = new OpenGLUniform(Program, "LightPositionWorld");
                LightColorUniform = new OpenGLUniform(Program, "LightColor");
                EyePositionWorldUniform = new OpenGLUniform(Program, "EyePositionWorld");
            }

            public void UpdateGUI(double deltaTime)
            {
                ImGui.Text($"LightProgram: {Program.GetHashCode()}");

                var ambientColor = new Vector3(AmbientColor.X, AmbientColor.Y, AmbientColor.Z);
                ImGui.SliderFloat3($"AmbientColor {Program.GetHashCode()}", ref ambientColor, 0f, 1f);
                AmbientColor = new OpenTK.Mathematics.Vector3(ambientColor.X, ambientColor.Y, ambientColor.Z);
                AmbientColorUniform.Uniform3(AmbientColor);

                var ambientStrength = AmbientStrength;
                ImGui.SliderFloat($"AmbientStrength {Program.GetHashCode()}", ref ambientStrength, 0f, 1f);
                AmbientStrength = ambientStrength;
                AmbientStrengthUniform.Uniform1(AmbientStrength);

                var specularShininess = SpecularShininess;
                ImGui.InputFloat($"SpecularShininess {Program.GetHashCode()}", ref specularShininess);
                SpecularShininess = specularShininess;
                SpecularShininessUniform.Uniform1(SpecularShininess);

                var specularStrength = SpecularStrength;
                ImGui.SliderFloat($"SpecularStrength {Program.GetHashCode()}", ref specularStrength, 0f, 1f);
                SpecularStrength = specularStrength;
                SpecularStrengthUniform.Uniform1(SpecularStrength);
            }

            public void Dispose()
            {
                Program.Dispose();
            }
        }

        private abstract class Shape : IDisposable
        {
            public Shape(Vector3 position, Vector3 rotation)
            {
                Position = position;
                Rotation = rotation;
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
            public Cube(Vector3 position, Vector3 rotation, Vector3 color) : base(position, rotation)
            {
                Color = color;
            }

            protected override void InitializeShape()
            {
                PrimitiveFactory.CreateCubeIndexed(1f, out var vertices, out var indices, out var normals, out var _);

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
            public Vector3 Color { get; set; }

            private int _indexCount;
        }

        private class LightCube : Cube
        {
            public LightCube(Vector3 position, Vector3 color) : base(position, Vector3.Zero, color)
            {
                Scale = new Vector3(0.1f, 0.1f, 0.1f);
            }
        }
    }
}