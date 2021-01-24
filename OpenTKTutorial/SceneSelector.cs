using System;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;

namespace OpenTKTutorial
{
    public class SceneSelector : IInitializable, IGUIUpdatable, IDisposable
    {
        private SceneDescription[] SceneList { get; set; }
        private GameManager Manager { get; set; }

        public void Initialize(InitializeContext context)
        {
            Manager = context.Manager;
            SceneList = new SceneDescription[]
            {
                new SceneDescription(
                    typeof(FirstTriangleScene)
                ),
                new SceneDescription(
                    typeof(ColoredTriangleScene)
                ),
                new SceneDescription(
                    typeof(TextureTestScene)
                ),
                new SceneDescription(
                    typeof(UniformScene)
                ),
                new SceneDescription(
                    typeof(IndexBufferObjectScene)
                ),
                new SceneDescription(
                    typeof(TransformTest)
                ),
                new SceneDescription(
                    typeof(BasicLightTestScene)
                ),
                new SceneDescription(
                    typeof(ModelLoadingScene)
                )
            };
        }

        public void UpdateGUI(double deltaTime)
        {
            ImGui.Begin("Scene Selector");

            ImGui.BeginGroup();
            foreach (var scene in SceneList)
            {
                if (ImGui.Button(scene.Name))
                {
                    Manager.ReplaceScene(scene.SceneType);
                }
            }
            ImGui.EndGroup();

            ImGui.End();
        }

        public void Dispose()
        {
            SceneList = null;
        }

        private struct SceneDescription
        {
            public Type SceneType;
            public string Name;

            public SceneDescription(Type type)
            {
                SceneType = type;
                Name = type.Name;
            }
        }
    }
}