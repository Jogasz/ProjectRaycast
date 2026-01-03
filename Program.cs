using System;
using System.Threading;

public class Program
{
    static void Main()
    {
        try
        {
            Settings.Load();
            Console.WriteLine(" - SETTINGS have been loaded!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($" - Something went wrong while loading SETTINGS...\n{ex}");
        }

        try
        {
            Level map = new Level();
            map.Load();
            Console.WriteLine(" - MAP has been loaded!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($" - Something went wrong while loading MAP...\n{ex}");
        }
        
        Engine.Engine engine = new Engine.Engine(800, 800, "ProjectRaycast");
        engine.Run();
    }
}