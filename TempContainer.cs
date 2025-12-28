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

//==============================================================================================================7

//v2
//void CeilingCast()
//{
//    //Y mentén lévő lépegetés mértéke
//    float yStep = (float)minimumScreenWidth / (float)rayCount;

//    //Quad's Y1 and Y2
//    float quadY1 = screenVerticalOffset;
//    float quadY2 = screenVerticalOffset + yStep;

//    //Height of the player (Middle of the screen)
//    int cameraZ = ClientSize.Y / 2;

//    //!!!TESZT!!!
//    //Unique raycount for each row depending on the distance
//    //====================================================
//    float aFirstRay = NormalizeAngle(playerAngle + FOVStart);
//    float aLastRay = NormalizeAngle(playerAngle - FOVStart);
//    //====================================================

//    //Sorokon megy végig
//    for (int i = 0; i < rayCount; i++)
//    {
//        //Jelenlegi sor Y-ja a képernyőn
//        float rowY = (ClientSize.Y / 2) - (quadY1 + ((quadY2 - quadY1) / 2)) + pitch;

//        //Distance between first and last ray's position on the map
//        //===================================================================================================
//        //First ray
//        //=========
//        //Distance between player and first ray world hit point
//        float dFirstRay = ((cameraZ / rowY) * tileSize) / (float)Math.Cos(playerAngle - aFirstRay);
//        //World position X
//        float firstRayPosX = playerPosition.X + ((float)Math.Cos(aFirstRay) * dFirstRay);
//        //World position Y
//        float firstRayPosY = playerPosition.Y + ((float)Math.Sin(aFirstRay) * dFirstRay);

//        //Last ray
//        //========
//        //Distance between player and last ray world hit point
//        float dLastRay = ((cameraZ / rowY) * tileSize) / (float)Math.Cos(playerAngle - aLastRay);
//        //World position X
//        float lastRayPosX = playerPosition.X + ((float)Math.Cos(aLastRay) * dLastRay);
//        //World position Y
//        float lastRayPosY = playerPosition.Y + ((float)Math.Sin(aLastRay) * dLastRay);

//        //Distance between the two rays
//        float dX = lastRayPosX - firstRayPosX;
//        float dY = lastRayPosY - firstRayPosY;
//        float rowD = (float)Math.Sqrt(dX * dX + dY * dY);
//        //===================================================================================================

//        float xStep = Math.Max(yStep, minimumScreenWidth / ((rowD / tileSize) * 36f));

//        float quadOffset = ((1 - ((rowD / tileSize * 36f) - (float)Math.Floor(rowD / tileSize * 36f))) / 2f) * xStep;

//        //Quad's X1 and X2
//        float quadX1 = screenHorizontalOffset - quadOffset;
//        float quadX2 = screenHorizontalOffset - quadOffset + xStep;

//        //Rad between rays based on final calculations of how many pixels will there be in a row
//        float tempRadBetweenRays = ((float)(FOV * (Math.PI / 180f)) / (minimumScreenWidth / xStep));

//        //Incrementing ray angle for the quads in a row
//        float incRayAngle = aFirstRay;

//        int incr = 0;
//        //A sorok pixelein megy végig
//        while (quadX1 < (screenHorizontalOffset + minimumScreenWidth))
//        {
//            quadX1 = quadX1 < screenHorizontalOffset ?
//                screenHorizontalOffset :
//                quadX1;

//            quadX2 = quadX2 > (screenHorizontalOffset + minimumScreenWidth) ?
//                (screenHorizontalOffset + minimumScreenWidth) :
//                quadX2;

//            //Distance of the floor pixel we are looking at
//            float ceilingPixelDistance = ((cameraZ / rowY) * tileSize) / (float)Math.Cos(playerAngle - incRayAngle);

//            //World X position of the pixel
//            float ceilingPixelX = playerPosition.X + ((float)Math.Cos(incRayAngle) * ceilingPixelDistance);

//            //World Y position of the pixel
//            float ceilingPixelY = playerPosition.Y + ((float)Math.Sin(incRayAngle) * ceilingPixelDistance);

//            //If it's outside of the map
//            //=========================================
//            if
//            (
//                ceilingPixelX >= mapCeiling.GetLength(1) * tileSize ||
//                ceilingPixelX < 0f ||
//                ceilingPixelY >= mapCeiling.GetLength(0) * tileSize ||
//                ceilingPixelY < 0f
//            )
//            {
//                quadX1 = quadX2;
//                quadX2 += xStep;
//                incRayAngle = NormalizeAngle(incRayAngle + tempRadBetweenRays);
//                continue;
//            }
//            //=========================================

//            //If it's inside the map, but index is zero
//            //=========================================
//            int[][] path = null;
//            path = TextureTranslator((int)mapCeiling[(int)Math.Floor(ceilingPixelY / tileSize), (int)Math.Floor(ceilingPixelX / tileSize)]);

//            if (path == null)
//            {
//                quadX1 = quadX2;
//                quadX2 += xStep;
//                incr++;
//                incRayAngle = NormalizeAngle(incRayAngle + tempRadBetweenRays);
//                continue;
//            }
//            //=========================================

//            //If inside the map and not zero:
//            //=========================================
//            //Calculating RGB variables
//            int RGBCalc = ((int)Math.Floor(path[0][1] / (tileSize / (ceilingPixelY % tileSize))) * path[0][1] * 3) +
//                      ((int)Math.Floor(path[0][0] / (tileSize / (ceilingPixelX % tileSize))) * 3);

//            //Calculating shading and lighting with distance
//            float shadeCalc = ceilingPixelDistance * distanceShade;

//            float debugOffset = 0.5f;

//            vertexAttributesList.AddRange(new float[]
//            {
//                            quadX1 + debugOffset,
//                            quadX2 - debugOffset,
//                            quadY1 + debugOffset,
//                            quadY2 - debugOffset,
//                            (path[1][RGBCalc] - shadeCalc) < 0 ? 0f : (path[1][RGBCalc] - shadeCalc) / 255f,
//                            (path[1][RGBCalc + 1] - shadeCalc) < 0 ? 0f : (path[1][RGBCalc + 1] - shadeCalc) / 255f,
//                            (path[1][RGBCalc + 2] - shadeCalc) < 0 ? 0f : (path[1][RGBCalc + 2] - shadeCalc) / 255f
//            });

//            //X inkrementálás
//            quadX1 = quadX2;
//            quadX2 += xStep;
//            incr++;

//            //Incrementing ray angle for next quad
//            incRayAngle = NormalizeAngle(incRayAngle + tempRadBetweenRays);
//            //=========================================
//        }

//        //If there was no pixel inside the map, break the drawing completely
//        if (incr == 0)
//        {
//            break;
//        }

//        //Incrementing Y
//        quadY1 = quadY2;
//        quadY2 += yStep;

//        //Resetting X
//        //quadX1 = screenHorizontalOffset;
//        //quadX2 = screenHorizontalOffset + yStep;
//    }
//}