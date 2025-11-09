using System;
using System.IO;
using Labyrinths.CLI;
using Xunit;

namespace Labyrinths.Tests;

public class CliTests
{
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
        // Act
        var output = RunWithCapture(() =>
        {
            CommandHandler.Handle(new[] { "--help" });
        });

        // Assert
        Assert.Contains("Usage", output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("generate", output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("solve", output, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void GenerateCommand_CreatesMaze()
    {
        // Arrange
        var outputFile = Path.Combine(Path.GetTempPath(), "maze_test.txt");
        if (File.Exists(outputFile)) { File.Delete(outputFile); }

        var args = new[]
        {
            "generate",
            "--algorithm=dfs",
            "--width=11",
            "--height=11",
            $"--output={outputFile}"
        };

        // Act
        var output = RunWithCapture(() => CommandHandler.Handle(args));

        // Assert
        Assert.True(File.Exists(outputFile), "Файл лабиринта должен быть создан");
        Assert.Contains("Maze saved", output, StringComparison.OrdinalIgnoreCase);

        var text = File.ReadAllText(outputFile);
        Assert.Contains("#", text);
        Assert.Contains(" ", text);

        File.Delete(outputFile);
    }

    [Fact]
    public void SolveCommand_FindsPath_AndSavesResult()
    {
        // Arrange
        var mazeFile = Path.Combine(Path.GetTempPath(), "maze_input.txt");
        var resultFile = Path.Combine(Path.GetTempPath(), "maze_solved.txt");

        // Простой лабиринт 7x7 с открытым проходом
        string mazeText = """
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

        // Act
        var output = RunWithCapture(() => CommandHandler.Handle(args));

        // Assert
        Assert.True(File.Exists(resultFile), "Файл решения должен быть создан");
        Assert.Contains("Solution saved", output, StringComparison.OrdinalIgnoreCase);

        var solved = File.ReadAllText(resultFile);
        Assert.Contains(".", solved); // путь должен содержать точки
    }

    [Fact]
    public void SolveCommand_PrintsPath_WhenNoOutputFile()
    {
        // Arrange
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

        // Act
        var output = RunWithCapture(() => CommandHandler.Handle(args));

        // Assert
        Assert.Contains(".", output); // путь должен быть нарисован в консоли
    }

    [Fact]
    public void SolveCommand_ReturnsMessage_WhenNoPath()
    {
        // Arrange
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

        // Act
        var output = RunWithCapture(() => CommandHandler.Handle(args));

        // Assert
        Assert.Contains("Path not found", output);
    }
}