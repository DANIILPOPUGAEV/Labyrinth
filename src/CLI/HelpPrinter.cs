namespace Labyrinths.CLI;
public static class HelpPrinter
{
    public static void Show()
    {
        Console.WriteLine("Maze Generator & Solver CLI");
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  generate --algorithm=[dfs|prim] --width=<w> --height=<h> [--output=file]");
        Console.WriteLine("  solve --algorithm=[astar|dijkstra] --file=maze.txt --start=x,y --end=x,y [--output=file]");
        Console.WriteLine();
        Console.WriteLine("Example:");
        Console.WriteLine("  MazeApp generate --algorithm=dfs --width=20 --height=10 --output=maze.txt");
        Console.WriteLine("  MazeApp solve --algorithm=astar --file=maze.txt --start=1,1 --end=18,8");
    }
}
