using Labyrinths.Enums;
using Labyrinths.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labyrinths.Solvers;
public class BfsSolver : ISolver
{
    public List<Point>? Solve(Maze maze, Point start, Point end)
    {
        if (!maze.InBounds(start) || !maze.InBounds(end)) { return null; }

        var visited = new HashSet<Point>();
        var previous = new Dictionary<Point, Point>();
        var queue = new Queue<Point>();

        queue.Enqueue(start);
        visited.Add(start);
        previous[start] = start;

        var directions = new[]
        {
            new Point(1, 0),
            new Point(-1, 0),
            new Point(0, 1),
            new Point(0, -1)
        };

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (current.Equals(end)) { break; }

            foreach (var dir in directions)
            {
                var neighbor = new Point(current.X + dir.X, current.Y + dir.Y);
                if (!maze.InBounds(neighbor)) { continue; }
                if (maze.Get(neighbor) == CellType.Wall) { continue; }
                if (visited.Contains(neighbor)) { continue; }

                visited.Add(neighbor);
                previous[neighbor] = current;
                queue.Enqueue(neighbor);
            }
        }

        if (!previous.ContainsKey(end)) { return null; }

        var path = new List<Point>();
        var p = end;
        while (!p.Equals(start))
        {
            path.Add(p);
            p = previous[p];
        }
        path.Add(start);
        path.Reverse();
        return path;
    }
}