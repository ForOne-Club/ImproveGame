using ImproveGame.Content;
using ImproveGame.Content.Items.Globes;
using ImproveGame.Packets.Notifications;
using Terraria.Chat;
using Terraria.DataStructures;

namespace ImproveGame.Packets.WorldFeatures;

[AutoSync]
public class RevealEnchantedSwordPacket : NetModule
{
    private Point16 _position;

    public static bool Reveal(Projectile projectile, bool onlyJudging)
    {
        if (Main.netMode is NetmodeID.MultiplayerClient)
            return true;

        var player = Main.player[projectile.owner];
        var playerPosition = player.position.ToTileCoordinates().ToVector2();
        Point16 position = Point16.Zero;
        float currentDistance = float.MaxValue;
        for (int i = 10; i < Main.maxTilesX - 10; i++)
        {
            for (int j = 10; j < Main.maxTilesY - 10; j++)
            {
                var tile = Framing.GetTileSafely(i, j);
                if (!tile.HasTile || tile.TileType is not TileID.LargePiles2 ||
                    tile.TileFrameX is not 918 || tile.TileFrameY is not 0)
                    continue;

                var tilePosition = new Vector2(i, j);
                if (StructureDatas.EnchantedSwordPositions.Contains(tilePosition.ToPoint16()))
                    continue;
                
                var newDistance = tilePosition.Distance(playerPosition);
                if (newDistance < currentDistance)
                {
                    currentDistance = newDistance;
                    position = tilePosition.ToPoint16();
                }
            }
        }

        if (onlyJudging)
            return position != Point16.Zero;

        if (position == Point16.Zero)
        {
            SyncNotificationKey.Send("Items.EnchantedSwordGlobe.NotFound", Color.PaleVioletRed * 1.4f, player.whoAmI);
            return false;
        }

        var module = NetModuleLoader.Get<RevealEnchantedSwordPacket>();
        module._position = position;
        module.Send(runLocally: true);

        // 由于服务器和客户端使用的语言可能不一样，所以用FromKey并专门设了个翻译文本
        var text = NetworkText.FromKey("Mods.ImproveGame.Items.EnchantedSwordGlobe.Reveal", player.name);
        ChatHelper.BroadcastChatMessage(text, Color.Pink);
        return true;
    }

    public override void Receive()
    {
        StructureDatas.EnchantedSwordPositions.Add(_position);
    }
}