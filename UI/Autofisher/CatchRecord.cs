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
        {
            _catches.Add(new Item(type));
            RecordedCatchesSyncer.Sync();
        }
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
        _catches ??= []; // 超级保险
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
        _catches ??= []; // 超级超级保险
        var catches = _catches.Select(item => new ItemTypeData(item)).ToList();
        writer.Write(_catches.Count);
        foreach (var itemData in catches)
            writer.Write(itemData);
    }

    public override void NetReceive(BinaryReader reader)
    {
        List<ItemTypeData> catchData = [];
        int count = reader.ReadInt32();
        for (int i = 0; i < count; i++)
            catchData.Add(reader.ReadItemTypeData());

        _catches = catchData.Select(data => data.Item).ToList();
    }
}