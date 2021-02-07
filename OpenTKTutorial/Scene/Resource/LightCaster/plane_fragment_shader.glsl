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
uniform Light light;

in vec3 VertexPositionWorld;
in vec3 VertexNormalWorld;
in vec2 VertexTextureCoordinate;

out vec4 FragColor;

void main()
{
    vec3 ambient = material.AmbientColor * texture(material.DiffuseTexture, VertexTextureCoordinate).xyz;

    vec3 diffuse = vec3(0.0, 0.0, 0.0);
    vec3 specular = vec3(0.0, 0.0, 0.0);
    if (light.Type == 1)
    {
        vec3 lightDirectionWorld = normalize(-light.DirectionWorld);

        // calc directional light
        diffuse += material.DiffuseColor * max(dot(VertexNormalWorld, lightDirectionWorld), 0.0) * texture(material.DiffuseTexture, VertexTextureCoordinate).xyz;

        vec3 viewDirectionWorld = normalize(camera.PositionWorld - VertexPositionWorld);
        vec3 reflectDirectionWorld = reflect(-lightDirectionWorld, VertexNormalWorld);
        specular += material.SpecularColor * pow(max(dot(viewDirectionWorld, reflectDirectionWorld), 0.0), max(2, material.SpecularShininess));
    }
    else if (light.Type == 2)
    {
        float distance = length(light.PositionWorld - VertexPositionWorld);
        float attenuation = 1.0 / (light.Constant + light.Linear * distance + light.Quadratic * (distance * distance));

        vec3 lightDirectionWorld = normalize(light.PositionWorld - VertexPositionWorld);
        diffuse += material.DiffuseColor * max(dot(VertexNormalWorld, lightDirectionWorld), 0.0) * texture(material.DiffuseTexture, VertexTextureCoordinate).xyz;

        vec3 viewDirectionWorld = normalize(camera.PositionWorld - VertexPositionWorld);
        vec3 reflectDirectionWorld = reflect(-lightDirectionWorld, VertexNormalWorld);
        specular += material.SpecularColor * pow(max(dot(viewDirectionWorld, reflectDirectionWorld), 0.0), max(2, material.SpecularShininess));

        ambient = ambient * attenuation;
        diffuse = diffuse * attenuation;
        specular = specular * attenuation;
    }
    else if (light.Type == 3)
    {
        vec3 lightDirectionWorld = normalize(light.PositionWorld - VertexPositionWorld);
        diffuse += material.DiffuseColor * max(dot(VertexNormalWorld, lightDirectionWorld), 0.0) * texture(material.DiffuseTexture, VertexTextureCoordinate).xyz;

        vec3 viewDirectionWorld = normalize(camera.PositionWorld - VertexPositionWorld);
        vec3 reflectDirectionWorld = reflect(-lightDirectionWorld, VertexNormalWorld);
        specular += material.SpecularColor * pow(max(dot(viewDirectionWorld, reflectDirectionWorld), 0.0), max(2, material.SpecularShininess));

        float theta = dot(normalize(-light.DirectionWorld), lightDirectionWorld);
        float epsilon = light.CutOff - light.OuterCutOff;
        float intensity = clamp((theta - light.OuterCutOff) / epsilon, 0.0, 1.0);

        diffuse = diffuse * intensity;
        specular = specular * intensity;

        float distance = length(light.PositionWorld - VertexPositionWorld);
        float attenuation = 1.0 / (light.Constant + light.Linear * distance + light.Quadratic * (distance * distance));

        ambient = ambient * attenuation;
        diffuse = diffuse * attenuation;
        specular = specular * attenuation;
    }
    else
    {
        vec3 lightDirectionWorld = normalize(light.PositionWorld - VertexPositionWorld);
        diffuse += material.DiffuseColor * max(dot(VertexNormalWorld, lightDirectionWorld), 0.0) * texture(material.DiffuseTexture, VertexTextureCoordinate).xyz;

        vec3 viewDirectionWorld = normalize(camera.PositionWorld - VertexPositionWorld);
        vec3 reflectDirectionWorld = reflect(-lightDirectionWorld, VertexNormalWorld);
        specular += material.SpecularColor * pow(max(dot(viewDirectionWorld, reflectDirectionWorld), 0.0), max(2, material.SpecularShininess));
    }

    vec3 resultColor = light.Color * (ambient + diffuse + specular);

    FragColor = vec4(resultColor, 1.0);
}