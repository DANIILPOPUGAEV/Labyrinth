namespace Labyrinths.Models;

/// <summary>
/// Представляет точку с координатами X и Y в лабиринте.
/// Используется для указания позиций клеток, стартовых и конечных точек.
/// </summary>
/// <param name="X">Координата X (горизонтальная)</param>
/// <param name="Y">Координата Y (вертикальная)</param>
public record Point(int X, int Y)
{
    /// <summary>
    /// Парсит строку в формате "x,y" и создает экземпляр Point.
    /// </summary>
    /// <param name="s">Строка для парсинга в формате "x,y"</param>
    /// <returns>Новый экземпляр Point с указанными координатами</returns>
    /// <exception cref="FormatException">Выбрасывается если строка имеет неверный формат</exception>
    /// <example>
    /// var point = Point.Parse("3,5"); // X=3, Y=5
    /// </example>
    public static Point Parse(string s)
    {
        var parts = s.Split(',');
        return parts.Length != 2 ? throw new FormatException("Expect format x,y") : new Point(int.Parse(parts[0]), int.Parse(parts[1]));
    }
}

