using ImproveGame.Content.Functions.ChainedAmmo;
using ImproveGame.Core;
using Terraria.DataStructures;

namespace ImproveGame.Common.Utils.Extensions;

public static class NetExtensions
{
    public static void Write(this BinaryWriter writer, IEnumerable<short> value)
    {
        var list = value.ToList();
        writer.Write(list.Count);
        foreach (var o in list)
            writer.Write(o);
    }

    public static IEnumerable<short> ReadShortEnumerable(this BinaryReader reader)
    {
        int count = reader.ReadInt32();
        for (int i = 0; i < count; i++)
            yield return reader.ReadInt16();
    }

    public static void Write(this BinaryWriter writer, IEnumerable<int> value)
    {
        var list = value.ToList();
        writer.Write(list.Count);
        foreach (var o in list)
            writer.Write(o);
    }

    public static IEnumerable<int> ReadIntEnumerable(this BinaryReader reader)
    {
        int count = reader.ReadInt32();
        for (int i = 0; i < count; i++)
            yield return reader.ReadInt32();
    }

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

    public static void Write(this BinaryWriter writer, AmmoChain value)
    {
        writer.WriteRGB(value.Color);
        writer.Write(value.Chain.Count);
        foreach ((ItemTypeData itemTypeData, int times) in value.Chain)
        {
            writer.Write(itemTypeData);
            writer.Write((ushort)times);
        }
    }

    public static AmmoChain ReadAmmoChain(this BinaryReader reader)
    {
        var chain = new AmmoChain
        {
            Color = reader.ReadRGB()
        };
        int count = reader.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            ItemTypeData data = reader.ReadItemTypeData();
            int times = reader.ReadUInt16();
            chain.Chain.Add(new AmmoChain.Ammo(data, times));
        }

        return chain;
    }
}