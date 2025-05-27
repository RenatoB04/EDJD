#include "camera.hpp"
#include <GLFW/glfw3.h>
#include <glm/gtc/matrix_transform.hpp>

static Camera* activeCamera = nullptr;

void setActiveCamera(Camera* cam) {
    activeCamera = cam;
}

void scroll_callback(GLFWwindow*, double, double yoffset) {
    if (!activeCamera) return;

    activeCamera->radius = glm::clamp(activeCamera->radius - static_cast<float>(yoffset) * 0.5f, 2.0f, 20.0f);
}

glm::mat4 Camera::getViewMatrix() const {
    float rad = glm::radians(yaw);
    glm::vec3 pos = glm::vec3(radius * sin(rad), 5.0f, radius * cos(rad));
    return glm::lookAt(pos, glm::vec3(0.0f), glm::vec3(0.0f, 1.0f, 0.0f));
}