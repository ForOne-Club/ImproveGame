using Terraria.ModLoader.IO;

namespace ImproveGame.UI.OpenBag;

public class LootKeeper : ModPlayer
{
    public Item Bag = new();
    public List<Item> Loots = new();

    public override void SaveData(TagCompound tag)
    {
        tag["bag"] = Bag;
        tag["loot"] = Loots;
    }

    public override void LoadData(TagCompound tag)
    {
        Bag = tag.Get<Item>("bag") ?? new Item();
        Loots = tag.Get<List<Item>>("loot") ?? new List<Item>();
    }
}