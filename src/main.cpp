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

using RendererLib::Model;

Camera camera;

LightState lightState = {
    false,
    false,
    false,
    false
};

bool noLightingMode = true;

void processInput(GLFWwindow* window) {
    static bool keyStates[4] = {};

    if (glfwGetKey(window, GLFW_KEY_1) == GLFW_PRESS && !keyStates[0]) {
        lightState.useAmbient = !lightState.useAmbient;
        std::cout << "[Luz Ambiente] " << (lightState.useAmbient ? "ON" : "OFF") << std::endl;
        keyStates[0] = true;
    } else if (glfwGetKey(window, GLFW_KEY_1) == GLFW_RELEASE) {
        keyStates[0] = false;
    }

    if (glfwGetKey(window, GLFW_KEY_2) == GLFW_PRESS && !keyStates[1]) {
        lightState.useDirectional = !lightState.useDirectional;
        std::cout << "[Luz Direcional] " << (lightState.useDirectional ? "ON" : "OFF") << std::endl;
        keyStates[1] = true;
    } else if (glfwGetKey(window, GLFW_KEY_2) == GLFW_RELEASE) {
        keyStates[1] = false;
    }

    if (glfwGetKey(window, GLFW_KEY_3) == GLFW_PRESS && !keyStates[2]) {
        lightState.usePoint = !lightState.usePoint;
        std::cout << "[Luz Pontual] " << (lightState.usePoint ? "ON" : "OFF") << std::endl;
        keyStates[2] = true;
    } else if (glfwGetKey(window, GLFW_KEY_3) == GLFW_RELEASE) {
        keyStates[2] = false;
    }

    if (glfwGetKey(window, GLFW_KEY_4) == GLFW_PRESS && !keyStates[3]) {
        lightState.useSpot = !lightState.useSpot;
        std::cout << "[Luz Conica] " << (lightState.useSpot ? "ON" : "OFF") << std::endl;
        keyStates[3] = true;
    } else if (glfwGetKey(window, GLFW_KEY_4) == GLFW_RELEASE) {
        keyStates[3] = false;
    }
    static bool key5 = false;
    if (glfwGetKey(window, GLFW_KEY_5) == GLFW_PRESS && !key5) {
        noLightingMode = !noLightingMode;
        std::cout << "[Modo Sem Iluminacao] " << (noLightingMode ? "ON " : "OFF") << std::endl;
        key5 = true;
    } else if (glfwGetKey(window, GLFW_KEY_5) == GLFW_RELEASE) {
        key5 = false;
    }
}

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
        auto ball = std::make_unique<Model>();
        ball->Load("assets/objects/Ball" + std::to_string(i) + ".obj");
        ball->Install();
        balls.push_back(std::move(ball));
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
        processInput(window);

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
        drawMesa(shaderProgram, vao, view, projection, lightState, noLightingMode);

        glUniform1i(glGetUniformLocation(shaderProgram, "useTexture"), true);
        for (size_t i = 0; i < balls.size(); ++i) {
            balls[i]->Render(positions[i], glm::vec3(0.0f), shaderProgram);
        }

        int miniHeight = height / 4;
        int miniWidth = static_cast<int>(miniHeight * (8.0f / 20.0f));
        glViewport(width - miniWidth - 10, height - miniHeight - 10, miniWidth, miniHeight);

        glm::mat4 miniProjection = glm::ortho(-4.0f, 4.0f, -10.0f, 10.0f, 0.1f, 100.0f);
        glm::mat4 miniView = glm::lookAt(
            glm::vec3(0.0f, 10.0f, 0.0f),
            glm::vec3(0.0f, 0.0f, 0.0f),
            glm::vec3(0.0f, 0.0f, -1.0f)
        );

        glUniformMatrix4fv(glGetUniformLocation(shaderProgram, "view"), 1, GL_FALSE, glm::value_ptr(miniView));
        glUniformMatrix4fv(glGetUniformLocation(shaderProgram, "projection"), 1, GL_FALSE, glm::value_ptr(miniProjection));

        glUniform1i(glGetUniformLocation(shaderProgram, "useTexture"), false);
        glBindTexture(GL_TEXTURE_2D, 0);
        drawMesa(shaderProgram, vao, miniView, miniProjection, lightState, noLightingMode);

        glUniform1i(glGetUniformLocation(shaderProgram, "useTexture"), true);
        for (size_t i = 0; i < balls.size(); ++i) {
            balls[i]->Render(positions[i], glm::vec3(0.0f), shaderProgram);
        }

        glfwSwapBuffers(window);
        glfwPollEvents();
    }

    glfwTerminate();
    return 0;
}