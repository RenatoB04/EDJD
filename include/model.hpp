#pragma once

#include <string>
#include <vector>
#include <GL/glew.h>
#include <glm/glm.hpp>

namespace RendererLib {

    // Classe que representa um modelo 3D simples (formato OBJ)
    class Model {
    public:
        Model();   // Construtor
        ~Model();  // Destrutor (liberta recursos da GPU)

        // Carrega um modelo a partir de um ficheiro .obj
        bool Load(const std::string& objPath);

        // Envia os dados carregados para a GPU (VAO + VBOs)
        void Install();

        // Renderiza o modelo com posição, orientação, shader e ângulo animado
        void Render(const glm::vec3& position, const glm::vec3& orientation, GLuint shader, float angulo) const;

        // Liga atributos personalizados do shader aos dados do modelo
        void BindAttributes(GLuint shader);

    private:
        // Identificadores OpenGL
        GLuint vao = 0;           // Vertex Array Object
        GLuint vboVertices = 0;   // VBO para posições
        GLuint vboNormals = 0;    // VBO para normais
        GLuint vboTexCoords = 0;  // VBO para coordenadas de textura
        GLuint textureID = 0;     // Textura carregada
        GLuint indexCount = 0;    // Número de vértices

        // Caminhos de ficheiros auxiliares
        std::string directory;     // Diretório do ficheiro OBJ
        std::string materialFile;  // Caminho do ficheiro .mtl
        std::string textureFile;   // Caminho da textura

        // Dados temporários e finais para posições, normais e coordenadas de textura
        std::vector<glm::vec3> temp_positions, temp_normals;
        std::vector<glm::vec3> positions, normals;
        std::vector<glm::vec2> temp_texcoords, texcoords;

        // Funções auxiliares
        void loadMTL(const std::string& path);      // Lê o ficheiro MTL
        void loadTexture(const std::string& path);  // Carrega a textura associada
    };

}