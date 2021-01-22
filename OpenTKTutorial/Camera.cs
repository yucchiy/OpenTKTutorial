using OpenTK.Mathematics;

namespace OpenTKTutorial
{
    public class Camera : IResizable
    {
        public Vector3 Position { get; set; }
        public Vector3 TargetPosition { get; set; }
        public float Fov { get; set; }
        public float Near { get; set; }
        public float Far { get; set; }

        private int ScreenWidth { get; set; }
        private int ScreenHeight { get; set; }

        public Camera(Vector3 position, Vector3 targetPosition, float fov, float near, float far, int screenWidth, int screenHeight)
        {
            Position = position;
            TargetPosition = targetPosition;
            Fov = fov;
            Near = near;
            Far = far;
            ScreenWidth = screenWidth;
            ScreenHeight = screenHeight;
        }

        public void Resize(int width, int height)
        {
            ScreenWidth = width;
            ScreenHeight = height;
        }

        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(
                Fov,
                (float)ScreenWidth / (float) ScreenHeight,
                Near,
                Far
            );
        }

        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(Position, TargetPosition, Vector3.UnitY);
        }
    }
}