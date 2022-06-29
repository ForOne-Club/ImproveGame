using ImproveGame.Common.GlobalItems;
using ImproveGame.Common.Systems;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace ImproveGame.Common.Players
{
    public class InfBuffPlayer : ModPlayer
    {
        public static InfBuffPlayer Get(Player player) => player.GetModPlayer<InfBuffPlayer>();

        public override void PreUpdateBuffs() {
            DataPlayer dataPlayer = DataPlayer.Get(Player);
            // 同情况下测试，下面的代码平均可以省下0.001ms，虽然很小，上面这个注释掉了
            //foreach (int buffType in dataPlayer.InfBuffDisabledVanilla) {
            //    Player.ClearBuff(buffType);
            //}
            //foreach (string buffFullName in dataPlayer.InfBuffDisabledMod) {
            //    string[] names = buffFullName.Split('/');
            //    string modName = names[0];
            //    string buffName = names[1];
            //    if (ModContent.TryFind<ModBuff>(modName, buffName, out var modBuff))
            //        Player.ClearBuff(modBuff.Type);
            //}
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
                string fullName = $"{modBuff.Mod}/{modBuff.Name}";
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
                string fullName = $"{modBuff.Mod}/{modBuff.Name}";
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
