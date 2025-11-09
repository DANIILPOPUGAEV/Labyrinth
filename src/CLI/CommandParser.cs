namespace Labyrinths.CLI;

/// <summary>
/// Парсер аргументов командной строки.
/// Преобразует аргументы в словарь ключ-значение, поддерживая различные форматы параметров.
/// </summary>
public static class CommandParser
{
    /// <summary>
    /// Парсит массив аргументов командной строки в словарь опций.
    /// </summary>
    /// <param name="args">Аргументы командной строки</param>
    /// <returns>Словарь опций, где ключ - название параметра, значение - значение параметра</returns>
    /// <remarks>
    /// Поддерживаемые форматы:
    /// --name=value
    /// --name value
    /// --name (значение по умолчанию "true")
    /// -n=value
    /// -n value
    /// -n (значение по умолчанию "true")
    /// </remarks>
    public static Dictionary<string, string> Parse(string[] args)
    {
        var options = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        for (int i = 0; i < args.Length; i++)
        {
            var arg = args[i];

            if (arg.StartsWith("--"))
            {
                var parts = arg[2..].Split('=', 2);
                string key = parts[0];
                string value = parts.Length == 2
                    ? parts[1]
                    : (i + 1 < args.Length && !args[i + 1].StartsWith("-") ? args[++i] : "true");
                options[key] = value;
                continue;
            }

            if (arg.StartsWith("-") && arg.Length >= 2)
            {
                string shortKey = arg[1].ToString();
                string key = shortKey switch
                {
                    "a" => "algorithm",
                    "w" => "width",
                    "h" => "height",
                    "o" => "output",
                    "f" => "file",
                    "s" => "start",
                    "e" => "end",
                    "u" => "unicode",
                    _ => shortKey
                };

                string value = null!;
                if (arg.Length > 2)
                {
                    if (arg[2] == '=')
                    {
                        value = arg[3..];
                    }
                    else
                    {
                        value = arg[2..];
                    }
                }
                else if (i + 1 < args.Length && !args[i + 1].StartsWith("-"))
                {
                    value = args[++i];
                }
                else
                {
                    value = "true";
                }

                options[key] = value;
            }
        }

        return options;
    }
}