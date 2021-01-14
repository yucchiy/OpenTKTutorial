using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace OpenTKTutorial
{
    public class Window : GameWindow
    {
        public GameManager Manager { get; }

        public Window() : base(
            new GameWindowSettings()
            {
                IsMultiThreaded = false,
                RenderFrequency = 60.0,
                UpdateFrequency = 60.0,
            },
            new NativeWindowSettings()
            {
                APIVersion = new System.Version(4, 1),
                Flags = ContextFlags.ForwardCompatible,
            }
        )
        {
            Manager = new GameManager(this);
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            Manager.ReplaceScene<SceneSelectScene>();
            Size = new OpenTK.Mathematics.Vector2i(1280, 720);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            Manager.Dispose();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            Manager.Resize(e.Width, e.Height);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            Manager.Update(args.Time);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            Manager.Render(args.Time);
            Context.SwapBuffers();
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            Manager.MouseScroll(e.Offset);
        }

        protected override void OnTextInput(TextInputEventArgs e)
        {
            base.OnTextInput(e);
            Manager.TextInput(e.Unicode);
        }
    }
}