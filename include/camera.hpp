#pragma once

#include <GLFW/glfw3.h>
#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>

struct Camera {
    float yaw   = -90.0f;
    float pitch = 0.0f;
    float radius = 4.0f;
    float sensitivity = 0.1f;

    double lastX = 0.0;
    double lastY = 0.0;
    bool firstMouse = true;
    bool rotating = false;

    glm::mat4 getViewMatrix() const {
        float x = radius * cos(glm::radians(pitch)) * cos(glm::radians(yaw));
        float y = radius * sin(glm::radians(pitch));
        float z = radius * cos(glm::radians(pitch)) * sin(glm::radians(yaw));

        glm::vec3 position = glm::vec3(x, y, z);
        glm::vec3 target = glm::vec3(0.0f, 0.0f, 0.0f);
        glm::vec3 up = glm::vec3(0.0f, 1.0f, 0.0f);

        return glm::lookAt(position, target, up);
    }
};

void mouse_callback(GLFWwindow* window, double xpos, double ypos);

void mouse_button_callback(GLFWwindow* window, int button, int action, int mods);

void scroll_callback(GLFWwindow* window, double xoffset, double yoffset);

void setActiveCamera(Camera* cam);
