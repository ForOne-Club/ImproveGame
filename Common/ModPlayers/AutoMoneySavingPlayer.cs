namespace ImproveGame.Common.ModPlayers
{
    // 自动省钱玩家（（
    public class AutoMoneySavingPlayer : ModPlayer
    {
        // 每帧判断一个储存是否有猪猪，尽量避免卡顿
        private static int _saveCounter = 0;
        private static int _delayStackMoney = 0;
        private static int _failDepositTimer = 0;

        public override void PostUpdateEquips() {
            if (!Config.AutoSaveMoney || Main.myPlayer != Player.whoAmI || Main.netMode == NetmodeID.Server)
                return;

            // 堆叠钱币
            if (_delayStackMoney == 0 || _failDepositTimer == 0) {
                StackMoneyInPiggy();
                _delayStackMoney = -1;
                _failDepositTimer = -1;
                // 一帧不要执行太多，所以这里先return
                return;
            }
            _delayStackMoney--;
            _failDepositTimer--;

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
            // 每帧判断一个储存是否有猪猪，尽量避免卡顿
            var hasPiggyInInventory = (_saveCounter % 6) switch {
                1 => HasItem(Player.bank.item, -1, PiggyBanks),
                2 => HasItem(Player.bank2.item, -1, PiggyBanks),
                3 => HasItem(Player.bank3.item, -1, PiggyBanks),
                4 => HasItem(Player.bank4.item, -1, PiggyBanks),
                5 => HasItem(Player.GetModPlayer<DataPlayer>().SuperVault, -1, PiggyBanks),
                _ => HasItem(Player.inventory, -1, PiggyBanks),
            };
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
                    Player.inventory[i] = ItemStackToInventory(Player.bank.item, Player.inventory[i], false);
                    Recipe.FindRecipes();
                    // 如果某一帧突然装不下，要及时整理背包让它装得下
                    if (Player.inventory[i].stack > 0 && _failDepositTimer < 0)
                        _failDepositTimer = 6;
                }
            }

            if (_failDepositTimer <= 0) {
                _delayStackMoney = 10;
            }
        }

        public void StackMoneyInPiggy() {
            // 数钱
            ulong countCoins = 0;
            for (int i = 0; i < Player.bank.item.Length; i++) {
                if (countCoins < 18446744053711551610) { // ulong.MaxValue - 1000000 * 9999 - 5
                    int multiplier = 0;
                    switch (Player.bank.item[i].type) {
                        case ItemID.CopperCoin:
                            multiplier = 1;
                            break;
                        case ItemID.SilverCoin:
                            multiplier = 100;
                            break;
                        case ItemID.GoldCoin:
                            multiplier = 10000;
                            break;
                        case ItemID.PlatinumCoin:
                            multiplier = 1000000;
                            break;
                    }
                    countCoins += (ulong)Player.bank.item[i].stack * (ulong)multiplier;
                    if (multiplier > 0)
                        Player.bank.item[i].TurnToAir();
                }
            }

            // 合并放回去
            for (int i = 0; i < Player.bank.item.Length; i++) {
                var item = Player.bank.item[i];
                while (countCoins >= 1000000) {
                    if (!TryStackMoney(ref countCoins, 1000000, ItemID.PlatinumCoin, ref item))
                        goto ContinueFor;
                }
                while (countCoins >= 10000) {
                    if (!TryStackMoney(ref countCoins, 10000, ItemID.GoldCoin, ref item))
                        goto ContinueFor;
                }
                while (countCoins >= 100) {
                    if (!TryStackMoney(ref countCoins, 100, ItemID.SilverCoin, ref item))
                        goto ContinueFor;
                }
                while (countCoins >= 1) {
                    if (!TryStackMoney(ref countCoins, 1, ItemID.CopperCoin, ref item))
                        goto ContinueFor;
                }
                ContinueFor: continue;
            }
        }

        public static bool TryStackMoney(ref ulong countCoins, int currency, int coinType, ref Item bankItem) {
            if (bankItem.type == coinType && bankItem.stack < bankItem.maxStack) {
                countCoins -= (ulong)currency;
                bankItem.stack++;
                return true;
            }
            if (bankItem.IsAir) {
                countCoins -= (ulong)currency;
                bankItem.SetDefaults(coinType);
                return true;
            }
            return false;
        }
    }
}
