using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Common;

namespace Engine;

internal partial class Engine
{
    /* Buttons ID Translator
     * 0. Contiune
     * 1. New Game
     * 2. Play
     * 3. Settings
     * 4. Statistics
     * 5. Exit
     * 6. Back to Game
     */

    void UploadButtons(int[] buttonIds)
    {
        for (int i = 0; i < buttonIds.Count(); i++)
        {
            ShaderHandler.ButtonsVertexAttribList.AddRange(new float[]
            {
                screenHorizontalOffset,
                screenHorizontalOffset + minimumScreenSize,
                screenVerticalOffset + minimumScreenSize,
                screenVerticalOffset,
                buttonIds[i]
            });
        }
    }
}
