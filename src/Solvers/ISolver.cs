using Labyrinths.Models;

namespace Labyrinths.Solvers;
/// <summary>
/// Интерфейс для алгоритмов решения лабиринтов.
/// Определяет контракт для всех алгоритмов поиска пути в лабиринте.
/// </summary>
interface ISolver
{
    /// <summary>
    /// Находит путь от начальной до конечной точки в лабиринте.
    /// </summary>
    List<Point>? Solve(Maze maze, Point start, Point end);
}
