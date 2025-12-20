//Ceiling calculation
//=============================================================================================
//float ceilingFloorPixelDistance,
//      ceilingPixelXWorldPosition,
//      ceilingPixelYWorldPosition,
//      floorPixelXWorldPosition,
//      floorPixelYWorldPosition;

////Floor and ceiling X position variables
//float floorCeilingPixelXLeft = i * wallWidth + screenHorizontalOffset;
//float floorCeilingPixelXRight = (i + 1) * wallWidth + screenHorizontalOffset;

//float ceilingStep = Math.Max(4f, wallWidth);

////Ceiling Y position variables
//float ceilingPixelYTop = screenVerticalOffset;
//float ceilingPixelYBottom = screenVerticalOffset + ceilingStep;

//while (ceilingPixelYTop < (screenVerticalOffset + ((minimumScreenHeight - wallHeight) / 2)) + pitch)
//{
//    /* 
//    * If the pixel's top position is in the correct screen, but the bottom position is in the wall,
//    * the bottom position's Y value may be equal to the wall's top value.
//    */

//    ceilingPixelYBottom =
//    (ceilingPixelYBottom > (screenVerticalOffset + ((minimumScreenHeight - wallHeight) / 2)) + pitch) ?
//    (screenVerticalOffset + ((minimumScreenHeight - wallHeight) / 2)) + pitch :
//    ceilingPixelYBottom;

//    //Y of the current pixel on the screen
//    rowY = (ClientSize.Y / 2) - (ceilingPixelYTop + ((ceilingPixelYBottom - ceilingPixelYTop) / 2)) + pitch;

//    //Distance of the pixel from the player
//    ceilingFloorPixelDistance = ((cameraZ / rowY) * tileSize) / (float)Math.Cos(playerAngle - rayAngle);

//    //World X position of the pixel
//    ceilingPixelXWorldPosition = playerPosition.X + ((float)Math.Cos(rayAngle) * ceilingFloorPixelDistance);

//    //World Y position of the pixel
//    ceilingPixelYWorldPosition = playerPosition.Y + ((float)Math.Sin(rayAngle) * ceilingFloorPixelDistance);

//    //Textures
//    path =
//    (ceilingPixelXWorldPosition >= mapCeiling.GetLength(1) * tileSize ||
//    ceilingPixelXWorldPosition < 0f ||
//    ceilingPixelYWorldPosition >= mapCeiling.GetLength(0) * tileSize ||
//    ceilingPixelYWorldPosition < 0f) ?
//        null :
//        TextureTranslator((int)mapCeiling[(int)Math.Floor(ceilingPixelYWorldPosition / tileSize), (int)Math.Floor(ceilingPixelXWorldPosition / tileSize)]);

//    if (path == null)
//    {
//        ceilingPixelYTop = ceilingPixelYBottom;
//        ceilingPixelYBottom += ceilingStep;
//        continue;
//    }

//    //Calculating RGB variables
//    RGBCalc = ((int)Math.Floor(path[0][1] / (tileSize / (ceilingPixelYWorldPosition % tileSize))) * path[0][1] * 3) +
//              ((int)Math.Floor(path[0][0] / (tileSize / (ceilingPixelXWorldPosition % tileSize))) * 3);

//    //Calculating shading and lighting with distance
//    shadeCalc = ceilingFloorPixelDistance * distanceShade;

//    //Declaring temporary drawing parameter holder
//    //float[] tempCeilingVertices = new float[7];

//    //Wall array translator
//    //newLine[0] = x1,4
//    //newLine[1] = x2,3
//    //newLine[2] = y1,2
//    //newLine[3] = y3,4
//    //newLine[4] = r
//    //newLine[5] = g
//    //newLine[6] = b

//    vertexAttributesList.AddRange(new float[]
//    {
//        floorCeilingPixelXLeft,
//        floorCeilingPixelXRight,
//        ceilingPixelYTop,
//        ceilingPixelYBottom,
//        (path[1][RGBCalc] - shadeCalc) < 0 ? 0f : (path[1][RGBCalc] - shadeCalc) / 255f,
//        (path[1][RGBCalc + 1] - shadeCalc) < 0 ? 0f : (path[1][RGBCalc + 1] - shadeCalc) / 255f,
//        (path[1][RGBCalc + 2] - shadeCalc) < 0 ? 0f : (path[1][RGBCalc + 2] - shadeCalc) / 255f
//    });

//    //tempCeilingVertices[0] = floorCeilingPixelXLeft;
//    //tempCeilingVertices[1] = floorCeilingPixelXRight;
//    //tempCeilingVertices[2] = ceilingPixelYTop;
//    //tempCeilingVertices[3] = ceilingPixelYBottom;
//    //tempCeilingVertices[4] = (path[1][RGBCalc] - shadeCalc) < 0 ? 0f : (path[1][RGBCalc] - shadeCalc) / 255f;
//    //tempCeilingVertices[5] = (path[1][RGBCalc + 1] - shadeCalc) < 0 ? 0f : (path[1][RGBCalc + 1] - shadeCalc) / 255f;
//    //tempCeilingVertices[6] = (path[1][RGBCalc + 2] - shadeCalc) < 0 ? 0f : (path[1][RGBCalc + 2] - shadeCalc) / 255f;

//    //Adding temporary array the the wall list of arrays
//    //vertexAttributesList.Add(tempCeilingVertices);

//    //Incremental increasing of the top and bottom values of the pixel
//    ceilingPixelYTop = ceilingPixelYBottom;
//    ceilingPixelYBottom += ceilingStep;
//}