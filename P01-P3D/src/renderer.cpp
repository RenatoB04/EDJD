#include "renderer.hpp"
#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <glm/gtc/type_ptr.hpp>

// Função que inicializa a geometria da mesa (VAO, VBO e EBO)
void setupMesa(GLuint &vao, GLuint &vbo, GLuint &ebo) {
    // Vértices da mesa: posição (x, y, z) e cor (r, g, b)
    float vertices[] = {
        // Base inferior da mesa
        -1.0f, 0.0f, -2.0f,  0.0f, 0.4f, 0.0f,
         1.0f, 0.0f, -2.0f,  0.0f, 0.4f, 0.0f,
         1.0f, 0.0f,  2.0f,  0.0f, 0.4f, 0.0f,
        -1.0f, 0.0f,  2.0f,  0.0f, 0.4f, 0.0f,
        // Parte superior da mesa
        -1.0f, 0.3f, -2.0f,  0.0f, 0.5f, 0.0f,
         1.0f, 0.3f, -2.0f,  0.0f, 0.5f, 0.0f,
         1.0f, 0.3f,  2.0f,  0.0f, 0.5f, 0.0f,
        -1.0f, 0.3f,  2.0f,  0.0f, 0.5f, 0.0f
    };

    // Índices para desenhar os triângulos da mesa com EBO
    unsigned int indices[] = {
        0, 1, 2, 2, 3, 0,     // base inferior
        4, 5, 6, 6, 7, 4,     // topo
        3, 2, 6, 6, 7, 3,     // frente
        0, 1, 5, 5, 4, 0,     // trás
        0, 3, 7, 7, 4, 0,     // esquerda
        1, 2, 6, 6, 5, 1      // direita
    };

    // Geração dos buffers
    glGenVertexArrays(1, &vao);
    glGenBuffers(1, &vbo);
    glGenBuffers(1, &ebo);

    // Associação do VAO e carregamento dos dados
    glBindVertexArray(vao);

    glBindBuffer(GL_ARRAY_BUFFER, vbo);
    glBufferData(GL_ARRAY_BUFFER, sizeof(vertices), vertices, GL_STATIC_DRAW);

    glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, ebo);
    glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(indices), indices, GL_STATIC_DRAW);

    // Atributo 0: posição (vec3)
    glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 6 * sizeof(float), (void*)0);
    glEnableVertexAttribArray(0);

    // Atributo 1: cor (vec3)
    glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 6 * sizeof(float), (void*)(3 * sizeof(float)));
    glEnableVertexAttribArray(1);
}

// Função que desenha a mesa no ecrã com shaders e iluminação
void drawMesa(GLuint shader, GLuint vao, const glm::mat4& view, const glm::mat4& proj, const LightState& lights, bool noLighting, float angulo) {
    // Criação da matriz modelo com rotação e escala
    glm::mat4 model = glm::scale(
        glm::rotate(glm::mat4(1.0f), angulo, glm::vec3(0, 1, 0)),
        glm::vec3(4.0f, 3.0f, 5.0f)
    );

    // Ativação do shader e envio das matrizes para os uniformes
    glUseProgram(shader);
    glUniformMatrix4fv(glGetUniformLocation(shader, "model"), 1, GL_FALSE, glm::value_ptr(model));
    glUniformMatrix4fv(glGetUniformLocation(shader, "view"), 1, GL_FALSE, glm::value_ptr(view));
    glUniformMatrix4fv(glGetUniformLocation(shader, "projection"), 1, GL_FALSE, glm::value_ptr(proj));

    // Define a posição da câmara (viewPos) para efeitos de iluminação
    glUniform3f(glGetUniformLocation(shader, "viewPos"), 0.0f, 2.0f, 5.0f);

    // Define se a iluminação está desativada
    glUniform1i(glGetUniformLocation(shader, "noLighting"), noLighting);

    // Parâmetros de iluminação ambiente
    glUniform1i(glGetUniformLocation(shader, "useAmbient"), lights.useAmbient);
    glUniform3f(glGetUniformLocation(shader, "ambientColor"), 0.2f, 0.2f, 0.2f);

    // Luz direcional
    glUniform1i(glGetUniformLocation(shader, "useDirectional"), lights.useDirectional);
    glUniform3f(glGetUniformLocation(shader, "dirLightDirection"), -1.0f, -1.0f, -1.0f);
    glUniform3f(glGetUniformLocation(shader, "dirLightColor"), 0.8f, 0.8f, 0.8f);

    // Luz pontual
    glUniform1i(glGetUniformLocation(shader, "usePoint"), lights.usePoint);
    glUniform3f(glGetUniformLocation(shader, "pointLightPosition"), 0.0f, 3.0f, 0.0f);
    glUniform3f(glGetUniformLocation(shader, "pointLightColor"), 1.0f, 1.0f, 1.0f);

    // Luz spot (focada)
    glUniform1i(glGetUniformLocation(shader, "useSpot"), lights.useSpot);
    glUniform3f(glGetUniformLocation(shader, "spotLightPosition"), 0.0f, 3.0f, 3.0f);
    glUniform3f(glGetUniformLocation(shader, "spotLightDirection"), 0.0f, -1.0f, -1.0f);
    glUniform1f(glGetUniformLocation(shader, "spotCutOff"), glm::cos(glm::radians(20.0f)));
    glUniform1f(glGetUniformLocation(shader, "spotOuterCutOff"), glm::cos(glm::radians(30.0f)));
    glUniform3f(glGetUniformLocation(shader, "spotLightColor"), 1.0f, 1.0f, 0.8f);

    // Desenho da geometria usando o VAO e EBO definidos
    glBindVertexArray(vao);
    glDrawElements(GL_TRIANGLES, 36, GL_UNSIGNED_INT, 0);
}