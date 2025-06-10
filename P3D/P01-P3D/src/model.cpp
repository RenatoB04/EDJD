#include "model.hpp"
#include <iostream>
#include <fstream>
#include <sstream>

#define STB_IMAGE_IMPLEMENTATION
#include <stb_image.h>

#include <glm/glm.hpp>
#include <glm/gtc/type_ptr.hpp>
#include <glm/gtc/matrix_transform.hpp>

namespace RendererLib {

    // Construtor: inicializa todos os identificadores como zero
    Model::Model() : vao(0), vboVertices(0), vboNormals(0), vboTexCoords(0), textureID(0), indexCount(0) {}

    // Destrutor: liberta recursos da GPU
    Model::~Model() {
        if (vboVertices) glDeleteBuffers(1, &vboVertices);
        if (vboNormals)  glDeleteBuffers(1, &vboNormals);
        if (vboTexCoords) glDeleteBuffers(1, &vboTexCoords);
        if (vao)         glDeleteVertexArrays(1, &vao);
        if (textureID)   glDeleteTextures(1, &textureID);
        std::cout << "[Model] Recursos libertados.\n";
    }

    // Carrega um ficheiro OBJ (geometria do modelo)
    bool Model::Load(const std::string& objPath) {
        std::ifstream file(objPath);
        if (!file) {
            std::cerr << "[Model] Erro ao abrir OBJ: " << objPath << '\n';
            return false;
        }

        // Diretório onde está o ficheiro, usado para localizar o MTL ou textura
        directory = objPath.substr(0, objPath.find_last_of("/\\") + 1);

        std::string line;
        while (std::getline(file, line)) {
            std::istringstream iss(line);
            std::string prefix;
            iss >> prefix;

            // Processa diferentes tipos de linha no OBJ
            if (prefix == "mtllib") {
                std::string mtlName; iss >> mtlName;
                materialFile = directory + mtlName;
            } else if (prefix == "v") {
                glm::vec3 pos; iss >> pos.x >> pos.y >> pos.z;
                temp_positions.push_back(pos);
            } else if (prefix == "vn") {
                glm::vec3 n; iss >> n.x >> n.y >> n.z;
                temp_normals.push_back(n);
            } else if (prefix == "vt") {
                glm::vec2 uv; iss >> uv.x >> uv.y;
                temp_texcoords.push_back(uv);
            } else if (prefix == "f") {
                // Faces com índice de posição, texcoord e normal
                for (int i = 0; i < 3; ++i) {
                    unsigned v, t, n; char sep;
                    iss >> v >> sep >> t >> sep >> n;
                    positions.push_back(temp_positions[v - 1]);
                    texcoords.push_back(temp_texcoords[t - 1]);
                    normals.push_back(temp_normals[n - 1]);
                }
            }
        }

        file.close();

        // Carrega material e textura, se existirem
        if (!materialFile.empty()) loadMTL(materialFile);
        if (!textureFile.empty())  loadTexture(textureFile);

        return true;
    }

    // Envia os dados do modelo para a GPU e configura os atributos
    void Model::Install() {
        indexCount = static_cast<GLuint>(positions.size());

        glGenVertexArrays(1, &vao);
        glBindVertexArray(vao);

        // Vértices
        glGenBuffers(1, &vboVertices);
        glBindBuffer(GL_ARRAY_BUFFER, vboVertices);
        glBufferData(GL_ARRAY_BUFFER, positions.size() * sizeof(glm::vec3), positions.data(), GL_STATIC_DRAW);
        glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 0, nullptr);
        glEnableVertexAttribArray(0);

