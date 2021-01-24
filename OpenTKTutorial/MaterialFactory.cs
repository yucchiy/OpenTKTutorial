using OpenTK.Mathematics;

namespace OpenTKTutorial
{
    public class MaterialFactory
    {
        public Material LitPhongMaterial { get; }

        public MaterialFactory()
        {
        }

        public Material CreateMaterial(MaterialType type)
        {
            switch (type)
            {
                case MaterialType.Phong:
                    return CreatePhongMaterial();
            }

            throw new OpenTKTutorialException($"MaterialType {type} is not support.");
        }

        public Material CreateMaterial(Assimp.Material assimpMaterial)
        {
            var material = default(Material);
            if (assimpMaterial.HasShadingMode)
            {
                switch (assimpMaterial.ShadingMode)
                {
                    case Assimp.ShadingMode.Phong:
                        material = CreateMaterial(MaterialType.Phong);
                        break;
                    default:
                        material = CreateMaterial(MaterialType.Phong);
                        break;
                }
            }
            else
            {
                // Use default material
                material = CreateMaterial(MaterialType.Phong);
            }

            if (assimpMaterial.HasColorAmbient)
            {
                material.SetVec3(
                    OpenTKTutorialConstant.Material.AmbientColorName,
                    new Vector3(
                        assimpMaterial.ColorAmbient.R,
                        assimpMaterial.ColorAmbient.G,
                        assimpMaterial.ColorAmbient.B
                    )
                );
            }

            if (assimpMaterial.HasColorDiffuse)
            {
                material.SetVec3(
                    OpenTKTutorialConstant.Material.DiffuseColorName,
                    new Vector3(
                        assimpMaterial.ColorDiffuse.R,
                        assimpMaterial.ColorDiffuse.G,
                        assimpMaterial.ColorDiffuse.B
                    )
                );
            }

            if (assimpMaterial.HasColorSpecular)
            {
                material.SetVec3(
                    OpenTKTutorialConstant.Material.SpecularColorName,
                    new Vector3(
                        assimpMaterial.ColorDiffuse.R,
                        assimpMaterial.ColorDiffuse.G,
                        assimpMaterial.ColorDiffuse.B
                    )
                );
            }

            if (assimpMaterial.HasShininess)
            {
                material.SetFloat(
                    OpenTKTutorialConstant.Material.SpecularShininessName,
                    assimpMaterial.Shininess
                );
            }

            return material;
        }

        private Material CreatePhongMaterial()
        {
            return new Material(
                "DefaultLitPhong",
                (new System.IO.StreamReader(Utility.GetEmbeddedResourceStream("Resource/Shader/lit_phong_vertex_shader.glsl"))).ReadToEnd(),
                (new System.IO.StreamReader(Utility.GetEmbeddedResourceStream("Resource/Shader/lit_phong_fragment_shader.glsl"))).ReadToEnd()
            );
        }
    }
}