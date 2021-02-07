#version 330 core

struct Material
{
    vec3 AmbientColor;
    vec3 DiffuseColor;
    vec3 SpecularColor;
    sampler2D DiffuseTexture;
    float SpecularShininess;
};

struct Camera
{
    vec3 PositionWorld;
};

struct Light
{
    int Type;
    vec3 PositionWorld;
    vec3 Color;
    vec3 DirectionWorld;
    float Constant;
    float Linear;
    float Quadratic;
    float CutOff;
    float OuterCutOff;
};

uniform Material material;
uniform Camera camera;

uniform Light directionalLight;
#define NUMBER_OF_POINT_LIGHT 3
uniform Light pointLights[NUMBER_OF_POINT_LIGHT];

in vec3 VertexPositionWorld;
in vec3 VertexNormalWorld;
in vec2 VertexTextureCoordinate;

out vec4 FragColor;

vec3 CalculateDirectionalLight(Light directional)
{
    vec3 ambient = material.AmbientColor * texture(material.DiffuseTexture, VertexTextureCoordinate).xyz;

    vec3 lightDirectionWorld = normalize(-directional.DirectionWorld);
    vec3 diffuse = material.DiffuseColor * max(dot(VertexNormalWorld, lightDirectionWorld), 0.0) * texture(material.DiffuseTexture, VertexTextureCoordinate).xyz;

    vec3 viewDirectionWorld = normalize(camera.PositionWorld - VertexPositionWorld);
    vec3 reflectDirectionWorld = reflect(-lightDirectionWorld, VertexNormalWorld);
    vec3 specular = material.SpecularColor * pow(max(dot(viewDirectionWorld, reflectDirectionWorld), 0.0), max(2, material.SpecularShininess));

    return directional.Color * (ambient + diffuse + specular);
}

vec3 CalculatePointLight(Light point)
{
    vec3 ambient = material.AmbientColor * texture(material.DiffuseTexture, VertexTextureCoordinate).xyz;

    vec3 lightDirectionWorld = normalize(point.PositionWorld - VertexPositionWorld);
    vec3 diffuse = material.DiffuseColor * max(dot(VertexNormalWorld, lightDirectionWorld), 0.0) * texture(material.DiffuseTexture, VertexTextureCoordinate).xyz;

    vec3 viewDirectionWorld = normalize(camera.PositionWorld - VertexPositionWorld);
    vec3 reflectDirectionWorld = reflect(-lightDirectionWorld, VertexNormalWorld);
    vec3 specular = material.SpecularColor * pow(max(dot(viewDirectionWorld, reflectDirectionWorld), 0.0), max(2, material.SpecularShininess));

    float distance = length(point.PositionWorld - VertexPositionWorld);
    float attenuation = 1.0 / (point.Constant + point.Linear * distance + point.Quadratic * (distance * distance));
    ambient = ambient * attenuation;
    diffuse = diffuse * attenuation;
    specular = specular * attenuation;

    return point.Color * (ambient + diffuse + specular);
}

void main()
{
    vec3 resultColor = CalculateDirectionalLight(directionalLight);
    for (int i = 0; i < NUMBER_OF_POINT_LIGHT; ++i)
    {
        resultColor += CalculatePointLight(pointLights[i]);
    }

    FragColor = vec4(resultColor, 1.0);
}