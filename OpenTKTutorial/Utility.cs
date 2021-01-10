using System.Reflection;
using OpenTK.Graphics.OpenGL4;
using System.IO;

namespace OpenTKTutorial
{
    public static class Utility
    {
        [System.Diagnostics.Conditional("DEBUG")]
        public static void CheckError()
        {
            var errorCode = GL.GetError();
            if (errorCode != ErrorCode.NoError)
            {
                throw new OpenGLException($"glGetError indicated an error: {errorCode}");
            }
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void Assert(bool condition, string message)
        {
            System.Diagnostics.Debug.Assert(condition, message);
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void AssertRange(int value, int min, int max, string message)
        {
            Assert(min <= value && value < max, message);
        }

        public static Stream GetEmbeddedResourceStream(string path)
        {
            return CurrentAssembly.GetManifestResourceStream(ProjectName + "." + path.Replace("/", "."));
        }

        private static readonly Assembly CurrentAssembly = typeof(Utility).GetTypeInfo().Assembly;
        private static readonly string ProjectName = "OpenTKTutorial";
    }
}