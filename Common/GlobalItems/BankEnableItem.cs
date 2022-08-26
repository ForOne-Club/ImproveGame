using ImproveGame.Common.Players;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Terraria.GameInput;

namespace ImproveGame.Common.GlobalItems
{
    public class BankEnableItem : GlobalItem
    {
        public override void Load() {
            On.Terraria.Player.HandleBeingInChestRange += DisableBankRange;
        }

        private void DisableBankRange(On.Terraria.Player.orig_HandleBeingInChestRange orig, Player player) {
            if (player.chest >= -1 || !MyUtils.Config.MiddleEnableBank) {
                orig.Invoke(player);
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
            if (!MyUtils.Config.MiddleEnableBank)
                return;

            if (MyUtils.Bank2Items.Contains(item.type) && MyUtils.Config.AutoSaveMoney) {
                tooltips.Add(new(Mod, "TagDetailed.AutoCollect", MyUtils.GetText("Tips.TagDetailed.AutoCollect")) { OverrideColor = Color.SkyBlue });
                TagItem.AddShiftForMoreTooltip(tooltips);
            }

            // 确保物品栏里面有才能用，不然就作弊了（比如把物品打到聊天框里面直接中键）
            bool hasItemInInventory = Main.LocalPlayer.HasItem(item.type) ||
                // 没有的话再判断一下大背包
                (Main.LocalPlayer.TryGetModPlayer<DataPlayer>(out var modPlayer) && MyUtils.HasItem(modPlayer.SuperVault, -1, item.type));

            if (hasItemInInventory && MyUtils.IsBankItem(item.type)) {
                var player = Main.LocalPlayer;

                // 决定文本显示的是“开启”还是“关闭”
                string tooltip = MyUtils.GetText("Tips.BankEnableOn");
                if ((player.chest == -2 && MyUtils.Bank2Items.Contains(item.type)) ||
                    (player.chest == -3 && MyUtils.Bank3Items.Contains(item.type)) ||
                    (player.chest == -4 && MyUtils.Bank4Items.Contains(item.type)) ||
                    (player.chest == -5 && MyUtils.Bank5Items.Contains(item.type))) {
                    tooltip = MyUtils.GetText("Tips.BankEnableOff");
                }

                tooltips.Add(new(Mod, "BankEnable", tooltip) { OverrideColor = Color.LightGreen });

                SoundStyle? sound = null;
                if (PlayerInput.Triggers.JustPressed.MouseMiddle) {
                    if (MyUtils.Bank2Items.Contains(item.type)) {
                        if (item.type == ItemID.ChesterPetItem) {
                            sound = player.chest == -2 ? SoundID.ChesterClose : SoundID.ChesterOpen;
                        }
                        if (item.type == ItemID.MoneyTrough) {
                            sound = SoundID.Item59;
                        }
                        MyUtils.ToggleChest(ref player, -2, sound: sound);
                        return;
                    }
                    if (MyUtils.Bank3Items.Contains(item.type)) {
                        MyUtils.ToggleChest(ref player, -3);
                        return;
                    }
                    if (MyUtils.Bank4Items.Contains(item.type)) {
                        MyUtils.ToggleChest(ref player, -4);
                        return;
                    }
                    if (MyUtils.Bank5Items.Contains(item.type)) {
                        if (item.type == ItemID.VoidLens) {
                            sound = SoundID.Item130;
                        }
                        MyUtils.ToggleChest(ref player, -5, sound: sound);
                        return;
                    }
                }
            }
        }

        public override bool PreDrawTooltip(Item item, ReadOnlyCollection<TooltipLine> lines, ref int x, ref int y) {
            if (ItemSlot.ShiftInUse && MyUtils.Bank2Items.Contains(item.type) && MyUtils.Config.AutoSaveMoney)
                TagItem.DrawTagTooltips(lines, TagItem.GenerateDetailedTags(Mod, lines), x, y);
            return base.PreDrawTooltip(item, lines, ref x, ref y);
        }
    }
}
