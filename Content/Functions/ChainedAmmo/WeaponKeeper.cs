using ImproveGame.Core;
using Terraria.ModLoader.IO;

namespace ImproveGame.Content.Functions.ChainedAmmo;

public class WeaponKeeper : ModPlayer
{
    public Item Weapon = new();

    public override void SaveData(TagCompound tag)
    {
        tag["weapon"] = Weapon;
    }

    public override void LoadData(TagCompound tag)
    {
        Weapon = tag.Get<Item>("weapon") ?? new Item();
    }
}