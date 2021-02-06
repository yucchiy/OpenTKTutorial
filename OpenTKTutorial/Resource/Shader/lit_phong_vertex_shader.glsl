#version 330 core

layout(location = 0) in vec3 InPosition;
layout(location = 1) in vec3 InNormal;
layout(location = 2) in vec2 InTextureCoordinate;

struct MVP
{
    mat4 ModelMatrix;
    mat4 ViewMatrix;
    mat4 ProjectionMatrix;
};

uniform MVP mvp;

out vec3 VertexPositionWorld;
out vec3 VertexNormalWorld;
out vec2 VertexTextureCoordinate;

void main()
{
    gl_Position = mvp.ProjectionMatrix * mvp.ViewMatrix * mvp.ModelMatrix * vec4(InPosition, 1.0);
    VertexPositionWorld = (mvp.ModelMatrix * vec4(InPosition, 1.0)).xyz;
    VertexNormalWorld = mat3(transpose(inverse(mvp.ModelMatrix))) * InNormal;
    VertexTextureCoordinate = InTextureCoordinate;
}