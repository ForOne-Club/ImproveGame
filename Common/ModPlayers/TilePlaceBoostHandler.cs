using ImproveGame.Common.Configs;
using ImproveGame.Content.Items;

namespace ImproveGame.Common.ModPlayers;

internal class TilePlaceBoostHandler : ModPlayer
{
    internal class MonoModHooksLoader : ILoadable
    {
        void ILoadable.Load(Mod mod)
        {
            // 动画速度不变
            On_Player.ApplyItemAnimation_Item += FixSuperFastAnimation;
        }

        private void FixSuperFastAnimation(On_Player.orig_ApplyItemAnimation_Item orig, Player player, Item sItem)
        {
            if (!ShouldItemAcceptSpeedBoost(sItem) || !player.TryGetModPlayer<TilePlaceBoostHandler>(out var modPlayer))
            {
                orig.Invoke(player, sItem);
                return;
            }

            float oldTileSpeed = player.tileSpeed;
            float oldWallSpeed = player.wallSpeed;
            player.tileSpeed = modPlayer.OriginalTileSpeed;
            player.wallSpeed = modPlayer.OriginalWallSpeed;

            orig.Invoke(player, sItem);

            player.tileSpeed = oldTileSpeed;
            player.wallSpeed = oldWallSpeed;
        }

        void ILoadable.Unload()
        {
        }
    }

    internal float OriginalTileSpeed;
    internal float OriginalWallSpeed;

    /// <summary>
    /// 设置玩家的放置范围和放置速度
    /// </summary>
    public override void UpdateEquips()
    {
        if (Player.whoAmI == Main.myPlayer)
        {
            // 设置放置范围
            Player.tileRangeX += ImproveConfigs.Instance.ModifyPlayerTileRange;
            Player.tileRangeY += ImproveConfigs.Instance.ModifyPlayerTileRange;

            if (!ShouldItemAcceptSpeedBoost(Player.HeldItem))
                return;

            // 物块和墙放置速度
            OriginalTileSpeed = Player.tileSpeed;
            OriginalWallSpeed = Player.wallSpeed;

            Player.tileSpeed = Math.Max(3f, Player.tileSpeed);
            Player.wallSpeed = Math.Max(3f, Player.wallSpeed);
        }
    }

    internal static bool ShouldItemAcceptSpeedBoost(Item item)
    {
        if (item.IsAir || !ImproveConfigs.Instance.ModifyPlayerPlaceSpeed)
            return false;

        // 《英文名》因为没法在非英语语言获取英文名，只能用内部名了
        string internalName = ItemID.Search.GetName(item.type).ToLower();
        string currentLanguageName = Lang.GetItemNameValue(item.type).ToLower();

        if (ImproveConfigs.Instance.TileSpeed_Blacklist.Any(str => internalName.Contains(str) || currentLanguageName.Contains(str)))
            return false;

        // 是特判捏嘿嘿
        if (item.ModItem is MoveChest)
            return false;

        return true;
    }
}
