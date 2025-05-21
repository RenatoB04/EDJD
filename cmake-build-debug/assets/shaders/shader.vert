#version 330 core

// Atributos dos vértices: posição, normal e coordenadas de textura
// O layout indica a localização do atributo nos buffers enviados para a GPU
layout (location = 0) in vec3 aPos;        // Posição do vértice
layout (location = 1) in vec3 aNormal;     // Normal do vértice
layout (location = 2) in vec2 aTexCoord;   // Coordenadas de textura

// Variáveis de saída para o fragment shader
out vec2 TexCoord;    // Coordenadas de textura
out vec3 FragPos;     // Posição do fragmento (no espaço do mundo)
out vec3 Normal;      // Normal do fragmento (no espaço do mundo)

// Matrizes de transformação
uniform mat4 model;      // Matriz de modelo (transformação do objeto)
uniform mat4 view;       // Matriz de vista (posição/orientação da câmara)
uniform mat4 projection; // Matriz de projeção (perspetiva/ortográfica)

void main()
{
    // Calcula a posição final do vértice no espaço de clip
    gl_Position = projection * view * model * vec4(aPos, 1.0);

    // Calcula a posição do fragmento no espaço do mundo
    FragPos = vec3(model * vec4(aPos, 1.0));

    // Transforma a normal do vértice para o espaço do mundo (corrige para não escalar)
    // Usa a matriz normal, que é a inversa transposta da matriz de modelo 3x3
    Normal = mat3(transpose(inverse(model))) * aNormal;

    // Passa as coordenadas de textura para o fragment shader
    TexCoord = aTexCoord;
}