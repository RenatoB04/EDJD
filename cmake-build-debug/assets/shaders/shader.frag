#version 330 core

in vec2 TexCoord;
out vec4 FragColor;

uniform sampler2D texture1;
uniform bool useTexture;

void main()
{
    if (useTexture)
    FragColor = texture(texture1, TexCoord);
    else
    FragColor = vec4(0.0, 0.3, 0.0, 1.0);
}
