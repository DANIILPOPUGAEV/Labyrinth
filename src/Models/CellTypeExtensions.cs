using Labyrinths.Enums;

namespace Labyrinths.Models
{
    public static class CellTypeExtensions
    {
        private static readonly Dictionary<CellType, (string ascii, string unicode)> VisualMap = new()
        {
            { CellType.Wall,  ("#", "█") },
            { CellType.Empty, (" ", " ") },
            { CellType.Path,  (".", "•") },
            { CellType.Start, ("O", "▲") },
            { CellType.End,   ("X", "▼") }
        };

        // Словарь для парсинга символа в CellType (ASCII + Unicode)
        private static readonly Dictionary<string, CellType> ReadMap =
            VisualMap
                .SelectMany(p => new[]
                {
                    new KeyValuePair<string, CellType>(p.Value.ascii, p.Key),
                    new KeyValuePair<string, CellType>(p.Value.unicode, p.Key)
                })
                .GroupBy(kv => kv.Key)
                .ToDictionary(g => g.Key, g => g.First().Value);

        /// <summary>
        /// Преобразует CellType в символ (ASCII или Unicode)
        /// </summary>
        public static string ToChar(this CellType type, bool unicode = false) =>
            VisualMap[type].unicode != null && unicode ? VisualMap[type].unicode : VisualMap[type].ascii;

        /// <summary>
        /// Преобразует символ (ASCII или Unicode) в CellType
        /// </summary>
        public static CellType FromChar(string symbol) =>
            ReadMap.TryGetValue(symbol, out var type) ? type : CellType.Empty;
    }
}