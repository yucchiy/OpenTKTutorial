using OpenTK.Graphics.OpenGL4;

namespace OpenTKTutorial
{
    public static class Utility
    {
        public static void CheckError()
        {
            var errorCode = GL.GetError();
            if (errorCode != ErrorCode.NoError)
            {
                throw new OpenGLException($"glGetError indicated an error: {errorCode}");
            }
        }
    }
}