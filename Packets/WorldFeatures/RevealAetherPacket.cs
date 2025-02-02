using ImproveGame.Content;
using ImproveGame.Content.Items.Globes;
using ImproveGame.Content.Items.Globes.Core;
using ImproveGame.Packets.Notifications;
using Terraria.Chat;
using Terraria.DataStructures;
using static ImproveGame.Content.Items.Globes.AetherGlobe;

namespace ImproveGame.Packets.WorldFeatures;

[AutoSync]
public class RevealAetherPacket : NetModule
{
    private Point16 _position;

    public static bool Reveal(Projectile projectile, bool onlyJudging)
    {
        if (StructureDatas.StructuresUnlocked[(byte)StructureDatas.UnlockID.Shimmer])
        {
            if (!onlyJudging && projectile.owner == Main.myPlayer)
                AddNotification(Language.GetTextValue("Mods.ImproveGame.Items.GlobeBase.AlreadyRevealed", 
                    Language.GetTextValue("Mods.ImproveGame.Items.AetherGlobe.BiomeName")), Color.PaleVioletRed * 1.4f);
            return false;
        }

        if (StructureDatas.ShimmerPosition != default)
        {
            if (onlyJudging)
                return true;

            StructureDatas.StructuresUnlocked[(byte)StructureDatas.UnlockID.Shimmer] = true;
            var text2 = Language.GetText("Mods.ImproveGame.Items.GlobeBase.Reveal")
                .WithFormatArgs(Language.GetText("Mods.ImproveGame.Items.AetherGlobe.BiomeName"), Main.player[projectile.owner].name);
            AddNotification(text2.Value, Color.Pink);
            return true;
        }

        if (Main.netMode is NetmodeID.MultiplayerClient)
            return true;

        var player = Main.player[projectile.owner];
        Point16 position = Point16.Zero;
        for (int i = 10; i < Main.maxTilesX - 10; i++)
        {
            for (int j = 10; j < Main.maxTilesY - 10; j++)
            {
                var tile = Framing.GetTileSafely(i, j);
                if (tile.LiquidType != LiquidID.Shimmer)
                    continue;

                for (int x = i - 2;x <= i + 2;x++)
                {
                    for (int y =  j - 2;y <= j + 2;y++)
                    {
                        tile = Framing.GetTileSafely(i, j);
                        if (tile.LiquidType != LiquidID.Shimmer)
                            continue;
                    }
                }

                position = new Vector2(i, j).ToPoint16();
                break;
            }

            if (position != Point16.Zero)
                break;
        }

        if (onlyJudging)
            return position != Point16.Zero;

        if (position == Point16.Zero)
        {
            SyncNotificationKey.Send("Items.AetherGlobe.NotFound", Color.PaleVioletRed * 1.4f, player.whoAmI);
            return false;
        }

        var module = NetModuleLoader.Get<RevealAetherPacket>();
        module._position = position;
        module.Send(runLocally: true);

        // 由于服务器和客户端使用的语言可能不一样，所以用FromKey并专门设了个翻译文本
        var text = NetworkText.FromKey("Mods.ImproveGame.Items.AetherGlobe.Reveal", player.name);
        ChatHelper.BroadcastChatMessage(text, Color.Pink);
        return true;
    }

    public override void Receive()
    {
        StructureDatas.StructuresUnlocked[(byte)StructureDatas.UnlockID.Shimmer] = true;
        StructureDatas.ShimmerPosition = _position;
    }
}