using Labyrinths.Enums;
using System.Text;

namespace Labyrinths.Models;

/// <summary>
/// Представляет лабиринт, состоящий из клеток различных типов.
/// </summary>
public class Maze
{
    /// <summary>Ширина лабиринта в клетках.</summary>
    public int Width { get; }

    /// <summary>Высота лабиринта в клетках.</summary>
    public int Height { get; }

    /// <summary>Двумерный массив, представляющий сетку лабиринта. Индексы: [y, x].</summary>
    public CellType[,] Grid { get; }

    /// <summary>
    /// Инициализирует новый экземпляр лабиринта указанных размеров.
    /// Все клетки инициализируются как стены.
    /// </summary>
    /// <param name="width">Ширина лабиринта (должна быть положительной)</param>
    /// <param name="height">Высота лабиринта (должна быть положительной)</param>
    public Maze(int width, int height)
    {
        Width = width;
        Height = height;
        Grid = new CellType[height, width];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Grid[y, x] = CellType.Wall;
            }
        }
    }

    /// <summary>Проверяет, находится ли указанная точка в пределах границ лабиринта.</summary>
    public bool InBounds(Point p) => p.X >= 0 && p.X < Width && p.Y >= 0 && p.Y < Height;

    /// <summary>Получает тип клетки в указанной точке.</summary>
    public CellType Get(Point p) => Grid[p.Y, p.X];

    /// <summary>Устанавливает тип клетки в указанной точке.</summary>
    public void Set(Point p, CellType t) => Grid[p.Y, p.X] = t;

    /// <summary>Создаёт текстовое представление лабиринта.</summary>
    public List<string> Render(bool unicode)
    {
        var lines = new List<string>();
        for (int y = 0; y < Height; y++)
        {
            var sb = new StringBuilder();
            for (int x = 0; x < Width; x++)
            {
                sb.Append(Grid[y, x].ToChar(unicode));
            }

            lines.Add(sb.ToString());
        }
        return lines;
    }

    ///<summary>Рисует лабиринт в консоль.</summary>
    public void Draw(bool unicode = false)
    {
        foreach (var line in Render(unicode))
        {
            Console.WriteLine(line);
        }
    }

    ///<summary>Сохраняет лабиринт в текстовый файл.</summary>

    public void SaveToFile(string path, bool unicode = false)
    {
        path = NormalizePath(path);
        EnsureDirectoryExists(path);

        File.WriteAllLines(path, Render(unicode), Encoding.UTF8);
    }

    /// <summary>Загружает лабиринт из текстового файла.</summary>
    public static Maze LoadFromFile(string path)
    {
        path = NormalizePath(path);

        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Maze file not found: {path}");
        }

        var lines = File.ReadAllLines(path, Encoding.UTF8);
        int height = lines.Length;
        int width = lines.Max(l => l.Length);

        var maze = new Maze(width, height);

        for (int y = 0; y < height; y++)
        {
            var line = lines[y].PadRight(width);
            for (int x = 0; x < width; x++)
            {
                string symbol = line.Substring(x, 1);
                maze.Grid[y, x] = CellTypeExtensions.FromChar(symbol);
            }
        }

        return maze;
    }

    /// <summary>Нормализует путь к файлу, обрабатывая относительные пути.</summary>
    private static string NormalizePath(string path)
    {
        if (Path.IsPathRooted(path) && path.StartsWith("/"))
        {
            return Path.Combine(Directory.GetCurrentDirectory(), path.TrimStart('/'));
        }

        return path;
    }

    /// <summary>Обеспечивает существование директории для указанного файлового пути.</summary>
    private static void EnsureDirectoryExists(string path)
    {
        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }
}