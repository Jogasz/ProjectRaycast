#version 330 core

in vec2 vUv;
out vec4 FragColor;

uniform sampler2D uImages[2];

void main()
{
    FragColor = texture(uImages[1], vUv);
}