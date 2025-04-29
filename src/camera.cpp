#include "camera.hpp"
#include <GLFW/glfw3.h>

static Camera* activeCamera = nullptr;

void setActiveCamera(Camera* cam) {
    activeCamera = cam;
}

void mouse_callback(GLFWwindow* window, double xpos, double ypos) {
    if (!activeCamera || !activeCamera->rotating)
        return;

    if (activeCamera->firstMouse) {
        activeCamera->lastX = xpos;
        activeCamera->lastY = ypos;
        activeCamera->firstMouse = false;
    }

    float xoffset = xpos - activeCamera->lastX;
    float yoffset = activeCamera->lastY - ypos;
    activeCamera->lastX = xpos;
    activeCamera->lastY = ypos;

    xoffset *= activeCamera->sensitivity;
    yoffset *= activeCamera->sensitivity;

    activeCamera->yaw   += xoffset;
    activeCamera->pitch += yoffset;

    if (activeCamera->pitch > 89.0f)
        activeCamera->pitch = 89.0f;
    if (activeCamera->pitch < -89.0f)
        activeCamera->pitch = -89.0f;
}

void scroll_callback(GLFWwindow* window, double xoffset, double yoffset) {
    if (!activeCamera)
        return;

    activeCamera->radius -= static_cast<float>(yoffset) * 0.5f;

    if (activeCamera->radius < 2.0f)
        activeCamera->radius = 2.0f;
    if (activeCamera->radius > 20.0f)
        activeCamera->radius = 20.0f;
}

void mouse_button_callback(GLFWwindow* window, int button, int action, int mods) {
    if (!activeCamera)
        return;

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
