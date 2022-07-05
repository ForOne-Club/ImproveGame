namespace ImproveGame.Common.Players
{
    public class InfBuffPlayer : ModPlayer
    {
        public static InfBuffPlayer Get(Player player) => player.GetModPlayer<InfBuffPlayer>();

        public override void Load() {
            On.Terraria.Player.AddBuff += BanBuffs;
        }

        private void BanBuffs(On.Terraria.Player.orig_AddBuff orig, Player player, int type, int timeToAdd, bool quiet, bool foodHack) {
            if (Main.myPlayer == Player.whoAmI) {
                DataPlayer dataPlayer = DataPlayer.Get(player);
                foreach (int buffType in dataPlayer.InfBuffDisabledVanilla) {
                    if (type == buffType) {
                        return;
                    }
                }
                foreach (string buffFullName in dataPlayer.InfBuffDisabledMod) {
                    string[] names = buffFullName.Split('/');
                    string modName = names[0];
                    string buffName = names[1];
                    if (ModContent.TryFind<ModBuff>(modName, buffName, out var modBuff) && type == modBuff.Type) {
                        return;
                    }
                }
            }
            orig.Invoke(player, type, timeToAdd, quiet, foodHack);
        }

        public override void PreUpdateBuffs() {
            if (Main.myPlayer != Player.whoAmI)
                return;
            DeleteBuffs();
        }

        public void DeleteBuffs() {
            DataPlayer dataPlayer = DataPlayer.Get(Player);
            for (int i = 0; i < Player.MaxBuffs; i++) {
                if (Player.buffType[i] > 0) {
                    foreach (int buffType in dataPlayer.InfBuffDisabledVanilla) {
                        if (Player.buffType[i] == buffType) {
                            Player.DelBuff(i);
                            i--;
                        }
                    }
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

        public bool CheckInfBuffEnable(int buffType) {
            DataPlayer dataPlayer = DataPlayer.Get(Player);
            ModBuff modBuff = BuffLoader.GetBuff(buffType);
            if (modBuff is null) { // 原版
                if (!dataPlayer.InfBuffDisabledVanilla.Contains(buffType)) {
                    return true;
                }
                else {
                    return false;
                }
            }
            else {
                string fullName = $"{modBuff.Mod.Name}/{modBuff.Name}";
                if (!dataPlayer.InfBuffDisabledMod.Contains(fullName)) {
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
