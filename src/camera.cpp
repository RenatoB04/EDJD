#include "camera.hpp"
#include <GLFW/glfw3.h>
#include <glm/gtc/matrix_transform.hpp>

// Ponteiro para a câmara ativa
static Camera* activeCamera = nullptr;

// Define a câmara ativa
void setActiveCamera(Camera* cam) {
    activeCamera = cam;
}

// Callback para o scroll do rato (controlo do zoom)
void scroll_callback(GLFWwindow* window, double xoffset, double yoffset) {
    if (!activeCamera) return;

    // Zoom in/out com limites
    activeCamera->radius -= static_cast<float>(yoffset) * 0.5f;

    if (activeCamera->radius < 2.0f)
        activeCamera->radius = 2.0f;
    if (activeCamera->radius > 20.0f)
        activeCamera->radius = 20.0f;
}

// Função que devolve a matriz de visualização (view matrix)
glm::mat4 Camera::getViewMatrix() const {
    // Posição da câmara baseada num ângulo fixo horizontal (porque já não há rotação com rato)
    float camX = radius * sin(glm::radians(yaw));
    float camZ = radius * cos(glm::radians(yaw));
    glm::vec3 position = glm::vec3(camX, 5.0f, camZ); // Altura constante

    return glm::lookAt(position, glm::vec3(0.0f), glm::vec3(0.0f, 1.0f, 0.0f));
}