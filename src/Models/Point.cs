namespace Labyrinths.Models;

// Запись для представления точки в лабиринте
record Point(int X, int Y)
{
    public static Point Parse(string s)
    {
        var parts = s.Split(',');
        return parts.Length != 2 ? throw new FormatException("Expect format x,y") : new Point(int.Parse(parts[0]), int.Parse(parts[1]));
    }
}
