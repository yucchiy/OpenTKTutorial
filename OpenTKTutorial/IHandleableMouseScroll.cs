namespace OpenTKTutorial
{
    public interface IHandleableMouseScroll
    {
        void OnMouseScroll(in OpenTK.Mathematics.Vector2 offset);
    }
}