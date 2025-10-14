using System;

public class Program
{
    static void Main()
    {
        Settings.Load();

        Map map = new Map();
        map.Load();

        Engine.Start();

        EngineWindow engineWindow = new EngineWindow();
        engineWindow.Run();
    }
}