using ImproveGame.Packets.Notifications;
using ImproveGame.Packets.WorldFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Chat;
using Terraria.DataStructures;

namespace ImproveGame.Content.Items.Globes.Core;

public static class GlobeRevealer
{
    public static void NoDataNotification(Globe dummyItem)
    {
        AddNotification(Language.GetText("Mods.ImproveGame.Items.GlobeBase.NotFound")
        .WithFormatArgs(dummyItem.GetLocalizedValue("BiomeName")).Value, Globe.hintTextColor);
    }
    public static void NotFoundNotification(Globe dummyItem)
    {
        AddNotification(dummyItem.GetLocalizedValue("NotFound"), Globe.hintTextColor);
    }
    public static void AlreadyRevealedNotification(Globe dummyItem)
    {
        if (dummyItem is OnceForAllGlobe)
            AddNotification(Language.GetText("Mods.ImproveGame.Items.GlobeBase.AlreadyRevealed")
            .WithFormatArgs(dummyItem.GetLocalizedValue("BiomeName")).Value, Globe.hintTextColor);
        else
            AddNotification(dummyItem.GetLocalizedValue("AlreadyRevealed"), Globe.hintTextColor);
    }
    public static void RevealNotification(Globe dummyItem, string name)
        => AddNotification(Language.GetText("Mods.ImproveGame.Items.GlobeBase.Reveal")
            .WithFormatArgs(dummyItem.GetLocalizedValue("BiomeName"), name).Value, Globe.foundColor);
    public static void RevealBroadcast(Globe dummyItem, string name) =>
        // 由于服务器和客户端使用的语言可能不一样，所以用FromKey并专门设了个翻译文本
        ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Mods.ImproveGame.Items.GlobeBase.Reveal", dummyItem.GetLocalizedValue("BiomeName"), name), Globe.foundColor);

    //这两个都是实时查找，一次解锁一个型
    public static bool RevealPlantera(Projectile projectile, Globe dummyItem, bool onlyJudging)
    {
        if (Main.netMode is NetmodeID.MultiplayerClient) //此时由服务器进行查找
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
                if (!tile.HasTile || tile.TileType is not TileID.PlanteraBulb ||
                    tile.TileFrameX is not 18 || tile.TileFrameY is not 18)
                    continue;

                var tilePosition = new Vector2(i, j);
                if (StructureDatas.PlanteraPositions.Contains(tilePosition.ToPoint16()))
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
            SyncNotificationKey.Send("Items.PlanteraGlobe.NotFound", Globe.hintTextColor, player.whoAmI);
            return false;
        }

        var module = NetModuleLoader.Get<RevealPlanteraPacket>();
        module._position = position;
        module.Send(runLocally: true);
        if (Main.netMode == NetmodeID.SinglePlayer)
            RevealNotification(dummyItem, player.name);
        else
            RevealBroadcast(dummyItem, player.name);

