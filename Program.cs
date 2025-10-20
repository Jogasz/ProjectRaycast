using System;
using System.Threading;

public class Program
{
    static void Main()
    {
        Settings.Load();

        Level map = new Level();
        map.Load();

        Engine.Start();

        Thread debugThread = new Thread(() =>
        {
            DebugWindow debugWindow = new DebugWindow();
            debugWindow.Run();
        });

        Thread graphicThread = new Thread(() =>
        {
            GraphicWindow graphicWindow = new GraphicWindow();
            graphicWindow.Run();
        });

        debugThread.Start();
        graphicThread.Start();
    }
}