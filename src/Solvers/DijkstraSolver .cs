using Labyrinths.Enums;
using Labyrinths.Models;

namespace Labyrinths.Solvers;

/// <summary>
/// Реализация алгоритма Дейкстры (Dijkstra's Algorithm) для поиска пути в лабиринте.
/// Алгоритм Дейкстры находит кратчайший путь во взвешенном графе, минимизируя общую стоимость пути.
/// 
/// Принцип работы алгоритма Дейкстры:
/// 1. Использует очередь с приоритетом для обработки точек в порядке возрастания стоимости пути
/// 2. Поддерживает текущие минимальные стоимости пути до каждой точки от старта
/// 3. При нахождении более короткого пути до точки обновляет ее стоимость
/// 4. Гарантирует нахождение оптимального пути при неотрицательных весах ребер
/// </summary>
public class DijkstraSolver : ISolver
{
    /// <summary>
    /// Находит кратчайший путь от начальной до конечной точки в лабиринте с использованием алгоритма Дейкстры.
    /// В данной реализации все перемещения имеют одинаковую стоимость (1), поэтому алгоритм эквивалентен BFS,
    /// но демонстрирует структуру для случаев с различными весами перемещений.
    /// </summary>
    /// <param name="maze">Лабиринт для поиска пути</param>
    /// <param name="start">Начальная точка пути</param>
    /// <param name="end">Конечная точка пути</param>
    /// <returns>
    /// Список точек, представляющий кратчайший путь от start до end с минимальной общей стоимостью,
    /// или null если путь не существует
    /// </returns>
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
            // Извлекаем точку с наименьшим текущим расстоянием от старта
            pointQueue.TryDequeue(out var currentPoint, out var currentDistance);

            // Если достигли конечной точки - завершаем поиск
            if (currentPoint!.Equals(end)) { break; }

            if (currentDistance > distanceFromStart[currentPoint]) { continue; }

            // Исследуем всех соседей текущей точки
            foreach (var direction in movementDirections)
            {
                var neighborPoint = new Point(currentPoint.X + direction.X, currentPoint.Y + direction.Y);
                // Проверяем, что сосед находится в пределах лабиринта
                if (!maze.InBounds(neighborPoint)) { continue; }

                // Проверяем, что соседняя клетка проходима (не стена)
                var cellType = maze.Get(neighborPoint);
                if (cellType == CellType.Wall) { continue; }

                // Вычисляем новую стоимость пути до соседа через текущую точку
                // Все перемещения имеют стоимость 1 в данной реализации
                int newDistance = currentDistance + 1;

                // Если нашли более короткий путь до соседа
                if (!distanceFromStart.ContainsKey(neighborPoint) || newDistance < distanceFromStart[neighborPoint])
                {
                    // Обновляем минимальное расстояние до соседа
                    distanceFromStart[neighborPoint] = newDistance;
                    // Запоминаем предшественника для восстановления пути
                    previousPoint[neighborPoint] = currentPoint;
                    // Добавляем соседа в очередь с новым приоритетом (расстоянием)
                    pointQueue.Enqueue(neighborPoint, newDistance);
                }
            }
        }

        if (!distanceFromStart.ContainsKey(end))
        {
            return null;
        }

        // Восстанавливаем путь от конечной точки до начальной
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