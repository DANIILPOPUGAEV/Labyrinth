using Labyrinths.Enums;
using Labyrinths.Models;
using Labyrinths.Solvers;

namespace Labyrinths.Tests;
public class ErrorHandlingTests
{
    [Fact]
    public void Maze_GetOrSet_OutOfBounds_ThrowsException()
    {
        // Тест проверяет обработку выхода за границы лабиринта при операциях Get и Set
        var maze = new Maze(5, 5);
        var invalidPoint = new Point(10, 10);

        Assert.False(maze.InBounds(invalidPoint));
        Assert.Throws<IndexOutOfRangeException>(() => maze.Set(invalidPoint, CellType.Path));
        Assert.Throws<IndexOutOfRangeException>(() => maze.Get(invalidPoint));
    }

    [Fact]
    public void LoadFromFile_FileDoesNotExist_ThrowsException()
    {
        // Тест проверяет, что загрузка несуществующего файла вызывает исключение
        string path = "non_existing_file.txt";
        Assert.False(File.Exists(path));
        Assert.Throws<FileNotFoundException>(() => Maze.LoadFromFile(path));
    }

    [Fact]
    public void LoadFromFile_InvalidContent_HandlesGracefully()
    {
        // Тест проверяет корректную обработку файла с невалидным содержимым
        string path = "invalid_maze.txt";
        File.WriteAllLines(path, new string[] { "###", "ABC", "###" });

        var maze = Maze.LoadFromFile(path);
        Assert.NotNull(maze);

        for (int y = 0; y < maze.Height; y++)
        {
            for (int x = 0; x < maze.Width; x++)
            {
                var cell = maze.Get(new Point(x, y));
                Assert.True(cell == CellType.Wall || cell == CellType.Empty);
            }
        }

        File.Delete(path);
    }

    [Fact]
    public void Solver_InvalidStartOrEnd_ReturnsNull()
    {
        // Тест проверяет, что решатель возвращает null при невалидных начальной/конечной точках
        var maze = new Maze(5, 5);
        var solver = new DijkstraSolver();
        var start = new Point(-1, 0);
        var end = new Point(10, 10);

        var path = solver.Solve(maze, start, end);
        Assert.Null(path);
    }

    [Fact]
    public void Solver_StartEqualsEnd_ReturnsSinglePoint()
    {
        // Тест проверяет корректную обработку случая, когда начальная и конечная точки совпадают
        var maze = new Maze(5, 5);
        maze.Set(new Point(2, 2), CellType.Empty);
        var solver = new DijkstraSolver();
        var startEnd = new Point(2, 2);

        var path = solver.Solve(maze, startEnd, startEnd);
        Assert.NotNull(path);
        Assert.Single(path);
        Assert.Equal(startEnd, path[0]);
    }
}
