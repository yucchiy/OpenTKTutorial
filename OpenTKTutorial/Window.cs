using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace OpenTKTutorial
{
    public class Window : GameWindow
    {
        public IScene CurrentScene { get; private set; }

        public Window(IScene firstScene) : base(
            new GameWindowSettings()
            {
                IsMultiThreaded = true,
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
            CurrentScene = firstScene;
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            
            (CurrentScene as IInitializable)?.Initialize();
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
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

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

            Context.SwapBuffers();
        }
    }
}