#pragma once

#include <GL/glew.h>

void setupMesa(GLuint &vao, GLuint &vbo, GLuint &ebo);
void drawMesa(GLuint shaderProgram, GLuint vao);
