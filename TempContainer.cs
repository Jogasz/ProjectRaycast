//v1
//void CeilingCast(int i, float wallHeight, float rayAngle)
//{
//    float quadX1 = screenHorizontalOffset + (i * wallWidth);
//    float quadX2 = screenHorizontalOffset + ((i + 1) * wallWidth);

//    float yStep = wallWidth;

//    float quadY1 = screenVerticalOffset;
//    float quadY2 = screenVerticalOffset + yStep;

//    //Height of the player
//    float cameraZ = ClientSize.Y / 2;

//    int[][] path;

//    while (quadY1 < (screenVerticalOffset + ((minimumScreenHeight - wallHeight) / 2)) + pitch)
//    {
//        /* 
//        * If the quad's top position is in the correct screen, but the bottom position is in the wall,
//        * the bottom position's Y value may be equal to the wall's top value.
//        */
//        if (quadY2 > (screenVerticalOffset + ((minimumScreenHeight - wallHeight) / 2)) + pitch)
//        {
//            quadY2 = screenVerticalOffset + ((minimumScreenHeight - wallHeight) / 2) + pitch;
//        }

//        //Y of the current quad on the screen
//        float rowY = (ClientSize.Y / 2) - (quadY1 + ((quadY2 - quadY1) / 2)) + pitch;

//        //Distance of the pixel from the player
//        float ceilingPixelDistance = ((cameraZ / rowY) * tileSize) / (float)Math.Cos(playerAngle - rayAngle);

//        //World X position of the pixel
//        float ceilingPixelX = playerPosition.X + ((float)Math.Cos(rayAngle) * ceilingPixelDistance);

//        //World Y position of the pixel
//        float ceilingPixelY = playerPosition.Y + ((float)Math.Sin(rayAngle) * ceilingPixelDistance);

//        //Texture
//        if
//        (
//            ceilingPixelX >= mapCeiling.GetLength(1) * tileSize ||
//            ceilingPixelX < 0f ||
//            ceilingPixelY >= mapCeiling.GetLength(0) * tileSize ||
//            ceilingPixelY < 0f
//        )
//        {
//            quadY1 = quadY2;
//            quadY2 += yStep;
//            continue;
//        }

//        path = TextureTranslator((int)mapCeiling[(int)Math.Floor(ceilingPixelY / tileSize), (int)Math.Floor(ceilingPixelX / tileSize)]);

//        if (path is null)
//        {
//            quadY1 = quadY2;
//            quadY2 += yStep;
//            continue;
//        }

//        //Calculating RGB variables
//        int RGBCalc = ((int)Math.Floor(path[0][1] / (tileSize / (ceilingPixelX % tileSize))) * path[0][1] * 3) +
//                  ((int)Math.Floor(path[0][0] / (tileSize / (ceilingPixelY % tileSize))) * 3);

//        //Calculating shading and lighting with distance
//        float shadeCalc = ceilingPixelDistance * distanceShade;

//        float debugBorder = 0.5f;

//        vertexAttributesList.AddRange(new float[]
//        {
//                            quadX1 + debugBorder,
//                            quadX2 - debugBorder,
//                            quadY1 + debugBorder,
//                            quadY2 - debugBorder,
//                            (path[1][RGBCalc] - shadeCalc) < 0 ? 0f : (path[1][RGBCalc] - shadeCalc) / 255f,
//                            (path[1][RGBCalc + 1] - shadeCalc) < 0 ? 0f : (path[1][RGBCalc + 1] - shadeCalc) / 255f,
//                            (path[1][RGBCalc + 2] - shadeCalc) < 0 ? 0f : (path[1][RGBCalc + 2] - shadeCalc) / 255f
//        });

//        //Incremental increasing of the top and bottom values of the pixel
//        quadY1 = quadY2;
//        quadY2 += yStep;
//    }
//}













//Shading by stepping
//    //Current pixel's Y divided by uStepSize gives the amount of shading the pixel needs
//    //max(uStepSize,1e-6):
//    //1e-6 is a scientific term for1 *10^-6 =0.000001, so uStepSize can't be smaller than or equal, only very close to zero
//    float YStepIndex = floor(pixelYInStrip / max(uStepSize,1e-6));
//
//    float shade = YStepIndex * 0.02;
//
//    // normalize current fragment position into 0..1 inside the quad
//    float u = (gl_FragCoord.x - stripX1) / max(1.0, (stripX2 - stripX1));
//    float v = (gl_FragCoord.y - stripY2) / max(1.0, (stripY1 - stripY2));
//
//    vec2 uv = clamp(vec2(u, v), 0.0, 1.0);
//
//    // always draw the first texture
//    vec4 tex = texture(uTextures[1], uv);
//
//    vec3 fClr = vec3(
//        tex.r - shade,
//        tex.g - shade,
//        tex.b - shade
//    );
//
//    FragColor = vec4(fClr, tex.a);
//
//    // Final outgoing color = texture * tint + step shading
//    //FragColor = vec4(clr, 1.0);