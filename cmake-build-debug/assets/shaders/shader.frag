#version 330 core

in vec2 TexCoord;
in vec3 FragPos;
in vec3 Normal;

out vec4 FragColor;

uniform sampler2D texture1;
uniform vec3 viewPos;

uniform bool noLighting, useTexture;
uniform bool useAmbient, useDirectional, usePoint, useSpot;

uniform vec3 ambientColor;
uniform vec3 dirLightDirection, dirLightColor;
uniform vec3 pointLightPosition, pointLightColor;
uniform vec3 spotLightPosition, spotLightDirection;
uniform float spotCutOff, spotOuterCutOff;
uniform vec3 spotLightColor;

void main()
{
    if (noLighting) {
        FragColor = useTexture ? texture(texture1, TexCoord) : vec4(0.0, 0.4, 0.0, 1.0);
        return;
    }

    vec3 norm = normalize(Normal);
    vec3 result = useAmbient ? ambientColor : vec3(0.0);

    if (useDirectional) {
        vec3 lightDir = normalize(-dirLightDirection);
        result += max(dot(norm, lightDir), 0.0) * dirLightColor;
    }

    if (usePoint) {
        vec3 lightDir = normalize(pointLightPosition - FragPos);
        float attenuation = 1.0 / pow(length(pointLightPosition - FragPos), 2);
        result += max(dot(norm, lightDir), 0.0) * pointLightColor * attenuation;
    }

    if (useSpot) {
        vec3 lightDir = normalize(spotLightPosition - FragPos);
        float theta = dot(lightDir, normalize(-spotLightDirection));
        float intensity = clamp((theta - spotOuterCutOff) / (spotCutOff - spotOuterCutOff), 0.0, 1.0);
        result += max(dot(norm, lightDir), 0.0) * spotLightColor * intensity;
    }

    vec4 baseColor = useTexture ? texture(texture1, TexCoord) : vec4(0.0, 0.3, 0.0, 1.0);
    FragColor = vec4(result * 2, 1.0) * baseColor;
}