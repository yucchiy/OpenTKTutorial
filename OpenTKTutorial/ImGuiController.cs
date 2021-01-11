using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ImGuiNET;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

namespace OpenTKTutorial
{
    public class ImGuiController : System.IDisposable, IResizable, IRenderable, IUpdatable
    {
        private GameWindow Window { get; }
        private int VertexBuffer { get; }
        private int IndexBuffer { get; }
        private int VertexArray { get; }

        private Texture FontAtlasTexture { get; }

        private int Program { get; }

        private int VertexBufferSize { get; set; }
        private int IndexBufferSize { get; set; }

        private int WindowWidth { get; set; }
        private int WindowHeight { get; set; }

        private bool BeganFrame { get; set; }

        private readonly System.Numerics.Vector2 ScaleFactor = System.Numerics.Vector2.One;
        private readonly List<char> PressedChars = new List<char>();


        public ImGuiController(int width, int height, GameWindow gameWindow)
        {
            Window = gameWindow;
            WindowWidth = width;
            WindowHeight = height;

            var context = ImGui.CreateContext();
            ImGui.SetCurrentContext(context);

            var io = ImGui.GetIO();
            io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;

            // device resources

            VertexBuffer = GL.GenBuffer();
            CheckError();
            IndexBuffer = GL.GenBuffer();
            CheckError();
            VertexArray = GL.GenVertexArray();
            CheckError();

            // font atlas
            io.Fonts.AddFontDefault();

            IntPtr fontTexturePixels;
            io.Fonts.GetTexDataAsRGBA32(out fontTexturePixels, out var fontTextureWidth, out var fontTextureHeight, out var fontTextureBytesPerPixel);
            FontAtlasTexture = new Texture(fontTextureWidth, fontTextureHeight, fontTexturePixels);
            io.Fonts.SetTexID((IntPtr)FontAtlasTexture.TextureId);
            io.Fonts.ClearTexData();

            var vertexShaderSource = @"#version 330 core
uniform mat4 projection_matrix;
layout(location = 0) in vec2 in_position;
layout(location = 1) in vec2 in_texCoord;
layout(location = 2) in vec4 in_color;
out vec4 color;
out vec2 texCoord;
void main()
{
    gl_Position = projection_matrix * vec4(in_position, 0, 1);
    color = in_color;
    texCoord = in_texCoord;
}";
            var fragmentShaderSource = @"#version 330 core
uniform sampler2D in_fontTexture;
in vec4 color;
in vec2 texCoord;
out vec4 outputColor;
void main()
{
    outputColor = color * texture(in_fontTexture, texCoord);
}";

            var vertexShader = new Shader(ShaderType.VertexShader, vertexShaderSource);
            var fragmentShader = new Shader(ShaderType.FragmentShader, fragmentShaderSource);

            Program = GL.CreateProgram();
            GL.AttachShader(Program, vertexShader.Id);
            CheckError();
            GL.AttachShader(Program, fragmentShader.Id);
            CheckError();
            GL.LinkProgram(Program);
            CheckError();

            GL.BindVertexArray(VertexArray);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBuffer);
            CheckError();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndexBuffer);
            CheckError();

