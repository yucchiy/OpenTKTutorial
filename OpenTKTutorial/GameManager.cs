using System;
using OpenTK.Windowing.Desktop;

namespace OpenTKTutorial
{
    public class GameManager : System.IDisposable, IUpdatable, IRenderable, IResizable
    {
        private GameWindow Window { get; }
        private ImGuiController ImGuiController { get; }
        private SceneSelector SceneSelector { get; }
        private IScene CurrentScene { get; set; }

        public GameManager(GameWindow window)
        {
            Window = window;
            ImGuiController = new ImGuiController(Window.Size.X, Window.Size.Y, Window);
            SceneSelector = new SceneSelector();
            SceneSelector.Initialize(new InitializeContext(this));
        }

        public void ReplaceScene<T>() where T : IScene, new()
        {
            InternalReplaceScene(new T());
        }

        public void ReplaceScene(Type type)
        {
            InternalReplaceScene(Activator.CreateInstance(type) as IScene);
        }

        public void Update(double deltaTime)
        {
            ImGuiController.Update(deltaTime);

            if (CurrentScene is IUpdatable updatable)
            {
                updatable.Update(deltaTime);
            }

            if (CurrentScene is IGUIUpdatable guiUpdatable)
            {
                guiUpdatable.UpdateGUI(deltaTime);
            }

            SceneSelector.UpdateGUI(deltaTime);
        }

        public void Render(double deltaTime)
        {
            if (CurrentScene is IRenderable renderable)
            {
                renderable.Render(deltaTime);
            }

            SceneSelector.UpdateGUI(deltaTime);
            ImGuiController.Render(deltaTime);
        }

        public void Resize(int width, int height)
        {
            ImGuiController.Resize(width, height);

            if (CurrentScene is IResizable resizable)
            {
                resizable.Resize(width, height);
            }
        }

        public void Dispose()
        {
            if (CurrentScene is System.IDisposable diposable)
            {
                diposable.Dispose();
            }

            CurrentScene = null;

            ImGuiController.Dispose();
        }

        public void MouseScroll(in OpenTK.Mathematics.Vector2 offset)
        {
            ImGuiController.OnMouseScroll(offset);
        }

        public void TextInput(int unicode)
        {
            ImGuiController.AddInputCharacter((char)unicode);
        }

        private void InternalReplaceScene(IScene scene)
        {
            if (scene == null) throw new OpenTKTutorialException($"Invalid scene instance.");

            var previousScene = CurrentScene;

            if (scene is IInitializable initializable)
            {
                initializable.Initialize(new InitializeContext(this));
            }

            if (previousScene is System.IDisposable disposable)
            {
                disposable.Dispose();
            }

            CurrentScene = scene;
        }
    }
}