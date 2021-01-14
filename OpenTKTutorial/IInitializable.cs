namespace OpenTKTutorial
{
    public interface IInitializable
    {
        void Initialize(InitializeContext context);
    }

    public struct InitializeContext
    {
        public GameManager Manager { get; }
        public InitializeContext(GameManager manager)
        {
            Manager = manager;
        }
    }
}