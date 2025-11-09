using Labyrinths.Enums;
using Labyrinths.Generators;
using Labyrinths.Models;
using Labyrinths.Solvers;

namespace Labyrinths.CLI;

public static class CommandHandler
{
    private const string Version = "1.0.0";

    public static void Handle(string[] args)
    {
        // Показываем помощь, если нет аргументов или указан флаг -h / --help
        if (args.Length == 0 || args.Contains("-H") || args.Contains("--help"))
        {
            HelpPrinter.Show();
            return;
        }

        // Показываем версию, если указан флаг -V / --version
        if (args.Contains("-V") || args.Contains("--version"))
        {
            Console.WriteLine($"maze-app version {Version}");
            return;
        }

        var cmd = args[0].ToLowerInvariant();
        var opts = CommandParser.Parse(args.Skip(1).ToArray());

        switch (cmd)
        {
            case "generate":
                HandleGenerate(opts);
                break;

            case "solve":
                HandleSolve(opts);
                break;

            default:
                Console.WriteLine($"Unknown command: {cmd}");
                Console.WriteLine("Use -h or --help for usage information.");
                break;
        }
    }

    private static void HandleGenerate(Dictionary<string, string> opts)
    {
        var algorithm = opts["algorithm"].ToLowerInvariant();
        int width = int.Parse(opts["width"]);
        int height = int.Parse(opts["height"]);

        IGenerator generator = algorithm switch
        {
            "dfs" => new DfsGenerator(),
            "prim" => new PrimGenerator(),
            _ => throw new ArgumentException("Unknown algorithm")
        };

        var maze = generator.Generate(width, height);

        if (opts.TryGetValue("output", out var output))
        {
            maze.SaveToFile(output);
            Console.WriteLine($"Maze saved to {output}");
        }
        else
        {
            maze.Draw();
        }
    }

    private static void HandleSolve(Dictionary<string, string> opts)
    {
        var algorithm = opts["algorithm"].ToLowerInvariant();
        var file = opts["file"];
        var start = Point.Parse(opts["start"]);
        var end = Point.Parse(opts["end"]);

        var maze = Maze.LoadFromFile(file);

        ISolver solver = algorithm switch
        {
            "astar" => new AStarSolver(),
            "dijkstra" => new DijkstraSolver(),
            _ => throw new ArgumentException("Unknown algorithm")
        };

        var path = solver.Solve(maze, start, end);
        if (path == null)
        {
            Console.WriteLine("Path not found.");
            return;
        }

        foreach (var p in path)
        {
            if (p == start || p == end) { continue; }
            maze.Set(p, CellType.Path);
        }

        maze.Set(start, CellType.Start);
        maze.Set(end, CellType.End);

        if (opts.TryGetValue("output", out var output))
        {
            maze.SaveToFile(output);
            Console.WriteLine($"Solution saved to {output}");
        }
        else
        {
            maze.Draw();
        }
    }
}