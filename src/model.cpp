#include "model.hpp"
#include <iostream>
#include <fstream>
#include <sstream>

#define STB_IMAGE_IMPLEMENTATION
#include <stb_image.h>

#include <glm/glm.hpp>
#include <glm/gtc/type_ptr.hpp>

namespace RendererLib {

Model::Model()
    : vao(0), vboVertices(0), vboNormals(0), vboTexCoords(0), textureID(0), indexCount(0) {}

Model::~Model() {
    if (vboVertices) glDeleteBuffers(1, &vboVertices);
    if (vboNormals) glDeleteBuffers(1, &vboNormals);
    if (vboTexCoords) glDeleteBuffers(1, &vboTexCoords);
    if (vao) glDeleteVertexArrays(1, &vao);
    if (textureID) glDeleteTextures(1, &textureID);
    std::cout << "[Model] Recursos libertados.\n";
}

bool Model::Load(const std::string& objPath) {
    std::ifstream file(objPath);
    if (!file.is_open()) {
        std::cerr << "[Model] Erro ao abrir ficheiro OBJ: " << objPath << std::endl;
        return false;
    }

    size_t lastSlash = objPath.find_last_of("/\\");
    directory = (lastSlash != std::string::npos) ? objPath.substr(0, lastSlash + 1) : "";

    std::string line;
    while (std::getline(file, line)) {
        std::istringstream iss(line);
        std::string prefix;
        iss >> prefix;

        if (prefix == "mtllib") {
            std::string mtlName;
            iss >> mtlName;
            materialFile = directory + mtlName;
        } else if (prefix == "v") {
            glm::vec3 pos;
            iss >> pos.x >> pos.y >> pos.z;
            temp_positions.push_back(pos);
        } else if (prefix == "vn") {
            glm::vec3 normal;
            iss >> normal.x >> normal.y >> normal.z;
            temp_normals.push_back(normal);
        } else if (prefix == "vt") {
            glm::vec2 tex;
            iss >> tex.x >> tex.y;
            temp_texcoords.push_back(tex);
        } else if (prefix == "f") {
            unsigned int vIdx[3], tIdx[3], nIdx[3];
            char slash;

            for (int i = 0; i < 3; ++i) {
                iss >> vIdx[i] >> slash >> tIdx[i] >> slash >> nIdx[i];
                positions.push_back(temp_positions[vIdx[i] - 1]);
                texcoords.push_back(temp_texcoords[tIdx[i] - 1]);
                normals.push_back(temp_normals[nIdx[i] - 1]);
            }
        }
    }

    file.close();

    if (!materialFile.empty()) {
        loadMTL(materialFile);
    }

    if (!textureFile.empty()) {
        loadTexture(textureFile);
    }

    return true;
}

void Model::Install() {
    indexCount = static_cast<GLuint>(positions.size());

    glGenVertexArrays(1, &vao);
    glBindVertexArray(vao);

    glGenBuffers(1, &vboVertices);
    glBindBuffer(GL_ARRAY_BUFFER, vboVertices);
    glBufferData(GL_ARRAY_BUFFER, positions.size() * sizeof(glm::vec3), positions.data(), GL_STATIC_DRAW);
    glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, sizeof(glm::vec3), (void*)0);
    glEnableVertexAttribArray(0);

    glGenBuffers(1, &vboNormals);
    glBindBuffer(GL_ARRAY_BUFFER, vboNormals);
    glBufferData(GL_ARRAY_BUFFER, normals.size() * sizeof(glm::vec3), normals.data(), GL_STATIC_DRAW);
    glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, sizeof(glm::vec3), (void*)0);
    glEnableVertexAttribArray(1);

    glGenBuffers(1, &vboTexCoords);
    glBindBuffer(GL_ARRAY_BUFFER, vboTexCoords);
    glBufferData(GL_ARRAY_BUFFER, texcoords.size() * sizeof(glm::vec2), texcoords.data(), GL_STATIC_DRAW);
    glVertexAttribPointer(2, 2, GL_FLOAT, GL_FALSE, sizeof(glm::vec2), (void*)0);
    glEnableVertexAttribArray(2);

    glBindVertexArray(0);

    std::cout << "[Model] Modelo instalado na GPU: " << indexCount << " vertices." << std::endl;
}

