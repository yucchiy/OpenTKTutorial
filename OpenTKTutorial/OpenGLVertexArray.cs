using OpenTK.Graphics.OpenGL4;

namespace OpenTKTutorial
{
    public class OpenGLVertexArray : System.IDisposable
    {
        public int Count { get; private set; }
        public int[] Ids { get; private set; }

        public OpenGLVertexArray(int count)
        {
            Utility.Assert(count > 0, "Invalid buffer count.");

            Count = count;
            Ids = new int[Count];

            GL.GenVertexArrays(Count, Ids);
            Utility.CheckError();
        }

        public void Bind(int index)
        {
            Utility.AssertRange(index, 0, Count, "Invalid buffer index.");

            GL.BindVertexArray(Ids[index]);
            Utility.CheckError();
        }

        public void Unbind()
        {
            GL.BindVertexArray(OpenGLConstant.NoneVertexArrayId);
            Utility.CheckError();
        }

        public void EnableAttribute(int index, int location, int elementCount, VertexAttribPointerType elementType, bool normalized, int strideInByte, int offsetInByte)
        {
            Bind(index);

            GL.EnableVertexAttribArray(location);
            Utility.CheckError();
            GL.VertexAttribPointer(location, elementCount, elementType, normalized, strideInByte, offsetInByte);
            Utility.CheckError();

            // Unbind
            GL.EnableVertexAttribArray(OpenGLConstant.NoneLocation);
            Utility.CheckError();
            Unbind();
        }

        public void Dispose()
        {
            GL.DeleteBuffers(Count, Ids);
        }
    }
}