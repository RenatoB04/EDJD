#include "model.hpp"
#include <iostream>
#include <fstream>
#include <sstream>
#include <vector>
#include <string>

#include <glm/glm.hpp>
#include <glm/gtc/type_ptr.hpp>

Model::Model(const std::string& objPath)
    : vao(0), vboVertices(0), vboNormals(0), vboTexCoords(0), textureID(0), indexCount(0)
{
    std::cout << "[Model] Criado modelo com caminho: " << objPath << std::endl;

    loadOBJ(objPath);
}

Model::~Model()
{
    if (vboVertices) glDeleteBuffers(1, &vboVertices);
    if (vboNormals) glDeleteBuffers(1, &vboNormals);
    if (vboTexCoords) glDeleteBuffers(1, &vboTexCoords);
    if (vao) glDeleteVertexArrays(1, &vao);
    if (textureID) glDeleteTextures(1, &textureID);

    std::cout << "[Model] Recursos libertados." << std::endl;
}

void Model::draw(GLuint shaderProgram) const
{
    if (vao) {
        glUseProgram(shaderProgram);
        glBindVertexArray(vao);
        glDrawArrays(GL_TRIANGLES, 0, indexCount);
        glBindVertexArray(0);
    }
}

void Model::loadOBJ(const std::string& path)
{
    std::ifstream file(path);
    if (!file.is_open()) {
        std::cerr << "[Model] Erro ao abrir ficheiro OBJ: " << path << std::endl;
        return;
    }

    std::vector<glm::vec3> temp_positions;
    std::vector<glm::vec3> temp_normals;
    std::vector<glm::vec2> temp_texcoords;

    std::vector<glm::vec3> final_positions;
    std::vector<glm::vec3> final_normals;
    std::vector<glm::vec2> final_texcoords;

    std::string line;
    while (std::getline(file, line)) {
        std::istringstream iss(line);

        std::string prefix;
        iss >> prefix;

        if (prefix == "v") {
            glm::vec3 position;
            iss >> position.x >> position.y >> position.z;
            temp_positions.push_back(position);
        }
        else if (prefix == "vn") {
            glm::vec3 normal;
            iss >> normal.x >> normal.y >> normal.z;
            temp_normals.push_back(normal);
        }
        else if (prefix == "vt") {
            glm::vec2 texcoord;
            iss >> texcoord.x >> texcoord.y;
            temp_texcoords.push_back(texcoord);
        }
        else if (prefix == "f") {
            unsigned int vIdx[3], tIdx[3], nIdx[3];
            char slash;

            for (int i = 0; i < 3; i++) {
                iss >> vIdx[i] >> slash >> tIdx[i] >> slash >> nIdx[i];
                final_positions.push_back(temp_positions[vIdx[i] - 1]);
                final_texcoords.push_back(temp_texcoords[tIdx[i] - 1]);
                final_normals.push_back(temp_normals[nIdx[i] - 1]);
            }
        }
    }

    file.close();

    indexCount = static_cast<GLuint>(final_positions.size());


    glGenVertexArrays(1, &vao);
    glBindVertexArray(vao);

    glGenBuffers(1, &vboVertices);
    glBindBuffer(GL_ARRAY_BUFFER, vboVertices);
    glBufferData(GL_ARRAY_BUFFER, final_positions.size() * sizeof(glm::vec3), final_positions.data(), GL_STATIC_DRAW);
    glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, sizeof(glm::vec3), (void*)0);
    glEnableVertexAttribArray(0);

    glGenBuffers(1, &vboNormals);
    glBindBuffer(GL_ARRAY_BUFFER, vboNormals);
    glBufferData(GL_ARRAY_BUFFER, final_normals.size() * sizeof(glm::vec3), final_normals.data(), GL_STATIC_DRAW);
    glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, sizeof(glm::vec3), (void*)0);
    glEnableVertexAttribArray(1);

    glGenBuffers(1, &vboTexCoords);
    glBindBuffer(GL_ARRAY_BUFFER, vboTexCoords);
    glBufferData(GL_ARRAY_BUFFER, final_texcoords.size() * sizeof(glm::vec2), final_texcoords.data(), GL_STATIC_DRAW);
    glVertexAttribPointer(2, 2, GL_FLOAT, GL_FALSE, sizeof(glm::vec2), (void*)0);
    glEnableVertexAttribArray(2);

    glBindVertexArray(0);

    std::cout << "[Model] OBJ carregado para GPU: " << indexCount << " vertices." << std::endl;
}