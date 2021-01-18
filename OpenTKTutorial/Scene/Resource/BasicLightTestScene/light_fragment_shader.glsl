#version 330 core

uniform vec3 ObjectColor;

uniform float AmbientStrength;
uniform vec3 AmbientColor;

uniform vec3 LightPositionWorld;
uniform vec3 LightColor;

uniform float SpecularStrength;
uniform float SpecularShininess;
uniform vec3 EyePositionWorld;

in vec3 VertexPositionWorld;
in vec3 VertexNormalWorld;

out vec4 FragColor;

void main()
{
    vec3 ambient = LightColor * AmbientColor * AmbientStrength;

    vec3 lightDirectionWorld = normalize(LightPositionWorld - VertexPositionWorld);
    vec3 diffuse = LightColor * max(dot(VertexNormalWorld, lightDirectionWorld), 0.0);

    vec3 viewDirectionWorld = normalize(EyePositionWorld - VertexPositionWorld);
    vec3 reflectDirectionWorld = reflect(-lightDirectionWorld, VertexNormalWorld);
    vec3 specular = LightColor * pow(max(dot(viewDirectionWorld, reflectDirectionWorld), 0.0), SpecularShininess) * SpecularStrength;

    vec3 resultColor = ObjectColor * (ambient + diffuse + specular);

    FragColor = vec4(resultColor, 1.0);
}