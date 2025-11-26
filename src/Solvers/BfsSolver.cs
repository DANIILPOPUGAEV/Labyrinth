using Labyrinths.Enums;
using Labyrinths.Models;

namespace Labyrinths.Solvers;

/// <summary>
/// Реализация алгоритма BFS (Breadth-First Search) для поиска пути в лабиринте.
/// BFS - это невзвешенный алгоритм поиска, который исследует все точки на одинаковом расстоянии от старта перед переходом к следующему уровню.
/// 
/// Принцип работы BFS:
/// 1. Использует очередь (FIFO) для обработки точек
/// 2. Исследует все соседей текущего уровня перед переходом к следующему
/// 3. Гарантирует нахождение кратчайшего пути в невзвешенном графе
/// 4. Равномерно "распространяется" во всех направлениях от стартовой точки
/// </summary>
public class BfsSolver : ISolver
{
    public List<Point>? Solve(Maze maze, Point start, Point end)
    {
        /// <summary>
        /// Находит кратчайший путь от начальной до конечной точки в лабиринте с использованием алгоритма BFS.
        /// </summary>
        /// <param name="maze">Лабиринт для поиска пути</param>
        /// <param name="start">Начальная точка пути</param>
        /// <param name="end">Конечная точка пути</param>
        /// <returns>
        /// Список точек, представляющий кратчайший путь от start до end по количеству шагов,
        /// или null если путь не существует
        /// </returns>
        /// <exception cref="ArgumentNullException">Если maze равен null</exception>
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

        // Основной цикл BFS - пока есть точки для обработки
        while (queue.Count > 0)
        {
            // Извлекаем следующую точку из очереди (FIFO)
            var current = queue.Dequeue();
            // Если достигли конечной точки - завершаем поиск
            if (current.Equals(end)) { break; }
            // Исследуем всех соседей текущей точки
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

        // Если конечная точка не была достигнута (нет в словаре предшественников)
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