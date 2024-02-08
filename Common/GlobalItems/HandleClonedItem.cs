using ImproveGame.Common.ModPlayers;
using ImproveGame.Content.Functions;

namespace ImproveGame.Common.GlobalItems;

/// <summary>
/// 如果被Clone的物品在无限增益速查哈希表中，那么克隆出来的物品也在列表中
/// </summary>
public class HandleClonedItem : GlobalItem
{
    public override GlobalItem Clone(Item from, Item to)
    {
        if (InfBuffPlayer.TryGet(Main.LocalPlayer, out var infBuffPlayer) && infBuffPlayer.AvailableItemsHash.Contains(from))
            infBuffPlayer.AvailableItemsHash.Add(to);
        if (BannerPatches.AvailableBanners.Contains(from))
            BannerPatches.AvailableBanners.Add(to);
        return base.Clone(from, to);
    }

    public override bool InstancePerEntity => true;
}