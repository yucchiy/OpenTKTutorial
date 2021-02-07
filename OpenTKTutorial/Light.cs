using OpenTK.Mathematics;

namespace OpenTKTutorial
{
    public class Light
    {
        public LightType Type { get; set; } = LightType.Unknown;
        public Vector3 Position { get; set; }
        public Vector3 Color { get; set; }

        // for directional light
        public Vector3 Direction { get; set; }
        
        // for point
        public float Constant { get; set; }
        public float Linear { get; set; }
        public float Quadratic { get; set; }

        // for spot

        public float CutOff { get; set; }
        public float OuterCutOff { get; set; }

        public Light(Vector3 position, Vector3 color)
        {
            Position = position;
            Color = color;
        }
    }

    public enum LightType : int
    {
        Unknown     = 0,
        Directional = 1,
        Point       = 2,
        Spot        = 3,
    }
}