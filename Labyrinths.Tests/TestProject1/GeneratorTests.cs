using Labyrinths.Enums;
using Labyrinths.Generators;
using Labyrinths.Models;

namespace Labyrinths.Tests;

public class GeneratorTests
{
    [Fact]
    public void DfsGenerator_CreatesMazeWithWallsAndPaths()
    {
        // Тест проверяет, что DFS генератор создает лабиринт с правильными размерами и содержит как стены, так и проходы
        var generator = new DfsGenerator();

        var maze = generator.Generate(11, 11);

        Assert.NotNull(maze);
        Assert.Equal(11, maze.Width);
        Assert.Equal(11, maze.Height);

        int wallCount = 0;
        int pathCount = 0;

        for (int y = 0; y < maze.Height; y++)
        {
            for (int x = 0; x < maze.Width; x++)
            {
                var cell = maze.Get(new Point(x, y));
                if (cell == Enums.CellType.Wall) { wallCount++; }
                if (cell == Enums.CellType.Empty) { pathCount++; }
            }
        }

        Assert.True(wallCount > 0);
        Assert.True(pathCount > 0);
    }
    [Fact]
    public void RandomGenerator_CreatesMaze_WithCorrectDimensions()
    {
        // Тест проверяет, что Random генератор создает лабиринт с заданными размерами
        int width = 10, height = 8;
        var generator = new RandomGenerator();
        var maze = generator.Generate(width, height);

        Assert.Equal(width, maze.Width);
        Assert.Equal(height, maze.Height);
    }

    [Fact]
    public void RandomGenerator_HasWallsAroundBorders()
    {
        // Тест проверяет, что Random генератор создает стены по границам лабиринт
        var generator = new RandomGenerator();
        var maze = generator.Generate(10, 10);

        for (int x = 0; x < maze.Width; x++)
        {
            Assert.Equal(CellType.Wall, maze.Get(new Point(x, 0)));
            Assert.Equal(CellType.Wall, maze.Get(new Point(x, maze.Height - 1)));
        }

        for (int y = 0; y < maze.Height; y++)
        {
            Assert.Equal(CellType.Wall, maze.Get(new Point(0, y)));
            Assert.Equal(CellType.Wall, maze.Get(new Point(maze.Width - 1, y)));
        }
    }

    [Fact]
    public void RandomGenerator_HasAtLeastOneEmptyCellInside()
    {
        // Тест проверяет, что Random генератор создает хотя бы одну пустую клетку внутри лабиринта
        var generator = new RandomGenerator();
        var maze = generator.Generate(10, 10);

        bool hasEmpty = false;
        for (int y = 1; y < maze.Height - 1; y++)
        {
            for (int x = 1; x < maze.Width - 1; x++)
            {
                if (maze.Get(new Point(x, y)) == CellType.Empty)
                {
                    hasEmpty = true;
                    break;
                }
            }
            if (hasEmpty) { break; }
        }

        Assert.True(hasEmpty);
    }

    [Fact]
    public void PrimGenerator_CreatesMazeWithWallsAndPaths()
    {
        // Тест проверяет, что Prim генератор создает лабиринт с правильными размерами и содержит как стены, так и проходы
        var generator = new PrimGenerator();

        var maze = generator.Generate(11, 11);

        Assert.NotNull(maze);
        Assert.Equal(11, maze.Width);
        Assert.Equal(11, maze.Height);

        int wallCount = 0;
        int pathCount = 0;

        for (int y = 0; y < maze.Height; y++)
        {
            for (int x = 0; x < maze.Width; x++)
            {
                var cell = maze.Get(new Point(x, y));
                if (cell == CellType.Wall) { wallCount++; }
                if (cell == CellType.Empty) { pathCount++; }
            }
        }

        Assert.True(wallCount > 0);
        Assert.True(pathCount > 0);
    }

    [Fact]
    public void PrimGenerator_HasWallsAroundBorders()
    {
        // Тест проверяет, что Prim генератор создает стены по границам лабиринта
        var generator = new PrimGenerator();
        var maze = generator.Generate(10, 10);

        for (int x = 0; x < maze.Width; x++)
        {
            Assert.Equal(CellType.Wall, maze.Get(new Point(x, 0)));
            Assert.Equal(CellType.Wall, maze.Get(new Point(x, maze.Height - 1)));
        }

        for (int y = 0; y < maze.Height; y++)
        {
            Assert.Equal(CellType.Wall, maze.Get(new Point(0, y)));
            Assert.Equal(CellType.Wall, maze.Get(new Point(maze.Width - 1, y)));
        }
    }
}