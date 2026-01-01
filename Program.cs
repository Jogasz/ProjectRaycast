using System;
using System.Threading;

public class Program
{
    static void Main()
    {
        try
        {
            Settings.Load();
            Level map = new Level();
            map.Load();
            Console.WriteLine("Assets successfully loaded!");
        }
        catch
        {
            Console.WriteLine("Something went wrong when loading assets...");
        }
        
        Engine.Engine engine = new Engine.Engine(1000, 800, "ProjectRaycast");
        engine.Run();

        //GraphicWindow.Run();
    }
}