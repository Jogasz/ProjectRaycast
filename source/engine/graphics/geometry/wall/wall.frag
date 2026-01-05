//Fragment shader is calculating the color output for the pixels

//Declaration of shader version and core profile functionality
//==============================================================
#version 330 core
//==============================================================

//Incoming and outgoing variables, uniforms
//==============================================================
    //Strip quad Y1, Y2 (in)
    // x: Y1
    // y: Y2
in vec2 vStripQuadY;
    //Wall's height (in)
in float vWallHeight;
    //Ray's length (in)
in float vRayLength;
    //Ray's pos in tile (in)
in float vRayTilePos;
    //Texture's index (in)
in float vTexIndex;
    //Wall's side (in)
in float vWallSide;

    //Final outgoing RGBA of the quad (out)
out vec4 FragColor;

    //Textures array (uIn)
uniform sampler2D uTextures[9];
    //Map Ceiling array (uIn)
uniform isampler2D uMapWalls;
    //Map's size (uIn)
uniform vec2 uMapSize;
//==============================================================

//Entry point
//==============================================================
void main()
{
        //Strip quad Y1 - top
    float stripY1 = vStripQuadY.x;
        //Strip quad Y2 - bottom
    float stripY2 = vStripQuadY.y;
        //Strip quad's height
    float stripQuadHeight = max(0.0, vWallHeight);
        //Current pixel's Y from strip quad's top (Clamp to avoid weird values on borders)
    float pixelYInStrip = clamp(stripY1 - gl_FragCoord.y, 0.0, stripQuadHeight);
        
        //Converting to INT !!!!!
    int texIndex = int(vTexIndex);

    //ToDo
        //UNIFORM float tileSize - Localizing TileSize
        //UNIFORM vec2 minimumScreenSize - Discarding not needed pixels
        //UNIFORM float distanceShade - Strength of shading
        //UNIFORM float texNum - How many textures in array + 0. dummy

    //Horizontal texture pixel position
        //Right side (no flip)
    float u = clamp(vRayTilePos / 50, 0.0, 1.0);
        //Wrong side (flip)
    if (vWallSide == 1 ||
        vWallSide == 3) u = clamp(1 - (vRayTilePos / 50), 0.0, 1.0);
        //Vertical texture pixel position
    float v = clamp(1 - (pixelYInStrip / stripQuadHeight), 0.0, 1.0);
        //Taking out the color from the selected texture
    vec4 tex = texture(uTextures[texIndex], vec2(u, v));
        //Returning the correct color
    FragColor = tex;
}
//==============================================================