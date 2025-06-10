#pragma once

#include <GL/glew.h>

// Cria e devolve um programa de shader OpenGL a partir dos caminhos
// para os ficheiros vertex e fragment shader.
// Retorna 0 em caso de erro.
GLuint createShaderProgram(const char* vertexPath, const char* fragmentPath);