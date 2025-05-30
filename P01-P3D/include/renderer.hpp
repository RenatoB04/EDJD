#pragma once

#include <GL/glew.h>
#include <glm/glm.hpp>

// Estrutura que define quais tipos de luz estão ativos
struct LightState {
    bool useAmbient = true;      // Luz ambiente (iluminação constante)
    bool useDirectional = true;  // Luz direcional (ex: sol)
    bool usePoint = true;        // Luz pontual (ex: lâmpada)
    bool useSpot = true;         // Luz spot (ex: lanterna)
};

// Inicializa a geometria da mesa (VAO, VBO, EBO)
// Cria buffers e carrega os dados da malha
void setupMesa(GLuint &vao, GLuint &vbo, GLuint &ebo);

// Desenha a mesa usando um shader e o estado atual de iluminação
// Requer as matrizes de view e projection, o estado de luzes, o VAO e um ângulo de rotação
void drawMesa(
    GLuint shader,
    GLuint vao,
    const glm::mat4& view,
    const glm::mat4& proj,
    const LightState& lights,
    bool noLighting,
    float angulo
);