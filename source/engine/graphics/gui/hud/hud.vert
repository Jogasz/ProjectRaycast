//Vertex shader is translating and constructing verticies
#version 330 core
//==============================================================
//Incoming
layout (location =0) in vec4 aPos; // x1,x2,y1,y2
layout (location =1) in float aTexIndex;

//Outgoing
out float texIndex;
out vec2 vUv;

uniform mat4 uProjection;

void main()
{
 int corner = gl_VertexID %4;
 vec2 pos;

 if (corner ==0) pos = vec2(aPos.x, aPos.z);
 else if (corner ==1) pos = vec2(aPos.y, aPos.z);
 else if (corner ==2) pos = vec2(aPos.x, aPos.w);
 else pos = vec2(aPos.y, aPos.w);

 // Fullscreen UV
 if (corner ==0) vUv = vec2(0.0,1.0);
 else if (corner ==1) vUv = vec2(1.0,1.0);
 else if (corner ==2) vUv = vec2(0.0,0.0);
 else vUv = vec2(1.0,0.0);

 gl_Position = uProjection * vec4(pos.xy,0.0,1.0);
 texIndex = aTexIndex;
}
//==============================================================
