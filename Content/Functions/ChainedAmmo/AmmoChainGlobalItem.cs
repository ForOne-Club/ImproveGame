using ImproveGame.Content.Items.IconDummies;
using ImproveGame.Core;
using System.Diagnostics.CodeAnalysis;
using Terraria.ModLoader.IO;

namespace ImproveGame.Content.Functions.ChainedAmmo;

public class AmmoChainGlobalItem : GlobalItem
{
    public int Count;
    public int Index;
    public AmmoChain Chain;
    public static bool IsPickingAmmo = false;

    public override void Load()
    {
        On_Player.PickAmmo_Item_refInt32_refSingle_refBoolean_refInt32_refSingle_refInt32_bool += OnPickAmmo;
        On_Player.ChooseAmmo += OnChooseAmmo;
    }

    private void OnPickAmmo(
        On_Player.orig_PickAmmo_Item_refInt32_refSingle_refBoolean_refInt32_refSingle_refInt32_bool orig, Player self,
        Item sitem, ref int projtoshoot, ref float speed, ref bool canshoot, ref int totaldamage, ref float knockback,
        out int usedammoitemid, bool dontconsume)
    {
        IsPickingAmmo = true;

        orig.Invoke(self, sitem, ref projtoshoot, ref speed, ref canshoot, ref totaldamage, ref knockback,
            out usedammoitemid, dontconsume);

        IsPickingAmmo = false;
    }

    private Item OnChooseAmmo(On_Player.orig_ChooseAmmo orig, Player player, Item weapon)
    {
        if (!IsPickingAmmo || !weapon.TryGetGlobalItem<AmmoChainGlobalItem>(out var globalItem) ||
            globalItem.Chain is null || globalItem.Chain.Chain.Count is 0)
        {
            return orig.Invoke(player, weapon);
        }
        // 超界重置
        if (!globalItem.Chain.Chain.IndexInRange(globalItem.Index))
            globalItem.ResetToZero();

        var ammoType = globalItem.Chain.Chain[globalItem.Index];
        
        // 即为沿用原版弹药
        if (ammoType.ItemData.Item.type == ModContent.ItemType<UniversalAmmoIcon>())
        {
            var foundedAmmo = orig.Invoke(player, weapon);
            // 这里不判断是否null，无论实际有没有找到弹药，都直接跳到下一个弹药
            GoToNextAmmo(globalItem);
            return foundedAmmo;
        }

        // 至少要有能用的弹药
        int failCounter = 0;
        while (ammoType.ItemData.Item.ammo != weapon.useAmmo ||
               player.inventory.All(i => ammoType.ItemData.Item.type != i.type))
        {
            globalItem.Index++;
            Count = 0;
            if (!globalItem.Chain.Chain.IndexInRange(globalItem.Index))
            {
                failCounter++;
                globalItem.ResetToZero();

                // 两轮没找到，说明就没有适合的，直接原版行为
                if (failCounter >= 2)
                    return orig.Invoke(player, weapon);
            }

            ammoType = globalItem.Chain.Chain[globalItem.Index];
        }

        // 找到对应弹药
        Item item = null;
        bool itemFound = false;
        for (int j = 54; j < 58; j++)
        {
            var ammo = player.inventory[j];
            if (ammo.stack > 0 && ammoType.ItemData.Item.type == ammo.type &&
                ItemLoader.CanChooseAmmo(weapon, ammo, player))
            {
                item = ammo;
                itemFound = true;
                break;
            }
        }

        if (!itemFound)
        {
            for (int k = 0; k < 54; k++)
            {
                var ammo = player.inventory[k];
                if (ammo.stack > 0 && ammoType.ItemData.Item.type == ammo.type &&
                    ItemLoader.CanChooseAmmo(weapon, ammo, player))
                {
                    item = ammo;
                    itemFound = true;
                    break;
                }
            }
        }

        // 找到了，前进
        if (itemFound)
            GoToNextAmmo(globalItem);

        return item;
    }

    private void GoToNextAmmo(AmmoChainGlobalItem globalItem)
    {
        var ammoType = globalItem.Chain.Chain[globalItem.Index];

        globalItem.Count++;
        if (globalItem.Count >= ammoType.Times)
        {
            globalItem.Index++;
            globalItem.Count = 0;
        }
    }

    private void ResetToZero()
    {
        Index = 0;
        Count = 0;
    }

    public override void SaveData(Item item, TagCompound tag)
    {
        tag["ammoChain"] = Chain;
    }

    public override void LoadData(Item item, TagCompound tag)
    {
        Chain = tag.Get<AmmoChain>("ammoChain") ?? new AmmoChain();
    }

    public override void NetSend(Item item, BinaryWriter writer)
    {
        writer.WriteRGB(Chain.Color);
        writer.Write(Chain.Chain.Count);
        foreach ((ItemTypeData itemTypeData, int times) in Chain.Chain)
        {
            writer.Write(itemTypeData);
            writer.Write((ushort)times);
        }
    }

    public override void NetReceive(Item item, BinaryReader reader)
    {
        Chain = new AmmoChain
        {
            Color = reader.ReadRGB()
        };
        int count = reader.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            ItemTypeData data = reader.ReadItemTypeData();
            int times = reader.ReadUInt16();
            Chain.Chain.Add(new AmmoChain.Ammo(data, times));
        }
    }

    public override GlobalItem Clone(Item from, Item to)
    {
        if (from.TryGetGlobalItem<AmmoChainGlobalItem>(out var globalFrom))
        {
            globalFrom.Chain = (AmmoChain) Chain?.Clone();
            return globalFrom;
        }

        return base.Clone(from, to);
    }

    public override bool AppliesToEntity(Item entity, bool lateInstantiation) => lateInstantiation && entity.damage > 0;

    public override bool InstancePerEntity => true;
}