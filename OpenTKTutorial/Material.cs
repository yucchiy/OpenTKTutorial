using OpenTK.Mathematics;

namespace OpenTKTutorial
{
    public class Material : System.IDisposable
    {
        public OpenGLProgram Program { get; }

        public System.Collections.Generic.Dictionary<string, OpenGLUniform> Uniforms { get; }

        public Material(string vertexShaderSourceCode, string fragmentShaderSourceCode)
        {
            using (var vertexShader = new OpenGLShader(OpenTK.Graphics.OpenGL4.ShaderType.VertexShader, vertexShaderSourceCode))
            {
                using (var fragmentShader = new OpenGLShader(OpenTK.Graphics.OpenGL4.ShaderType.FragmentShader, fragmentShaderSourceCode))
                {
                    Program = new OpenGLProgram(vertexShader, fragmentShader);
                }
            }

            Uniforms = new System.Collections.Generic.Dictionary<string, OpenGLUniform>();
        }

        public void Use()
        {
            Program.Use();
        }

        public void SetInt(string name, int value)
        {
            GetOrCreateUniform(name, out var uniform);
            uniform.Uniform1(value);
        }

        public void SetFloat(string name, float value)
        {
            GetOrCreateUniform(name, out var uniform);
            uniform.Uniform1(value);
        }

        public void SetVec2(string name, in Vector2 value)
        {
            GetOrCreateUniform(name, out var uniform);
            uniform.Uniform2(value);
        }

        public void SetVec3(string name, in Vector3 value)
        {
            GetOrCreateUniform(name, out var uniform);
            uniform.Uniform3(value);
        }

        public void SetVec4(string name, in Vector4 value)
        {
            GetOrCreateUniform(name, out var uniform);
            uniform.Uniform4(value);
        }

        public void SetMat4(string name, bool tranpose, ref Matrix4 value)
        {
            GetOrCreateUniform(name, out var uniform);
            uniform.Matrix4(tranpose, ref value);
        }

        private void GetOrCreateUniform(string name, out OpenGLUniform uniform)
        {
            if (Uniforms.TryGetValue(name, out uniform)) return;

            uniform = new OpenGLUniform(Program, name);
            Uniforms[name] = uniform;
        }

        public void Dispose()
        {
            Uniforms.Clear();
            Program.Dispose();
        }
    }
}