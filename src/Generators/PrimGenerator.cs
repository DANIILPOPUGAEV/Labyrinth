using Labyrinths.Enums;
using Labyrinths.Models;

namespace Labyrinths.Generators;

/// <summary>
/// Генератор лабиринта с использованием алгоритма Прима (Randomized Prim's Algorithm).
/// Алгоритм создает лабиринты с более равномерным распределением ветвлений и меньшим количеством длинных коридоров по сравнению с DFS.
/// 
/// Принцип работы алгоритма Прима:
/// 1. Начинается с одной случайной клетки, помеченной как проходимая
/// 2. Все стены, соседствующие с проходимыми клетками, добавляются в список кандидатов
/// 3. Случайным образом выбирается стена из списка кандидатов
/// 4. Если клетка за выбранной стеной еще не проходима, стена "пробивается" и клетка добавляется в лабиринт
/// 5. Стены, соседствующие с новой клеткой, добавляются в список кандидатов
/// 6. Процесс повторяется пока есть стены-кандидаты
/// </summary>
public class PrimGenerator : IGenerator
{
    private static readonly Random _rnd = new();

    /// <summary>
    /// Генерирует лабиринт указанных размеров с использованием алгоритма Прима.
    /// </summary>
    /// <param name="width">Ширина лабиринта (минимум 3, приводится к нечетному числу)</param>
    /// <param name="height">Высота лабиринта (минимум 3, приводится к нечетному числу)</param>
    /// <returns>Сгенерированный лабиринт с равномерным распределением ветвлений</returns>
    /// <example>
    /// Генерация лабиринта 21x21:
    /// <code>
    /// var generator = new PrimGenerator();
    /// var maze = generator.Generate(21, 21);
    /// </code>
    /// </example>
    public Maze Generate(int width, int height)
    {
        // Приведение размеров к нечетным числам для симметричной сетки
        int w = Math.Max(3, width | 1);
        int h = Math.Max(3, height | 1);
        var maze = new Maze(w, h);

        // Стартовая точка - всегда (1,1) для создания границ
        int sx = 1, sy = 1;
        maze.Set(new Point(sx, sy), CellType.Empty);

        // Список стен возможных для обработки
        // Каждая запись содержит: (стена, клетка-источник)
        var walls = new List<(Point wall, Point from)>();

        // Локальная функция для добавления стен в список кандидатов
        void TryAddWall(int x, int y, Point from)
        {
            var p = new Point(x, y);
            if (maze.InBounds(p) && maze.Get(p) == CellType.Wall) { walls.Add((p, from)); }
        }

        TryAddWall(sx + 1, sy, new Point(sx, sy));
        TryAddWall(sx - 1, sy, new Point(sx, sy));
        TryAddWall(sx, sy + 1, new Point(sx, sy));
        TryAddWall(sx, sy - 1, new Point(sx, sy));

        // Основной цикл алгоритма для обработки стены пока они есть
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