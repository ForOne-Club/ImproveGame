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
}