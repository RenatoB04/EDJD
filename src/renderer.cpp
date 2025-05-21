#include "renderer.hpp"
#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <glm/gtc/type_ptr.hpp>

// Função para configurar a geometria e buffers da mesa
void setupMesa(GLuint &vao, GLuint &vbo, GLuint &ebo) {
    // Coordenadas dos vértices da mesa: posição (x,y,z) e cor (r,g,b)
    float vertices[] = {
        -1.0f, 0.0f, -2.0f,  0.0f, 0.4f, 0.0f, // Vértice 0
         1.0f, 0.0f, -2.0f,  0.0f, 0.4f, 0.0f, // Vértice 1
         1.0f, 0.0f,  2.0f,  0.0f, 0.4f, 0.0f, // Vértice 2
        -1.0f, 0.0f,  2.0f,  0.0f, 0.4f, 0.0f, // Vértice 3

        -1.0f, 0.3f, -2.0f,  0.0f, 0.5f, 0.0f, // Vértice 4
         1.0f, 0.3f, -2.0f,  0.0f, 0.5f, 0.0f, // Vértice 5
         1.0f, 0.3f,  2.0f,  0.0f, 0.5f, 0.0f, // Vértice 6
        -1.0f, 0.3f,  2.0f,  0.0f, 0.5f, 0.0f  // Vértice 7
    };

    // Índices dos triângulos que formam as faces da mesa
    unsigned int indices[] = {
        0, 1, 2, 2, 3, 0,    // Face inferior
        4, 5, 6, 6, 7, 4,    // Face superior
        3, 2, 6, 6, 7, 3,    // Face frontal
        0, 1, 5, 5, 4, 0,    // Face traseira
        0, 3, 7, 7, 4, 0,    // Face lateral esquerda
        1, 2, 6, 6, 5, 1     // Face lateral direita
    };

    // Gera e associa os buffers de vértices, índices e vertex array object (VAO)
    glGenVertexArrays(1, &vao);
    glGenBuffers(1, &vbo);
    glGenBuffers(1, &ebo);

    glBindVertexArray(vao);

    // Copia os vértices para o buffer de vértices
    glBindBuffer(GL_ARRAY_BUFFER, vbo);
    glBufferData(GL_ARRAY_BUFFER, sizeof(vertices), vertices, GL_STATIC_DRAW);

    // Copia os índices para o buffer de elementos
    glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, ebo);
    glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(indices), indices, GL_STATIC_DRAW);

    // Define o layout dos dados dos vértices: posição (atributo 0)
    glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 6 * sizeof(float), (void*)0);
    glEnableVertexAttribArray(0);

    // Define o layout dos dados dos vértices: cor (atributo 1)
    glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 6 * sizeof(float), (void*)(3 * sizeof(float)));
    glEnableVertexAttribArray(1);
}

// Função para desenhar a mesa na cena
void drawMesa(GLuint shaderProgram, GLuint vao, const glm::mat4& view, const glm::mat4& projection, const LightState& lights, bool noLightingMode) {
    // Cria a matriz de modelo para a mesa, aplicando uma escala para aumentar o tamanho
    glm::mat4 model = glm::mat4(1.0f);
    model = glm::scale(model, glm::vec3(4.0f, 3.0f, 5.0f));

    // Usa o programa de shader
    glUseProgram(shaderProgram);

    // Obtém as localizações das matrizes nos shaders
    GLint modelLoc = glGetUniformLocation(shaderProgram, "model");
    GLint viewLoc  = glGetUniformLocation(shaderProgram, "view");
    GLint projLoc  = glGetUniformLocation(shaderProgram, "projection");

    // Envia as matrizes para o shader
    glUniformMatrix4fv(modelLoc, 1, GL_FALSE, glm::value_ptr(model));
    glUniformMatrix4fv(viewLoc, 1, GL_FALSE, glm::value_ptr(view));
    glUniformMatrix4fv(projLoc, 1, GL_FALSE, glm::value_ptr(projection));

    // Envia a posição da câmara para o shader
    glUniform3f(glGetUniformLocation(shaderProgram, "viewPos"), 0.0f, 2.0f, 5.0f);

    // Ativa ou desativa o modo sem iluminação
    glUniform1i(glGetUniformLocation(shaderProgram, "noLighting"), noLightingMode);

    // Configura a iluminação ambiente
    glUniform1i(glGetUniformLocation(shaderProgram, "useAmbient"), lights.useAmbient);
    glUniform3f(glGetUniformLocation(shaderProgram, "ambientColor"), 0.2f, 0.2f, 0.2f);

    // Configura a iluminação direcional
    glUniform1i(glGetUniformLocation(shaderProgram, "useDirectional"), lights.useDirectional);
    glUniform3f(glGetUniformLocation(shaderProgram, "dirLightDirection"), -1.0f, -1.0f, -1.0f);
    glUniform3f(glGetUniformLocation(shaderProgram, "dirLightColor"), 0.8f, 0.8f, 0.8f);

    // Configura a iluminação pontual
    glUniform1i(glGetUniformLocation(shaderProgram, "usePoint"), lights.usePoint);
    glUniform3f(glGetUniformLocation(shaderProgram, "pointLightPosition"), 0.0f, 3.0f, 0.0f);
    glUniform3f(glGetUniformLocation(shaderProgram, "pointLightColor"), 1.0f, 1.0f, 1.0f);

    // Configura a iluminação spot
    glUniform1i(glGetUniformLocation(shaderProgram, "useSpot"), lights.useSpot);
    glUniform3f(glGetUniformLocation(shaderProgram, "spotLightPosition"), 0.0f, 3.0f, 3.0f);
    glUniform3f(glGetUniformLocation(shaderProgram, "spotLightDirection"), 0.0f, -1.0f, -1.0f);
    glUniform1f(glGetUniformLocation(shaderProgram, "spotCutOff"), glm::cos(glm::radians(20.0f)));
    glUniform1f(glGetUniformLocation(shaderProgram, "spotOuterCutOff"), glm::cos(glm::radians(30.0f)));
    glUniform3f(glGetUniformLocation(shaderProgram, "spotLightColor"), 1.0f, 1.0f, 0.8f);

    // Desenha a mesa utilizando o VAO e o buffer de índices
    glBindVertexArray(vao);
    glDrawElements(GL_TRIANGLES, 36, GL_UNSIGNED_INT, 0);
}