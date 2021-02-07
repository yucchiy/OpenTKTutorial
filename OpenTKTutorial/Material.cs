using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using OpenTKTutorial;

namespace OpenTKTutorial
{
    public class Material : System.IDisposable
    {
        public string Name { get; }
        public OpenGLProgram Program { get; }

        public System.Collections.Generic.Dictionary<string, OpenGLUniform> Uniforms { get; }
        public System.Collections.Generic.Dictionary<TextureUnit, (string Name, Texture2D Texture)> Textures { get; }

        public Material(string name, string vertexShaderSourceCode, string fragmentShaderSourceCode)
        {
            Name = name;
            using (var vertexShader = new OpenGLShader(OpenTK.Graphics.OpenGL4.ShaderType.VertexShader, vertexShaderSourceCode))
            {
                using (var fragmentShader = new OpenGLShader(OpenTK.Graphics.OpenGL4.ShaderType.FragmentShader, fragmentShaderSourceCode))
                {
                    Program = new OpenGLProgram(vertexShader, fragmentShader);
                }
            }

            Uniforms = new System.Collections.Generic.Dictionary<string, OpenGLUniform>();
            Textures = new System.Collections.Generic.Dictionary<TextureUnit, (string, Texture2D)>();
        }

        public void Use()
        {
            Program.Use();
            foreach (var texturePair in Textures)
            {
                texturePair.Value.Texture.Active(texturePair.Key);
                SetInt(texturePair.Value.Name, (int)texturePair.Key - (int)TextureUnit.Texture0);
            }
        }

        public void Unuse()
        {
            Program.Unuse();
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

        public void SetTexture(TextureUnit unit, string name, Texture2D texture)
        {
            Utility.Assert(!Textures.ContainsKey(unit), "This texture slot already filled");
            Textures[unit] = (name, texture);
        }

        private void GetOrCreateUniform(string name, out OpenGLUniform uniform)
        {
            if (Uniforms.TryGetValue(name, out uniform)) return;

            uniform = new OpenGLUniform(Program, name);
            Uniforms[name] = uniform;
        }

        public void Dispose()
        {
            Textures.Clear();
            Uniforms.Clear();
            Program.Dispose();
        }
    }
}