            var stride = Unsafe.SizeOf<ImDrawVert>();

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, stride, 0);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, stride, 8);
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 4, VertexAttribPointerType.UnsignedByte, true, stride, 16);
            GL.EnableVertexAttribArray(0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            CheckError();
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            CheckError();

            GL.BindVertexArray(0);
            CheckError();

            SetKeyMappings(io);
            SetPerFramImGuiData(1f / 60f);

            NewFrame();
        }

        public void Resize(int width, int height)
        {
            WindowWidth = width;
            WindowHeight = height;
        }

        public void Update(double deltaTime)
        {
            if (BeganFrame)
            {
                ImGui.Render();
            }

            SetPerFramImGuiData((float)deltaTime);
            UpdateInput();

            NewFrame();
        }

        public void Render(double deltaTime)
        {
            if (BeganFrame)
            {
                BeganFrame = false;
                ImGui.Render();
                RenderImDrawData(ImGui.GetDrawData());
            }
        }

        public void Dispose()
        {
            GL.DeleteBuffer(VertexBuffer);
            GL.DeleteBuffer(IndexBuffer);
            GL.DeleteVertexArray(VertexArray);
            FontAtlasTexture.Dispose();
            GL.DeleteProgram(Program);
        }

        private void UpdateInput()
        {
            var io = ImGui.GetIO();

            var MouseState = Window.MouseState;
            var KeyboardState = Window.KeyboardState;

            io.MouseDown[0] = MouseState[MouseButton.Left];
            io.MouseDown[1] = MouseState[MouseButton.Right];
            io.MouseDown[2] = MouseState[MouseButton.Middle];

            var screenPoint = new Vector2i((int)MouseState.X, (int)MouseState.Y);
            var point = screenPoint;
            io.MousePos = new System.Numerics.Vector2(point.X, point.Y);
            
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                if (key == Keys.Unknown)
                {
                    continue;
                }

                io.KeysDown[(int)key] = KeyboardState.IsKeyDown(key);
            }

            foreach (var c in PressedChars)
            {
                io.AddInputCharacter(c);
            }
            PressedChars.Clear();

            io.KeyCtrl = KeyboardState.IsKeyDown(Keys.LeftControl) || KeyboardState.IsKeyDown(Keys.RightControl);
            io.KeyAlt = KeyboardState.IsKeyDown(Keys.LeftAlt) || KeyboardState.IsKeyDown(Keys.RightAlt);
            io.KeyShift = KeyboardState.IsKeyDown(Keys.LeftShift) || KeyboardState.IsKeyDown(Keys.RightShift);
            io.KeySuper = KeyboardState.IsKeyDown(Keys.LeftSuper) || KeyboardState.IsKeyDown(Keys.RightSuper);
        }

        private void SetKeyMappings(ImGuiIOPtr io)
        {
            io.KeyMap[(int)ImGuiKey.Tab] = (int)Keys.Tab;
            io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)Keys.Left;
            io.KeyMap[(int)ImGuiKey.RightArrow] = (int)Keys.Right;
            io.KeyMap[(int)ImGuiKey.UpArrow] = (int)Keys.Up;
            io.KeyMap[(int)ImGuiKey.DownArrow] = (int)Keys.Down;
            io.KeyMap[(int)ImGuiKey.PageUp] = (int)Keys.PageUp;
            io.KeyMap[(int)ImGuiKey.PageDown] = (int)Keys.PageDown;
            io.KeyMap[(int)ImGuiKey.Home] = (int)Keys.Home;
            io.KeyMap[(int)ImGuiKey.End] = (int)Keys.End;
            io.KeyMap[(int)ImGuiKey.Delete] = (int)Keys.Delete;
            io.KeyMap[(int)ImGuiKey.Backspace] = (int)Keys.Backspace;
            io.KeyMap[(int)ImGuiKey.Enter] = (int)Keys.Enter;
            io.KeyMap[(int)ImGuiKey.Escape] = (int)Keys.Escape;
            io.KeyMap[(int)ImGuiKey.A] = (int)Keys.A;
            io.KeyMap[(int)ImGuiKey.C] = (int)Keys.C;
            io.KeyMap[(int)ImGuiKey.V] = (int)Keys.V;
            io.KeyMap[(int)ImGuiKey.X] = (int)Keys.X;
            io.KeyMap[(int)ImGuiKey.Y] = (int)Keys.Y;
            io.KeyMap[(int)ImGuiKey.Z] = (int)Keys.Z;
        }

        private void SetPerFramImGuiData(float deltaTimeSeconds)
        {
            var io = ImGui.GetIO();
            io.DisplaySize = new System.Numerics.Vector2(WindowWidth / ScaleFactor.X, WindowHeight / ScaleFactor.Y);
            io.DisplayFramebufferScale = ScaleFactor;
            io.DeltaTime = deltaTimeSeconds;
        }

        private void NewFrame()
        {
            ImGui.NewFrame();
            BeganFrame = true;
        }

        private void RenderImDrawData(ImDrawDataPtr data)
        {
            if (data.CmdListsCount == 0) return;

            var io = ImGui.GetIO();

            var mvpMatrix = Matrix4.CreateOrthographicOffCenter(
                0.0f,
                io.DisplaySize.X,
                io.DisplaySize.Y,
                0.0f,
                -1.0f,
                1.0f
            );

            GL.UseProgram(Program);
            CheckError();
            GL.UniformMatrix4(GL.GetUniformLocation(Program, "projection_matrix"), false, ref mvpMatrix);
            CheckError();
            GL.Uniform1(GL.GetUniformLocation(Program, "in_fontTexture"), 0);
            CheckError();

            data.ScaleClipRects(io.DisplayFramebufferScale);

            GL.Enable(EnableCap.Blend);
            CheckError();
            GL.Enable(EnableCap.ScissorTest);
            CheckError();
            GL.BlendEquation(BlendEquationMode.FuncAdd);
            CheckError();
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            CheckError();
            GL.Disable(EnableCap.CullFace);
            CheckError();
            GL.Disable(EnableCap.DepthTest);
            CheckError();

            GL.BindVertexArray(VertexArray);

            for (var i = 0; i < data.CmdListsCount; ++i)
            {
                var commandList = data.CmdListsRange[i];

                GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBuffer);
                GL.BufferData(BufferTarget.ArrayBuffer, commandList.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>(), commandList.VtxBuffer.Data, BufferUsageHint.StreamDraw);
                GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBuffer);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndexBuffer);
                GL.BufferData(BufferTarget.ElementArrayBuffer, commandList.IdxBuffer.Size * sizeof(ushort), commandList.IdxBuffer.Data, BufferUsageHint.StreamDraw);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndexBuffer);

                for (var j = 0; j < commandList.CmdBuffer.Size; ++j)
                {
                    var command = commandList.CmdBuffer[j];
                    if (command .UserCallback != IntPtr.Zero)
                    {
                        throw new NotImplementedException();
                    }

                    FontAtlasTexture.ActiveTexture(TextureUnit.Texture0);
                    FontAtlasTexture.BindTexture();

                    var clip = command.ClipRect;
                    GL.Scissor((int)clip.X, WindowHeight - (int)clip.W, (int)(clip.Z - clip.X), (int)(clip.W - clip.Y));
                    CheckError();

                    if ((io.BackendFlags & ImGuiBackendFlags.RendererHasVtxOffset) != 0)
                    {
                        GL.DrawElementsBaseVertex(PrimitiveType.Triangles, (int)command.ElemCount, DrawElementsType.UnsignedShort, (IntPtr)(command.IdxOffset * sizeof(ushort)), (int)command.VtxOffset);
                    }
                    else
                    {
                        GL.DrawElements(BeginMode.Triangles, (int)command.ElemCount, DrawElementsType.UnsignedShort, (int)command.IdxOffset * sizeof(ushort));
                    }

                    CheckError();
                }
            }

            GL.BindVertexArray(0);

            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.ScissorTest);
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void CheckError()
        {
            var errorCode = GL.GetError();
            if (errorCode != OpenTK.Graphics.OpenGL4.ErrorCode.NoError)
            {
                throw new OpenGLException($"glGetError indicated an error: {errorCode}");
            }
        }

        private class Texture : IDisposable
        {
            public int Width { get; }
            public int Height { get; }
            public int MimpapLevels { get; }
            public SizedInternalFormat InternalFormat { get; }

            public int TextureIndex { get; }
            public int TextureId { get; }

            public Texture(int width, int height, IntPtr data)
            {
                Width = width;
                Height = height;
                MimpapLevels = 0;
                InternalFormat = SizedInternalFormat.Rgba8;
                TextureId = GL.GenTexture();
                TextureIndex = 0;

                GL.BindTexture(TextureTarget.Texture2D, TextureId);
                CheckError();

                GL.TexImage2D(TextureTarget.Texture2D, MimpapLevels, PixelInternalFormat.Rgba, Width, Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);
                CheckError();

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                CheckError();
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
                CheckError();
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                CheckError();
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Linear);
                CheckError();

                GL.BindTexture(TextureTarget.Texture2D, 0);
            }

            public void ActiveTexture(TextureUnit unit)
            {
                GL.ActiveTexture(unit);
                CheckError();
            }

            public void BindTexture()
            {
                GL.BindTexture(TextureTarget.Texture2D, TextureId);
                CheckError();
            }

            public void UnbindTexture()
            {
                GL.BindTexture(TextureTarget.Texture2D, 0);
                CheckError();
            }

            public void Dispose()
            {
                GL.DeleteTexture(TextureId);
            }
        }

        private class Shader : IDisposable
        {
            public int Id { get; }
            public Shader(ShaderType shaderType, string sourceCode)
            {
                Id = GL.CreateShader(shaderType);
                CheckError();

                GL.ShaderSource(Id, sourceCode);
                CheckError();

                GL.CompileShader(Id);
                CheckError();

                var log = GL.GetShaderInfoLog(Id);
                if (!string.IsNullOrEmpty(log)) throw new OpenGLException($"Shader Error: {log}");
            }

            public void Dispose()
            {
                GL.DeleteShader(Id);
            }
        }
    }
}