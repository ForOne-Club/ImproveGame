namespace ImproveGame.Core;

public static class Paths
{
    internal static string SavePath => Path.Combine(ModLoader.ModPath, "ImproveGame");

    internal static List<char> IllegalChars = ['\\', '/', ':', '*', '?', '\"', '\'', '<', '>', '|'];
    
    internal static bool IsPathIllegal(this string path) => path.IndexOfAny(IllegalChars.ToArray()) != -1;
}