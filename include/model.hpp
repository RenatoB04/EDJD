#pragma once

#include <string>
#include <GL/glew.h>

class Model {
public:
    Model(const std::string& objPath);
    ~Model();

    void draw(GLuint shaderProgram) const;

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

    void loadOBJ(const std::string& path);
    void loadMTL(const std::string& path);
    void loadTexture(const std::string& texturePath);
};
