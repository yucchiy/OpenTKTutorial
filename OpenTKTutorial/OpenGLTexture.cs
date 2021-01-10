using System;
using OpenTK.Graphics.OpenGL4;

namespace OpenTKTutorial
{
    public class OpenGLTexture : IDisposable
    {
        public int Count { get; }
        public TextureTarget Target { get; }
        public int[] Ids { get; }

        public OpenGLTexture(int count, TextureTarget target)
        {
            Utility.Assert(count > 0, "Invalid buffer count.");

            Count = count;
            Target = target;
            Ids = new int[Count];

            GL.GenTextures(Count, Ids);
            Utility.CheckError();
        }

        public void SetTexture2D(int index, int width, int height, int level, PixelInternalFormat internalFormat, PixelFormat format, PixelType type, byte[] data)
        {
            Bind(index);
            GL.TexImage2D(Target, level, internalFormat, width, height, 0, format, type, data);
            Utility.CheckError();
            Unbind();
        }

        public void Bind(int index)
        {
            Utility.AssertRange(index, 0, Count, "Invalid texture index.");

            GL.BindTexture(Target, Ids[index]);
            Utility.CheckError();
        }

        public void Active(int index, TextureUnit unit)
        {
            Utility.AssertRange(index, 0, Count, "Invalid texture index.");

            GL.ActiveTexture(Ids[index], unit);
            Utility.CheckError();
        }

        public void SetParameter(int index, TextureParameterName parameterName, int value)
        {
            Bind(index);
            GL.TexParameter(Target, parameterName, value);
            Utility.CheckError();
            Unbind();
        }

        public void Unbind()
        {
            GL.BindTexture(Target, OpenGLConstant.NoneTextureId);
            Utility.CheckError();
        }

        public void Dispose()
        {
            GL.DeleteTextures(Count, Ids);
        }
    }
}