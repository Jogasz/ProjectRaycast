using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Common;

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
     * 2. Play
     * 3. Settings
     * 4. Statistics
     * 5. Exit
     * 6. Back to Game
     */

    //Handling main menu

    internal static int[] buttonIds;
    void MainMenu()
    {
        if (IsKeyPressed(Keys.Escape))
        {
            Close();
        }
        else if (IsKeyPressed(Keys.Enter))
        {
            isInMainMenu = false;
            CursorState = CursorState.Grabbed;
        }

        if (isSaveState) buttonIds = [0, 1, 3, 4, 5];
        else buttonIds = [2, 3, 4, 5];

        UploadMenus(0);
        UploadButtons(buttonIds);
    }

    void PauseMenu()
    {
        if (IsKeyPressed(Keys.Enter))
        {
            isInPauseMenu = false;
            CursorState = CursorState.Grabbed;
            return;
        }

        if (!escConsumed && IsKeyPressed(Keys.Escape))
        {
            CursorState = CursorState.Normal;

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
            screenHorizontalOffset,
            screenHorizontalOffset + minimumScreenSize,
            screenVerticalOffset + minimumScreenSize,
            screenVerticalOffset,
            backgroundIndex
        });
    }
}
