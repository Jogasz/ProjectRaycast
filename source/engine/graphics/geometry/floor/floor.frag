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
    //Ray's angle (in)
in float rayAngle;

    //Final outgoing RGBA of the quad (out)
out vec4 FragColor;

//OnRenderFrame uniforms
//======================
    //Textures array (uIn)
uniform sampler2D uTextures[9];
    //Map Floor array (uIn)
uniform isampler2D uMapFloor;
    //Map's size (uIn)
uniform vec2 uMapSize;
    //Y step value for quads in strip quad (uIn)
uniform float uStepSize;
    //Player's position (uIn)
uniform vec2 uPlayerPos;
    //Player's angle (uIn)
uniform float uPlayerAngle;
    //Player's pitch (uIn)
uniform float uPitch;

//OnLoad / OnFramebufferResize uniforms
//=====================================
    //Minimum window's size (uIn)
uniform vec2 uMinimumScreen;
    //TileSize (uIn)
uniform float uTileSize;
    //Distance shade value (uIn)
uniform float uDistanceShade;
//==============================================================

//Entry point
//==============================================================
void main()
{
    //Screen positions
    //====================================================================================
        //Strip quad Y1 - top (near horizon for floor)
    float stripY1 = vStripQuadY.x;
        //Strip quad Y2 - bottom (screen bottom)
    float stripY2 = vStripQuadY.y;
        //Strip quad's height
    float stripQuadHeight = max(0.0, stripY1 - stripY2);
        //Current pixel's Y from strip quad's top (Clamp to avoid weird values on borders)
    float pixelYInStrip = clamp(stripY1 - gl_FragCoord.y, 0.0, stripQuadHeight);
        //Nth mini quad from the top in strip quad (starting from zero)
    float YStepIndex = floor(pixelYInStrip / uStepSize);
    //====================================================================================

    //World positions (flipped vs ceiling)
    //=======================================================================================================================
        //Height of the player
    float cameraZ = uMinimumScreen.y / 2;
        //Y of the miniQuad's middle (below horizon)
    float rowY = (uStepSize / 2 + YStepIndex * uStepSize) - uPitch;
        //Distance of the miniQuad from the player
    float floorPixelDistance = ((cameraZ / rowY) * uTileSize) / cos(uPlayerAngle - rayAngle);
        //World X position of the pixel
    float floorPixelX = uPlayerPos.x + (cos(rayAngle) * floorPixelDistance);
        //World Y position of the pixel
    float floorPixelY = uPlayerPos.y + (sin(rayAngle) * floorPixelDistance);
    //=======================================================================================================================

    //Coloring
    //=====================================================================================================================
        //Strength of shading by distance
    float distanceShade = uDistanceShade / 255;
        //Current miniQuad's shade value based on it's distance from the player
    float shade = floorPixelDistance * distanceShade;
        //Selecting texture based on map array
    int texIndex = texelFetch(uMapFloor, ivec2(floor(floorPixelX / uTileSize), floor(floorPixelY / uTileSize)), 0).r;
        //If index is zero, empty tile
    if (texIndex == 0)
    {
        FragColor = vec4(0.0, 1.0, 0.0, 1.0);
        return;
    }
        //Corresponding color's position in the selected texture
    vec2 uv = fract(vec2(floorPixelX, floorPixelY) / uTileSize);
        //Taking out the color from the selected texture
    vec4 tex = texture(uTextures[texIndex], uv);
        //Returning the correct color
    FragColor = vec4(tex - shade);
    //=====================================================================================================================
}
//==============================================================