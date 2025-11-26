using Labyrinths.Enums;
using Labyrinths.Models;

namespace Labyrinths.Generators;
/// <summary>
/// Генератор случайных лабиринтов.
/// Создает лабиринты путем случайного заполнения клеток стенами и пустотами.
/// 
/// Принцип работы:
/// 1. Создает лабиринт указанных размеров
/// 2. Устанавливает границы лабиринта как стены
/// 3. Внутренние клетки заполняются случайным образом с вероятностью 70% - пустота, 30% - стена
/// 4. Не гарантирует связность или проходимость лабиринта 
///  
/// Сгенерированные лабиринты могут быть непроходимыми!
/// Для гарантированно проходимых лабиринтов лучше использовать DfsGenerator или PrimGenerator.
/// </summary>
public class RandomGenerator : IGenerator
{
    private static readonly Random rnd = new Random();

    /// <summary>
    /// Генерирует случайный лабиринт указанных размеров.
    /// </summary>
    /// <param name="width">Ширина лабиринта (минимум 3)</param>
    /// <param name="height">Высота лабиринта (минимум 3)</param>
    /// <returns>Случайно сгенерированный лабиринт (может быть непроходимым)</returns>
    /// <remarks>
    public Maze Generate(int width, int height)
    {
        // Создание лабиринта с указанными размерами
        var maze = new Maze(width, height);

        // Создаем границы как стены
        for (int x = 0; x < width; x++)
        {
            maze.Set(new Point(x, 0), CellType.Wall);
            maze.Set(new Point(x, height - 1), CellType.Wall);
        }
        for (int y = 0; y < height; y++)
        {
            maze.Set(new Point(0, y), CellType.Wall);
            maze.Set(new Point(width - 1, y), CellType.Wall);
        }

        // Остальные клетки делаем случайно пустыми или стенами
        // Вероятность пустоты: 70%, вероятность стены: 30%
        for (int y = 1; y < height - 1; y++)
        {
            for (int x = 1; x < width - 1; x++)
            {
                maze.Set(new Point(x, y), rnd.NextDouble() < 0.7 ? CellType.Empty : CellType.Wall);
            }
        }

        return maze;
    }
}
