using Labyrinths.Enums;
using Labyrinths.Models;

namespace Labyrinths.Generators;
public class PrimGenerator : IGenerator
{
    private static readonly Random _rnd = new();
    public Maze Generate(int width, int height)
    {
        int w = Math.Max(3, width | 1);
        int h = Math.Max(3, height | 1);
        var maze = new Maze(w, h);

        int sx = 1, sy = 1;
        maze.Set(new Point(sx, sy), CellType.Empty);

        var walls = new List<(Point wall, Point from)>();

        void TryAddWall(int x, int y, Point from)
        {
            var p = new Point(x, y);
            if (maze.InBounds(p) && maze.Get(p) == CellType.Wall) { walls.Add((p, from)); }
        }

        TryAddWall(sx + 1, sy, new Point(sx, sy));
        TryAddWall(sx - 1, sy, new Point(sx, sy));
        TryAddWall(sx, sy + 1, new Point(sx, sy));
        TryAddWall(sx, sy - 1, new Point(sx, sy));

        while (walls.Count > 0)
        {
            int idx = _rnd.Next(walls.Count);
            var (wall, from) = walls[idx];
            walls.RemoveAt(idx);

            var dx = wall.X - from.X;
            var dy = wall.Y - from.Y;
            var opposite = new Point(wall.X + dx, wall.Y + dy);

            if (!maze.InBounds(opposite)) { continue; }
            if (maze.Get(opposite) == CellType.Wall)
            {
                maze.Set(wall, CellType.Empty);
                maze.Set(opposite, CellType.Empty);

                TryAddWall(opposite.X + 1, opposite.Y, opposite);
                TryAddWall(opposite.X - 1, opposite.Y, opposite);
                TryAddWall(opposite.X, opposite.Y + 1, opposite);
                TryAddWall(opposite.X, opposite.Y - 1, opposite);
            }
        }

        return maze;
    }
}