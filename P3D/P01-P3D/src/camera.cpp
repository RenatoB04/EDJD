#include "camera.hpp"
#include <GLFW/glfw3.h>
#include <glm/gtc/matrix_transform.hpp>

// Ponteiro para a câmara atualmente ativa
static Camera* activeCamera = nullptr;

// Define qual a câmara ativa a ser utilizada
void setActiveCamera(Camera* cam) {
    activeCamera = cam;
}

// Callback associado ao scroll do rato (zoom)
void scroll_callback(GLFWwindow*, double, double yoffset) {
    if (!activeCamera) return;

    // Ajusta o raio (distância da câmara ao centro da cena), com limites mínimos e máximos
    activeCamera->radius = glm::clamp(
        activeCamera->radius - static_cast<float>(yoffset) * 0.5f,
        2.0f, 20.0f
    );
}

// Gera a matriz de visualização (view matrix) da câmara
glm::mat4 Camera::getViewMatrix() const {
    float rad = glm::radians(yaw); // Converte o ângulo horizontal (yaw) para radianos

    // Calcula a posição da câmara num círculo horizontal à volta da origem
    glm::vec3 pos = glm::vec3(
        radius * sin(rad), // X
        5.0f,              // Y fixo (altura constante)
        radius * cos(rad)  // Z
    );

    // Cria a matriz de visualização: olha da posição da câmara para a origem
    return glm::lookAt(
        pos,                 // Posição da câmara
        glm::vec3(0.0f),     // Ponto para onde está a olhar (origem)
        glm::vec3(0.0f, 1.0f, 0.0f) // Vector para cima (eixo Y)
    );
}