#version 330 core

// Modo sem iluminação: se verdadeiro, ignora todas as luzes
uniform bool noLighting;

// Coordenadas de textura, posição do fragmento e normal
in vec2 TexCoord;
in vec3 FragPos;
in vec3 Normal;

// Cor final do fragmento
out vec4 FragColor;

// Textura e indicador de uso de textura
uniform sampler2D texture1;
uniform bool useTexture;

// Luz ambiente: ativação e cor
uniform bool useAmbient;
uniform vec3 ambientColor;

// Luz direcional: ativação, direção e cor
uniform bool useDirectional;
uniform vec3 dirLightDirection;
uniform vec3 dirLightColor;

// Luz pontual: ativação, posição e cor
uniform bool usePoint;
uniform vec3 pointLightPosition;
uniform vec3 pointLightColor;

// Luz cónica (spot): ativação, posição, direção, ângulos e cor
uniform bool useSpot;
uniform vec3 spotLightPosition;
uniform vec3 spotLightDirection;
uniform float spotCutOff;
uniform float spotOuterCutOff;
uniform vec3 spotLightColor;

// Posição da câmara
uniform vec3 viewPos;

void main()
{
    // Normaliza a normal do fragmento
    vec3 norm = normalize(Normal);
    // Calcula a direção da vista
    vec3 viewDir = normalize(viewPos - FragPos);
    // Inicializa o resultado da iluminação
    vec3 result = vec3(0.0);

    // Se o modo sem iluminação estiver ativo, usa apenas a cor base ou textura
    if (noLighting) {
        FragColor = useTexture ? texture(texture1, TexCoord) : vec4(0.0, 0.4, 0.0, 1.0);
        return;
    }

    // Luz ambiente: adiciona a cor ambiente ao resultado
    if (useAmbient) {
        result += ambientColor;
    }

    // Luz direcional: calcula a componente difusa e adiciona ao resultado
    if (useDirectional) {
        vec3 lightDir = normalize(-dirLightDirection);
        float diff = max(dot(norm, lightDir), 0.0);
        vec3 diffuse = diff * dirLightColor;
        result += diffuse;
    }

    // Luz pontual: calcula a componente difusa com atenuação e adiciona ao resultado
    if (usePoint) {
        vec3 lightDir = normalize(pointLightPosition - FragPos);
        float diff = max(dot(norm, lightDir), 0.0);
        float distance = length(pointLightPosition - FragPos);
        float attenuation = 1.0 / (distance * distance);
        vec3 diffuse = diff * pointLightColor * attenuation;
        result += diffuse;
    }

    // Luz cónica (spot): calcula a componente difusa com intensidade baseada no ângulo e adiciona ao resultado
    if (useSpot) {
        vec3 lightDir = normalize(spotLightPosition - FragPos);
        float theta = dot(lightDir, normalize(-spotLightDirection));
        float epsilon = spotCutOff - spotOuterCutOff;
        float intensity = clamp((theta - spotOuterCutOff) / epsilon, 0.0, 1.0);
        float diff = max(dot(norm, lightDir), 0.0);
        vec3 diffuse = diff * spotLightColor * intensity;
        result += diffuse;
    }

    // Escolhe a cor base (textura ou cor sólida)
    vec4 baseColor = useTexture ? texture(texture1, TexCoord) : vec4(0.0, 0.3, 0.0, 1.0);
    // Combina o resultado da iluminação com a cor base
    FragColor = vec4(result * 2, 1.0) * baseColor;
}