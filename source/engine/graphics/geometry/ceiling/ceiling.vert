//Vertex shader is translating and constructing verticies

//Declaration of shader version and core profile functionality
//==============================================================
#version 330 core
//==============================================================

//Incoming and outgoing variables, uniforms
//==============================================================
    //Verticies X1, X2, Y1, Y2, per instance
layout (location = 0) in vec4 _aStripQuadPos;
    //Colors per instance (same color for 4 vertex)
layout (location = 1) in vec3 _aStripQuadClr;
    //Ray iteration value (in)
layout (location = 2) in float _nthLine;

    //Strip quad rect (out)
out vec4 vStripQuadRect;
    //Strip quad color (out)
out vec3 aStripQuadClr;
    //Ray iteration value (out)
out float nthLine;

    //Projection matrix
uniform mat4 uProjMat;
//==============================================================

//Entry point
//==============================================================
void main()
{
    //4 vertices per instance
    //Corner translator:
    // 0: (X1, Y1) - top-right
    // 1: (X2, Y1) - top-left
    // 2: (X1, Y2) - bottom-right
    // 3: (X2, Y2) - bottom-left

        //Corner instancing
    int corner = gl_VertexID % 4;

        //Converted/constructed vertex positions
    vec2 convStripQuadPos;

        //Constructing 4 vertex points from the datas
    if      (corner == 0) convStripQuadPos = vec2(_aStripQuadPos.x, _aStripQuadPos.z); // x1,y1
    else if (corner == 1) convStripQuadPos = vec2(_aStripQuadPos.y, _aStripQuadPos.z); // x2,y1
    else if (corner == 2) convStripQuadPos = vec2(_aStripQuadPos.x, _aStripQuadPos.w); // x1,y2
    else                  convStripQuadPos = vec2(_aStripQuadPos.y, _aStripQuadPos.w); // x2,y2

        //Setting projection matrix
    gl_Position = uProjMat * vec4(convStripQuadPos.xy, 0.0, 1.0);

        //Strip quad rect (out)
    vStripQuadRect = _aStripQuadPos;
        //Strip quad color (out)
    aStripQuadClr = _aStripQuadClr;
        //Ray iteration value (out)
    nthLine = _nthLine;
}
//==============================================================