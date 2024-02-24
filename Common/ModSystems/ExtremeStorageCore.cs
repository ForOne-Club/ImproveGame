using ImproveGame.Common.ModPlayers;
using ImproveGame.Content.Tiles;
using ImproveGame.Packets.NetChest;
using ImproveGame.UI.ExtremeStorage;
using ImproveGame.UI.ExtremeStorage.ToolButtons;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.ID;

namespace ImproveGame.Common.ModSystems;

public class ExtremeStorageCore : ModSystem
{
    private int _chestOld; // 这个是客户端变量，监测该玩家的 player.chest

    public override void Load()
    {
        // 同步快速堆叠
        IL_Chest.PutItemInNearbyChest += il =>
        {
            // 根据上面的IL代码，写出对应的查找代码
            var c = new ILCursor(il);
            
            /* 第一部分: 把物品堆叠到同类物品内
             * IL_00d0: ldsfld       class Terraria.Chest[] Terraria.Main::chest
             * IL_00d5: ldloc.1      // i
             * IL_00d6: ldelem.ref
             * IL_00d7: ldfld        class Terraria.Item[] Terraria.Chest::item
             * IL_00dc: ldloc.s      index
             * IL_00de: ldelem.ref
             * IL_00df: ldarg.0      // item
             * IL_00e0: ldloca.s     numTransferred
             * IL_00e2: ldc.i4.0
             * IL_00e3: call         bool Terraria.ModLoader.ItemLoader::TryStackItems(class Terraria.Item, class Terraria.Item, int32&, bool)
             * IL_00e8: brfalse.s    IL_010e
             */
            if (!c.TryGotoNext(
                    MoveType.After,
                    i => i.MatchLdsfld(typeof(Main), nameof(Main.chest)),
                    i => i.Match(OpCodes.Ldloc_1),
                    i => i.MatchLdelemRef(),
                    i => i.MatchLdfld(typeof(Chest), nameof(Chest.item)),
                    i => i.Match(OpCodes.Ldloc_S)))
                return;

            // 获取物品处在的格子
            int itemSlot = -1;
            c.EmitDelegate<Func<int, int>>(returnValue =>
            {
                itemSlot = returnValue;
                return returnValue;
            });

            if (!c.TryGotoNext(
                    MoveType.After,
                    i => i.MatchLdelemRef(),
                    i => i.MatchLdarg(0),
                    i => i.Match(OpCodes.Ldloca_S),
                    i => i.Match(OpCodes.Ldc_I4_0),
                    i => i.MatchCall(typeof(ItemLoader), nameof(ItemLoader.TryStackItems)),
                    i => i.Match(OpCodes.Brfalse_S)))
                return;

            c.Emit(OpCodes.Ldloc_1); // 箱子 index
            c.EmitDelegate<Action<int>>(chestIndex => TrySend(chestIndex, itemSlot));
            
            /* 第二部分: 箱子里有空位，把物品放到空位里面
             * IL_017a: ldloc.0      // flag1
             * IL_017b: brfalse.s    IL_0199
             * IL_017d: ldsfld       class Terraria.Chest[] Terraria.Main::chest
             * IL_0182: ldloc.1      // i
             * IL_0183: ldelem.ref
             * IL_0184: ldfld        class Terraria.Item[] Terraria.Chest::item
             * IL_0189: ldloc.s      index_V_8
             * IL_018b: ldarg.0      // item
             * IL_018c: callvirt     instance class Terraria.Item Terraria.Item::Clone()
             * IL_0191: stelem.ref
             */
            if (!c.TryGotoNext(
                    MoveType.After,
                    i => i.Match(OpCodes.Ldloc_0),
                    i => i.Match(OpCodes.Brfalse_S),
                    i => i.MatchLdsfld(typeof(Main), nameof(Main.chest)),
                    i => i.Match(OpCodes.Ldloc_1),
                    i => i.MatchLdelemRef(),
                    i => i.MatchLdfld(typeof(Chest), nameof(Chest.item)),
                    i => i.Match(OpCodes.Ldloc_S)))
                return;

            // 获取物品处在的格子
            itemSlot = -1;
            c.EmitDelegate<Func<int, int>>(returnValue =>
            {
                itemSlot = returnValue;
                return returnValue;
            });

            if (!c.TryGotoNext(
                    MoveType.After,
                    i => i.MatchLdarg(0),
                    i => i.MatchCallvirt(typeof(Item), nameof(Item.Clone)),
                    i => i.MatchStelemRef()))
                return;

            c.Emit(OpCodes.Ldloc_1); // 箱子 index
            c.EmitDelegate<Action<int>>(chestIndex => TrySend(chestIndex, itemSlot));
        };
    }

    private void TrySend(int chestIndex, int itemSlot)
    {
        if (Main.netMode is not NetmodeID.Server || itemSlot is < 0 or >= Chest.maxItems ||
            !Main.chest.IndexInRange(chestIndex))
            return;
        ChestItemOperation.SendItem(chestIndex, itemSlot);
    }

    public override void PostUpdateEverything()
    {
        if (Main.netMode is NetmodeID.Server || Main.LocalPlayer.chest == _chestOld)
            return;

        if (_chestOld is not -1 && Main.chest.IndexInRange(_chestOld) && Main.chest[_chestOld] is not null)
        {
            // 为了降低传输压力，只传输在 ExtremeStorage 附近的箱子
            // (所以这就是你用遍历增加运算压力的理由!?)
            foreach ((_, TileEntity tileEntity) in TileEntity.ByID)
            {
                if (tileEntity is not TEExtremeStorage storage || !storage.ChestInRange(_chestOld))
                {
                    continue;
                }

                // 发送带转发的全物品包
                ChestItemOperation.SendAllItemsWithSync(_chestOld);
                break;
            }
        }

        _chestOld = Main.LocalPlayer.chest;
    }
}

public class StorageMaterialConsumer : ModPlayer
{
    public override IEnumerable<Item> AddMaterialsForCrafting(out ItemConsumedCallback itemConsumedCallback)
    {
        if (ExtremeStorageGUI.Visible && ExtremeStorageGUI.Storage.UseForCrafting &&
            ExtremeStorageGUI.CurrentGroup is not ItemGroup.Setting)
        {
            var chests = ExtremeStorageGUI.FindChestsWithCurrentGroup();

            // 建立物品列表
            var itemList = new List<Item>();
            chests.ForEach(i => itemList.AddRange(Main.chest[i].item));

            itemConsumedCallback = (item, index) =>
            {
                int chestIndexInList = index / Chest.maxItems;
                int itemIndex = index % Chest.maxItems;
                if (chestIndexInList >= chests.Count)
                    return;

                int chestIndex = chests[chestIndexInList];
                if (!Main.chest.IndexInRange(chestIndex))
                    return;

                if (Main.netMode is NetmodeID.MultiplayerClient)
                {
                    // 同步箱子信息
                    ChestItemOperation.SendItemWithSync(chestIndex, itemIndex);
                }
            };

            return itemList;
        }

        return base.AddMaterialsForCrafting(out itemConsumedCallback);
    }
}