using ImproveGame.Core;
using ImproveGame.Packets.NetAutofisher;
using Terraria.ModLoader.IO;

namespace ImproveGame.UI.Autofisher;

public class CatchRecord : ModSystem
{
    private static List<Item> _catches = [];

    /// <summary>
    /// 添加捕获物品，不可能在客户端调用
    /// </summary>
    public static void AddCatch(int type)
    {
        if (_catches.All(i => i.type != type))
            _catches.Add(new Item(type));
        RecordedCatchesSyncer.Sync();
    }

    public static void SetCatchesList(List<Item> list)
    {
        _catches = list ?? [];
    }

    public static List<Item> GetRecordedCatches => _catches;

    public override void ClearWorld()
    {
        _catches = [];
    }

    public override void SaveWorldData(TagCompound tag)
    {
        var catches = _catches.Select(item => new ItemTypeData(item)).ToList();
        tag["catches"] = catches;
    }

    public override void LoadWorldData(TagCompound tag)
    {
        var catchData = tag.Get<List<ItemTypeData>>("catches") ?? [];
        _catches = catchData.Select(data => data.Item).ToList();
    }

    public override void NetSend(BinaryWriter writer)
    {
        writer.Write(_catches.Count);
        foreach (var item in _catches)
            writer.Write(item.type);
    }

    public override void NetReceive(BinaryReader reader)
    {
        _catches.Clear();
        int count = reader.ReadInt32();
        for (int i = 0; i < count; i++)
            _catches.Add(new Item(reader.ReadInt32()));
    }
}