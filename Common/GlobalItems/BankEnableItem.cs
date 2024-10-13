﻿using ImproveGame.Common.ModHooks;
using ImproveGame.Common.ModSystems;
using ImproveGame.Core;
using System.Collections.ObjectModel;

namespace ImproveGame.Common.GlobalItems
{
    public class BankEnableItem : GlobalItem, IItemOverrideHover, IItemMiddleClickable
    {
        public override void Load()
        {
            On_Player.HandleBeingInChestRange += (orig, player) =>
            {
                if (player.chest >= -1 || !Config.MiddleEnableBank)
                {
                    orig.Invoke(player);
                }
            };
        }

        public void OnMiddleClicked(Item item)
        {
            SoundStyle? sound = null;
            var player = Main.LocalPlayer;
            if (Lookups.Bank2Items.Contains(item.type))
            {
                sound = item.type switch
                {
                    ItemID.ChesterPetItem => player.chest is -2 ? SoundID.ChesterClose : SoundID.ChesterOpen,
                    ItemID.MoneyTrough => SoundID.Item59,
                    _ => null
                };

                ToggleChest(ref player, -2, sound: sound);
                return;
            }

            if (Lookups.Bank3Items.Contains(item.type))
            {
                ToggleChest(ref player, -3);
                return;
            }

            if (Lookups.Bank4Items.Contains(item.type))
            {
                ToggleChest(ref player, -4);
                return;
            }

            if (Lookups.Bank5Items.Contains(item.type))
            {
                if (item.type == ItemID.VoidLens)
                {
                    sound = SoundID.Item130;
                }

                ToggleChest(ref player, -5, sound: sound);
            }
        }

        public bool MiddleClickable(Item item)
        {
            var player = Main.LocalPlayer;
            if (player.frozen || player.tongued || player.webbed || player.stoned ||
                player.dead || player.noItems)
                return false;
            return Config.MiddleEnableBank && item.IsBankItem();
        }

        public void ManageHoverTooltips(Item item, List<TooltipLine> tooltips)
        {
            // 决定文本显示的是“开启”还是“关闭”
            var player = Main.LocalPlayer;
            TryGetKeybindString(KeybindSystem.ItemInteractKeybind, out string keybind);
            string tooltip = GetTextWith("Tips.BankEnableOn", new { KeybindName = keybind });
            if ((player.chest is -2 && Lookups.Bank2Items.Contains(item.type)) ||
                (player.chest is -3 && Lookups.Bank3Items.Contains(item.type)) ||
                (player.chest is -4 && Lookups.Bank4Items.Contains(item.type)) ||
                (player.chest is -5 && Lookups.Bank5Items.Contains(item.type)))
            {
                tooltip = GetTextWith("Tips.BankEnableOff", new { KeybindName = keybind });
            }

            tooltips.Add(new TooltipLine(Mod, "BankEnable", tooltip) {OverrideColor = Color.LightGreen});
        }

        // 中键功能
        public bool OverrideHover(Item[] inventory, int context, int slot)
        {
            ((IItemMiddleClickable)this).HandleHover(inventory, context, slot);
            return false;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            ((IItemMiddleClickable)this).HandleTooltips(item, tooltips);

            if (Lookups.Bank2Items.Contains(item.type))
            {
                tooltips.Add(new TooltipLine(Mod, "TagDetailed.AutoCollect", GetText("Tips.TagDetailed.AutoCollect"))
                    {OverrideColor = Color.SkyBlue});
                TagItem.AddShiftForMoreTooltip(tooltips);
            }
        }

        public override bool PreDrawTooltip(Item item, ReadOnlyCollection<TooltipLine> lines, ref int x, ref int y)
        {
            if (ItemSlot.ShiftInUse && Lookups.Bank2Items.Contains(item.type))
                TagItem.DrawTagTooltips(lines, TagItem.GenerateDetailedTags(Mod, lines), x, y);
            return base.PreDrawTooltip(item, lines, ref x, ref y);
        }
    }
}