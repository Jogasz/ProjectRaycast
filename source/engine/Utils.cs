using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Engine;

internal partial class Engine
{
    public void ViewportSetUp(int width, int height)
    {
        GL.Viewport(0, 0, width, height);
        projection = Matrix4.CreateOrthographicOffCenter(0f, width, height, 0f, -1f, 1f);
        if (shader != null)
        {
            shader.Use();
            shader.SetMatrix4("uProjection", projection);
        }

        //Window's default color
        GL.ClearColor(0.2f, 0.2f, 0.2f, 1.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit);
    }

    float NormalizeAngle(float angle)
    {
        if (angle > MathX.Quadrant4)
        {
            angle -= MathX.Quadrant4;
        }

        else if (angle < 0)
        {
            angle += MathX.Quadrant4;
        }

        return angle;
    }

    void VertexLoader(float X1, float X2, float Y1, float Y2, float R, float G, float B)
    {

        /*
         *Translator for shaderVerticies:
         *
         * 0: X1
         * 1: X2
         * 2: Y1
         * 3: Y2
         * 4: R
         * 5: G
         * 6: B
         * 
         * (x2, y1) ┌──────┐ (x1, y1)
         *          │      │
         *          │      │
         * (x2, y2) └──────┘ (x1, y2)
         * 
         * 0 -> (x1, y1)  (top-right)
         * 1 -> (x2, y1)  (top-left)
         * 2 -> (x1, y2)  (bottom-right)
         * 3 -> (x2, y2)  (bottom-left)
         * 
         * first triangle: 0-1-2
         * second triangle: 2-1-3
         * 
         */

        vertexAttributesList.AddRange(new float[]
        {
            X1,
            X2,
            Y1,
            Y2,
            R,
            G,
            B
        });
    }

    public int[][] TextureTranslator(int x)
    {
        //No wall hit
        int[][] path = null;

        switch (x)
        {
            //Wall hit
            //Default textures
            case 1:
                path = Textures.planksTexture;
                break;
            case 2:
                path = Textures.mossyPlanksTexture;
                break;
            case 3:
                path = Textures.stoneBricksTexture;
                break;
            case 4:
                path = Textures.mossyStoneBricksTexture;
                break;

            //Door textures
            case 5:
                path = Textures.doorStoneBricksTexture;
                break;
            case 6:
                path = Textures.doorMossyStoneBricksTexture;
                break;

            //Window textures
            case 7:
                path = Textures.windowStoneBricksTexture;
                break;
            case 8:
                path = Textures.windowMossyStoneBricksTexture;
                break;

            //Painting textures
            case 9:
                path = Textures.painting_a_stoneBricksTexture;
                break;
            case 10:
                path = Textures.painting_b_stoneBricksTexture;
                break;
            case 11:
                path = Textures.painting_a_mossyStoneBricksTexture;
                break;
            case 12:
                path = Textures.painting_b_mossyStoneBricksTexture;
                break;

            //Test textures
            case 13:
                path = Textures.floorTestTexture;
                break;
        }
        return path;
    }
}
