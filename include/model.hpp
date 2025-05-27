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
        void Render(const glm::vec3& position, const glm::vec3& orientation, GLuint shader, float angulo) const;
        void BindAttributes(GLuint shader);

    private:
        GLuint vao = 0, vboVertices = 0, vboNormals = 0, vboTexCoords = 0, textureID = 0, indexCount = 0;

        std::string directory, materialFile, textureFile;

        std::vector<glm::vec3> temp_positions, temp_normals, positions, normals;
        std::vector<glm::vec2> temp_texcoords, texcoords;

        void loadMTL(const std::string& path);
        void loadTexture(const std::string& path);
    };

}