        // Normais
        glGenBuffers(1, &vboNormals);
        glBindBuffer(GL_ARRAY_BUFFER, vboNormals);
        glBufferData(GL_ARRAY_BUFFER, normals.size() * sizeof(glm::vec3), normals.data(), GL_STATIC_DRAW);
        glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 0, nullptr);
        glEnableVertexAttribArray(1);

        // Coordenadas de textura
        glGenBuffers(1, &vboTexCoords);
        glBindBuffer(GL_ARRAY_BUFFER, vboTexCoords);
        glBufferData(GL_ARRAY_BUFFER, texcoords.size() * sizeof(glm::vec2), texcoords.data(), GL_STATIC_DRAW);
        glVertexAttribPointer(2, 2, GL_FLOAT, GL_FALSE, 0, nullptr);
        glEnableVertexAttribArray(2);

        glBindVertexArray(0);

        std::cout << "[Model] Modelo instalado na GPU: " << indexCount << " vertices.\n";
    }

    // Liga atributos personalizados do shader aos VBOs do modelo
    void Model::BindAttributes(GLuint shaderProgram) {
        glBindVertexArray(vao);

        auto bind = [&](const char* name, GLuint vbo, GLint size) {
            GLint loc = glGetAttribLocation(shaderProgram, name);
            if (loc >= 0) {
                glEnableVertexAttribArray(loc);
                glBindBuffer(GL_ARRAY_BUFFER, vbo);
                glVertexAttribPointer(loc, size, GL_FLOAT, GL_FALSE, 0, nullptr);
            }
        };

        bind("aPos",      vboVertices,  3);
        bind("aNormal",   vboNormals,   3);
        bind("aTexCoord", vboTexCoords, 2);

        glBindVertexArray(0);
    }

    // Renderiza o modelo na posição e rotação indicadas
    void Model::Render(const glm::vec3& pos, const glm::vec3& rot, GLuint shader, float ang) const {
        if (!vao) return;

        glm::mat4 model = glm::mat4(1.0f);
        model = glm::rotate(model, ang,         glm::vec3(0, 1, 0));  // rotação animada
        model = glm::translate(model, pos);                          // posição
        model = glm::rotate(model, rot.y,       glm::vec3(0, 1, 0));  // rotação Y
        model = glm::rotate(model, rot.x,       glm::vec3(1, 0, 0));  // rotação X
        model = glm::rotate(model, rot.z,       glm::vec3(0, 0, 1));  // rotação Z
        model = glm::scale(model, glm::vec3(0.3f));                  // escala fixa

        glUniformMatrix4fv(glGetUniformLocation(shader, "model"), 1, GL_FALSE, glm::value_ptr(model));

        if (textureID) {
            glActiveTexture(GL_TEXTURE0);
            glBindTexture(GL_TEXTURE_2D, textureID);
            glUniform1i(glGetUniformLocation(shader, "texture1"), 0);
        }

        glBindVertexArray(vao);
        glDrawArrays(GL_TRIANGLES, 0, indexCount);
        glBindVertexArray(0);
    }

    // Lê ficheiro .mtl e extrai a textura, se especificada
    void Model::loadMTL(const std::string& path) {
        std::ifstream file(path);
        if (!file) {
            std::cerr << "[Model] Erro ao abrir MTL: " << path << '\n';
            return;
        }

        std::string line;
        while (std::getline(file, line)) {
            std::istringstream iss(line);
            std::string prefix;
            iss >> prefix;

            if (prefix == "map_Kd") {
                std::string texName;
                iss >> texName;
                textureFile = directory + texName;
                break; // Só usa a primeira textura difusa encontrada
            }
        }

        file.close();
    }

    // Carrega a textura do ficheiro e configura-a no OpenGL
    void Model::loadTexture(const std::string& texPath) {
        int w, h, ch;
        stbi_set_flip_vertically_on_load(true);
        unsigned char* data = stbi_load(texPath.c_str(), &w, &h, &ch, 0);

        if (!data) {
            std::cerr << "[Model] Erro ao carregar textura: " << texPath << '\n';
            return;
        }

        glGenTextures(1, &textureID);
        glBindTexture(GL_TEXTURE_2D, textureID);

        GLenum format = (ch == 4) ? GL_RGBA : GL_RGB;
        glTexImage2D(GL_TEXTURE_2D, 0, format, w, h, 0, format, GL_UNSIGNED_BYTE, data);
        glGenerateMipmap(GL_TEXTURE_2D);

        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR);
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);

        stbi_image_free(data);

        std::cout << "[Model] Textura carregada: " << texPath << '\n';
    }

}