        return true;
    }
    public static bool RevealEnchantedSword(Projectile projectile, Globe dummyItem, bool onlyJudging)
    {
        if (Main.netMode is NetmodeID.MultiplayerClient) //此时由服务器进行查找
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
            SyncNotificationKey.Send("Items.EnchantedSwordGlobe.NotFound", Globe.hintTextColor, player.whoAmI);
            return false;
        }
        var module = NetModuleLoader.Get<RevealEnchantedSwordPacket>();
        module._position = position;
        module.Send(runLocally: true);
        if (Main.netMode == NetmodeID.SinglePlayer)
            RevealNotification(dummyItem, player.name);
        else
            RevealBroadcast(dummyItem, player.name);
        return true;
    }

    //这两个都是生成时查找，一次解锁一个
    public static bool RevealMarble(Projectile projectile, Globe dummyItem, bool onlyJudging)
    {
        if (StructureDatas.AllMarbleCavePositions.Count <= StructureDatas.MarbleCavePositions.Count)//没有判定0的必要，因为如果是0一定满足这个
        {
            if (!onlyJudging && projectile.owner == Main.myPlayer)
            {
                if (!StructureDatas.QotEanbledInWorldGeneration)
                    NoDataNotification(dummyItem);
                else
                    NotFoundNotification(dummyItem);
            }
            return false;
        }
        if (onlyJudging)
            return true;

        StructureDatas.MarbleCavePositions.Add(StructureDatas.AllMarbleCavePositions
            .Except(StructureDatas.MarbleCavePositions)
            .MinBy(position => projectile.Center.Distance(position.ToVector2() * 16)));
        var playerName = Main.player[projectile.owner].name;
        if (projectile.owner == Main.myPlayer)
            RevealNotification(dummyItem, playerName);
        if (Main.netMode == NetmodeID.Server)
            RevealBroadcast(dummyItem, playerName);
        return true;
    }
    public static bool RevealGranite(Projectile projectile, Globe dummyItem, bool onlyJudging)
    {
        if (StructureDatas.AllGraniteCavePositions.Count <= StructureDatas.GraniteCavePositions.Count)//没有判定0的必要，因为如果是0一定满足这个
        {
            if (!onlyJudging && projectile.owner == Main.myPlayer)
            {
                if (!StructureDatas.QotEanbledInWorldGeneration)
                    NoDataNotification(dummyItem);
                else
                    NotFoundNotification(dummyItem);
            }
            return false;
        }
        if (onlyJudging)
            return true;

        StructureDatas.GraniteCavePositions.Add(StructureDatas.AllGraniteCavePositions
            .Except(StructureDatas.GraniteCavePositions)
            .MinBy(position => projectile.Center.Distance(position.ToVector2() * 16)));
        var playerName = Main.player[projectile.owner].name;
        if (projectile.owner == Main.myPlayer)
            RevealNotification(dummyItem, playerName);
        if (Main.netMode == NetmodeID.Server)
            RevealBroadcast(dummyItem, playerName);
        return true;
    }

    //这里都是生成时查找，一次解锁完毕，有对于生成时数据未记录的额外查找处理
    public static bool RevealOnceForAll(Projectile projectile, Globe dummyItem, bool onlyJudging)
    {
        var onceForAll = projectile.ModProjectile as IOnceForAllGlobeProj;
        bool extraChecked = false;
        //这一遍是看看世界数据有没有记录
        if ((!StructureDatas.QotEanbledInWorldGeneration || onceForAll.NotFoundCheck()) && Main.netMode != NetmodeID.MultiplayerClient)
        {
            //没有就当场另作检测
            onceForAll.ExtraCheckWhenNotRecorded();
            extraChecked = true;
        }

        //你要再没我也没办法了
        if (onceForAll.NotFoundCheck())
        {

            if (!onlyJudging && projectile.owner == Main.myPlayer)
            {
                if (StructureDatas.QotEanbledInWorldGeneration)
                    NotFoundNotification(dummyItem);
                else
                    NoDataNotification(dummyItem);
            }
            return false;
        }
        if (StructureDatas.StructuresUnlocked[(byte)onceForAll.StructureType])
        {
            if (!onlyJudging && projectile.owner == Main.myPlayer)
                AlreadyRevealedNotification(dummyItem);
            return false;
        }
        if (onlyJudging)
            return true;
        StructureDatas.StructuresUnlocked[(byte)onceForAll.StructureType] = true;
        string name = Main.player[projectile.owner].name;

        //多人下只有自己看见弹窗
        if (projectile.owner == Main.myPlayer)
            RevealNotification(dummyItem, name);

        //由服务器发送聊天信息
        if (Main.netMode == NetmodeID.Server)
            RevealBroadcast(dummyItem, name);

        if (extraChecked) //只有额外检测了才有发包的意义
        {
            var packet = NetModuleLoader.Get<RevealOnceForAllPacket>();
            packet._type = onceForAll.StructureType;
            packet._position = onceForAll.Positions;
            packet._positionAnother = onceForAll.PositionsAnother;
            packet.Send(runLocally: true);
        }
        else
            StructureDatas.StructuresUnlocked[(byte)onceForAll.StructureType] = true;
        return true;
    }
}
