#pragma once

#include <string>
#include <vector>
#include <GL/glew.h>
#include <glm/glm.hpp>

namespace RendererLib {

    // Classe que representa um modelo 3D carregado a partir de um ficheiro OBJ
    class Model {
    public:
        Model();        // Construtor: inicializa os membros da classe
        ~Model();       // Destrutor: liberta os recursos alocados

        // Carrega um modelo 3D a partir de um ficheiro OBJ
        bool Load(const std::string& objPath);

        // Instala o modelo na GPU, criando e preenchendo os buffers necessários
        void Install();

        // Renderiza o modelo na cena, aplicando transformações e textura
        void Render(const glm::vec3& position, const glm::vec3& orientation, GLuint shaderProgram) const;

        // Associa os buffers do modelo aos atributos do shader
        void BindAttributes(GLuint shaderProgram);

    private:
        // Identificadores dos buffers e da textura na GPU
        GLuint vao;             // Vertex Array Object
        GLuint vboVertices;     // Buffer de vértices (posições)
        GLuint vboNormals;      // Buffer de normais
        GLuint vboTexCoords;    // Buffer de coordenadas de textura
        GLuint textureID;       // Identificador da textura
        GLuint indexCount;      // Número de índices/vertices a renderizar

        // Caminhos e nomes de ficheiros
        std::string directory;   // Diretório do ficheiro OBJ
        std::string materialFile; // Caminho do ficheiro de material (MTL)
        std::string textureFile;  // Caminho do ficheiro de textura

        // Listas temporárias para guardar posições, normais e coordenadas de textura lidas do OBJ
        std::vector<glm::vec3> temp_positions;
        std::vector<glm::vec3> temp_normals;
        std::vector<glm::vec2> temp_texcoords;

        // Listas finais de posições, normais e coordenadas de textura dos vértices a renderizar
        std::vector<glm::vec3> positions;
        std::vector<glm::vec3> normals;
        std::vector<glm::vec2> texcoords;

        // Carrega o ficheiro de material (MTL) para obter o nome da textura
        void loadMTL(const std::string& path);

        // Carrega a textura a partir de uma imagem
        void loadTexture(const std::string& texturePath);
    };

}