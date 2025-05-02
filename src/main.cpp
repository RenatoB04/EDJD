#include <GL/glew.h>
#include <GLFW/glfw3.h>
#include <iostream>
#include <vector>
#include <memory>
#include <string>
#include <glm/gtc/type_ptr.hpp>
#include <glm/gtc/matrix_transform.hpp>

#include "shader_utils.hpp"
#include "renderer.hpp"
#include "camera.hpp"
#include "model.hpp"

Camera camera;

int main() {
    if (!glfwInit()) {
        std::cerr << "Erro ao inicializar GLFW\n";
        return -1;
    }

    GLFWwindow* window = glfwCreateWindow(800, 600, "P01-P3D", nullptr, nullptr);
    if (!window) {
        std::cerr << "Erro ao criar janela GLFW\n";
        glfwTerminate();
        return -1;
    }

    glfwMakeContextCurrent(window);
    glewExperimental = true;

    if (glewInit() != GLEW_OK) {
        std::cerr << "Erro ao inicializar GLEW\n";
        return -1;
    }

    glEnable(GL_DEPTH_TEST);

    GLuint shaderProgram = createShaderProgram("assets/shaders/shader.vert", "assets/shaders/shader.frag");

    GLuint vao, vbo, ebo;
    setupMesa(vao, vbo, ebo);

    std::vector<std::unique_ptr<Model>> balls;
    for (int i = 1; i <= 15; ++i) {
        balls.push_back(std::make_unique<Model>("assets/objects/Ball" + std::to_string(i) + ".obj"));
    }

    std::vector<glm::vec3> positions;
    float spacing = 0.65f;
    float height = 1.2f;

    int ballIndex = 0;
    for (int row = 0; row < 5; ++row) {
        float z = row * spacing;
        float xOffset = -row * spacing * 0.5f;
        for (int col = 0; col <= row && ballIndex < 15; ++col) {
            float x = xOffset + col * spacing;
            positions.emplace_back(glm::vec3(x, height, z));
            ballIndex++;
        }
    }

    setActiveCamera(&camera);
    glfwSetCursorPosCallback(window, mouse_callback);
    glfwSetMouseButtonCallback(window, mouse_button_callback);
    glfwSetScrollCallback(window, scroll_callback);

    while (!glfwWindowShouldClose(window)) {
        int width, height;
        glfwGetFramebufferSize(window, &width, &height);

        glClearColor(0.1f, 0.1f, 0.1f, 1.0f);
        glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

        glUseProgram(shaderProgram);
        glViewport(0, 0, width, height);

        glm::mat4 view = camera.getViewMatrix();
        glm::mat4 projection = glm::perspective(glm::radians(45.0f), (float)width / (float)height, 0.1f, 100.0f);

        glUniform1i(glGetUniformLocation(shaderProgram, "useTexture"), false);
        glBindTexture(GL_TEXTURE_2D, 0);
        drawMesa(shaderProgram, vao, view, projection);

        GLint modelLoc = glGetUniformLocation(shaderProgram, "model");
        GLint viewLoc = glGetUniformLocation(shaderProgram, "view");
        GLint projLoc = glGetUniformLocation(shaderProgram, "projection");

        glUniformMatrix4fv(viewLoc, 1, GL_FALSE, glm::value_ptr(view));
        glUniformMatrix4fv(projLoc, 1, GL_FALSE, glm::value_ptr(projection));
        glUniform1i(glGetUniformLocation(shaderProgram, "useTexture"), true);

        for (size_t i = 0; i < balls.size(); ++i) {
            glm::mat4 model = glm::mat4(1.0f);
            model = glm::translate(model, positions[i]);
            model = glm::scale(model, glm::vec3(0.3f));
            glUniformMatrix4fv(modelLoc, 1, GL_FALSE, glm::value_ptr(model));
            balls[i]->draw(shaderProgram);
        }

        int miniHeight = height / 4;
        int miniWidth = (int)(miniHeight * (8.0f / 20.0f));

        glViewport(width - miniWidth - 10, height - miniHeight - 10, miniWidth, miniHeight);

        glm::mat4 miniProjection = glm::ortho(-4.0f, 4.0f, -10.0f, 10.0f, 0.1f, 100.0f);

        glm::mat4 miniView = glm::lookAt(
            glm::vec3(0.0f, 10.0f, 0.0f),
            glm::vec3(0.0f, 0.0f, 0.0f),
            glm::vec3(0.0f, 0.0f, -1.0f)
        );

        glUniform1i(glGetUniformLocation(shaderProgram, "useTexture"), false);
        glBindTexture(GL_TEXTURE_2D, 0);
        drawMesa(shaderProgram, vao, miniView, miniProjection);

        glUniformMatrix4fv(viewLoc, 1, GL_FALSE, glm::value_ptr(miniView));
        glUniformMatrix4fv(projLoc, 1, GL_FALSE, glm::value_ptr(miniProjection));
        glUniform1i(glGetUniformLocation(shaderProgram, "useTexture"), true);

        for (size_t i = 0; i < balls.size(); ++i) {
            glm::mat4 model = glm::mat4(1.0f);
            model = glm::translate(model, positions[i]);
            model = glm::scale(model, glm::vec3(0.3f));
            glUniformMatrix4fv(modelLoc, 1, GL_FALSE, glm::value_ptr(model));
            balls[i]->draw(shaderProgram);
        }

        glfwSwapBuffers(window);
        glfwPollEvents();
    }

    glfwTerminate();
    return 0;
}
