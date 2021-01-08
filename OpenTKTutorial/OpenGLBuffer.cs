using OpenTK.Graphics.OpenGL4;

namespace OpenTKTutorial
{
    public class OpenGLBuffer : System.IDisposable
    {
        public int Count { get; private set; }
        public BufferTarget Target { get; private set; }
        public int[] Ids { get; private set; }

        public OpenGLBuffer(int count, BufferTarget bufferTarget)
        {
            Utility.Assert(count > 0, "Invalid buffer count.");

            Count = count;
            Target = bufferTarget;
            Ids = new int[Count];

            GL.GenBuffers(Count, Ids);
            Utility.CheckError();
        }

        public void Bind(int index)
        {
            Utility.AssertRange(index, 0, Count, "Invalid buffer index.");

            GL.BindBuffer(Target, Ids[index]);
            Utility.CheckError();
        }

        public void Unbind()
        {
            GL.BindBuffer(Target, OpenGLConstant.NoneBufferId);
            Utility.CheckError();
        }

        public void SetData<T>(int index, int sizeInByte, T[] data, BufferUsageHint hint) where T : struct
        {
            Bind(index);

            GL.BufferData(Target, sizeInByte, data, hint);
            Utility.CheckError();

            Unbind();
        }

        public void Dispose()
        {
            GL.DeleteBuffers(Count, Ids);
        }
    }
}