void Model::BindAttributes(GLuint shaderProgram) {
    glBindVertexArray(vao);

    GLint posLoc = glGetAttribLocation(shaderProgram, "aPos");
    if (posLoc >= 0) {
        glEnableVertexAttribArray(posLoc);
        glBindBuffer(GL_ARRAY_BUFFER, vboVertices);
        glVertexAttribPointer(posLoc, 3, GL_FLOAT, GL_FALSE, 0, (void*)0);
    }

    GLint normLoc = glGetAttribLocation(shaderProgram, "aNormal");
    if (normLoc >= 0) {
        glEnableVertexAttribArray(normLoc);
        glBindBuffer(GL_ARRAY_BUFFER, vboNormals);
        glVertexAttribPointer(normLoc, 3, GL_FLOAT, GL_FALSE, 0, (void*)0);
    }

    GLint texLoc = glGetAttribLocation(shaderProgram, "aTexCoord");
    if (texLoc >= 0) {
        glEnableVertexAttribArray(texLoc);
        glBindBuffer(GL_ARRAY_BUFFER, vboTexCoords);
        glVertexAttribPointer(texLoc, 2, GL_FLOAT, GL_FALSE, 0, (void*)0);
    }

    glBindVertexArray(0);
}

void Model::Render(const glm::vec3& position, const glm::vec3& orientation, GLuint shaderProgram) const {
    if (!vao) return;

    glm::mat4 model = glm::mat4(1.0f);
    model = glm::translate(model, position);
    model = glm::rotate(model, orientation.y, glm::vec3(0, 1, 0));
    model = glm::rotate(model, orientation.x, glm::vec3(1, 0, 0));
    model = glm::rotate(model, orientation.z, glm::vec3(0, 0, 1));
    model = glm::scale(model, glm::vec3(0.3f));

    GLint modelLoc = glGetUniformLocation(shaderProgram, "model");
    glUniformMatrix4fv(modelLoc, 1, GL_FALSE, glm::value_ptr(model));

    if (textureID) {
        glActiveTexture(GL_TEXTURE0);
        glBindTexture(GL_TEXTURE_2D, textureID);
        glUniform1i(glGetUniformLocation(shaderProgram, "texture1"), 0);
    }

    glBindVertexArray(vao);
    glDrawArrays(GL_TRIANGLES, 0, indexCount);
    glBindVertexArray(0);
}

void Model::loadMTL(const std::string& path) {
    std::ifstream file(path);
    if (!file.is_open()) {
        std::cerr << "[Model] Erro ao abrir ficheiro MTL: " << path << std::endl;
        return;
    }

    std::string line;
    while (std::getline(file, line)) {
        std::istringstream iss(line);
        std::string prefix;
        iss >> prefix;

        if (prefix == "map_Kd") {
            std::string textureName;
            iss >> textureName;
            textureFile = directory + textureName;
            break;
        }
    }

    file.close();
}

void Model::loadTexture(const std::string& texturePath) {
    int width, height, nrChannels;
    stbi_set_flip_vertically_on_load(true);
    unsigned char* data = stbi_load(texturePath.c_str(), &width, &height, &nrChannels, 0);

    if (!data) {
        std::cerr << "[Model] Erro ao carregar textura: " << texturePath << std::endl;
        return;
    }

    glGenTextures(1, &textureID);
    glBindTexture(GL_TEXTURE_2D, textureID);

    GLenum format = (nrChannels == 4) ? GL_RGBA : GL_RGB;

    glTexImage2D(GL_TEXTURE_2D, 0, format, width, height, 0, format, GL_UNSIGNED_BYTE, data);
    glGenerateMipmap(GL_TEXTURE_2D);

    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);

    stbi_image_free(data);

    std::cout << "[Model] Textura carregada: " << texturePath << std::endl;
}

}