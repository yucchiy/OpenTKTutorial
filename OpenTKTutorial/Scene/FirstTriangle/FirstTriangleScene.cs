using System;
using OpenTK.Graphics.OpenGL4;

namespace OpenTKTutorial
{
    public class FirstTriangleScene : IScene, IInitializable, IRenderable, IResizable, IDisposable
    {
        private readonly float[] Vertices =
        {
            -0.5f, -0.5f, 0.0f, // left-bottom
             0.5f, -0.5f, 0.0f, // right-bottom
             0.0f,  0.5f, 0.0f, // top
        };

        private int[] VertexBufferObjects { get; set; } = new int[1];

        private int[] VertexArrayObjects { get; set; } = new int[1];

        private readonly string VertexShaderSourceCode = 
@"
#version 330 core

layout(location = 0) in vec3 InPosition;

void main()
{
    gl_Position = vec4(InPosition, 1.0);
}
";
        private readonly string FragmentShaderSourceCode = 
@"
#version 330 core

out vec4 FragColor;

void main()
{
    FragColor = vec4(1.0, 1.0, 1.0, 1.0);
}
";

        private int VertexShader { get; set; }
        private int FragmentShader { get; set; }
        private int Program { get; set; }
        private int PositionLocation { get; set; }

        public void Initialize(InitializeContext context)
        {
            InitializeBuffer();
            InitializeShader();
            InitializeProgram();
            InitializeVertexArrayObject();
        }

        private void InitializeBuffer()
        {
            GL.GenBuffers(1, VertexBufferObjects);
            Utility.CheckError();

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObjects[0]);
            Utility.CheckError();

            GL.BufferData(BufferTarget.ArrayBuffer, 4 * 9, Vertices, BufferUsageHint.StaticDraw);
            Utility.CheckError();

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            Utility.CheckError();
        }

        private void InitializeProgram()
        {
            Program = GL.CreateProgram();

            GL.AttachShader(Program, VertexShader);
            Utility.CheckError();

            GL.AttachShader(Program, FragmentShader);
            Utility.CheckError();

            GL.LinkProgram(Program);
            Utility.CheckError();

            GL.UseProgram(Program);

            PositionLocation = GL.GetAttribLocation(Program, "InPosition");
            Utility.CheckError();

            GL.UseProgram(0);
            Utility.CheckError();
        }

        private void InitializeShader()
        {
            VertexShader = CreateShader(ShaderType.VertexShader, VertexShaderSourceCode);
            FragmentShader = CreateShader(ShaderType.FragmentShader, FragmentShaderSourceCode);
        }

        private int CreateShader(ShaderType shaderType, string sourceCode)
        {
            var shader = GL.CreateShader(shaderType);
            Utility.CheckError();

            GL.ShaderSource(shader, sourceCode);
            Utility.CheckError();

            GL.CompileShader(shader);
            Utility.CheckError();

            var log = GL.GetShaderInfoLog(shader);
            if (!string.IsNullOrEmpty(log)) throw new OpenGLException($"Shader Error: {log}");

            return shader;
        }

        private void InitializeVertexArrayObject()
        {
            GL.GenVertexArrays(1, VertexArrayObjects);
            Utility.CheckError();

            GL.BindVertexArray(VertexArrayObjects[0]);
            Utility.CheckError();

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObjects[0]);
            Utility.CheckError();

            GL.EnableVertexAttribArray(PositionLocation);
            Utility.CheckError();

            GL.VertexAttribPointer(PositionLocation, 3, VertexAttribPointerType.Float, false, 12, 0);
            Utility.CheckError();

            GL.EnableVertexAttribArray(0);
            Utility.CheckError();

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            Utility.CheckError();

            GL.BindVertexArray(0);
            Utility.CheckError();
        }

        public void Render(double deltaTime)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            Utility.CheckError();

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObjects[0]);
            Utility.CheckError();

            GL.BindVertexArray(VertexArrayObjects[0]);
            Utility.CheckError();

            GL.UseProgram(Program);
            Utility.CheckError();

            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
            Utility.CheckError();
        }

        public void Resize(int width, int height)
        {
            GL.Viewport(0, 0, width, height);
        }

        public void Dispose()
        {
            GL.DeleteVertexArrays(1, VertexArrayObjects);
            GL.DeleteBuffers(1, VertexBufferObjects);
            GL.DeleteProgram(Program);
        }
    }
}