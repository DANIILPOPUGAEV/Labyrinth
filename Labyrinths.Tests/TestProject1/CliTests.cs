using Labyrinths.CLI;

namespace Labyrinths.Tests;

public class CliTests
{
    // Вспомогательный метод для захвата вывода в консоль
    private string RunWithCapture(Action action)
    {
        using var sw = new StringWriter();
        var original = Console.Out;
        Console.SetOut(sw);
        action();
        Console.SetOut(original);
        return sw.ToString();
    }

    [Fact]
    public void HelpCommand_ShowsUsage()
    {
        // Тест проверяет, что команда --help выводит справочную информацию
        var output = RunWithCapture(() =>
        {
            CommandHandler.Handle(new[] { "--help" });
        });

        Assert.Contains("Usage", output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("generate", output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("solve", output, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void GenerateCommand_CreatesMaze()
    {
        // Тест проверяет генерацию лабиринта и сохранение в файл
        var outputFile = Path.Combine(Path.GetTempPath(), "maze_test.txt");
        if (File.Exists(outputFile))
        {
            File.Delete(outputFile);
        }

        var args = new[]
        {
        "generate",
        "--algorithm=dfs",
        "--width=11",
        "--height=11",
        $"--output={outputFile}"
    };

        CommandHandler.Handle(args);

        Assert.True(File.Exists(outputFile), "Файл лабиринта должен быть создан");

        var text = File.ReadAllText(outputFile);
        Assert.Contains("#", text);
        Assert.Contains(" ", text);

        File.Delete(outputFile);
    }

    [Fact]
    public void SolveCommand_FindsPath_AndSavesResult()
    {
        // Тест проверяет решение лабиринта и сохранение результата с путем
        var mazeFile = Path.Combine(Path.GetTempPath(), "maze_input.txt");
        var resultFile = Path.Combine(Path.GetTempPath(), "maze_solved.txt");

        string mazeText = 
        """
        #######
        #     #
        # ### #
        # # # #
        # ### #
        #     #
        #######
        """;

        File.WriteAllText(mazeFile, mazeText);

        var args = new[]
        {
            "solve",
            "--algorithm=dijkstra",
            $"--file={mazeFile}",
            "--start=1,1",
            "--end=5,5",
            $"--output={resultFile}"
        };

        CommandHandler.Handle(args);

        Assert.True(File.Exists(resultFile), "Файл решения должен быть создан");

        var solved = File.ReadAllText(resultFile);
        Assert.Contains(".", solved);

        File.Delete(mazeFile);
        File.Delete(resultFile);
    }

    [Fact]
    public void SolveCommand_PrintsPath_WhenNoOutputFile()
    {
        // Тест проверяет вывод решения в консоль при отсутствии файла вывода
        string mazeText = """
        #######
        #     #
        # ### #
        # # # #
        # ### #
        #     #
        #######
        """;

        var mazeFile = Path.Combine(Path.GetTempPath(), "maze_stdout.txt");
        File.WriteAllText(mazeFile, mazeText);

        var args = new[]
        {
            "solve",
            "--algorithm=dijkstra",
            $"--file={mazeFile}",
            "--start=1,1",
            "--end=5,5"
        };

        var output = RunWithCapture(() => CommandHandler.Handle(args));

        Assert.Contains(".", output);
    }

    [Fact]
    public void SolveCommand_ReturnsMessage_WhenNoPath()
    {
        // Тест проверяет обработку случая, когда путь невозможен
        string mazeText = """
        #######
        #     #
        #######
        #     #
        #######
        """;

        var mazeFile = Path.Combine(Path.GetTempPath(), "maze_nopath.txt");
        File.WriteAllText(mazeFile, mazeText);

        var args = new[]
        {
            "solve",
            "--algorithm=dijkstra",
            $"--file={mazeFile}",
            "--start=1,1",
            "--end=5,3"
        };

        var output = RunWithCapture(() => CommandHandler.Handle(args));

        Assert.Contains("Path not found", output);
    }
}