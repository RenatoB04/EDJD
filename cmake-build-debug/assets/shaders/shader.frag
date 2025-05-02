#version 330 core

uniform bool noLighting;

in vec2 TexCoord;
in vec3 FragPos;
in vec3 Normal;

out vec4 FragColor;

uniform sampler2D texture1;
uniform bool useTexture;

uniform bool useAmbient;
uniform vec3 ambientColor;

uniform bool useDirectional;
uniform vec3 dirLightDirection;
uniform vec3 dirLightColor;

uniform bool usePoint;
uniform vec3 pointLightPosition;
uniform vec3 pointLightColor;

uniform bool useSpot;
uniform vec3 spotLightPosition;
uniform vec3 spotLightDirection;
uniform float spotCutOff;
uniform float spotOuterCutOff;
uniform vec3 spotLightColor;

uniform vec3 viewPos;

void main()
{
    vec3 norm = normalize(Normal);
    vec3 viewDir = normalize(viewPos - FragPos);
    vec3 result = vec3(0.0);

    if (noLighting) {
        FragColor = useTexture ? texture(texture1, TexCoord) : vec4(0.0, 0.4, 0.0, 1.0);
        return;
    }

    if (useAmbient) {
        result += ambientColor;
    }

    if (useDirectional) {
        vec3 lightDir = normalize(-dirLightDirection);
        float diff = max(dot(norm, lightDir), 0.0);
        vec3 diffuse = diff * dirLightColor;
        result += diffuse;
    }

    if (usePoint) {
        vec3 lightDir = normalize(pointLightPosition - FragPos);
        float diff = max(dot(norm, lightDir), 0.0);
        float distance = length(pointLightPosition - FragPos);
        float attenuation = 1.0 / (distance * distance);
        vec3 diffuse = diff * pointLightColor * attenuation;
        result += diffuse;
    }

    if (useSpot) {
        vec3 lightDir = normalize(spotLightPosition - FragPos);
        float theta = dot(lightDir, normalize(-spotLightDirection));
        float epsilon = spotCutOff - spotOuterCutOff;
        float intensity = clamp((theta - spotOuterCutOff) / epsilon, 0.0, 1.0);
        float diff = max(dot(norm, lightDir), 0.0);
        vec3 diffuse = diff * spotLightColor * intensity;
        result += diffuse;
    }

    vec4 baseColor = useTexture ? texture(texture1, TexCoord) : vec4(0.0, 0.3, 0.0, 1.0);
    FragColor = vec4(result * 2, 1.0) * baseColor;
}
