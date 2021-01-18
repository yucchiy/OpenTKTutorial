#version 330 core

layout(location = 0) in vec3 InPosition;
layout(location = 1) in vec3 InNormal;

uniform mat4 ModelMatrix;
uniform mat4 ViewMatrix;
uniform mat4 ProjectionMatrix;

out vec3 VertexPositionWorld;
out vec3 VertexNormalWorld;

void main()
{
    gl_Position = ProjectionMatrix * ViewMatrix * ModelMatrix * vec4(InPosition, 1.0);
    VertexPositionWorld = (ModelMatrix * vec4(InPosition, 1.0)).xyz;
    VertexNormalWorld = mat3(transpose(inverse(ModelMatrix))) * InNormal;
}