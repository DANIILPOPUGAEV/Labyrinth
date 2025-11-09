using Labyrinths.Enums;
using Labyrinths.Generators;
using Labyrinths.Models;
using Labyrinths.Solvers;

namespace Labyrinths.CLI;

/// <summary>
/// Обработчик команд CLI для генерации и решения лабиринтов.
/// Предоставляет точку входа для работы с приложением через командную строку.
/// </summary>
public static class CommandHandler
{
    private const string Version = "1.0.0";

    /// <summary>
    /// Обрабатывает аргументы командной строки и выполняет соответствующие действия.
    /// </summary>
    /// <param name="args">Аргументы командной строки</param>
    public static void Handle(string[] args)
    {
        if (args.Length == 0 || args.Contains("-H") || args.Contains("--help"))
        {
            HelpPrinter.Show();
            return;
        }

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

    /// <summary>
    /// Обрабатывает команду генерации лабиринта.
    /// </summary>
    /// <param name="opts">Опции команды</param>
    private static void HandleGenerate(Dictionary<string, string> opts)
    {
        var algorithm = opts["algorithm"].ToLowerInvariant();
        int width = int.Parse(opts["width"]);
        int height = int.Parse(opts["height"]);
        bool useUnicode = opts.TryGetValue("unicode", out var u) && u.ToLower() == "true";

        IGenerator generator = algorithm switch
        {
            "dfs" => new DfsGenerator(),
            "prim" => new PrimGenerator(),
            "random" => new RandomGenerator(),
            _ => throw new ArgumentException("Unknown algorithm")
        };

        var maze = generator.Generate(width, height);

        if (opts.TryGetValue("output", out var output))
        {
            maze.SaveToFile(output);
            //Console.WriteLine($"Maze saved to {output}");
        }
        else
        {
            maze.Draw(useUnicode);
        }
    }

    /// <summary>
    /// Обрабатывает команду решения лабиринта.
    /// </summary>
    /// <param name="opts">Опции команды</param>
    private static void HandleSolve(Dictionary<string, string> opts)
    {
        var algorithm = opts["algorithm"].ToLowerInvariant();
        var file = opts["file"];

        Point start, end;

        // Парсинг стартовой точки с обработкой ошибок формата
        try
        {
            start = Point.Parse(opts["start"]);
        }
        catch (FormatException)
        {
            Console.WriteLine($"Invalid point format: {opts["start"]}, expected format: x,y");
            Environment.Exit(1);
            return;
        }

        // Парсинг конечной точки с обработкой ошибок формата
        try
        {
            end = Point.Parse(opts["end"]);
        }
        catch (FormatException)
        {
            Console.WriteLine($"Invalid point format: {opts["end"]}, expected format: x,y");
            Environment.Exit(1);
            return;
        }

        bool useUnicode = opts.TryGetValue("unicode", out var u) && u.ToLower() == "true";

        var maze = Maze.LoadFromFile(file);

        ISolver solver = algorithm switch
        {
            "astar" => new AStarSolver(),
            "dijkstra" => new DijkstraSolver(),
            "bfs" => new BfsSolver(),
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
            //Console.WriteLine($"Solution saved to {output}");
        }
        else
        {
            maze.Draw(useUnicode);
        }
    }
}