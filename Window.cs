using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace OpenTKTutorial
{
    public class Window : GameWindow
    {
        public Window() : base(
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
        }
    }
}