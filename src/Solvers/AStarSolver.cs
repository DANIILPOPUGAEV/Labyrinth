using Labyrinths.Enums;
using Labyrinths.Models;

namespace Labyrinths.Solvers;

// <summary>
/// Реализация алгоритма A* (A-star) для поиска пути в лабиринте.
/// 
/// Принцип работы A*:
/// 1. Использует эвристическую функцию для оценки стоимости пути до цели
/// 2. Поддерживает два множества: открытые (кандидаты) и закрытые (обработанные) точки
/// 3. На каждой итерации выбирает точку с наименьшей общей оценкой стоимости
/// 4. Обновляет стоимости соседей при нахождении более короткого пути
/// 
/// Формула оценки: f(n) = g(n) + h(n)
/// - g(n): фактическая стоимость пути от старта до точки n
/// - h(n): эвристическая оценка стоимости от точки n до цели
/// </summary>
public class AStarSolver : ISolver
{
    /// <summary>
    /// Вычисляет манхэттенское расстояние между двумя точками.
    /// Манхэттенское расстояние - сумма абсолютных разностей координат.
    /// Является допустимой эвристикой для сеток с движением в 4 направлениях.
    /// </summary>
    /// <param name="a">Начальная точка</param>
    /// <param name="b">Конечная точка</param>
    /// <returns>Манхэттенское расстояние между точками</returns>
    /// <remarks>
    /// Для точек (x1,y1) и (x2,y2): |x1-x2| + |y1-y2|
    /// Всегда меньше или равно фактическому расстоянию, что гарантирует оптимальность A*.
    /// </remarks>
    private static int Heuristic(Point a, Point b) =>
        Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);


    /// <summary>
    /// Находит кратчайший путь от начальной до конечной точки в лабиринте с использованием алгоритма A*.
    /// </summary>
    /// <param name="maze">Лабиринт для поиска пути</param>
    /// <param name="start">Начальная точка пути</param>
    /// <param name="end">Конечная точка пути</param>
    /// <returns>
    /// Список точек, представляющий кратчайший путь от start до end, 
    /// или null если путь не существует
    /// </returns>
    /// <exception cref="ArgumentNullException">Если maze равен null</exception>
    public List<Point>? Solve(Maze maze, Point start, Point end)
    {
        // Проверка валидности входных точек
        if (!maze.InBounds(start) || !maze.InBounds(end)) { return null; }

        // g(n) - фактическая стоимость от старта до точки n
        var distanceFromStart = new Dictionary<Point, int>();

        // f(n) = g(n) + h(n) - общая оценка стоимости
        var totalEstimatedCost = new Dictionary<Point, int>();

        // Для восстановления пути - храним предшественников
        var previousPoint = new Dictionary<Point, Point>();

        // Очередь с приоритетом для выбора следующей точки с минимальной f(n)
        var openPoints = new PriorityQueue<Point, int>();

        // Множество уже обработанных точек
        var closedPoints = new HashSet<Point>();

        distanceFromStart[start] = 0;
        totalEstimatedCost[start] = Heuristic(start, end);
        previousPoint[start] = start;

        openPoints.Enqueue(start, totalEstimatedCost[start]);

        // Возможные направления движения (вверх, вниз, влево, вправо)
        var movementDirections = new[]
        {
            new Point(1, 0),
            new Point(-1, 0),
            new Point(0, 1),
            new Point(0, -1)
        };

        // Основной цикл алгоритма A*
        while (openPoints.Count > 0)
        {
            // Извлекаем точку с наименьшей общей оценкой стоимости
            openPoints.TryDequeue(out var currentPoint, out var _);
            if (currentPoint!.Equals(end)) { break; }

            if (closedPoints.Contains(currentPoint)) { continue; }

            closedPoints.Add(currentPoint);

            // Исследуем всех соседей текущей точки
            foreach (var direction in movementDirections)
            {
                var neighborPoint = new Point(currentPoint.X + direction.X, currentPoint.Y + direction.Y);

                if (!maze.InBounds(neighborPoint)) { continue; }

                if (maze.Get(neighborPoint) == CellType.Wall) { continue; }

                int tentativeDistance = distanceFromStart[currentPoint] + 1;

                // Если нашли более короткий путь до соседа
                if (!distanceFromStart.ContainsKey(neighborPoint) || tentativeDistance < distanceFromStart[neighborPoint])
                {
                    // Обновляем стоимость g(n)
                    distanceFromStart[neighborPoint] = tentativeDistance;
                    // Запоминаем предшественника для восстановления пути
                    previousPoint[neighborPoint] = currentPoint;
                    // Обновляем общую оценку f(n) = g(n) + h(n)
                    totalEstimatedCost[neighborPoint] = tentativeDistance + Heuristic(neighborPoint, end);
                    // Добавляем соседа в очередь с новым приоритетом
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

