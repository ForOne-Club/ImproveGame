using ImproveGame.Common.Packets.NetChest;
using ImproveGame.Common.ModPlayers;
using ImproveGame.Content.Tiles;
using ImproveGame.Interface.ExtremeStorage;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria.DataStructures;

namespace ImproveGame.Common.ModSystems;

public class ExtremeStorageCore : ModSystem
{
    private static bool _chestsSetUpdatedThisFrame;
    private static HashSet<int> _chestsThatShouldSync = new();
    private int _chestOld; // 这个是客户端变量，监测该玩家的 player.chest

    public override void Load()
    {
        ToolButtonBase.ToolIcons = GetTexture("UI/ExtremeStorage/ToolIcons");

        // TODO: 实际的多人测试
        // 以前用的IL，1.4.4之后炸了，我也没心思再写一个了，直接禁用吧
        On_Chest.PutItemInNearbyChest += (orig, item, position) =>
        {
            if (Main.netMode is NetmodeID.SinglePlayer)
                return orig.Invoke(item, position);

            var storagesUsing = ExtremeStoragePlayer.StoragesBeingUsed();
            foreach ((int id, TileEntity tileEntity) in TileEntity.ByID)
            {
                if (!storagesUsing.Contains(id) || tileEntity is not TEExtremeStorage storage)
                    continue;
                // 判断距离，超出1.5个屏幕(1920*1.5)的不管，用DistanceSQ没有开根号的开销
                if (storage.Position.ToWorldCoordinates().DistanceSQ(position) > Math.Pow(2880, 2))
                    continue;

                // 有储存，不堆叠
                Main.NewText(GetText("UI.ExtremeStorage.RejectNearbyStack"));
                return item;
            }

            // 全部检查通过
            return orig.Invoke(item, position);
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