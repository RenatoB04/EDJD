#pragma once

#include <GL/glew.h>
#include <glm/glm.hpp>

// Estrutura que define o estado das luzes na cena
struct LightState {
    bool useAmbient = true;      // Indica se a luz ambiente está ativa
    bool useDirectional = true;  // Indica se a luz direcional está ativa
    bool usePoint = true;        // Indica se a luz pontual está ativa
    bool useSpot = true;         // Indica se a luz cónica (spot) está ativa
};

// Função para configurar a geometria e buffers da mesa
void setupMesa(GLuint &vao, GLuint &vbo, GLuint &ebo);

// Função para desenhar a mesa na cena com rotação global
void drawMesa(GLuint shaderProgram, GLuint vao, const glm::mat4& view, const glm::mat4& projection, const LightState& lights, bool noLightingMode, float anguloCena);