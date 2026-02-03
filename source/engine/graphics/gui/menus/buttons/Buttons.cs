using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.GraphicsLibraryFramework;
using static OpenTK.Windowing.Common.Input.MouseCursor;

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

    static int[] buttonsWidth = [
        101,
        118,
        64,
        87,
        93,
        64,
        139
    ];

    static int originalButtonsHeight = 21;

    const float buttonsAtlasWidth = 417f;
    const float buttonsAtlasHeight = 147f;

    //Validating hover
    static bool IsPointInQuad(float px, float py, float x1, float x2, float y1, float y2)
    {
        float minX = Math.Min(x1, x2);
        float maxX = Math.Max(x1, x2);
        float minY = Math.Min(y1, y2);
        float maxY = Math.Max(y1, y2);
        return px >= minX && px <= maxX && py >= minY && py <= maxY;
    }

    void HandleClickActions(int id)
    {
        _menuClickConsumed = true;

        //Main menu buttons
        if (isInMainMenu)
        {
            switch (id)
            {
                //Play
                case 2:
                    isInMainMenu = false;
                    CursorState = CursorState.Grabbed;
                    break;

                //Exit
                case 5:
                    Close();
                    break;
            }
        }

        //Pause menu buttons
        if (isInPauseMenu)
        {
            switch (id)
            {
                //Back to Game
                case 6:
                    isInPauseMenu = false;
                    CursorState = CursorState.Grabbed;
                    break;

                //Exit
                case 5:
                    isInPauseMenu = false;
                    isInMainMenu = true;
                    break;
            }
        }
    }

    //Click debounce for menu buttons
    bool _menuClickConsumed;

    //Previous mouse state tracking
    bool _prevMouseDown;

    void UploadButtons(int[] buttonIds)
    {
        float buttonHeight = minimumScreenSize /15f;
        float buttonsGap = minimumScreenSize /100f;
        float horizontalHalfScreen = screenHorizontalOffset + minimumScreenSize /2f;
        float verticalHalfScreen = screenVerticalOffset + minimumScreenSize /2f;

        //If hover
        bool anyHover = false;

        //Mouse position (Ortho coord)
        float mouseX = MouseState.X;
        float mouseY = ClientSize.Y - MouseState.Y;

        //Is mouse down
        bool mouseDown = MouseState.IsButtonDown(MouseButton.Left);

        //Action registers on release
        bool mouseReleased = _prevMouseDown && !mouseDown;

        //Release debounce
        if (!mouseDown)
            _menuClickConsumed = false;

        for (int i =0; i < buttonIds.Length; i++)
        {
            int id = buttonIds[i];

            float buttonWidth = (buttonHeight / originalButtonsHeight) * buttonsWidth[id];

            float quadX1 = horizontalHalfScreen - buttonWidth /2f;
            float quadX2 = horizontalHalfScreen + buttonWidth /2f;
            float quadY1 = verticalHalfScreen - (i +1f) * buttonHeight - i * buttonsGap;
            float quadY2 = verticalHalfScreen - (i +2f) * buttonHeight - i * buttonsGap;

            bool isHover = IsPointInQuad(mouseX, mouseY, quadX1, quadX2, quadY1, quadY2);
            bool isClick = isHover && mouseDown;

            anyHover |= isHover;

            if (anyHover) Cursor = MouseCursor.PointingHand;
            else Cursor = MouseCursor.Default;

            //Action registers on release on the button
            if (isHover && mouseReleased && !_menuClickConsumed)
                HandleClickActions(id);

            //V: pick correct row in the sheet
            float pyTop = id * originalButtonsHeight;
            float pyBottom = (id +1) * originalButtonsHeight;

            //X: choose state (Requirement: shift by button's own width)
            float px0 =0f;
            if (isClick) px0 =2f * buttonsWidth[id];
            else if (isHover) px0 =1f * buttonsWidth[id];
            float px1 = px0 + buttonsWidth[id];

            float u0 = px0 / buttonsAtlasWidth;
            float u1 = px1 / buttonsAtlasWidth;

            float vTop =1f - (pyTop / buttonsAtlasHeight);
            float vBottom =1f - (pyBottom / buttonsAtlasHeight);

            float v0 = vBottom;
            float v1 = vTop;

            ShaderHandler.ButtonsVertexAttribList.AddRange(new float[]
            {
                quadX1,
                quadX2,
                quadY1,
                quadY2,
                id,
                u0,
                v0,
                u1,
                v1
            });
        }

        //Update previous mouse state
        _prevMouseDown = mouseDown;
    }
}
