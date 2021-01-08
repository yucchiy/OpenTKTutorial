using OpenTK.Graphics.OpenGL4;

namespace OpenTKTutorial
{
    public class OpenGLShader : System.IDisposable
    {
        public int Id { get; }
        public ShaderType Type { get; }
        public string SourceCode { get; }

        public OpenGLShader(ShaderType shaderType, string sourceCode)
        {
            Utility.Assert(!string.IsNullOrEmpty(sourceCode), "Empty shader source code.");

            Type = shaderType;
            SourceCode = sourceCode;

            Id = GL.CreateShader(Type);
            Utility.CheckError();

            GL.ShaderSource(Id, SourceCode);
            Utility.CheckError();

            GL.CompileShader(Id);
            Utility.CheckError();

            var log = GL.GetShaderInfoLog(Id);
            if (!string.IsNullOrEmpty(log)) throw new OpenGLException($"Shader Error: {log}");
        }

        public void Dispose()
        {
            GL.DeleteShader(Id);
        }
    }
}