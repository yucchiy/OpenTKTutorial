using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace OpenTKTutorial
{
    public class OpenGLUniform : System.IDisposable
    {
        public int Location { get; }
        public string Name { get; }

        public OpenGLProgram Program { get; }

        public OpenGLUniform(OpenGLProgram program, string name)
        {
            Utility.Assert(program != null, "Invalid program.");
            Utility.Assert(!string.IsNullOrEmpty(name), "Uniform name is empty.");

            Program = program;
            Name = name;

            Program.Use();
            Location = GL.GetUniformLocation(Program.Id, Name);
            Utility.CheckError();

            Utility.Assert(Location != -1, $"Uniform(name = {Name}) is not in found the program(program = {Program.Id})");
        }

        public void Uniform1(double value)
        {
            Program.Use();
            GL.Uniform1(Location, value);
            Utility.CheckError();
        }

        public void Uniform1(float value)
        {
            Program.Use();
            GL.Uniform1(Location, value);
            Utility.CheckError();
        }

        public void Uniform1(int value)
        {
            Program.Use();
            GL.Uniform1(Location, value);
            Utility.CheckError();
        }

        public void Uniform2(in Vector2 value)
        {
            Program.Use();
            GL.Uniform2(Location, value);
            Utility.CheckError();
        }

        public void Uniform3(in Vector3 value)
        {
            Program.Use();
            GL.Uniform3(Location, value);
            Utility.CheckError();
        }

        public void Uniform4(in Vector4 value)
        {
            Program.Use();
            GL.Uniform4(Location, value);
            Utility.CheckError();
        }

        public void Matrix4(bool transpose, ref Matrix4 value)
        {
            Program.Use();
            GL.UniformMatrix4(Location, transpose, ref value);
            Utility.CheckError();
        }

        public void Dispose()
        {
        }
    }
}