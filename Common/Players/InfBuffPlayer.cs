using ImproveGame.Common.GlobalItems;
using ImproveGame.Content.Items;

namespace ImproveGame.Common.Players
{
    public class InfBuffPlayer : ModPlayer
    {
        public static InfBuffPlayer Get(Player player) => player.GetModPlayer<InfBuffPlayer>();
        public static bool TryGet(Player player, out InfBuffPlayer modPlayer) => player.TryGetModPlayer(out modPlayer);

        public override void Load() {
            On.Terraria.Player.AddBuff += BanBuffs;
        }

        public override void PostUpdateBuffs() {
            if (Player.whoAmI != Main.myPlayer || Main.netMode == NetmodeID.Server)
                return;

            var items = MyUtils.GetAllInventoryItemsList(Player, false);
            foreach (var item in items) {
                HandleBuffItem(item);
                if (!item.IsAir && item.type == ModContent.ItemType<PotionBag>() &&
                    item.ModItem is not null && item.ModItem is PotionBag &&
                    (item.ModItem as PotionBag).storedPotions.Count > 0) {
                    var potionBag = item.ModItem as PotionBag;
                    foreach (var p in potionBag.storedPotions) {
                        HandleBuffItem(p);
                    }
                }
            }
        }

        public void HandleBuffItem(Item item) {
            int buffType = ApplyBuffItem.GetItemBuffType(item);
            if (buffType is not -1 && CheckInfBuffEnable(buffType)) {
                // 饱食三级Buff不应该覆盖，而是取最高级
                bool wellFed3Enabled = Player.FindBuffIndex(BuffID.WellFed3) != -1;
                if (buffType == BuffID.WellFed && (Player.FindBuffIndex(BuffID.WellFed2) != -1 || wellFed3Enabled))
                    return;
                if (buffType == BuffID.WellFed2 && wellFed3Enabled)
                    return;

                Player.AddBuff(buffType, 2);
            }
        }

        private void BanBuffs(On.Terraria.Player.orig_AddBuff orig, Player player, int type, int timeToAdd, bool quiet, bool foodHack) {
            if (Main.myPlayer == player.whoAmI && DataPlayer.TryGet(player, out var dataPlayer)) {
                if (dataPlayer.InfBuffDisabledVanilla is not null) {
                    foreach (int buffType in dataPlayer.InfBuffDisabledVanilla) {
                        if (type == buffType) {
                            return;
                        }
                    }
                }
                if (dataPlayer.InfBuffDisabledMod is not null) {
                    foreach (string buffFullName in dataPlayer.InfBuffDisabledMod) {
                        string[] names = buffFullName.Split('/');
                        string modName = names[0];
                        string buffName = names[1];
                        if (ModContent.TryFind<ModBuff>(modName, buffName, out var modBuff) && type == modBuff.Type) {
                            return;
                        }
                    }
                }
            }
            orig.Invoke(player, type, timeToAdd, quiet, foodHack);
        }

        public override void PreUpdateBuffs() {
            if (Main.myPlayer != Player.whoAmI || !DataPlayer.TryGet(Player, out var dataPlayer))
                return;
            DeleteBuffs(dataPlayer);
        }

        public void DeleteBuffs(DataPlayer dataPlayer) {
            for (int i = 0; i < Player.MaxBuffs; i++) {
                if (Player.buffType[i] > 0) {
                    if (dataPlayer.InfBuffDisabledVanilla is not null) {
                        foreach (int buffType in dataPlayer.InfBuffDisabledVanilla) {
                            if (Player.buffType[i] == buffType) {
                                Player.DelBuff(i);
                                i--;
                            }
                        }
                    }
                    if (dataPlayer.InfBuffDisabledVanilla is not null) {
                        foreach (string buffFullName in dataPlayer.InfBuffDisabledMod) {
                            string[] names = buffFullName.Split('/');
                            string modName = names[0];
                            string buffName = names[1];
                            if (ModContent.TryFind<ModBuff>(modName, buffName, out var modBuff) && Player.buffType[i] == modBuff.Type) {
                                Player.DelBuff(i);
                                i--;
                            }
                        }
                    }
                }
            }
        }

        public bool CheckInfBuffEnable(int buffType) {
            DataPlayer dataPlayer = DataPlayer.Get(Player);
            ModBuff modBuff = BuffLoader.GetBuff(buffType);
            if (modBuff is null) { // 原版
                if (dataPlayer.InfBuffDisabledVanilla is null || !dataPlayer.InfBuffDisabledVanilla.Contains(buffType)) {
                    return true;
                }
                else {
                    return false;
                }
            }
            else {
                string fullName = $"{modBuff.Mod.Name}/{modBuff.Name}";
                if (dataPlayer.InfBuffDisabledMod is null || !dataPlayer.InfBuffDisabledMod.Contains(fullName)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }

        /// <summary>
        /// 开关无限Buff
        /// </summary>
        /// <param name="buffType">Buff的ID</param>
        public void ToggleInfBuff(int buffType) {
            DataPlayer dataPlayer = DataPlayer.Get(Player);
            ModBuff modBuff = BuffLoader.GetBuff(buffType);
            if (modBuff is null) { // 原版
                if (!dataPlayer.InfBuffDisabledVanilla.Contains(buffType)) {
                    dataPlayer.InfBuffDisabledVanilla.Add(buffType);
                    dataPlayer.InfBuffDisabledVanilla.Sort();
                }
                else {
                    dataPlayer.InfBuffDisabledVanilla.Remove(buffType);
                }
            }
            else {
                string fullName = $"{modBuff.Mod.Name}/{modBuff.Name}";
                if (!dataPlayer.InfBuffDisabledMod.Contains(fullName)) {
                    dataPlayer.InfBuffDisabledMod.Add(fullName);
                    dataPlayer.InfBuffDisabledMod.Sort();
                }
                else {
                    dataPlayer.InfBuffDisabledMod.Remove(fullName);
                }
            }
        }
    }
}
