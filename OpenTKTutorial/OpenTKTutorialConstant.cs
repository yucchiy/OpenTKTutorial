namespace OpenTKTutorial
{
    public static class OpenTKTutorialConstant
    {
        public static class Material
        {
            public static readonly string AmbientColorName      = "material.AmbientColor";
            public static readonly string DiffuseColorName      = "material.DiffuseColor";
            public static readonly string SpecularColorName     = "material.SpecularColor";
            public static readonly string SpecularShininessName = "material.SpecularShininess";
            public static readonly string DiffuseTextureName    = "material.DiffuseTexture";
            public static readonly string SpecularTextureName   = "material.SpecularTexture";

            public static readonly string CameraPositionName    = "camera.PositionWorld";

            public static readonly string LightPositionName = "light.PositionWorld";
            public static readonly string LightColorName    = "light.Color";
            public static readonly string LightTypeName     = "light.Type";

            public static readonly string LightDirectionalDirectionWorld = "light.DirectionWorld";
            public static readonly string LightPointConstant  = "light.Constant";
            public static readonly string LightPointLinear    = "light.Linear";
            public static readonly string LightPointQuadratic = "light.Quadratic";
            public static readonly string LightSpotCutOff      = "light.CutOff";
            public static readonly string LightSpotOuterCutOff = "light.OuterCutOff";

            public static readonly string LightDirectionalLightColorName = "directionalLight.Color";
            public static readonly string LightDirectionalLightDirectionWorldName = "directionalLight.DirectionWorld";

            public static string LightPointLightNColorName(int index) => $"pointLights[{index}].Color";
            public static string LightPointLightNPositionWorld(int index) => $"pointLights[{index}].PositionWorld";
            public static string LightPointLightNConstantName(int index) => $"pointLights[{index}].Constant";
            public static string LightPointLightNLinearName(int index) => $"pointLights[{index}].Linear";
            public static string LightPointLightNQuadratic(int index) => $"pointLights[{index}].Quadratic";

            public static readonly string MVPModelMatrixName      = "mvp.ModelMatrix";
            public static readonly string MVPViewMatrixName       = "mvp.ViewMatrix";
            public static readonly string MVPProjectionMatrixName = "mvp.ProjectionMatrix";
        }
    }
}