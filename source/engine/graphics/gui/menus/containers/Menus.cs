using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Common;

namespace Engine;

internal partial class Engine
{
    /* Menu background ID translator
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
        if (isSaveState) buttonIds = [0, 1, 3, 4, 5];
        else buttonIds = [2, 3, 4, 5];

        UploadMenus(0);
        UploadButtons(buttonIds);
    }

    void PauseMenu()
    {
        buttonIds = [6, 3, 5];

        UploadMenus(1);
        UploadButtons(buttonIds);
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
