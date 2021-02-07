#version 330 core

struct Material
{
    vec3 AmbientColor;
};

uniform Material material;

in vec3 VertexPositionWorld;

out vec4 FragColor;

void main()
{
    vec3 ambient = material.AmbientColor;
    vec3 resultColor = ambient;

    FragColor = vec4(resultColor, 1.0);
}