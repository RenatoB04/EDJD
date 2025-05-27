#pragma once

#include <GL/glew.h>
#include <glm/glm.hpp>

struct LightState {
    bool useAmbient = true;
    bool useDirectional = true;
    bool usePoint = true;
    bool useSpot = true;
};

void setupMesa(GLuint &vao, GLuint &vbo, GLuint &ebo);

void drawMesa(GLuint shader, GLuint vao, const glm::mat4& view, const glm::mat4& proj, const LightState& lights, bool noLighting, float angulo);