using System.Linq;

namespace OpenTKTutorial
{
    public static partial class PrimitiveFactory
    {
        private static readonly float[] PlaneVertices = new float[]
        {
            -0.5f,  0.0f,  0.5f,
             0.5f,  0.0f,  0.5f,
             0.5f,  0.0f, -0.5f,
            -0.5f,  0.0f, -0.5f,
        };

        private static readonly float[] PlaneNormals = new float[]
        {
            // top
            0f, 1f, 0f,
            0f, 1f, 0f,
            0f, 1f, 0f,
            0f, 1f, 0f,
        };

        private static readonly uint[] PlaneIndices = new uint[]
        {
            0,  1,  2,
            2,  3,  0,
        };

        private static readonly float[] PlaneTextureCoordinates = new float[]
        {
            0.0f, 0.0f,
            1.0f, 0.0f,
            1.0f, 1.0f,
            0.0f, 1.0f,
        };

        public static void CreatePlaneIndexed(int scale, out float[] vertices, out uint[] indices, out float[] normals, out float[] textureCoordinates)
        {
            vertices = PlaneVertices.Select(v => v * scale).ToArray();
            indices = (uint[]) PlaneIndices.Clone();
            normals = (float[]) PlaneNormals.Clone();
            textureCoordinates = PlaneTextureCoordinates.Select(v => v * scale).ToArray();
        }
    }
}