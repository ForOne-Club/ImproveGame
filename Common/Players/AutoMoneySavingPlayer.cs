using ImproveGame.Interface.GUI;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace ImproveGame.Common.Players
{
    // 自动省钱玩家（（
    public class AutoMoneySavingPlayer : ModPlayer
    {
        // 每帧判断一个储存是否有猪猪，尽量避免卡顿
        private static int _saveCounter = 0;

        public override void PostUpdateEquips() {
            if (!MyUtils.Config.AutoSaveMoney)
                return;

            // 先判断有没有钱这种开销小的，再去判断后面那些
            bool hasMoney = false;
            for (int i = 50; i < 54; i++) {
                if (Player.inventory[i].stack > 0 && ItemID.Sets.CommonCoin[Player.inventory[i].type]) {
                    hasMoney = true;
                    break;
                }
            }
            if (!hasMoney)
                return;

            int[] PiggyBanks = new int[] { ItemID.PiggyBank, ItemID.ChesterPetItem, ItemID.MoneyTrough };
            bool hasPiggyInInventory;
            // 每帧判断一个储存是否有猪猪，尽量避免卡顿
            switch (_saveCounter % 6) {
                case 1:
                    hasPiggyInInventory = MyUtils.HasItem(Player.bank.item, -1, PiggyBanks);
                    break;
                case 2:
                    hasPiggyInInventory = MyUtils.HasItem(Player.bank2.item, -1, PiggyBanks);
                    break;
                case 3:
                    hasPiggyInInventory = MyUtils.HasItem(Player.bank3.item, -1, PiggyBanks);
                    break;
                case 4:
                    hasPiggyInInventory = MyUtils.HasItem(Player.bank4.item, -1, PiggyBanks);
                    break;
                case 5:
                    hasPiggyInInventory = MyUtils.HasItem(Player.GetModPlayer<DataPlayer>().SuperVault, -1, PiggyBanks);
                    break;
                default:
                    hasPiggyInInventory = MyUtils.HasItem(Player.inventory, -1, PiggyBanks);
                    break;
            }
            _saveCounter++;
            if (!hasPiggyInInventory)
                return;

            // 全部放入
            for (int i = 50; i < 54; i++) {
                if (Player.inventory[i].stack > 0 && ItemID.Sets.CommonCoin[Player.inventory[i].type]) {
                    // 合并金币
                    if (Player.inventory[i].stack == 100 && Player.inventory[i].type == ItemID.CopperCoin)
                        Player.inventory[i].SetDefaults(ItemID.SilverCoin);
                    if (Player.inventory[i].stack == 100 && Player.inventory[i].type == ItemID.SilverCoin)
                        Player.inventory[i].SetDefaults(ItemID.GoldCoin);
                    if (Player.inventory[i].stack == 100 && Player.inventory[i].type == ItemID.GoldCoin)
                        Player.inventory[i].SetDefaults(ItemID.PlatinumCoin);
                    Player.inventory[i] = MyUtils.ItemStackToInventory(Player.bank.item, Player.inventory[i], false);
                }
            }
        }
    }
}
