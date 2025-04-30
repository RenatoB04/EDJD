#include <GL/glew.h>
#include <GLFW/glfw3.h>
#include <iostream>
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

    Model ballModel("assets/objects/Ball1.obj");

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

        glm::mat4 model = glm::mat4(1.0f);
        model = glm::translate(model, glm::vec3(0.0f, 0.6f, 0.0f));
        model = glm::scale(model, glm::vec3(0.3f));

        GLint modelLoc = glGetUniformLocation(shaderProgram, "model");
        GLint viewLoc = glGetUniformLocation(shaderProgram, "view");
        GLint projLoc = glGetUniformLocation(shaderProgram, "projection");

        glUniformMatrix4fv(modelLoc, 1, GL_FALSE, glm::value_ptr(model));
        glUniformMatrix4fv(viewLoc, 1, GL_FALSE, glm::value_ptr(view));
        glUniformMatrix4fv(projLoc, 1, GL_FALSE, glm::value_ptr(projection));

        glUniform1i(glGetUniformLocation(shaderProgram, "useTexture"), true);
        ballModel.draw(shaderProgram);

        int miniWidth = width / 4;
        int miniHeight = height / 4;
        glViewport(width - miniWidth - 10, height - miniHeight - 10, miniWidth, miniHeight);

        glm::mat4 miniView = glm::lookAt(
            glm::vec3(0.0f, 5.0f, 0.0f),
            glm::vec3(0.0f, 0.0f, 0.0f),
            glm::vec3(0.0f, 0.0f, -1.0f)
        );

        glm::mat4 miniProjection = glm::ortho(-3.0f, 3.0f, -3.0f, 3.0f, 0.1f, 100.0f);

        glUniform1i(glGetUniformLocation(shaderProgram, "useTexture"), false);
        glBindTexture(GL_TEXTURE_2D, 0);
        drawMesa(shaderProgram, vao, miniView, miniProjection);

        glfwSwapBuffers(window);
        glfwPollEvents();
    }

    glfwTerminate();
    return 0;
}