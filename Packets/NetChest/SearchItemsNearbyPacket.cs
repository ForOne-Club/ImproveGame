using ImproveGame.UI.ItemSearcher;

namespace ImproveGame.Packets.NetChest;

public class SearchItemsNearbyPacket : NetModule
{
    private record ChestInfo(ushort ChestId, HashSet<int> ItemTypes);
    private string _searchString;
    private List<ChestInfo> _chestInfos;

    public static void Request(string searchString)
    {
        if (Main.netMode is NetmodeID.Server) return;

        var module = NetModuleLoader.Get<SearchItemsNearbyPacket>();
        module._searchString = searchString;
        module.Send(runLocally: true);
    }

    public override void Send(ModPacket p)
    {
        switch (Main.netMode)
        {
            case NetmodeID.MultiplayerClient:
                p.Write(_searchString);
                break;
            case NetmodeID.Server:
                p.Write(_chestInfos.Count);
                foreach (var chestInfo in _chestInfos)
                {
                    p.Write(chestInfo.ChestId);
                    p.Write(chestInfo.ItemTypes);
                }
                break;
        }
    }

    public override void Read(BinaryReader r)
    {
        switch (Main.netMode)
        {
            case NetmodeID.MultiplayerClient:
                int count = r.ReadInt32();
                _chestInfos = new List<ChestInfo>(count);
                for (int i = 0; i < count; i++)
                {
                    ushort chestId = r.ReadUInt16();
                    var itemTypes = r.ReadIntEnumerable().ToHashSet();
                    _chestInfos.Add(new ChestInfo(chestId, itemTypes));
                }
                break;
            case NetmodeID.Server:
                _searchString = r.ReadString();
                break;
        }
    }

    private bool ChestWithinRange(Chest c, int range)
    {
        var player = Main.netMode is NetmodeID.SinglePlayer ? Main.LocalPlayer : Main.player[Sender];
        var chestCenter = new Vector2((c.x * 16 + 16), (c.y * 16 + 16));
        return (chestCenter - player.Center).Length() < range;
    }

    public override void Receive()
    {
        // 服务器收包，必定是客户端请求
        if (Main.netMode is NetmodeID.Server or NetmodeID.SinglePlayer)
        {
            _chestInfos = new List<ChestInfo>();
            const int itemSearchRange = 400; // 25格
            for (int chestIndex = 0; chestIndex < Main.maxChests; chestIndex++)
            {
                Chest chest = Main.chest[chestIndex];
                if (chest == null || Chest.IsLocked(chest.x, chest.y) || !ChestWithinRange(chest, itemSearchRange))
                    continue;
                
                var chestInfo = new ChestInfo((ushort)chestIndex, new HashSet<int>());

                foreach (Item item in chest.item)
                    chestInfo.ItemTypes.Add(item.type);
                
                _chestInfos.Add(chestInfo);
                
                // 本来的思路是服务器匹配名字相应物品，但是服务器语言可能和客户端不一样，所以还是要把全部type发去
                // if (DealWithItems(_itemTypes, chest.item))
                //     _matchedChests.Add((short)chestIndex);
            }

            if (Main.netMode is not NetmodeID.SinglePlayer)
                Send();
        }

        // 本地收包，必定是服务器那边做好了传来的
        if (Main.netMode is NetmodeID.MultiplayerClient or NetmodeID.SinglePlayer)
        {
            var matchedChests = new HashSet<short>();
            var itemTypes = new HashSet<int>();
            
            foreach ((ushort chestId, HashSet<int> types) in _chestInfos)
            {
                if (DealWithItems(itemTypes, types))
                    matchedChests.Add((short)chestId);
            }

            // 加上本地私货
            var items = Main.LocalPlayer.bank.item
                .Concat(Main.LocalPlayer.bank2.item)
                .Concat(Main.LocalPlayer.bank3.item)
                .Concat(Main.LocalPlayer.bank4.item);
            DealWithItems(itemTypes, items);
            ItemSearcherGUI.TryUpdateItems(matchedChests, itemTypes);
        }
    }

    /// <summary>
    /// 将匹配到的物品存到 typesCollected 内，如果有匹配到任何物品返回 true，否则返回 false
    /// </summary>
    /// <returns>如果有匹配到任何物品返回 true，否则返回 false</returns>
    private bool DealWithItems(ISet<int> typesCollected, IEnumerable<Item> items)
    {
        if (items is null) return false;

        bool matched = false;
        string searchContent = _searchString;
        foreach (Item item in items)
            if (item.MatchWithString(searchContent))
            {
                typesCollected.Add(item.type);
                matched = true;
            }

        return matched;
    }

    /// <summary>
    /// 将匹配到的物品存到 typesCollected 内，如果有匹配到任何物品返回 true，否则返回 false
    /// </summary>
    /// <returns>如果有匹配到任何物品返回 true，否则返回 false</returns>
    private bool DealWithItems(ISet<int> typesCollected, IEnumerable<int> itemTypes)
    {
        if (itemTypes is null) return false;

        bool matched = false;
        string searchContent = _searchString;
        foreach (var type in itemTypes)
        {
            var item = new Item(type);
            if (item.MatchWithString(searchContent))
            {
                typesCollected.Add(item.type);
                matched = true;
            }
        }

        return matched;
    }
}