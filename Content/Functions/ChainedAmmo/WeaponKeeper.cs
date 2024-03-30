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

    // 测试用
    // public override void OnEnterWorld()
    // {
    //     var chain = new AmmoChain
    //     {
    //         Chain = new List<AmmoChain.Ammo>
    //         {
    //             new (new ItemTypeData(new Item(ItemID.ChlorophyteBullet)), 1),
    //             new (new ItemTypeData(new Item(ItemID.ChlorophyteBullet)), 10),
    //             new (new ItemTypeData(new Item(ItemID.ChlorophyteBullet)), 580),
    //             new (new ItemTypeData(new Item(ItemID.ChlorophyteBullet)), 100),
    //             new (new ItemTypeData(new Item(ItemID.ChlorophyteBullet)), 100),
    //             new (new ItemTypeData(new Item(ItemID.ChlorophyteBullet)), 100),
    //             new (new ItemTypeData(new Item(ItemID.ChlorophyteBullet)), 100),
    //             new (new ItemTypeData(new Item(ItemID.ChlorophyteBullet)), 100),
    //             new (new ItemTypeData(new Item(ItemID.ChlorophyteBullet)), 100),
    //             new (new ItemTypeData(new Item(ItemID.ChlorophyteBullet)), 100),
    //             new (new ItemTypeData(new Item(ItemID.ChlorophyteBullet)), 100),
    //         }
    //     };
    //     
    //     ChainSaver.SaveAsFile(chain);
    // }
}