using Terraria.DataStructures;

namespace ImproveGame.Common.Utils.Extensions;

public static class NetExtensions
{
    public static void Write(this BinaryWriter writer, IEnumerable<Point16> value)
    {
        var list = value.ToList();
        writer.Write(list.Count);
        foreach (var o in list)
            writer.Write(o);
    }

    public static IEnumerable<Point16> ReadPoint16List(this BinaryReader reader)
    {
        int count = reader.ReadInt32();
        for (int i = 0; i < count; i++)
            yield return reader.ReadPoint16();
    }
}