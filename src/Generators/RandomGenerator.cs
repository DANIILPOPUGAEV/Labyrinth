using Labyrinths.Enums;
using Labyrinths.Models;

namespace Labyrinths.Generators;
public class RandomGenerator : IGenerator
{
    private static readonly Random rnd = new Random();

    public Maze Generate(int width, int height)
    {
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
