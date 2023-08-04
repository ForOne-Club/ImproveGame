using Microsoft.Xna.Framework.Input;
using System.Linq;
using Terraria.ModLoader.IO;

namespace ImproveGame.Common.Players;

/// <summary>
/// 方法需求
///     4. 添加 近期列表
///     5. 删除 近期列表
/// </summary>
public class AutoTrashPlayer : ModPlayer
{
    #region 设置 Instance
    public static AutoTrashPlayer Instance { get; private set; }

    public override void OnEnterWorld()
    {
        Instance = this;
    }

    public override void Unload()
    {
        Instance = null;
    }
    #endregion

    #region 基本属性
    /// <summary>
    /// 自动被丢弃的物品的 type
    /// </summary>
    public readonly List<Item> AutoDiscardItems = new List<Item>();

    /// <summary>
    /// 近期被丢弃的 <see cref="MaxCapacity"/> 件物品
    /// </summary>
    public readonly List<Item> TrashItems = new List<Item>();

    /// <summary>
    /// 垃圾桶最大容量，当然我现在还不会写设置调这个
    /// </summary>
    public int MaxCapacity = 6;

    public AutoTrashPlayer()
    {
        TrashItems = new List<Item>(MaxCapacity);
    }
    #endregion

    public override bool ShiftClickSlot(Item[] inventory, int context, int slot)
    {
        if (!inventory[slot].IsAir && context is ItemSlot.Context.InventoryItem or ItemSlot.Context.InventoryCoin or ItemSlot.Context.InventoryAmmo)
        {
            if (Main.keyState.IsKeyDown(Keys.LeftControl) || Main.keyState.IsKeyDown(Keys.RightControl))
            {
                if (!AutoDiscardItems.Any(item => item.type == inventory[slot].type))
                {
                    AutoDiscardItems.Add(new Item(inventory[slot].type));
                }

                StackToLastItemsWithCleanUp(inventory[slot]);
                inventory[slot] = new Item();
                SoundEngine.PlaySound(SoundID.Grab);
            }
        }

        return false;
    }

    /// <summary>
    /// 添加物品到垃圾桶，并清理多余的
    /// </summary>
    /// <param name="item"></param>
    public void StackToLastItemsWithCleanUp(Item item)
    {
        int itemIndex = TrashItems.FindIndex(0, Math.Min(MaxCapacity, TrashItems.Count), trashItem =>
        {
            return trashItem.type == item.type;
        });

        if (itemIndex > -1)
        {
            Item trashItem = TrashItems[itemIndex];
            trashItem.stack = Math.Min(trashItem.maxStack, trashItem.stack + item.stack);

            if (itemIndex != 0)
            {
                TrashItems.RemoveAt(itemIndex);
                TrashItems.Insert(0, trashItem);
            }
        }
        else
        {
            TrashItems.Insert(0, item);
        }

        CleanUpTrashItems();
    }

    /// <summary>
    /// 清理、整理垃圾物品
    /// </summary>
    public void CleanUpTrashItems()
    {
        for (int i = 0; i < TrashItems.Count; i++)
        {
            if (TrashItems is null || TrashItems[i].IsAir)
            {
                TrashItems.RemoveAt(i--);
            }
        }

        if (TrashItems.Count > MaxCapacity)
        {
            TrashItems.RemoveRange(MaxCapacity, TrashItems.Count - MaxCapacity);
        }
    }

    public override void SaveData(TagCompound tag)
    {
        tag["AutoDiscardItems"] = AutoDiscardItems;
        tag["TrashItems"] = TrashItems;
    }

    public override void LoadData(TagCompound tag)
    {
        if (tag.TryGet("AutoDiscardItems", out List<Item> autoDiscardItems))
        {
            foreach (Item item in autoDiscardItems)
            {
                AutoDiscardItems.Add(item);
            }
        }

        if (tag.TryGet("TrashItems", out List<Item> lastItems))
        {
            for (int i = 0; i < lastItems.Count; i++)
            {
                TrashItems.Add(lastItems[i]);
            }
        }
    }
}
