using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine;

internal partial class Engine
{
    /* Menu background IS translator
     * 0. Main Menu
     * 1. Pause Menu
     * 2. Statistics Menu
     * 3. Settings Menu
     */

    /* Buttons ID Translator
     * 0. Contiune
     * 1. New Game
     * 3. Play
     * 4. Settings
     * 5. Statistics
     * 6. Exit
     * 7. Back to Game
     */

    //Handling main menu
    void MainMenu()
    {
        if (!escConsumed && IsKeyPressed(Keys.Escape))
        {
            escConsumed = true;
            Close();
        }
        else if (IsKeyPressed(Keys.Enter))
        {
            isInMainMenu = false;
        }

        UploadMenus(0);
    }

    void PauseMenu()
    {
        if (IsKeyPressed(Keys.Enter))
        {
            isInPauseMenu = false;
            return;
        }

        if (!escConsumed && IsKeyPressed(Keys.Escape))
        {
            escConsumed = true;

            isInPauseMenu = false;
            playerPosition = (250, 250);
            playerAngle = 0f;
            isInMainMenu = true;
            return;
        }

        UploadMenus(1);
    }

    void UploadMenus(int backgroundIndex)
    {
        ShaderHandler.MenusVertexAttribList.AddRange(new float[]
        {
            screenVerticalOffset,
            screenVerticalOffset + minimumScreenSize,
            screenHorizontalOffset+ minimumScreenSize,
            screenHorizontalOffset,
            backgroundIndex
        });
    }
}
