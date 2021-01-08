using OpenTK.Graphics.OpenGL4;

namespace OpenTKTutorial
{
    public class OpenGLProgram : System.IDisposable
    {
        public int Id { get; }

        public OpenGLShader VertexShader { get; }
        public OpenGLShader FragmentShader { get; }

        public OpenGLProgram(OpenGLShader vertexShader, OpenGLShader fragmentShader)
        {
            Utility.Assert(vertexShader != null, "Invalid vertexShader.");
            Utility.Assert(vertexShader.Type == ShaderType.VertexShader, "Given shader is not a vertex shader.");
            Utility.Assert(fragmentShader != null, "Invalid fragmentShader.");
            Utility.Assert(fragmentShader.Type == ShaderType.FragmentShader, "Given shader is not a fragment shader.");

            VertexShader = vertexShader; 
            FragmentShader = fragmentShader; 

            Id = GL.CreateProgram();
            Utility.CheckError();

            GL.AttachShader(Id, VertexShader.Id);
            Utility.CheckError();
            GL.AttachShader(Id, FragmentShader.Id);
            Utility.CheckError();

            GL.LinkProgram(Id);
            Utility.CheckError();
        }

        public void Use()
        {
            GL.UseProgram(Id);
        }

        public void Unuse()
        {
            GL.UseProgram(OpenGLConstant.NoneProgramId);
        }

        public void Dispose()
        {
            GL.DeleteProgram(Id);
        }
    }
}