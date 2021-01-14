using OpenTK.Graphics.OpenGL4;

namespace OpenTKTutorial
{
    public class FirstScene : IScene, IRenderable
    {
        public void Render(double deltaTime)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }
    }
}