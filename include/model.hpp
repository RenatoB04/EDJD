#pragma once

#include <string>
#include <vector>
#include <GL/glew.h>
#include <glm/glm.hpp>

namespace RendererLib {

    class Model {
    public:
        Model();
        ~Model();

        bool Load(const std::string& objPath);
        void Install();
        void Render(const glm::vec3& position, const glm::vec3& orientation, GLuint shaderProgram) const;

    private:
        GLuint vao;
        GLuint vboVertices;
        GLuint vboNormals;
        GLuint vboTexCoords;
        GLuint textureID;
        GLuint indexCount;

        std::string directory;
        std::string materialFile;
        std::string textureFile;

        std::vector<glm::vec3> temp_positions;
        std::vector<glm::vec3> temp_normals;
        std::vector<glm::vec2> temp_texcoords;

        std::vector<glm::vec3> positions;
        std::vector<glm::vec3> normals;
        std::vector<glm::vec2> texcoords;

        void loadMTL(const std::string& path);
        void loadTexture(const std::string& texturePath);
    };

}