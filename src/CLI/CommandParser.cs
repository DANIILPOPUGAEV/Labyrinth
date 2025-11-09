
namespace Labyrinths.CLI;
public static class CommandParser
{
    public static Dictionary<string, string> Parse(string[] args)
    {
        var options = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        for (int i = 0; i < args.Length; i++)
        {
            var arg = args[i];

            // Длинная форма --key=value или --key value
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

            // Короткая форма -k=value или -k value
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
                    _ => shortKey
                };

                string value = null!;
                if (arg.Length > 2)
                {
                    if (arg[2] == '=') // -a=dfs
                    {
                        value = arg[3..];
                    }
                    else // случай вроде -w21
                    {
                        value = arg[2..];
                    }
                }
                else if (i + 1 < args.Length && !args[i + 1].StartsWith("-")) // -a dfs
                {
                    value = args[++i];
                }
                else // флаг без значения
                {
                    value = "true";
                }

                options[key] = value;
            }
        }

        return options;
    }
}