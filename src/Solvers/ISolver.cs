using Labyrinths.Models;

namespace Labyrinths.Solvers;
interface ISolver
{
    List<Point>? Solve(Maze maze, Point start, Point end);
}
