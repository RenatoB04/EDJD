#pragma once

#include <glm/glm.hpp>
#include <GLFW/glfw3.h>

// Classe que representa uma câmara orbital
class Camera {
public:
    // Parâmetros da câmara:
    // pitch: ângulo vertical (não está a ser usado neste exemplo)
    // yaw: ângulo horizontal, define a rotação em torno da origem
    // radius: distância da câmara à origem (permite zoom)
    float pitch = 30.0f, yaw = 120.0f, radius = 15.0f;

    // Devolve a matriz de visualização (view matrix) com base na posição da câmara
    glm::mat4 getViewMatrix() const;
};

// Define a câmara ativa (utilizada para input ou rendering)
void setActiveCamera(Camera* cam);

// Callback que responde ao scroll do rato (para fazer zoom)
void scroll_callback(GLFWwindow* window, double xoffset, double yoffset);