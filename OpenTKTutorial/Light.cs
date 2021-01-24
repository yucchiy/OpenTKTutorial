using OpenTK.Mathematics;

namespace OpenTKTutorial
{
    public class Light
    {
        public Vector3 Position { get; set; }
        public Vector3 Color { get; set; }

        public Light(Vector3 position, Vector3 color)
        {
            Position = position;
            Color = color;
        }
    }
}