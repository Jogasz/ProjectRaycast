//Fragment shader is calculating the color output for the pixels

//Declaration of shader version and core profile functionality
//==============================================================
#version 330 core
//==============================================================

//Incoming and outgoing variables, uniforms
//==============================================================
    //Strip quad rect (in)
    // x: X1
    // y: X2
    // z: Y1
    // w: Y2
in vec4 vStripQuadRect;
    //Strip quad color (in)
in vec3 aStripQuadClr;
    //Ray iteration value (in)
in float nthLine;

    //Final outgoing RGBA of the quad
out vec4 FragColor;

    //Y step value for quads in strip quad
uniform float uStepSize;
    //Textures
uniform sampler2D uTexture;
//==============================================================

//Entry point
//==============================================================
void main()
{
     //Strip quad vertexes
        //X1 - left
     float stripX1 = vStripQuadRect.x;
        //X2 - right
     float stripX2 = vStripQuadRect.y;
        //Y1 - top
     float stripY1 = vStripQuadRect.z;
        //Y2 - bottom
     float stripY2 = vStripQuadRect.w;

     //Strip quad's height
     float stripQuadHeight = max(0.0, stripY1 - stripY2);

     //Current pixel's Y from strip quad's top (Clamp to avoid weird values on borders)
     float pixelYInStrip = clamp(stripY1 - gl_FragCoord.y,0.0, stripQuadHeight);

     //Shading by stepping
     //Current pixel's Y divided by uStepSize gives the amount of shading the pixel needs
     //max(uStepSize,1e-6):
     //1e-6 is a scientific term for1 *10^-6 =0.000001, so uStepSize can't be smaller than or equal, only very close to zero
     float YStepIndex = floor(pixelYInStrip / max(uStepSize,1e-6));

     float shade = YStepIndex * 0.02;

     vec3 clr = vec3(
     0.0 + shade * 2,
     0.0 + shade * 2,
     1.0) - shade;

     // Final outgoing color = texture * tint + step shading
     FragColor = vec4(clr, 1.0);
 }
//==============================================================