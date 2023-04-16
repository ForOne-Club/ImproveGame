using ImproveGame.Common.Players;
using System.Collections.ObjectModel;

namespace ImproveGame.Common.GlobalItems
{
    public class BankEnableItem : GlobalItem
    {
        public override void Load()
        {
            Terraria.On_Player.HandleBeingInChestRange += (orig, player) =>
            {
                if (player.chest >= -1 || !Config.MiddleEnableBank)
                {
                    orig.Invoke(player);
                }
            };
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (!Config.MiddleEnableBank)
                return;

            if (Bank2Items.Contains(item.type) && Config.AutoSaveMoney)
            {
                tooltips.Add(new(Mod, "TagDetailed.AutoCollect", GetText("Tips.TagDetailed.AutoCollect")) { OverrideColor = Color.SkyBlue });
                TagItem.AddShiftForMoreTooltip(tooltips);
            }

            // 确保物品栏里面有才能用，不然就作弊了（比如把物品打到聊天框里面直接中键）
            bool hasItemInInventory = Main.LocalPlayer.HasItem(item.type) ||
                // 没有的话再判断一下大背包
                (Main.LocalPlayer.TryGetModPlayer<DataPlayer>(out var modPlayer) && HasItem(modPlayer.SuperVault, -1, item.type));

            if (hasItemInInventory && IsBankItem(item.type))
            {
                var player = Main.LocalPlayer;

                // 决定文本显示的是“开启”还是“关闭”
                string tooltip = GetText("Tips.BankEnableOn");
                if ((player.chest == -2 && Bank2Items.Contains(item.type)) ||
                    (player.chest == -3 && Bank3Items.Contains(item.type)) ||
                    (player.chest == -4 && Bank4Items.Contains(item.type)) ||
                    (player.chest == -5 && Bank5Items.Contains(item.type)))
                {
                    tooltip = GetText("Tips.BankEnableOff");
                }

                tooltips.Add(new(Mod, "BankEnable", tooltip) { OverrideColor = Color.LightGreen });

                SoundStyle? sound = null;
                if (Main.mouseMiddle && Main.mouseMiddleRelease)
                {
                    if (Bank2Items.Contains(item.type))
                    {
                        if (item.type == ItemID.ChesterPetItem)
                        {
                            sound = player.chest == -2 ? SoundID.ChesterClose : SoundID.ChesterOpen;
                        }
                        if (item.type == ItemID.MoneyTrough)
                        {
                            sound = SoundID.Item59;
                        }
                        ToggleChest(ref player, -2, sound: sound);
                        return;
                    }
                    if (Bank3Items.Contains(item.type))
                    {
                        ToggleChest(ref player, -3);
                        return;
                    }
                    if (Bank4Items.Contains(item.type))
                    {
                        ToggleChest(ref player, -4);
                        return;
                    }
                    if (Bank5Items.Contains(item.type))
                    {
                        if (item.type == ItemID.VoidLens)
                        {
                            sound = SoundID.Item130;
                        }
                        ToggleChest(ref player, -5, sound: sound);
                        return;
                    }
                }
            }
        }

        public override bool PreDrawTooltip(Item item, ReadOnlyCollection<TooltipLine> lines, ref int x, ref int y)
        {
            if (ItemSlot.ShiftInUse && Bank2Items.Contains(item.type) && Config.AutoSaveMoney)
                TagItem.DrawTagTooltips(lines, TagItem.GenerateDetailedTags(Mod, lines), x, y);
            return base.PreDrawTooltip(item, lines, ref x, ref y);
        }
    }
}
