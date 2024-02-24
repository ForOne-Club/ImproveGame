using ImproveGame.Content.Items;
using ImproveGame.Content.Tiles;
using ImproveGame.Packets.NetChest;
using ImproveGame.Packets.NetStorager;
using ImproveGame.UI.ExtremeStorage;
using ImproveGame.UIFramework;
using Terraria.DataStructures;

namespace ImproveGame.Common.ModPlayers;

public class ExtremeStoragePlayer : ModPlayer
{
    /// <summary> 用于服务器确定该玩家是否打开了某个储存，客户端无需设置，值为 tileEntityID </summary>
    public int UsingStorage = -1;
    
    /// <summary>
    /// 获取所有正在被使用的储存
    /// </summary>
    public static HashSet<int> StoragesBeingUsed()
    {
        // 确保不出现重复元素
        var seenStorages = new HashSet<int>();

        for (int i = 0; i < Main.maxPlayers; i++)
        {
            var player = Main.player[i];
            if (player.active && !player.dead && player.TryGetModPlayer<ExtremeStoragePlayer>(out var modPlayer) &&
                modPlayer.UsingStorage != -1)
            {
                seenStorages.Add(modPlayer.UsingStorage);
            }
        }

        return seenStorages;
    }
    
    public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
    {
        if (Main.netMode is not NetmodeID.Server) return;

        foreach ((int id, TileEntity tileEntity) in TileEntity.ByID)
        {
            if (tileEntity is not TEExtremeStorage storage) continue;

            // 这玩意在服务器上 fromWho 才是正在加入的客户端... toWho 是 -1
            storage.FindAllNearbyChests().ForEach(i => ChestItemOperation.SendAllItems(i, fromWho, toWho));
            SyncDataPacket.Get(id).Send(fromWho, toWho);
        }
    }

    public override void PreUpdate()
    {
        int communicator = ModContent.ItemType<StorageCommunicator>();
        if (Player.whoAmI != Main.myPlayer || Main.netMode == NetmodeID.Server || !ExtremeStorageGUI.Visible ||
            ExtremeStorageGUI.Storage is null || Main.LocalPlayer.HeldItem?.type == communicator ||
            Main.LocalPlayer.HasItem(communicator))
            return;

        var tileEntity = ExtremeStorageGUI.Storage;
        int playerX = (int)(Player.Center.X / 16f);
        int playerY = (int)(Player.Center.Y / 16f);
        if (playerX < tileEntity.Position.X - Player.lastTileRangeX ||
            playerX > tileEntity.Position.X + Player.lastTileRangeX + 1 ||
            playerY < tileEntity.Position.Y - Player.lastTileRangeY ||
            playerY > tileEntity.Position.Y + Player.lastTileRangeY + 1)
        {
            SidedEventTrigger.ToggleViewBody(UISystem.Instance.ExtremeStorageGUI);
        }
        else if (TileLoader.GetTile(Main.tile[tileEntity.Position.ToPoint()].TileType) is not ExtremeStorage)
        {
            SidedEventTrigger.ToggleViewBody(UISystem.Instance.ExtremeStorageGUI);
        }
    }
}