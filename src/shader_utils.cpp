#include "shader_utils.hpp"
#include <fstream>
#include <sstream>
#include <iostream>

std::string loadShaderSource(const char* path) {
    std::ifstream file(path);
    if (!file) {
        std::cerr << "Erro ao abrir shader: " << path << '\n';
        return "";
    }
    std::stringstream buffer;
    buffer << file.rdbuf();
    return buffer.str();
}

GLuint compileShader(GLenum type, const char* source) {
    GLuint shader = glCreateShader(type);
    glShaderSource(shader, 1, &source, nullptr);
    glCompileShader(shader);

    GLint ok;
    glGetShaderiv(shader, GL_COMPILE_STATUS, &ok);
    if (!ok) {
        char log[512];
        glGetShaderInfoLog(shader, 512, nullptr, log);
        std::cerr << "Erro ao compilar shader: " << log << '\n';
    }

    return shader;
}

GLuint createShaderProgram(const char* vertPath, const char* fragPath) {
    std::string vert = loadShaderSource(vertPath);
    std::string frag = loadShaderSource(fragPath);
    if (vert.empty() || frag.empty()) {
        std::cerr << "Erro: Shader vazio ou caminho incorreto.\n";
        return 0;
    }

    GLuint vs = compileShader(GL_VERTEX_SHADER, vert.c_str());
    GLuint fs = compileShader(GL_FRAGMENT_SHADER, frag.c_str());

    GLuint program = glCreateProgram();
    glAttachShader(program, vs);
    glAttachShader(program, fs);
    glLinkProgram(program);

    GLint ok;
    glGetProgramiv(program, GL_LINK_STATUS, &ok);
    if (!ok) {
        char log[512];
        glGetProgramInfoLog(program, 512, nullptr, log);
        std::cerr << "Erro ao ligar programa shader: " << log << '\n';
    }

    glDeleteShader(vs);
    glDeleteShader(fs);

    return program;
}