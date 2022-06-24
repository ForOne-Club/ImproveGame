using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

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
            if (Main.HoverItem == item && MyUtils.IsBankItem(item.type)) {
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
                
                if (Main.mouseMiddle && Main.mouseMiddleRelease) {
                    if (MyUtils.Bank2Items.Contains(item.type)) {
                        MyUtils.ToggleChest(ref player, -2);
                        if (item.type == ItemID.ChesterPetItem) {
                            SoundEngine.PlaySound(player.chest == -2 ? SoundID.ChesterOpen : SoundID.ChesterClose);
                        }
                        if (item.type == ItemID.MoneyTrough) {
                            SoundEngine.PlaySound(SoundID.Item59);
                        }
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
                        MyUtils.ToggleChest(ref player, -5);
                        if (item.type == ItemID.VoidLens) {
                            SoundEngine.PlaySound(SoundID.Item130);
                        }
                        return;
                    }
                }
            }
        }
    }
}
