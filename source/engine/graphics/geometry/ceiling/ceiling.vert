//Vertex shader is translating and constructing verticies

//Declaration of shader version and core profile functionality
//==============================================================
#version 330 core
//==============================================================

//Incoming and outgoing variables, uniforms
//==============================================================
    //Verticies X1, X2, Y1, Y2, per instance
layout (location = 0) in vec4 _aStripQuadPos;
    //Ray's angle (in)
layout (location = 1) in float _rayAngle;

    //Strip quad Y1, Y2 (out)
out vec2 vStripQuadY;
    //Ray's angle (out)
out float rayAngle;

    //Projection matrix
uniform mat4 uProjMat;
//==============================================================

//Entry point
//==============================================================
void main()
{
    //Constructing quad from vertices
    //=========================================================================================
        //Translator:
        // - 4 vertices per instance
        // 0: (X1, Y1) - top-right
        // 1: (X2, Y1) - top-left
        // 2: (X1, Y2) - bottom-right
        // 3: (X2, Y2) - bottom-left

        //Corner instancing
    int corner = gl_VertexID % 4;
        //Converted/constructed vertex position's outgoing variable
    vec2 convStripQuadPos;
        //Constructing 4 vertex points (quad) from the datas
    if      (corner == 0) convStripQuadPos = vec2(_aStripQuadPos.x, _aStripQuadPos.z); // x1,y1
    else if (corner == 1) convStripQuadPos = vec2(_aStripQuadPos.y, _aStripQuadPos.z); // x2,y1
    else if (corner == 2) convStripQuadPos = vec2(_aStripQuadPos.x, _aStripQuadPos.w); // x1,y2
    else                  convStripQuadPos = vec2(_aStripQuadPos.y, _aStripQuadPos.w); // x2,y2
    //=========================================================================================

    //Setting projection matrix
    //===========================================================
    gl_Position = uProjMat * vec4(convStripQuadPos.xy, 0.0, 1.0);
    //===========================================================

    //Sending out variables to fragment shadercallcoherent
    //==============================
        //Strip quad Y1, Y2 (out)
    vStripQuadY = _aStripQuadPos.zw;
        //Ray's angle (out)
    rayAngle = _rayAngle;
    //==============================
}
//==============================================================