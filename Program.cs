using System;
using System.Threading;

public class Program
{
    static void Main()
    {
        try
        {
            Settings.Load();
            Textures.Load();
            Level map = new Level();
            map.Load();
            Console.WriteLine("Assets successfully loaded!");
        }
        catch
        {
            Console.WriteLine("Something went wrong when loading assets...");
        }

        Engine engine = new Engine(800, 800, "ProjectRaycast");
        engine.Run();

        //GraphicWindow.Run();
    }
}