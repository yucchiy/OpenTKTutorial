#version 330 core

struct Material
{
    vec3 AmbientColor;
    vec3 DiffuseColor;
    vec3 SpecularColor;
    float SpecularShininess;
};

struct Camera
{
    vec3 PositionWorld;
};

struct Light
{
    vec3 PositionWorld;
    vec3 Color;
};

uniform Material material;
uniform Camera camera;
uniform Light light;

in vec3 VertexPositionWorld;
in vec3 VertexNormalWorld;

out vec4 FragColor;

void main()
{
    vec3 ambient = material.AmbientColor;

    vec3 lightDirectionWorld = normalize(light.PositionWorld - VertexPositionWorld);
    vec3 diffuse = max(dot(VertexNormalWorld, lightDirectionWorld), 0.0) * material.DiffuseColor;

    vec3 viewDirectionWorld = normalize(camera.PositionWorld - VertexPositionWorld);
    vec3 reflectDirectionWorld = reflect(-lightDirectionWorld, VertexNormalWorld);
    vec3 specular = pow(max(dot(viewDirectionWorld, reflectDirectionWorld), 0.0), material.SpecularShininess) * material.SpecularColor;

    vec3 resultColor = light.Color * (ambient + diffuse + specular);

    FragColor = vec4(resultColor, 1.0);
}