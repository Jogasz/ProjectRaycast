#version 330 core

layout (location = 0) in vec4 aPos;
layout (location = 1) in vec3 aColor;

out vec3 quadColor;
out vec2 vUv;

uniform mat4 uProjMat;

void main()
{
    int corner = gl_VertexID % 4;
    vec2 pos;

    if (corner == 0)      { pos = vec2(aPos.x, aPos.z); vUv = vec2(0.0, 0.0); }
    else if (corner == 1) { pos = vec2(aPos.y, aPos.z); vUv = vec2(1.0, 0.0); }
    else if (corner == 2) { pos = vec2(aPos.x, aPos.w); vUv = vec2(0.0, 1.0); }
    else                  { pos = vec2(aPos.y, aPos.w); vUv = vec2(1.0, 1.0); }

    gl_Position = uProjMat * vec4(pos.xy, 0.0, 1.0);
    quadColor = aColor;
}