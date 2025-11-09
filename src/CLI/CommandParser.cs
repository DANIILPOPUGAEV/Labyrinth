
namespace Labyrinths.CLI;
public static class CommandParser
{
    public static Dictionary<string, string> Parse(string[] args)
    {
        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var a in args)
        {
            if (!a.StartsWith("--")) { continue; }
            var parts = a[2..].Split('=', 2);
            dict[parts[0]] = parts.Length == 2 ? parts[1] : "true";
        }
        return dict;
    }
}
