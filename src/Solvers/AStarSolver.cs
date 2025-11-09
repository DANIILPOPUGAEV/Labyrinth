using Labyrinths.Enums;
using Labyrinths.Models;

namespace Labyrinths.Solvers;
public class AStarSolver : ISolver
{
    // Манхэттенское расстояние (оценка эвристики)
    private static int Heuristic(Point a, Point b) =>
        Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);

    public List<Point>? Solve(Maze maze, Point start, Point end)
    {
        if (!maze.InBounds(start) || !maze.InBounds(end)) { return null; }

        var distanceFromStart = new Dictionary<Point, int>();

        var totalEstimatedCost = new Dictionary<Point, int>();

        var previousPoint = new Dictionary<Point, Point>();

        var openPoints = new PriorityQueue<Point, int>();

        var closedPoints = new HashSet<Point>();

        distanceFromStart[start] = 0;
        totalEstimatedCost[start] = Heuristic(start, end);
        previousPoint[start] = start;

        openPoints.Enqueue(start, totalEstimatedCost[start]);

        var movementDirections = new[]
        {
            new Point(1, 0),
            new Point(-1, 0),
            new Point(0, 1),
            new Point(0, -1)
        };

        while (openPoints.Count > 0)
        {
            openPoints.TryDequeue(out var currentPoint, out var _);
            if (currentPoint!.Equals(end)) { break; }

            if (closedPoints.Contains(currentPoint)) { continue; }

            closedPoints.Add(currentPoint);

            foreach (var direction in movementDirections)
            {
                var neighborPoint = new Point(currentPoint.X + direction.X, currentPoint.Y + direction.Y);

                if (!maze.InBounds(neighborPoint)) { continue; }

                if (maze.Get(neighborPoint) == CellType.Wall) { continue; }

                int tentativeDistance = distanceFromStart[currentPoint] + 1;

                if (!distanceFromStart.ContainsKey(neighborPoint) || tentativeDistance < distanceFromStart[neighborPoint])
                {
                    distanceFromStart[neighborPoint] = tentativeDistance;
                    previousPoint[neighborPoint] = currentPoint;
                    totalEstimatedCost[neighborPoint] = tentativeDistance + Heuristic(neighborPoint, end);
                    openPoints.Enqueue(neighborPoint, totalEstimatedCost[neighborPoint]);
                }
            }
        }

        if (!distanceFromStart.ContainsKey(end)) { return null; }

        var path = new List<Point>();
        var pathPoint = end;
        while (!pathPoint.Equals(start))
        {
            path.Add(pathPoint);
            if (!previousPoint.ContainsKey(pathPoint)) { return null; }

            pathPoint = previousPoint[pathPoint];
        }
        path.Add(start);
        path.Reverse();

        return path;
    }
}

