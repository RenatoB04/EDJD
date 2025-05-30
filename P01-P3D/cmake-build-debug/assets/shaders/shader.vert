#version 330 core

// Atributos de entrada do vértice
layout (location = 0) in vec3 aPos;        // Posição do vértice
layout (location = 1) in vec3 aNormal;     // Normal do vértice
layout (location = 2) in vec2 aTexCoord;   // Coordenadas da textura

// Variáveis de saída para o fragment shader
out vec2 TexCoord;
out vec3 FragPos;
out vec3 Normal;

// Matrizes de transformação
uniform mat4 model, view, projection;

void main()
{
    // Calcula a posição do vértice no espaço do mundo
    vec4 worldPos = model * vec4(aPos, 1.0);
    FragPos = vec3(worldPos);

    // Transforma a normal para o espaço do mundo, corrigindo distorções
    Normal = mat3(transpose(inverse(model))) * aNormal;
    // É necessário inverter e transpor a matriz modelo quando há escalas não uniformes

    // Passa as coordenadas da textura para o fragment shader
    TexCoord = aTexCoord;

    // Calcula a posição final do vértice no espaço de clip (projectado)
    gl_Position = projection * view * worldPos;
}