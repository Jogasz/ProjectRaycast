//Vertex shader is translating and constructing verticies

//Declaration of shader version and core profile functionality
//==============================================================
#version 330 core
//==============================================================

//Incoming and outgoing variables, uniforms
//==============================================================
    //Verticies X1, X2, Y1, Y2, per instance (in)
layout (location = 0) in vec4 _aStripQuadPos;
    //Wall's height (in)
layout (location = 1) in float _aWallHeight;
    //Ray's length (in)
layout (location = 2) in float _aRayLength;
    //Ray's pos in tile (in)
layout (location = 3) in float _aRayTilePos;
    //Texture's index (in)
layout (location = 4) in float _aTexIndex;

    //Strip quad Y1, Y2 (out)
out vec2 vStripQuadY;
    //Wall's height (out)
out float vWallHeight;
    //Ray's length (out)
out float vRayLength;
    //Ray's pos in tile (out)
out float vRayTilePos;
    //Texture's index (out)
out float vTexIndex;

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
         //Wall's height (out)
    vWallHeight = _aWallHeight;
        //Ray's length (out)
    vRayLength = _aRayLength;
        //Ray's pos in tile (out)
    vRayTilePos = _aRayTilePos;
        //Texture's index (out)
    vTexIndex = _aTexIndex;
    //==============================
}
//==============================================================