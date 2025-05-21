#pragma once

#include <string>
#include <GL/glew.h>

// Função para criar um programa de shader
// Recebe os caminhos para os ficheiros do vertex shader e do fragment shader
// Retorna o identificador do programa de shader criado
GLuint createShaderProgram(const char* vertexPath, const char* fragmentPath);