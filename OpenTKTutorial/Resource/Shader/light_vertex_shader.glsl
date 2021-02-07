#version 330 core

layout(location = 0) in vec3 InPosition;

struct MVP
{
    mat4 ModelMatrix;
    mat4 ViewMatrix;
    mat4 ProjectionMatrix;
};

uniform MVP mvp;

out vec3 VertexPositionWorld;

void main()
{
    gl_Position = mvp.ProjectionMatrix * mvp.ViewMatrix * mvp.ModelMatrix * vec4(InPosition, 1.0);
    VertexPositionWorld = (mvp.ModelMatrix * vec4(InPosition, 1.0)).xyz;
}