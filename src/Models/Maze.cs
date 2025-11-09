using Labyrinths.Enums;
using System.Text;

namespace Labyrinths.Models;

/// <summary>
/// Представляет лабиринт, состоящий из клеток различных типов.
/// Лабиринт инициализируется как полностью состоящий из стен, после чего может быть модифицирован.
/// </summary>
public class Maze
{
    /// <summary>
    /// Ширина лабиринта в клетках.
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// Высота лабиринта в клетках.
    /// </summary>
    public int Height { get; }

    /// <summary>
    /// Двумерный массив, представляющий сетку лабиринта.
    /// Индексы: [y, x], где y - строка, x - столбец.
    /// </summary>
    public CellType[,] Grid { get; }

    // <summary>
    /// Инициализирует новый экземпляр лабиринта указанных размеров.
    /// Все клетки инициализируются как стены.
    /// </summary>
    /// <param name="width">Ширина лабиринта (должна быть положительной).</param>
    /// <param name="height">Высота лабиринта (должна быть положительной).</param>
    public Maze(int width, int height)
    {
        Width = width;
        Height = height;
        Grid = new CellType[Height, Width];
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Grid[y, x] = CellType.Wall;
            }
        }
    }

    /// <summary>
    /// Проверяет, находится ли указанная точка в пределах границ лабиринта.
    /// </summary>
    /// <param name="p">Точка для проверки.</param>
    /// <returns>true, если точка находится в пределах лабиринта; иначе false.</returns>
    public bool InBounds(Point p) => p.X >= 0 && p.X < Width && p.Y >= 0 && p.Y < Height;

    /// <summary>
    /// Получает тип клетки в указанной точке.
    /// </summary>
    /// <param name="p">Точка для получения типа клетки.</param>
    /// <returns>Тип клетки в указанной точке.</returns>
    public CellType Get(Point p) => Grid[p.Y, p.X];

    /// <summary>
    /// Устанавливает тип клетки в указанной точке.
    /// </summary>
    /// <param name="p">Точка для установки типа клетки.</param>
    /// <param name="t">Новый тип клетки.</param>
    public void Set(Point p, CellType t) => Grid[p.Y, p.X] = t;

    /// <summary>
    /// Отрисовывает лабиринт в консоли с использованием ASCII или Unicode символов.
    /// </summary>
    /// <param name="useUnicode">Если true, используются Unicode символы для отрисовки; иначе ASCII символы.</param>
    public void Draw(bool useUnicode = false)
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                var cell = Grid[y, x];
                string c = useUnicode ? cell switch
                {
                    CellType.Wall => "█",
                    CellType.Empty => " ",
                    CellType.Path => "•",
                    CellType.Start => "▲",
                    CellType.End => "▼",
                    _ => " "
                } : cell switch
                {
                    CellType.Wall => "#",
                    CellType.Empty => " ",
                    CellType.Path => ".",
                    CellType.Start => "O",
                    CellType.End => "X",
                    _ => " "
                };

                Console.Write(c);
            }
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Сохраняет лабиринт в текстовый файл.
    /// Каждая строка файла представляет строку лабиринта, символы кодируют типы клеток.
    /// </summary>
    /// <param name="path">Путь к файлу для сохранения.</param>
    public void SaveToFile(string path)
    {
        var lines = new List<string>();
        for (int y = 0; y < Height; y++)
        {
            var sb = new StringBuilder();
            for (int x = 0; x < Width; x++)
            {
                var c = Grid[y, x];
                char ch = c switch
                {
                    CellType.Wall => '#',
                    CellType.Empty => ' ',
                    CellType.Start => 'O',
                    CellType.End => 'X',
                    CellType.Path => '.',
                    _ => ' '
                };
                sb.Append(ch);
            }
            lines.Add(sb.ToString());
        }
        File.WriteAllLines(path, lines);
    }

    /// <summary>
    /// Загружает лабиринт из текстового файла.
    /// </summary>
    /// <param name="path">Путь к файлу для загрузки.</param>
    /// <returns>Новый экземпляр лабиринта, загруженный из файла.</returns>
    public static Maze LoadFromFile(string path)
    {
        var lines = File.ReadAllLines(path);
        int height = lines.Length;
        int width = lines.Max(l => l.Length);
        var maze = new Maze(width, height);

        for (int y = 0; y < height; y++)
        {
            var line = lines[y].PadRight(width);
            for (int x = 0; x < width; x++)
            {
                char ch = line[x];
                maze.Grid[y, x] = ch switch
                {
                    '#' => CellType.Wall,
                    'O' => CellType.Start,
                    'X' => CellType.End,
                    '.' => CellType.Path,
                    ' ' => CellType.Empty,
                    _ => CellType.Empty
                };
            }
        }
        return maze;
    }
}