#include <GL/glew.h>
#include <GLFW/glfw3.h>
#include <iostream>
#include <vector>
#include <memory>
#include <string>
#include <glm/glm.hpp>
#include <glm/gtc/type_ptr.hpp>
#include <glm/gtc/matrix_transform.hpp>

#include "shader_utils.hpp"
#include "renderer.hpp"
#include "camera.hpp"
#include "model.hpp"

using RendererLib::Model;

Camera camera;
LightState luz = { false, false, false, false };
bool semLuz = true, aRodar = false, bolaMove = false;
glm::vec3 bolaVel = glm::vec3(0.0f);
float bolaRot = 0.0f, deltaTime = 0.0f, lastFrame = 0.0f, angulo = 0.0f;

void mouse_button_callback(GLFWwindow*, int btn, int action, int) {
    if (btn == GLFW_MOUSE_BUTTON_RIGHT) aRodar = (action == GLFW_PRESS);
}

void mouse_callback(GLFWwindow*, double xpos, double) {
    static double lastX = xpos;
    if (aRodar) angulo += (xpos - lastX) * 0.01f;
    lastX = xpos;
}

void processInput(GLFWwindow* w) {
    static bool keys[6] = {};

    auto toggle = [&](int key, int i, bool& val, const char* nome) {
        if (glfwGetKey(w, key) == GLFW_PRESS && !keys[i]) {
            val = !val;
            std::cout << nome << (val ? " ON" : " OFF") << '\n';
            keys[i] = true;
        } else if (glfwGetKey(w, key) == GLFW_RELEASE) keys[i] = false;
    };

    toggle(GLFW_KEY_1, 0, luz.useAmbient, "[Ambiente]");
    toggle(GLFW_KEY_2, 1, luz.useDirectional, "[Direcional]");
    toggle(GLFW_KEY_3, 2, luz.usePoint, "[Pontual]");
    toggle(GLFW_KEY_4, 3, luz.useSpot, "[Cónica]");
    toggle(GLFW_KEY_5, 4, semLuz, "[Sem Luz]");

    if (glfwGetKey(w, GLFW_KEY_SPACE) == GLFW_PRESS && !keys[5]) {
        if (!bolaMove) {
            bolaMove = true;
            bolaVel = glm::normalize(glm::vec3(0, 0, 1)) * 2.0f;
        }
        keys[5] = true;
    } else if (glfwGetKey(w, GLFW_KEY_SPACE) == GLFW_RELEASE) keys[5] = false;
}

