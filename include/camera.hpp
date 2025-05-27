#pragma once
#include <glm/glm.hpp>
#include <GLFW/glfw3.h>

class Camera {
public:
    float pitch = 30.0f, yaw = 120.0f, radius = 15.0f;

    glm::mat4 getViewMatrix() const;
};

void setActiveCamera(Camera* cam);
void scroll_callback(GLFWwindow* window, double xoffset, double yoffset);