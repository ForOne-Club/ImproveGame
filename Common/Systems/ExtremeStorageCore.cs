using ImproveGame.Common.Packets.NetChest;
using ImproveGame.Common.Players;
using ImproveGame.Content.Tiles;
using ImproveGame.Interface.ExtremeStorage;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria.DataStructures;

namespace ImproveGame.Common.Systems;

public class ExtremeStorageCore : ModSystem
{
    private static bool _chestsSetUpdatedThisFrame;
    private static HashSet<int> _chestsThatShouldSync = new();
    private int _chestOld; // 这个是客户端变量，监测该玩家的 player.chest

    public override void Load()
    {
        ToolButtonBase.ToolIcons = GetTexture("UI/ExtremeStorage/ToolIcons");

        // TODO: 实际的多人测试
        IL.Terraria.Chest.PutItemInNearbyChest += il =>
        {
            var c = new ILCursor(il);
            
            // 开头更新 _chestsThatShouldSync 的值
            c.EmitDelegate(() =>
            {
                if (Main.netMode is not NetmodeID.Server || _chestsSetUpdatedThisFrame) return;

                var storages = TileEntity.ByID.Where(i => i.Value is TEExtremeStorage)
                    .Select(i => (TEExtremeStorage)i.Value).ToList();

                _chestsThatShouldSync = new HashSet<int>();
                _chestsSetUpdatedThisFrame = true;

                for (int i = 0; i < Main.maxChests; i++)
                {
                    var chest = Main.chest[i];
                    if (chest is null) continue;
                    if (storages.Any(t => t.ChestInRange(chest)))
                        _chestsThatShouldSync.Add(i);
                }

                // 调试代码
                // Console.WriteLine(_chestsThatShouldSync.Count);
            });

            // IL_018d: ldsfld       class Terraria.Chest[] Terraria.Main::chest
            // IL_0192: ldloc.0      // i
            // IL_0193: ldelem.ref
            // IL_0194: ldfld        class Terraria.Item[] Terraria.Chest::item
            // IL_0199: ldloc.s      j
            // IL_019b: ldelem.ref
            // IL_019c: dup
            // IL_019d: ldfld        int32 Terraria.Item::stack
            // IL_01a2: ldloc.s      num
            // IL_01a4: add
            // IL_01a5: stfld        int32 Terraria.Item::stack

            // 先获取到索引
            int index = -1;
            if (!c.TryGotoNext(MoveType.After,
                    i => i.MatchLdsfld<Main>(nameof(Main.chest)),
                    i => i.Match(OpCodes.Ldloc_0),
                    i => i.MatchLdelemRef(),
                    i => i.MatchLdfld<Chest>(nameof(Chest.item)),
                    i => i.Match(OpCodes.Ldloc_S)))
                throw new Exception("IL code changed");
            EmitIndexGettingCodes();

            if (!c.TryGotoNext(MoveType.After,
                    i => i.MatchLdelemRef(),
                    i => i.MatchDup(),
                    i => i.MatchLdfld<Item>("stack"),
                    i => i.Match(OpCodes.Ldloc_S),
                    i => i.MatchAdd(),
                    i => i.MatchStfld<Item>("stack")))
                throw new Exception("IL code changed");
            EmitSyncCodes();

            /* IL_0251: ldsfld       class Terraria.Chest[] Terraria.Main::chest
             * IL_0256: ldloc.0      // i
             * IL_0257: ldelem.ref
             * IL_0258: ldfld        class Terraria.Item[] Terraria.Chest::item
             * IL_025d: ldloc.s      k
             * IL_025f: ldarg.0      // item
             * IL_0260: callvirt     instance class Terraria.Item Terraria.Item::Clone()
             * IL_0265: stelem.ref
             */
            if (!c.TryGotoNext(MoveType.After,
                    i => i.MatchLdsfld<Main>(nameof(Main.chest)),
                    i => i.Match(OpCodes.Ldloc_0),
                    i => i.MatchLdelemRef(),
                    i => i.MatchLdfld<Chest>(nameof(Chest.item)),
                    i => i.Match(OpCodes.Ldloc_S)))
                throw new Exception("IL code changed");
            EmitIndexGettingCodes();

            if (!c.TryGotoNext(MoveType.After,
                    i => i.Match(OpCodes.Ldarg_0),
                    i => i.MatchCallvirt<Item>("Clone"),
                    i => i.MatchStelemRef()))
                throw new Exception("IL code changed");
            EmitSyncCodes();

            void EmitIndexGettingCodes()
            {
                c.EmitDelegate<Func<sbyte, sbyte>>(slotIndex =>
                {
                    index = slotIndex;
                    return slotIndex;
                });
            }

            void EmitSyncCodes()
            {
                // 推入i, j/k
                c.Emit(OpCodes.Ldloc_0);
                c.EmitDelegate<Action<int>>(chestIndex =>
                {
                    if (Main.netMode is not NetmodeID.Server) return;
                    if (_chestsThatShouldSync is null || !_chestsThatShouldSync.Contains(chestIndex)) return;
                    // 发送数据
                    for (int i = 0; i < Main.maxPlayers; i++)
                    {
                        if (Main.player[i].active && !Main.player[i].dead &&
                            Main.player[i].TryGetModPlayer<ExtremeStoragePlayer>(out var modPlayer) &&
                            modPlayer.UsingStorage != -1) // 只要在使用 ExtremeStorage 就发送，不管箱子在不在附近，不再判断了
                        {
                            ChestItemOperation.SendItem(chestIndex, index, i);
                        }
                    }
                });
            }
        };
    }

    public override void PostUpdateEverything()
    {
        _chestsSetUpdatedThisFrame = false;

        if (Main.netMode is NetmodeID.Server || Main.LocalPlayer.chest == _chestOld)
            return;

        if (_chestOld is not -1 && Main.chest.IndexInRange(_chestOld) && Main.chest[_chestOld] is not null)
        {
            var chest = Main.chest[_chestOld];
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