int main() {
    if (!glfwInit()) return std::cerr << "Erro GLFW\n", -1;

    GLFWwindow* win = glfwCreateWindow(800, 600, "P01-P3D", nullptr, nullptr);
    if (!win) return std::cerr << "Erro janela GLFW\n", glfwTerminate(), -1;

    glfwMakeContextCurrent(win);
    glewExperimental = true;
    if (glewInit() != GLEW_OK) return std::cerr << "Erro GLEW\n", -1;

    glfwSetScrollCallback(win, scroll_callback);
    glfwSetCursorPosCallback(win, mouse_callback);
    glfwSetMouseButtonCallback(win, mouse_button_callback);
    setActiveCamera(&camera);

    glEnable(GL_DEPTH_TEST);
    GLuint shader = createShaderProgram("assets/shaders/shader.vert", "assets/shaders/shader.frag");

    GLuint vao, vbo, ebo;
    setupMesa(vao, vbo, ebo);

    std::vector<std::unique_ptr<Model>> bolas;
    for (int i = 1; i <= 15; ++i) {
        auto b = std::make_unique<Model>();
        b->Load("assets/objects/Ball" + std::to_string(i) + ".obj");
        b->Install();
        bolas.push_back(std::move(b));
    }

    std::vector<glm::vec3> pos = {
        {-2,1.2f,-8}, {0,1.2f,-7}, {2,1.2f,-6}, {-3,1.2f,-5}, {3,1.2f,-4},
        {-2.5f,1.2f,-3}, {0,1.2f,-2}, {2.5f,1.2f,-1}, {-3.5f,1.2f,0}, {3.5f,1.2f,1},
        {-2,1.2f,2}, {0,1.2f,3}, {2,1.2f,4}, {-1,1.2f,5}, {1,1.2f,6}
    };

    while (!glfwWindowShouldClose(win)) {
        float now = glfwGetTime();
        deltaTime = now - lastFrame;
        lastFrame = now;

        processInput(win);

        if (bolaMove) {
            pos[0] += bolaVel * deltaTime;
            bolaRot += 2.0f * deltaTime;

            if (std::abs(pos[0].x) > 6.5f || std::abs(pos[0].z) > 10.0f) {
                bolaMove = false;
                std::cout << "[STOP] Fora da mesa\n";
            }

            for (size_t i = 1; i < pos.size(); ++i) {
                if (glm::distance(pos[0], pos[i]) < 0.6f) {
                    bolaMove = false;
                    std::cout << "[STOP] Colisão\n";
                    break;
                }
            }
        }

        int w, h;
        glfwGetFramebufferSize(win, &w, &h);
        glViewport(0, 0, w, h);
        glClearColor(0.1f, 0.1f, 0.1f, 1.0f);
        glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
        glUseProgram(shader);

        glm::vec3 eye(
            camera.radius * sin(glm::radians(camera.yaw)),
            camera.radius * sin(glm::radians(camera.pitch)),
            camera.radius * cos(glm::radians(camera.yaw))
        );
        glm::mat4 view = glm::lookAt(eye, glm::vec3(0), glm::vec3(0, 1, 0));
        glm::mat4 proj = glm::perspective(glm::radians(45.0f), (float)w / h, 0.1f, 100.0f);

        glUniform1i(glGetUniformLocation(shader, "useTexture"), false);
        glBindTexture(GL_TEXTURE_2D, 0);
        drawMesa(shader, vao, view, proj, luz, semLuz, angulo);

        glUniform1i(glGetUniformLocation(shader, "useTexture"), true);
        for (size_t i = 0; i < bolas.size(); ++i) {
            glm::vec3 rot = (i == 0) ? glm::vec3(bolaRot, 0, 0) : glm::vec3(0);
            bolas[i]->Render(pos[i], rot, shader, angulo);
        }

        int miniH = h / 4;
        int miniW = static_cast<int>(miniH * 8.0f / 20.0f);
        glViewport(w - miniW - 10, h - miniH - 10, miniW, miniH);

        glm::mat4 projMini = glm::ortho(-4.0f, 4.0f, -10.0f, 10.0f, 0.1f, 100.0f);
        glm::mat4 viewMini = glm::lookAt(glm::vec3(0, 10, 0), glm::vec3(0), glm::vec3(0, 0, -1));

        glUniformMatrix4fv(glGetUniformLocation(shader, "view"), 1, GL_FALSE, glm::value_ptr(viewMini));
        glUniformMatrix4fv(glGetUniformLocation(shader, "projection"), 1, GL_FALSE, glm::value_ptr(projMini));

        glUniform1i(glGetUniformLocation(shader, "useTexture"), false);
        glBindTexture(GL_TEXTURE_2D, 0);
        drawMesa(shader, vao, viewMini, projMini, { false, false, false, false }, true, 0);

        glUniform1i(glGetUniformLocation(shader, "useTexture"), true);
        for (size_t i = 0; i < bolas.size(); ++i) {
            glm::vec3 rot = (i == 0) ? glm::vec3(0, bolaRot, 0) : glm::vec3(0);
            bolas[i]->Render(pos[i], rot, shader, 0);
        }

        glfwSwapBuffers(win);
        glfwPollEvents();
    }

    glfwTerminate();
    return 0;
}