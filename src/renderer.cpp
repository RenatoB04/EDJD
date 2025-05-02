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
        0, 1, 2, 2, 3, 0,
        4, 5, 6, 6, 7, 4,
        3, 2, 6, 6, 7, 3,
        0, 1, 5, 5, 4, 0,
        0, 3, 7, 7, 4, 0,
        1, 2, 6, 6, 5, 1
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

void drawMesa(GLuint shaderProgram, GLuint vao, const glm::mat4& view, const glm::mat4& projection, const LightState& lights) {
    glm::mat4 model = glm::mat4(1.0f);
    model = glm::scale(model, glm::vec3(4.0f, 3.0f, 5.0f));

    glUseProgram(shaderProgram);

    GLint modelLoc = glGetUniformLocation(shaderProgram, "model");
    GLint viewLoc  = glGetUniformLocation(shaderProgram, "view");
    GLint projLoc  = glGetUniformLocation(shaderProgram, "projection");

    glUniformMatrix4fv(modelLoc, 1, GL_FALSE, glm::value_ptr(model));
    glUniformMatrix4fv(viewLoc, 1, GL_FALSE, glm::value_ptr(view));
    glUniformMatrix4fv(projLoc, 1, GL_FALSE, glm::value_ptr(projection));

    glUniform3f(glGetUniformLocation(shaderProgram, "viewPos"), 0.0f, 2.0f, 5.0f);

    glUniform1i(glGetUniformLocation(shaderProgram, "useAmbient"), lights.useAmbient);
    glUniform3f(glGetUniformLocation(shaderProgram, "ambientColor"), 0.2f, 0.2f, 0.2f);

    glUniform1i(glGetUniformLocation(shaderProgram, "useDirectional"), lights.useDirectional);
    glUniform3f(glGetUniformLocation(shaderProgram, "dirLightDirection"), -1.0f, -1.0f, -1.0f);
    glUniform3f(glGetUniformLocation(shaderProgram, "dirLightColor"), 0.8f, 0.8f, 0.8f);

    glUniform1i(glGetUniformLocation(shaderProgram, "usePoint"), lights.usePoint);
    glUniform3f(glGetUniformLocation(shaderProgram, "pointLightPosition"), 0.0f, 3.0f, 0.0f);
    glUniform3f(glGetUniformLocation(shaderProgram, "pointLightColor"), 1.0f, 1.0f, 1.0f);

    glUniform1i(glGetUniformLocation(shaderProgram, "useSpot"), lights.useSpot);
    glUniform3f(glGetUniformLocation(shaderProgram, "spotLightPosition"), 0.0f, 3.0f, 3.0f);
    glUniform3f(glGetUniformLocation(shaderProgram, "spotLightDirection"), 0.0f, -1.0f, -1.0f);
    glUniform1f(glGetUniformLocation(shaderProgram, "spotCutOff"), glm::cos(glm::radians(20.0f)));
    glUniform1f(glGetUniformLocation(shaderProgram, "spotOuterCutOff"), glm::cos(glm::radians(30.0f)));
    glUniform3f(glGetUniformLocation(shaderProgram, "spotLightColor"), 1.0f, 1.0f, 0.8f);

    glBindVertexArray(vao);
    glDrawElements(GL_TRIANGLES, 36, GL_UNSIGNED_INT, 0);
}