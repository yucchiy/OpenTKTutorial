namespace OpenTKTutorial
{
    public interface IInitializable
    {
        void Initialize(InitializeContext context);
    }

    public struct InitializeContext
    {
    }
}