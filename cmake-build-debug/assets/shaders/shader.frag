#version 330 core

in vec2 TexCoord;     // Coordenadas da textura interpoladas para este fragmento
in vec3 FragPos;      // Posição do fragmento no espaço do mundo
in vec3 Normal;       // Normal interpolada do fragmento

out vec4 FragColor;   // Cor final do fragmento

// Uniformes para textura e posição da câmara
uniform sampler2D texture1;
uniform vec3 viewPos;

// Flags para controlar o tipo de iluminação e textura
uniform bool noLighting, useTexture;
uniform bool useAmbient, useDirectional, usePoint, useSpot;

// Parâmetros para luz ambiente
uniform vec3 ambientColor;

// Parâmetros para luz direcional
uniform vec3 dirLightDirection, dirLightColor;

// Parâmetros para luz pontual
uniform vec3 pointLightPosition, pointLightColor;

// Parâmetros para luz spot (focada)
uniform vec3 spotLightPosition, spotLightDirection;
uniform float spotCutOff, spotOuterCutOff;
uniform vec3 spotLightColor;

void main()
{
    // Se a iluminação estiver desligada, usa apenas a textura ou uma cor base
    if (noLighting) {
        FragColor = useTexture ? texture(texture1, TexCoord) : vec4(0.0, 0.4, 0.0, 1.0);
        return;
    }

    // Normalização da normal para garantir direcção unitária
    vec3 norm = normalize(Normal);

    // Inicialização da cor com o contributo da luz ambiente (se estiver activa)
    vec3 result = useAmbient ? ambientColor : vec3(0.0);

    // Contributo da luz direcional (ex: sol)
    if (useDirectional) {
        vec3 lightDir = normalize(-dirLightDirection); // Direcção oposta à da luz
        result += max(dot(norm, lightDir), 0.0) * dirLightColor;
        // A intensidade depende do ângulo entre a normal e a luz
    }

    // Contributo da luz pontual (ex: lâmpada)
    if (usePoint) {
        vec3 lightDir = normalize(pointLightPosition - FragPos);
        float attenuation = 1.0 / pow(length(pointLightPosition - FragPos), 2);
        // Atenuação com base na distância à fonte de luz
        result += max(dot(norm, lightDir), 0.0) * pointLightColor * attenuation;
    }

    // Contributo da luz spot (ex: lanterna)
    if (useSpot) {
        vec3 lightDir = normalize(spotLightPosition - FragPos);
        float theta = dot(lightDir, normalize(-spotLightDirection));
        float intensity = clamp((theta - spotOuterCutOff) / (spotCutOff - spotOuterCutOff), 0.0, 1.0);
        // Intensidade decresce fora do cone do spotlight
        result += max(dot(norm, lightDir), 0.0) * spotLightColor * intensity;
    }

    // Aplica textura ou cor base ao fragmento
    vec4 baseColor = useTexture ? texture(texture1, TexCoord) : vec4(0.0, 0.3, 0.0, 1.0);

    // Combina a iluminação com a cor base
    FragColor = vec4(result * 2, 1.0) * baseColor;
    // Multiplicação por 2 para intensificar o efeito da luz
}