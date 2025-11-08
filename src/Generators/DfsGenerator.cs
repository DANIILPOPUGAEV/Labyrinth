using Labyrinths.Enums;
using Labyrinths.Models;
namespace Labyrinths.Generators;

// Генератор лабиринта с использованием алгоритма DFS (Depth-First Search)
class DfsGenerator : IGenerator
{
    private static readonly Random _rnd = new();
    private int width, height;
    private Maze? _maze;

    public Maze Generate(int width, int height)
    {
        this.width = Math.Max(3, width | 1);
        this.height = Math.Max(3, height | 1);
        _maze = new Maze(this.width, this.height);
        
        VisitAll();
        return _maze;
    }

    // Основной метод генерации с использованием DFS
    private void VisitAll()
    {
        var stack = new Stack<Point>();
        var start = new Point(1, 1);
        _maze!.Set(start, CellType.Empty);
        stack.Push(start);

        var dirs = new Point[] { new(2, 0), new(-2, 0), new(0, 2), new(0, -2) };

        while (stack.Count > 0)
        {
            var cur = stack.Pop();
            var neighbors = dirs.Select(d => new Point(cur.X + d.X, cur.Y + d.Y))
                                .Where(p => _maze.InBounds(p))
                                .Where(p => _maze.Get(p) == CellType.Wall)
                                .OrderBy(_ => _rnd.Next()).ToArray();

            foreach (var n in neighbors)
            {
                var mid = new Point((cur.X + n.X) / 2, (cur.Y + n.Y) / 2);
                _maze.Set(mid, CellType.Empty);
                _maze.Set(n, CellType.Empty);
                stack.Push(n);
            }
        }
    }
}
