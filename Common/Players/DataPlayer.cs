using ImproveGame.Common.GlobalItems;
using ImproveGame.Common.Systems;
using System.Collections.Generic;
using Terraria.ModLoader.IO;

namespace ImproveGame.Common.Players
{
    public class DataPlayer : ModPlayer
    {
        public static bool TryGet(Player player, out DataPlayer modPlayer) => player.TryGetModPlayer(out modPlayer);
        public static DataPlayer Get(Player player) => player.GetModPlayer<DataPlayer>();

        // 保存的物品前缀，哥布林重铸栏
        public int ReforgeItemPrefix = 0;
        public Item[] SuperVault;
        public bool SuperVaultVisable;
        private Vector2 SuperVaultPos;

        /// <summary>
        /// 记录ID
        /// </summary>
        public List<int> InfBuffDisabledVanilla;

        /// <summary>
        /// 格式：Mod内部名/Buff类名
        /// </summary>
        public List<string> InfBuffDisabledMod;

        // 有些数据必须在这里初始化。
        public override void Initialize() {
            SuperVault = new Item[100];
            for (int i = 0; i < SuperVault.Length; i++) {
                SuperVault[i] = new();
            }
        }

        /// <summary>
        /// 进入地图时候
        /// </summary>
        /// <param name="player"></param>
        public override void OnEnterWorld(Player player) {
            if (!Main.dedServ && Main.myPlayer == player.whoAmI) {
                UISystem.Instance.BigBagGUI.SetSuperVault(SuperVault, SuperVaultPos);
            }
        }

        public override void SaveData(TagCompound tag) {
            if (Main.reforgeItem.type > ItemID.None) {
                tag.Add("ReforgeItemPrefix", Main.reforgeItem.GetGlobalItem<GlobalItemData>().recastCount);
            }

            for (int i = 0; i < 100; i++) {
                tag.Add($"SuperVault_{i}", SuperVault[i]);
            }

            tag["SuperVaultPos"] = UISystem.Instance.BigBagGUI.MainPanel.GetDimensions().Position();

            tag["InfBuffDisabledVanilla"] = InfBuffDisabledVanilla;
            tag["InfBuffDisabledMod"] = InfBuffDisabledMod;
        }

        public override void LoadData(TagCompound tag) {
            // 哥布林重铸栏内属于 Mod 的数据无法保存，这个是专门用于保存这个无法保存到 Item 上的数据。
            tag.TryGet("ReforgeItemPrefix", out ReforgeItemPrefix);

            // 大背包 Item 数据，选择了错误的保存方式，但是现在没办法改了。。。
            for (int i = 0; i < SuperVault.Length; i++) {
                tag.TryGet($"SuperVault_{i}", out SuperVault[i]);
            }

            // 大背包的位置，保存一下吧。
            tag.TryGet("SuperVaultPos", out SuperVaultPos);

            // 原版 Buff 禁用列表
            tag.TryGet("InfBuffDisabledVanilla", out InfBuffDisabledVanilla);
            InfBuffDisabledVanilla ??= new();

            // MOD Buff 禁用列表
            tag.TryGet("InfBuffDisabledMod", out InfBuffDisabledMod);
            InfBuffDisabledMod ??= new();
        }
    }
}
