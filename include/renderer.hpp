#pragma once

#include <GL/glew.h>
#include <glm/glm.hpp>

void setupMesa(GLuint &vao, GLuint &vbo, GLuint &ebo);

void drawMesa(GLuint shaderProgram, GLuint vao, const glm::mat4& view, const glm::mat4& projection);