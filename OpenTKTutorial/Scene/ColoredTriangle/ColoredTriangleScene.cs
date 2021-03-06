using System;
using OpenTK.Graphics.OpenGL4;

namespace OpenTKTutorial
{
    public class ColoredTriangleScene : IScene, IInitializable, IRenderable, IResizable, IDisposable
    {
        private readonly float[] Vertices =
        {
            -0.5f, -0.5f, 0.0f, 1.0f, 0.0f, 0.0f, // left-bottom
             0.5f, -0.5f, 0.0f, 0.0f, 1.0f, 0.0f, // right-bottom
             0.0f,  0.5f, 0.0f, 0.0f, 0.0f, 1.0f, // top
        };

        private OpenGLBuffer VertexBuffer { get; set; }
        private OpenGLVertexArray VertexArray { get; set; }

        private readonly string VertexShaderSourceCode = 
@"
#version 330 core

layout(location = 0) in vec3 InPosition;
layout(location = 1) in vec3 InColor;

out vec3 OutColor;

void main()
{
    gl_Position = vec4(InPosition, 1.0);
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

        public void Initialize(InitializeContext context)
        {
            InitializeBuffer();
            InitializeShader();
            InitializeProgram();
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

        public void Render(double deltaTime)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            Utility.CheckError();

            VertexBuffer.Bind(0);
            VertexArray.Bind(0);
            Program.Use();

            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
            Utility.CheckError();

            Program.Unuse();
            VertexArray.Unbind();
            VertexBuffer.Unbind();
        }

        public void Resize(int width, int height)
        {
            GL.Viewport(0, 0, width, height);
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