#include "camera.hpp"
#include <GLFW/glfw3.h>

// Ponteiro para a câmara ativa, que será controlada pelo utilizador
static Camera* activeCamera = nullptr;

// Define qual a câmara ativa, para ser controlada pelas funções de callback
void setActiveCamera(Camera* cam) {
    activeCamera = cam;
}

// Callback para o movimento do rato: atualiza a orientação da câmara
void mouse_callback(GLFWwindow* window, double xpos, double ypos) {
    // Se não houver câmara ativa ou não estiver a rodar, sai da função
    if (!activeCamera || !activeCamera->rotating)
        return;

    // Se for o primeiro movimento do rato, guarda as coordenadas para evitar saltos bruscos
    if (activeCamera->firstMouse) {
        activeCamera->lastX = xpos;
        activeCamera->lastY = ypos;
        activeCamera->firstMouse = false;
    }

    // Calcula o deslocamento do rato desde a última posição
    float xoffset = xpos - activeCamera->lastX;
    float yoffset = activeCamera->lastY - ypos;
    activeCamera->lastX = xpos;
    activeCamera->lastY = ypos;

    // Aplica a sensibilidade ao deslocamento do rato
    xoffset *= activeCamera->sensitivity;
    yoffset *= activeCamera->sensitivity;

    // Atualiza o yaw (rotação horizontal) e o pitch (rotação vertical)
    activeCamera->yaw   += xoffset;
    activeCamera->pitch += yoffset;

    // Limita o pitch para evitar viragens demasiado extremas
    if (activeCamera->pitch > 89.0f)
        activeCamera->pitch = 89.0f;
    if (activeCamera->pitch < -89.0f)
        activeCamera->pitch = -89.0f;
}

// Callback para a roda do rato: controla o zoom da câmara (distância ao alvo)
void scroll_callback(GLFWwindow* window, double xoffset, double yoffset) {
    if (!activeCamera)
        return;

    // Atualiza o raio (distância ao centro) da câmara, limitando-o
    activeCamera->radius -= static_cast<float>(yoffset) * 0.5f;

    if (activeCamera->radius < 2.0f)
        activeCamera->radius = 2.0f;
    if (activeCamera->radius > 20.0f)
        activeCamera->radius = 20.0f;
}

// Callback para os botões do rato: ativa/desativa a rotação da câmara
void mouse_button_callback(GLFWwindow* window, int button, int action, int mods) {
    if (!activeCamera)
        return;

    // Se o botão direito do rato for pressionado, ativa a rotação
    if (button == GLFW_MOUSE_BUTTON_RIGHT) {
        if (action == GLFW_PRESS) {
            activeCamera->rotating = true;
            activeCamera->firstMouse = true;
        }
        else if (action == GLFW_RELEASE) {
            activeCamera->rotating = false;
        }
    }
}