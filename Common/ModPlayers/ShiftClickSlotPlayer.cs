using ImproveGame.Common.ModHooks;
using ImproveGame.Packets.NetAutofisher;
using ImproveGame.Packets.NetStorager;
using ImproveGame.UI;
using ImproveGame.UI.AmmoChainPanel;
using ImproveGame.UI.Autofisher;
using ImproveGame.UI.ExtremeStorage;
using ImproveGame.UI.ItemContainer;
using ImproveGame.UIFramework;
using ItemSlot = Terraria.UI.ItemSlot;

namespace ImproveGame.Common.ModPlayers;

public class ShiftClickSlotPlayer : ModPlayer
{
    public override bool HoverSlot(Item[] inventory, int context, int slot)
    {
        var item = inventory[slot];

        bool result = false;
        foreach (var globalItem in from i in GlobalList<GlobalItem>.Globals where i is IItemOverrideHover select i)
            result |= ((IItemOverrideHover)globalItem).OverrideHover(inventory, context, slot);

        if (item.ModItem is IItemOverrideHover overrideHover && overrideHover.OverrideHover(inventory, context, slot))
            result |= overrideHover.OverrideHover(inventory, context, slot);

        if (result)
            return true;

        if (Main.LocalPlayer.chest == -1 & Main.LocalPlayer.talkNPC == -1 &&
            (context is ItemSlot.Context.InventoryItem or 114514) && ItemSlot.ShiftInUse && !item.IsAir &&
            !item.favorited)
        {
            // 弹药链
            if (AmmoChainUI.Instance.Enabled && AmmoChainUI.Instance.SlotQuickPutAvailable(item))
            {
                Main.cursorOverride = CursorOverrideID.InventoryToChest;
                return true;
            }

            // 至尊储存
            if (ExtremeStorageGUI.Visible && ExtremeStorageGUI.AllItemsCached
                    .Any(s => CanPlaceInSlot(s, item) is 2 or 3))
            {
                Main.cursorOverride = CursorOverrideID.InventoryToChest;
                return true;
            }

            // 建筑法杖
            if (ArchitectureGUI.Visible &&
                UISystem.Instance.ArchitectureGUI.ItemSlot.Any(s =>
                    s.Value.CanPlaceItem(item) &&
                    CanPlaceInSlot(s.Value.Item, item) is 2 or 3))
            {
                Main.cursorOverride = CursorOverrideID.InventoryToChest;
                return true;
            }

            // 旗帜收纳箱 + 药水袋
            if (ItemContainerGUI.Instace.Enabled && ItemContainerGUI.Instace.Container.MeetEntryCriteria(item))
            {
                Main.cursorOverride = CursorOverrideID.InventoryToChest;
                return true;
            }

            if (BigBagGUI.Instance.Enabled && Main.LocalPlayer.TryGetModPlayer<DataPlayer>(out var dataPlayer) &&
                dataPlayer.SuperVault.Any(s => CanPlaceInSlot(s, item) is 2 or 3))
            {
                Main.cursorOverride = CursorOverrideID.InventoryToChest;
                return true;
            }

            var fisher = AutofishPlayer.LocalPlayer.Autofisher;
            if (AutofisherGUI.Visible && fisher is not null && fisher.fish.Any(s => CanPlaceInSlot(s, item) is 2 or 3))
            {
                Main.cursorOverride = CursorOverrideID.InventoryToChest;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Shift左键单击物品栏
    /// </summary>
    public override bool ShiftClickSlot(Item[] inventory, int context, int slot)
    {
        if (Player.chest == -1 & Player.talkNPC == -1 && context == ItemSlot.Context.InventoryItem &&
            !inventory[slot].IsAir && !inventory[slot].favorited)
        {
            // 弹药链
            if (AmmoChainUI.Instance.Enabled && AmmoChainUI.Instance.SlotQuickPutAvailable(inventory[slot]))
            {
                AmmoChainUI.Instance.TryQuickPlaceIntoSlot(ref inventory[slot]);
                return true; // 阻止原版代码运行
            }

            // 至尊储存
            if (ExtremeStorageGUI.Visible &&
                ExtremeStorageGUI.AllItemsCached.Any(s => CanPlaceInSlot(s, inventory[slot]) is 2 or 3))
            {
                switch (Main.netMode)
                {
                    case NetmodeID.MultiplayerClient:
                        InvToChestPacket.Send(ExtremeStorageGUI.Storage.ID, slot, ExtremeStorageGUI.RealGroup);
                        break;
                    default:
                        inventory[slot] =
                            ExtremeStorageGUI.Storage.StackToNearbyChests(inventory[slot], ExtremeStorageGUI.RealGroup);
                        ExtremeStorageGUI.RefreshCachedAllItems();
                        Recipe.FindRecipes();
                        break;
                }

                SoundEngine.PlaySound(SoundID.Grab);
                return true; // 阻止原版代码运行
            }

            // 旗帜收纳箱 + 药水袋
            if (ItemContainerGUI.Instace.Enabled)
            {
                if (ItemContainerGUI.Instace.Container.MeetEntryCriteria(inventory[slot]))
                {
                    ItemContainerGUI.Instace.Container.ItemIntoContainer(inventory[slot]);

                    Recipe.FindRecipes();
                    SoundEngine.PlaySound(SoundID.Grab);
                }

                return true; // 阻止原版代码运行
            }

            if (BigBagGUI.Instance.Enabled)
            {
                inventory[slot] =
                    ItemStackToInventory(Player.GetModPlayer<DataPlayer>().SuperVault, inventory[slot], false);
                Recipe.FindRecipes();
                SoundEngine.PlaySound(SoundID.Grab);

                return true; // 阻止原版代码运行
            }

            var fisher = AutofishPlayer.LocalPlayer.Autofisher;
            if (AutofisherGUI.Visible && fisher is not null &&
                fisher.fish.Any(s => CanPlaceInSlot(s, inventory[slot]) is 2 or 3))
            {
                inventory[slot] = ItemStackToInventory(fisher.fish, inventory[slot], false);
                AutofisherGUI.RequireRefresh = true;
                UISystem.Instance.AutofisherGUI.RefreshItems();
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    for (int i = 0; i < fisher.fish.Length; i++)
                        ItemSyncPacket.Get(fisher.ID, (byte)i).Send(runLocally: false);
                Recipe.FindRecipes();
                SoundEngine.PlaySound(SoundID.Grab);
                return true; // 阻止原版代码运行
            }

            if (!inventory[slot].IsAir && ArchitectureGUI.Visible)
            {
                foreach (var itemSlot in from s in UISystem.Instance.ArchitectureGUI.ItemSlot
                         where s.Value.CanPlaceItem(inventory[slot])
                         select s)
                {
                    // 放到建筑GUI里面
                    ref Item slotItem = ref itemSlot.Value.Item;
                    ref Item placeItem = ref inventory[slot];

                    byte placeMode = CanPlaceInSlot(slotItem, placeItem);

                    // type不同直接切换吧
                    if (placeMode is 1 or 3)
                    {
                        itemSlot.Value.SwapItem(ref placeItem);
                        SoundEngine.PlaySound(SoundID.Grab);
                        Recipe.FindRecipes();
                        // 和魔杖实例同步
                        itemSlot.Value.ItemChange();
                        return true; // 阻止原版代码运行
                    }

                    // type相同，里面的能堆叠，放进去
                    if (placeMode is 2)
                    {
                        int stackAvailable = slotItem.maxStack - slotItem.stack;
                        int stackAddition = Math.Min(placeItem.stack, stackAvailable);
                        placeItem.stack -= stackAddition;
                        slotItem.stack += stackAddition;
                        SoundEngine.PlaySound(SoundID.Grab);
                        Recipe.FindRecipes();
                        // 和魔杖实例同步
                        itemSlot.Value.ItemChange();
                        return true; // 阻止原版代码运行
                    }
                }
            }
        }

        return false;
    }
}