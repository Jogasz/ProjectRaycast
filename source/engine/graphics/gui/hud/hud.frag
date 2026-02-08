//Fragment shader is calculating the color output for the pixels
#version 330 core
//==============================================================

in float texIndex;
in vec2 vUv;

uniform sampler2D uTextures[3];

out vec4 FragColor;

void main()
{
 vec4 tex = texture(uTextures[int(texIndex)], vUv);
 if (tex.a <=0.0 || distance(tex.rgb, vec3(255,0,220) /255) <0.1) discard;
 FragColor = tex;
}
//==============================================================
