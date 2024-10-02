using Terraria.DataStructures;

namespace ImproveGame.UI.QuickShimmer;

public class ShimmerLootListener : GlobalItem
{
    /// <summary>
    /// -1为不在监听中，此外的数值为正在为某个袋子监听，该袋子的ID
    /// </summary>
    public static int _listening;

    public override void OnSpawn(Item item, IEntitySource source)
    {
        if (_listening is -1 || Main.netMode is NetmodeID.Server)
            return;

        if (source is not QuickShimmerGUI.EntitySource_Shimmer_QOT openSource || openSource.Type != _listening)
            return;

        // 钱币直接过掉，进玩家口袋
        if (item.IsACoin)
            return;

        if (Main.LocalPlayer is null || !Main.LocalPlayer.TryGetModPlayer(out ShimmerLootKeeper keeper))
            return;


        // 堆叠和物品相同的
        foreach (var destination in keeper.Loots.Where(destination =>
                     destination is not null && !destination.IsAir && destination.type == item.type))
        {
            int s = item.stack;
            ItemLoader.TryStackItems(destination, item, out var _);

            if (item.IsAir)
            {
                item.TurnToAir();
                return;
            }
        }

        // 还有剩余，加在后面
        keeper.Loots.Add(item.Clone());

        item.TurnToAir();
    }
}