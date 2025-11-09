using Labyrinths.Enums;
using Labyrinths.Models;

namespace Labyrinths.Solvers;
public class DijkstraSolver : ISolver
{
    public List<Point>? Solve(Maze maze, Point start, Point end)
    {
        if (!maze.InBounds(start) || !maze.InBounds(end)) { return null; }

        var distanceFromStart = new Dictionary<Point, int>();
        var previousPoint = new Dictionary<Point, Point>();
        var pointQueue = new PriorityQueue<Point, int>();

        distanceFromStart[start] = 0;
        previousPoint[start] = start;
        pointQueue.Enqueue(start, 0);

        var movementDirections = new[]
        {
            new Point(1, 0),
            new Point(-1, 0),
            new Point(0, 1),
            new Point(0, -1)
        };

        while (pointQueue.Count > 0)
        {
            pointQueue.TryDequeue(out var currentPoint, out var currentDistance);

            if (currentPoint!.Equals(end)) { break; }

            if (currentDistance > distanceFromStart[currentPoint]) { continue; }

            foreach (var direction in movementDirections)
            {
                var neighborPoint = new Point(currentPoint.X + direction.X, currentPoint.Y + direction.Y);
                if (!maze.InBounds(neighborPoint)) { continue; }

                var cellType = maze.Get(neighborPoint);
                if (cellType == CellType.Wall) { continue; }

                int newDistance = currentDistance + 1;
                if (!distanceFromStart.ContainsKey(neighborPoint) || newDistance < distanceFromStart[neighborPoint])
                {
                    distanceFromStart[neighborPoint] = newDistance;
                    previousPoint[neighborPoint] = currentPoint;
                    pointQueue.Enqueue(neighborPoint, newDistance);
                }
            }
        }

        if (!distanceFromStart.ContainsKey(end))
        {
            return null;
        }

        var path = new List<Point>();
        var pathPoint = end;
        while (!pathPoint.Equals(start))
        {
            path.Add(pathPoint);
            pathPoint = previousPoint[pathPoint];
        }
        path.Add(start);
        path.Reverse();

        return path;
    }
}