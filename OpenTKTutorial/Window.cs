using ImGuiNET;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;

namespace OpenTKTutorial
{
    public class Window : GameWindow
    {
        public IScene CurrentScene { get; private set; }

        public ImGuiController ImGuiController { get; private set; }

        public Window(IScene firstScene) : base(
            new GameWindowSettings()
            {
                IsMultiThreaded = true,
                RenderFrequency = 60.0,
                UpdateFrequency = 60.0,
            },
            new NativeWindowSettings()
            {
                APIVersion = new System.Version(3, 3),
                Flags = ContextFlags.ForwardCompatible,
            }
        )
        {
            CurrentScene = firstScene;
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            ImGuiController = new ImGuiController(Size.X, Size.Y, this);
            
            (CurrentScene as IInitializable)?.Initialize(new InitializeContext());
        }

        protected override void OnUnload()
        {
            (CurrentScene as System.IDisposable)?.Dispose();

            base.OnUnload();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            (CurrentScene as IResizable)?.Resize(e.Width, e.Height);

            GL.Viewport(0, 0, e.Width, e.Height);
            ImGuiController.Resize(e.Width, e.Height);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            ImGuiController.Update(args.Time);

            (CurrentScene as IUpdatable)?.Update(args.Time);

            if (CurrentScene is ISceneName hasName)
            {
                Title = $"OpenTK Tutorial - {hasName.SceneName}";
            }
            else
            {
                Title = $"OpenTK Tutorial - {CurrentScene.ToString()}";
            }
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            (CurrentScene as IRenderable)?.Render(args.Time);

            ImGuiController.Render(args.Time);

            Context.SwapBuffers();
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            ImGuiController.OnMouseScroll(e.Offset);
        }

        protected override void OnTextInput(TextInputEventArgs e)
        {
            base.OnTextInput(e);
            ImGuiController.AddInputCharacter((char)e.Unicode);
        }
    }
}