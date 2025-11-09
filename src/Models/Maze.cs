using Labyrinths.Enums;
using System.Text;

namespace Labyrinths.Models;
// Класс, представляющий лабиринт (в конструкторе инициализирует все клетки как стены)
public class Maze
{
    public int Width { get; }
    public int Height { get; }
    public CellType[,] Grid { get; }

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

    // Проверка, находится ли точка в пределах лабиринта
    public bool InBounds(Point p) => p.X >= 0 && p.X < Width && p.Y >= 0 && p.Y < Height;

    // Получение типа клетки по координатам точки
    public CellType Get(Point p) => Grid[p.Y, p.X];

    // Установка типа клетки по координатам точки
    public void Set(Point p, CellType t) => Grid[p.Y, p.X] = t;

    // Отрисовка лабиринта в консоль
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

    // Сохранение лабиринта в файл
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

    // Загрузка лабиринта из файла
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