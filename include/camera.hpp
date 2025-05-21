#pragma once

#include <GLFW/glfw3.h>
#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>

// Estrutura que representa uma câmara para visualização 3D
struct Camera {
    // Ângulo horizontal da câmara (yaw)
    float yaw = 120.0f;
    // Ângulo vertical da câmara (pitch)
    float pitch = 30.0f;
    // Distância da câmara ao alvo
    float radius = 15.0f;
    // Sensibilidade do movimento do rato
    float sensitivity = 0.1f;

    // Posição anterior do rato
    double lastX = 0.0;
    double lastY = 0.0;
    // Flag para controlar o primeiro movimento do rato
    bool firstMouse = true;
    // Flag para controlar se a câmara está a rodar
    bool rotating = false;

    // Calcula a matriz de vista da câmara
    glm::mat4 getViewMatrix() const {
        // Converte os ângulos em coordenadas cartesianas
        float x = radius * cos(glm::radians(pitch)) * cos(glm::radians(yaw));
        float y = radius * sin(glm::radians(pitch));
        float z = radius * cos(glm::radians(pitch)) * sin(glm::radians(yaw));

        // Define a posição da câmara, o alvo e o vetor "up"
        glm::vec3 position = glm::vec3(x, y, z);
        glm::vec3 target = glm::vec3(0.0f, 0.0f, 0.0f);
        glm::vec3 up = glm::vec3(0.0f, 1.0f, 0.0f);

        // Retorna a matriz de vista usando a função lookAt do GLM
        return glm::lookAt(position, target, up);
    }
};

// Declaração das funções de callback para o rato
void mouse_callback(GLFWwindow* window, double xpos, double ypos);

void mouse_button_callback(GLFWwindow* window, int button, int action, int mods);

void scroll_callback(GLFWwindow* window, double xoffset, double yoffset);

// Função para definir qual a câmara ativa (controlada pelo rato)
void setActiveCamera(Camera* cam);