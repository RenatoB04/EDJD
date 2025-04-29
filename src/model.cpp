#include "model.hpp"
#include <iostream>

Model::Model(const std::string& objPath)
    : vao(0), vboVertices(0), vboNormals(0), vboTexCoords(0), textureID(0)
{
    std::cout << "[Model] Criado modelo com caminho: " << objPath << std::endl;

    // loadOBJ(objPath);
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
    /*
    if (vao) {
        glUseProgram(shaderProgram);
        glBindVertexArray(vao);
        glDrawElements(GL_TRIANGLES, *NUMERO DE INDICES* , GL_UNSIGNED_INT, 0);
        glBindVertexArray(0);
    }
    */
}