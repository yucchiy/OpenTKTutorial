using System;
using OpenTK.Graphics.OpenGL4;

namespace OpenTKTutorial
{
    public class TextureTestScene : IScene, ISceneName, IInitializable, IRenderable, IResizable, IDisposable
    {
        public string SceneName { get; private set; }

        private readonly float[] Vertices = new float[]
        {
            -0.5f, -0.5f, 0.0f, 0.0f, 1.0f,
             0.5f, -0.5f, 0.0f, 1.0f, 1.0f,
             0.5f,  0.5f, 0.0f, 1.0f, 0.0f,
            -0.5f,  0.5f, 0.0f, 0.0f, 0.0f,
        };

        private readonly uint[] Indices = new uint[]
        {
            0, 1, 2,
            0, 2, 3,
        };

        private OpenGLBuffer VertexBuffer { get; set; }
        private OpenGLBuffer IndexBuffer { get; set; }
        private OpenGLVertexArray VertexArray { get; set; }

        private readonly string VertexShaderSourceCode = 
@"
#version 330 core

layout(location = 0) in vec3 InPosition;
layout(location = 1) in vec2 InTextureCoordinate;

out vec2 OutTextureCoordinate;

void main()
{
    gl_Position = vec4(InPosition, 1.0);
    OutTextureCoordinate = InTextureCoordinate;
}
";
        private readonly string FragmentShaderSourceCode = 
@"
#version 330 core

uniform sampler2D texture1;

in vec2 OutTextureCoordinate;

out vec4 FragColor;

void main()
{
    FragColor = vec4(texture(texture1, OutTextureCoordinate).xyz, 1.0);
}
";
        private OpenGLShader VertexShader { get; set; }
        private OpenGLShader FragmentShader { get; set; }
        private OpenGLProgram Program { get; set; }
        private Texture2D Texture { get; set; }


        public void Initialize(InitializeContext context)
        {
            InitializeBuffer();
            InitializeTexture();
            InitializeShader();
            InitializeProgram();
            InitializeVertexArrayObject();
        }

        private void InitializeBuffer()
        {
            VertexBuffer = new OpenGLBuffer(1, BufferTarget.ArrayBuffer);
            VertexBuffer.SetData(0, 4 * Vertices.Length, Vertices, BufferUsageHint.StaticDraw);

            IndexBuffer = new OpenGLBuffer(1, BufferTarget.ElementArrayBuffer);
            IndexBuffer.SetData(0, 4 * Indices.Length, Indices, BufferUsageHint.StaticDraw);
        }

        private void InitializeTexture()
        {
            Texture = new Texture2D("Scene/Resource/checker_01.jpg");
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

            VertexArray.EnableAttribute(0, 0, 3, VertexAttribPointerType.Float, false, 20, 0);
            VertexArray.EnableAttribute(0, 1, 2, VertexAttribPointerType.Float, false, 20, 12);
            
            VertexArray.Unbind();
            VertexBuffer.Unbind();
            Program.Unuse();
        }

        public void Render(double deltaTime)
        {
            SceneName = this.ToString() + " (FPS: " + (1/deltaTime).ToString("0.") + ")";
            GL.Clear(ClearBufferMask.ColorBufferBit);
            Utility.CheckError();

            VertexBuffer.Bind(0);
            VertexArray.Bind(0);
            Texture.Bind();
            Program.Use();

            IndexBuffer.Bind(0);
            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
            Utility.CheckError();
            IndexBuffer.Unbind();

            Program.Unuse();
            Texture.Unbind();
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
            Texture?.Dispose();
            VertexBuffer?.Dispose();
        }
    }
}