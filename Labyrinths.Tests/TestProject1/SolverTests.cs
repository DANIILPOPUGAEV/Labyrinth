using Labyrinths.Enums;
using Labyrinths.Models;
using Labyrinths.Solvers;

namespace Labyrinths.Tests;
public class SolverTests
{
    private Maze CreateSimpleMaze()
    {
        // Заготовка лабиринта 5x5:
        // #####
        // #   #
        // # # #
        // #   #
        // #####
        var maze = new Maze(5, 5);

        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 5; x++)
            {
                if (x == 0 || y == 0 || x == 4 || y == 4) { maze.Set(new Point(x, y), CellType.Wall); }
                else if (x == 2 && y == 2) { maze.Set(new Point(x, y), CellType.Wall); }
                else { maze.Set(new Point(x, y), CellType.Empty); }
            }
        }

        return maze;
    }

    [Fact]
    public void DijkstraSolver_FindsPath_WhenPathExists()
    {
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
        var maze = CreateSimpleMaze();
        maze.Set(new Point(2, 1), CellType.Wall);
        maze.Set(new Point(2, 3), CellType.Wall);

        var solver = new DijkstraSolver();

        var path = solver.Solve(maze, new Point(1, 1), new Point(3, 3));

        Assert.Null(path);
    }

    [Fact]
    public void AStarSolver_FindsSamePath_AsDijkstra()
    {
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
        var maze = CreateSimpleMaze();
        var solver = new DijkstraSolver();

        var result = solver.Solve(maze, new Point(-1, -1), new Point(10, 10));

        Assert.Null(result);
    }
}
