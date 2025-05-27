#include "renderer.hpp"
#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <glm/gtc/type_ptr.hpp>

void setupMesa(GLuint &vao, GLuint &vbo, GLuint &ebo) {
    float vertices[] = {
        -1.0f, 0.0f, -2.0f,  0.0f, 0.4f, 0.0f,
         1.0f, 0.0f, -2.0f,  0.0f, 0.4f, 0.0f,
         1.0f, 0.0f,  2.0f,  0.0f, 0.4f, 0.0f,
        -1.0f, 0.0f,  2.0f,  0.0f, 0.4f, 0.0f,
        -1.0f, 0.3f, -2.0f,  0.0f, 0.5f, 0.0f,
         1.0f, 0.3f, -2.0f,  0.0f, 0.5f, 0.0f,
         1.0f, 0.3f,  2.0f,  0.0f, 0.5f, 0.0f,
        -1.0f, 0.3f,  2.0f,  0.0f, 0.5f, 0.0f
    };

    unsigned int indices[] = {
        0, 1, 2, 2, 3, 0,     // base
        4, 5, 6, 6, 7, 4,     // topo
        3, 2, 6, 6, 7, 3,     // frente
        0, 1, 5, 5, 4, 0,     // trás
        0, 3, 7, 7, 4, 0,     // esquerda
        1, 2, 6, 6, 5, 1      // direita
    };

    glGenVertexArrays(1, &vao);
    glGenBuffers(1, &vbo);
    glGenBuffers(1, &ebo);

    glBindVertexArray(vao);

    glBindBuffer(GL_ARRAY_BUFFER, vbo);
    glBufferData(GL_ARRAY_BUFFER, sizeof(vertices), vertices, GL_STATIC_DRAW);

    glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, ebo);
    glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(indices), indices, GL_STATIC_DRAW);

    glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 6 * sizeof(float), (void*)0);
    glEnableVertexAttribArray(0);
    glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 6 * sizeof(float), (void*)(3 * sizeof(float)));
    glEnableVertexAttribArray(1);
}

void drawMesa(GLuint shader, GLuint vao, const glm::mat4& view, const glm::mat4& proj, const LightState& lights, bool noLighting, float angulo) {
    glm::mat4 model = glm::scale(glm::rotate(glm::mat4(1.0f), angulo, glm::vec3(0, 1, 0)), glm::vec3(4.0f, 3.0f, 5.0f));

    glUseProgram(shader);
    glUniformMatrix4fv(glGetUniformLocation(shader, "model"), 1, GL_FALSE, glm::value_ptr(model));
    glUniformMatrix4fv(glGetUniformLocation(shader, "view"), 1, GL_FALSE, glm::value_ptr(view));
    glUniformMatrix4fv(glGetUniformLocation(shader, "projection"), 1, GL_FALSE, glm::value_ptr(proj));

    glUniform3f(glGetUniformLocation(shader, "viewPos"), 0.0f, 2.0f, 5.0f);
    glUniform1i(glGetUniformLocation(shader, "noLighting"), noLighting);

    glUniform1i(glGetUniformLocation(shader, "useAmbient"), lights.useAmbient);
    glUniform3f(glGetUniformLocation(shader, "ambientColor"), 0.2f, 0.2f, 0.2f);

    glUniform1i(glGetUniformLocation(shader, "useDirectional"), lights.useDirectional);
    glUniform3f(glGetUniformLocation(shader, "dirLightDirection"), -1.0f, -1.0f, -1.0f);
    glUniform3f(glGetUniformLocation(shader, "dirLightColor"), 0.8f, 0.8f, 0.8f);

    glUniform1i(glGetUniformLocation(shader, "usePoint"), lights.usePoint);
    glUniform3f(glGetUniformLocation(shader, "pointLightPosition"), 0.0f, 3.0f, 0.0f);
    glUniform3f(glGetUniformLocation(shader, "pointLightColor"), 1.0f, 1.0f, 1.0f);

    glUniform1i(glGetUniformLocation(shader, "useSpot"), lights.useSpot);
    glUniform3f(glGetUniformLocation(shader, "spotLightPosition"), 0.0f, 3.0f, 3.0f);
    glUniform3f(glGetUniformLocation(shader, "spotLightDirection"), 0.0f, -1.0f, -1.0f);
    glUniform1f(glGetUniformLocation(shader, "spotCutOff"), glm::cos(glm::radians(20.0f)));
    glUniform1f(glGetUniformLocation(shader, "spotOuterCutOff"), glm::cos(glm::radians(30.0f)));
    glUniform3f(glGetUniformLocation(shader, "spotLightColor"), 1.0f, 1.0f, 0.8f);

    glBindVertexArray(vao);
    glDrawElements(GL_TRIANGLES, 36, GL_UNSIGNED_INT, 0);
}