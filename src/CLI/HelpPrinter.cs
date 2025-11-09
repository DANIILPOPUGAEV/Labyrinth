namespace Labyrinths.CLI;

/// <summary>
/// Выводит справочную информацию по использованию CLI приложения.
/// Предоставляет информацию о доступных командах и опциях.
/// </summary>
public static class HelpPrinter
{
    /// <summary>
    /// Отображает основную справочную информацию о приложении.
    /// Включает общее описание, опции и доступные команды.
    /// </summary>
    public static void Show()
    {
        Console.WriteLine("Usage: maze-app [-hV] [COMMAND]");
        Console.WriteLine("Maze generator and solver CLI application.");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  -h, --help      Show this help message and exit.");
        Console.WriteLine("  -V, --version   Print version information and exit.");
        Console.WriteLine();
        Console.WriteLine("Commands:");
        Console.WriteLine("  generate        Generate a maze with specified algorithm and dimensions.");
        Console.WriteLine("  solve           Solve a maze with specified algorithm and points.");
    }
}
