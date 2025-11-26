using Labyrinths.Enums;
using Labyrinths.Models;
using Labyrinths.Solvers;

namespace Labyrinths.Tests;
public class SolverTests
{
    private Maze CreateSimpleMaze()
    {
        var maze = new Maze(5, 5);

        // Инициализируем все клетки как пустые
        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 5; x++)
            { maze.Set(new Point(x, y), CellType.Empty); }
        }
        // Можно добавить стены для теста
        maze.Set(new Point(2, 0), CellType.Wall);
        maze.Set(new Point(2, 1), CellType.Wall);
        maze.Set(new Point(2, 2), CellType.Wall);
        maze.Set(new Point(2, 3), CellType.Wall);

        // Старт и конец оставляем пустыми
        maze.Set(new Point(0, 0), CellType.Empty);
        maze.Set(new Point(4, 4), CellType.Empty);

        return maze;
    }

    [Fact]
    public void DijkstraSolver_FindsPath_WhenPathExists()
    {
        // Тест проверяет, что алгоритм Дейкстры находит путь когда он существует
        var maze = CreateSimpleMaze();
        var solver = new DijkstraSolver();

        var path = solver.Solve(maze, new Point(1, 1), new Point(3, 3));

        Assert.NotNull(path);
        Assert.True(path!.Count > 0);
        Assert.Equal(new Point(1, 1), path.First());
        Assert.Equal(new Point(3, 3), path.Last());
    }

    [Fact]
    public void DijkstraSolver_ReturnsNull_WhenNoPathExists()
    {
        // Тест проверяет, что алгоритм Дейкстры возвращает null когда путь невозможен
        var maze = new Maze(5, 5);
        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 5; x++)
            {
                maze.Set(new Point(x, y), CellType.Empty);
            }
        }

        maze.Set(new Point(0, 1), CellType.Wall);
        maze.Set(new Point(1, 0), CellType.Wall);
        maze.Set(new Point(1, 2), CellType.Wall);
        maze.Set(new Point(2, 1), CellType.Wall);

        var solver = new DijkstraSolver();

        var path = solver.Solve(maze, new Point(1, 1), new Point(3, 3));

        Assert.Null(path);
    }

    [Fact]
    public void AStarSolver_FindsSamePath_AsDijkstra()
    {
        // Тест проверяет, что A* и Дейкстра находят одинаковый путь в простом лабиринте
        var maze = CreateSimpleMaze();
        var dijkstra = new DijkstraSolver();
        var astar = new AStarSolver();

        var path1 = dijkstra.Solve(maze, new Point(1, 1), new Point(3, 3));
        var path2 = astar.Solve(maze, new Point(1, 1), new Point(3, 3));

        Assert.NotNull(path1);
        Assert.NotNull(path2);
        Assert.Equal(path1!.Count, path2!.Count);
        Assert.Equal(path1!.First(), path2!.First());
        Assert.Equal(path1!.Last(), path2!.Last());
    }

    [Fact]
    public void Solver_ReturnsNull_WhenPointsOutOfBounds()
    {
        // Тест проверяет, что решатели возвращают null при точках вне границ лабиринта
        var maze = CreateSimpleMaze();
        var solver = new DijkstraSolver();

        var result = solver.Solve(maze, new Point(-1, -1), new Point(10, 10));

        Assert.Null(result);
    }

    [Fact]
    public void BfsSolver_FindsPath_WhenExists()
    {
        // Тест проверяет, что BFS алгоритм находит путь когда он существует
        var maze = CreateSimpleMaze();
        var solver = new BfsSolver();
        var path = solver.Solve(maze, new Point(0, 0), new Point(4, 4));

        Assert.NotNull(path);
        Assert.Equal(new Point(0, 0), path.First());
        Assert.Equal(new Point(4, 4), path.Last());
    }

    [Fact]
    public void BfsSolver_ReturnsNull_WhenNoPath()
    {
        // Тест проверяет, что BFS алгоритм возвращает null когда путь невозможен
        var maze = CreateSimpleMaze();
        maze.Set(new Point(1, 0), CellType.Wall);
        maze.Set(new Point(0, 1), CellType.Wall);

        var solver = new BfsSolver();
        var path = solver.Solve(maze, new Point(0, 0), new Point(4, 4));

        Assert.Null(path);
    }

    [Fact]
    public void BfsSolver_PathIsCorrectLength()
    {
        // Тест проверяет, что BFS находит путь правильной длины (кратчайший)
        var maze = CreateSimpleMaze();
        var solver = new BfsSolver();
        var path = solver.Solve(maze, new Point(0, 0), new Point(4, 4));

        Assert.NotNull(path); // проверка, что путь найден

        int expectedLength = 9;
        Assert.Equal(expectedLength, path!.Count);
    }
}
