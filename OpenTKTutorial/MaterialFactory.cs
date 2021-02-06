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

        public Material CreateMaterial(Assimp.Material assimpMaterial, string baseDirectory = null)
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
            else
            {
                material.SetVec3(
                    OpenTKTutorialConstant.Material.AmbientColorName,
                    new Vector3(Vector3.Zero)
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
            else
            {
                material.SetVec3(
                    OpenTKTutorialConstant.Material.DiffuseColorName,
                    new Vector3(Vector3.Zero)
                );
            }

            if (assimpMaterial.HasColorSpecular)
            {
                material.SetVec3(
                    OpenTKTutorialConstant.Material.SpecularColorName,
                    new Vector3(
                        assimpMaterial.ColorSpecular.R,
                        assimpMaterial.ColorSpecular.G,
                        assimpMaterial.ColorSpecular.B
                    )
                );
            }
            else
            {
                material.SetVec3(
                    OpenTKTutorialConstant.Material.SpecularColorName,
                    new Vector3(Vector3.Zero)
                );
            }

            material.SetVec3(
                    OpenTKTutorialConstant.Material.AmbientColorName,
                    new Vector3(Vector3.Zero)
            );

            // material.SetVec3(
            //     OpenTKTutorialConstant.Material.DiffuseColorName,
            //     new Vector3(Vector3.Zero)
            // );

            material.SetVec3(
                OpenTKTutorialConstant.Material.SpecularColorName,
                new Vector3(Vector3.Zero)
            );

            if (assimpMaterial.HasShininess)
            {
                material.SetFloat(
                    OpenTKTutorialConstant.Material.SpecularShininessName,
                    assimpMaterial.Shininess
                );
            }

            if (assimpMaterial.HasTextureDiffuse)
            {
                var fileName = System.IO.Path.GetFileNameWithoutExtension(assimpMaterial.TextureDiffuse.FilePath);
                var fileExtension = System.IO.Path.GetExtension(assimpMaterial.TextureDiffuse.FilePath);
                var filePath = fileExtension == ".psd" ? fileName + ".png" : fileName + fileExtension;

                var texture = new Texture2D(baseDirectory + "/" + filePath);

                switch (assimpMaterial.TextureDiffuse.TextureIndex)
                {
                    case 0:
                        material.SetTexture(OpenTK.Graphics.OpenGL4.TextureUnit.Texture0, texture);
                        break;
                    case 1:
                        material.SetTexture(OpenTK.Graphics.OpenGL4.TextureUnit.Texture1, texture);
                        break;
                    case 2:
                        material.SetTexture(OpenTK.Graphics.OpenGL4.TextureUnit.Texture2, texture);
                        break;
                    case 3:
                        material.SetTexture(OpenTK.Graphics.OpenGL4.TextureUnit.Texture3, texture);
                        break;

                }
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