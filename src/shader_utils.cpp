#include "shader_utils.hpp"
#include <fstream>
#include <sstream>
#include <iostream>

// Carrega o conteúdo de um ficheiro de shader para uma string
std::string loadShaderSource(const char* path) {
    std::ifstream file(path);
    if (!file) {
        std::cerr << "Erro ao abrir shader: " << path << '\n';
        return "";
    }

    std::stringstream buffer;
    buffer << file.rdbuf(); // Lê todo o conteúdo do ficheiro para um buffer
    return buffer.str();    // Devolve o código do shader como string
}

// Compila um shader (vertex ou fragment) a partir de código fonte
GLuint compileShader(GLenum type, const char* source) {
    GLuint shader = glCreateShader(type);
    glShaderSource(shader, 1, &source, nullptr); // Envia o código fonte para o OpenGL
    glCompileShader(shader);                    // Compila o shader

    // Verifica se a compilação foi bem-sucedida
    GLint ok;
    glGetShaderiv(shader, GL_COMPILE_STATUS, &ok);
    if (!ok) {
        char log[512];
        glGetShaderInfoLog(shader, 512, nullptr, log); // Mostra log de erro se houver
        std::cerr << "Erro ao compilar shader: " << log << '\n';
    }

    return shader;
}

// Cria um programa de shader a partir de ficheiros vertex e fragment
GLuint createShaderProgram(const char* vertPath, const char* fragPath) {
    std::string vert = loadShaderSource(vertPath);
    std::string frag = loadShaderSource(fragPath);

    // Verifica se os ficheiros foram carregados corretamente
    if (vert.empty() || frag.empty()) {
        std::cerr << "Erro: Shader vazio ou caminho incorreto.\n";
        return 0;
    }

    // Compila os shaders individuais
    GLuint vs = compileShader(GL_VERTEX_SHADER, vert.c_str());
    GLuint fs = compileShader(GL_FRAGMENT_SHADER, frag.c_str());

    // Cria o programa e liga os shaders compilados
    GLuint program = glCreateProgram();
    glAttachShader(program, vs);
    glAttachShader(program, fs);
    glLinkProgram(program); // Liga os shaders num único programa utilizável

    // Verifica se o link foi bem-sucedido
    GLint ok;
    glGetProgramiv(program, GL_LINK_STATUS, &ok);
    if (!ok) {
        char log[512];
        glGetProgramInfoLog(program, 512, nullptr, log);
        std::cerr << "Erro ao ligar programa shader: " << log << '\n';
    }

    // Liberta os shaders individuais (já não são necessários depois do link)
    glDeleteShader(vs);
    glDeleteShader(fs);

    return program;
}