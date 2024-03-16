using ImproveGame.Content.Tiles;
using ImproveGame.UI.Autofisher;

namespace ImproveGame.Packets.NetAutofisher;

public class MachineExcludedItemSyncer : NetModule
{
    [AutoSync] private int _tileEntityID;
    private List<CatchData> _excludedItemData;

    /// <summary>
    /// 这个方法仅可能由客户端同步给服务器调用
    /// </summary>
    public static void Sync(int tileEntityID, List<CatchData> excludedItem)
    {
        var module = NetModuleLoader.Get<MachineExcludedItemSyncer>();
        module._tileEntityID = tileEntityID;
        module._excludedItemData = excludedItem;
        module.Send();
    }

    public override void Receive()
    {
        if (TryGetTileEntityAs<TEAutofisher>(_tileEntityID, out var autofisher))
            autofisher.ExcludedItems = _excludedItemData;
        
        if (Main.netMode is NetmodeID.Server)
            Send(-1, Sender);
    }

    public override void Send(ModPacket p)
    {
        p.Write(_excludedItemData);
    }

    public override void Read(BinaryReader r)
    {
        _excludedItemData = r.ReadListCatchData();
    }
}

/// <summary>
/// 同步世界记录捕获物品，仅可能由服务器发送到全体客户端
/// </summary>
public class RecordedCatchesSyncer : NetModule
{
    private List<CatchData> _worldExcludedItem;

    public static void Sync()
    {
        var module = NetModuleLoader.Get<RecordedCatchesSyncer>();
        module._worldExcludedItem = CatchRecord.GetRecordedCatches.Select(i => new CatchData(i)).ToList();
        module.Send();
    }

    public override void Receive()
    {
        CatchRecord.SetCatchesList(_worldExcludedItem.Select(i => i.Item).ToList());
    }

    public override void Send(ModPacket p)
    {
        p.Write(_worldExcludedItem);
    }

    public override void Read(BinaryReader r)
    {
        _worldExcludedItem = r.ReadListCatchData();
    }
}