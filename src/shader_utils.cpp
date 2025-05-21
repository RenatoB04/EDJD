#include "shader_utils.hpp"
#include <fstream>
#include <sstream>
#include <iostream>

// Função para carregar o código fonte de um shader a partir de um ficheiro
std::string loadShaderSource(const char* filepath) {
    std::ifstream file(filepath);
    if (!file) {
        std::cerr << "Erro ao abrir shader: " << filepath << "\n";
        return ""; // Retorna string vazia em caso de erro
    }

    // Lê todo o conteúdo do ficheiro para uma string
    std::stringstream buffer;
    buffer << file.rdbuf();
    return buffer.str(); // Retorna o código fonte do shader
}

// Função para compilar um shader do tipo especificado (vertex, fragment, etc)
GLuint compileShader(GLenum type, const char* source) {
    // Cria um identificador de shader
    GLuint shader = glCreateShader(type);

    // Associa o código fonte ao shader
    glShaderSource(shader, 1, &source, nullptr);

    // Compila o shader
    glCompileShader(shader);

    // Verifica se a compilação foi bem sucedida
    GLint success;
    glGetShaderiv(shader, GL_COMPILE_STATUS, &success);
    if (!success) {
        char infoLog[512];
        glGetShaderInfoLog(shader, 512, nullptr, infoLog);
        std::cerr << "Erro ao compilar shader: " << infoLog << std::endl;
    }

    return shader; // Retorna o identificador do shader compilado
}

// Função para criar um programa de shader a partir de ficheiros de vertex e fragment shader
GLuint createShaderProgram(const char* vertexPath, const char* fragmentPath) {
    // Carrega o código fonte dos shaders
    std::string vertexCode = loadShaderSource(vertexPath);
    std::string fragmentCode = loadShaderSource(fragmentPath);

    // Verifica se os shaders foram carregados corretamente
    if (vertexCode.empty() || fragmentCode.empty()) {
        std::cerr << "Erro: Shader vazio ou caminho incorreto.\n";
        return 0; // Retorna 0 em caso de erro
    }

    // Compila os shaders
    GLuint vertexShader = compileShader(GL_VERTEX_SHADER, vertexCode.c_str());
    GLuint fragmentShader = compileShader(GL_FRAGMENT_SHADER, fragmentCode.c_str());

    // Cria um programa de shader
    GLuint shaderProgram = glCreateProgram();

    // Associa os shaders ao programa
    glAttachShader(shaderProgram, vertexShader);
    glAttachShader(shaderProgram, fragmentShader);

    // Liga o programa (link)
    glLinkProgram(shaderProgram);

    // Verifica se a ligação foi bem sucedida
    GLint success;
    glGetProgramiv(shaderProgram, GL_LINK_STATUS, &success);
    if (!success) {
        char infoLog[512];
        glGetProgramInfoLog(shaderProgram, 512, nullptr, infoLog);
        std::cerr << "Erro ao ligar programa shader: " << infoLog << std::endl;
    }

    // Apaga os shaders individuais, pois já estão incorporados no programa
    glDeleteShader(vertexShader);
    glDeleteShader(fragmentShader);

    return shaderProgram; // Retorna o identificador do programa de shader
}