using Terraria.ModLoader.IO;

namespace ImproveGame.UI.QuickShimmer;

public class ShimmerLootKeeper : ModPlayer
{
    public Item targetItem = new();
    public List<Item> Loots = new();

    public override void SaveData(TagCompound tag)
    {
        tag["targetItem"] = targetItem;
        tag["loot"] = Loots;
    }

    public override void LoadData(TagCompound tag)
    {
        targetItem = tag.Get<Item>("targetItem") ?? new Item();
        Loots = tag.Get<List<Item>>("loot") ?? new List<Item>();
    }

    public void AddToLoots(Item item)
    {
        // 堆叠和物品相同的
        foreach (var destination in Loots.Where(destination =>
                     destination is not null && !destination.IsAir && destination.type == item.type))
        {
            ItemLoader.TryStackItems(destination, item, out _);

            if (!item.IsAir)
                continue;

            item.TurnToAir();
            return;
        }

        // 还有剩余，加在后面
        Loots.Add(item.Clone());
    }
}