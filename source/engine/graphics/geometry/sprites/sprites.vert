//Declaration of shader version and core profile functionality
    //Vertex shader is translating and constructing verticies
#version 330 core
//==============================================================
//Incoming verticies and colors
    //Verticies X1 X2 Y1 Y2 per instance
layout (location = 0) in vec4 aPos;
    //Colors per instance (same color for 4 vertex)
layout (location = 1) in float aTexId;
//==============================================================
//In-and outgoing variables
out float vTexId;

uniform mat4 uProjection;

void main()
{
    // We draw 4 vertices per instance (DrawArraysInstanced with count = 4).
    // Use gl_VertexID % 4 to pick corner. The order chosen works with TriangleStrip:
    // 0 -> (x1, y1)  (top-right)
    // 1 -> (x2, y1)  (top-left)
    // 2 -> (x1, y2)  (bottom-right)
    // 3 -> (x2, y2)  (bottom-left)
    int corner = gl_VertexID % 4;
    vec2 pos;

        //Constructing 4 vertex points from the datas
    if (corner == 0)      pos = vec2(aPos.x, aPos.z); // x1,y1
    else if (corner == 1) pos = vec2(aPos.y, aPos.z); // x2,y1
    else if (corner == 2) pos = vec2(aPos.x, aPos.w); // x1,y2
    else                  pos = vec2(aPos.y, aPos.w); // x2,y2

    gl_Position = uProjection * vec4(pos.xy, 0.0, 1.0);
    vTexId = aTexId;
}