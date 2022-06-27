using ImproveGame.Common.GlobalItems;
using ImproveGame.Common.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace ImproveGame.Common.Players
{
    public class DataPlayer : ModPlayer
    {
        // 保存的物品前缀，哥布林重铸栏
        public int ReforgeItemPrefix = 0;
        public Item[] SuperVault;
        public bool SuperVaultVisable;
        private Vector2 SuperVaultPos;

        /*public override ModPlayer Clone(Player newEntity)
        {
            DataPlayer dataPlayer = (DataPlayer)base.Clone(newEntity);
            return dataPlayer;
        }*/

        /// <summary>
        /// 初始化数据
        /// </summary>
        public override void Initialize() {
            SuperVault = new Item[100];
            for (int i = 0; i < SuperVault.Length; i++) {
                SuperVault[i] = new Item();
            }
            SuperVaultPos = Vector2.Zero;
        }

        /// <summary>
        /// 进入地图时候
        /// </summary>
        /// <param name="player"></param>
        public override void OnEnterWorld(Player player) {
            if (!Main.dedServ && Main.myPlayer == player.whoAmI) {
                UISystem.JuVaultUIGUI.SetSuperVault(SuperVault, SuperVaultPos);
            }
        }

        public override void SaveData(TagCompound tag) {
            if (Main.reforgeItem.type > ItemID.None) {
                tag.Add("ReforgeItemPrefix", Main.reforgeItem.GetGlobalItem<GlobalItemData>().recastCount);
            }
            for (int i = 0; i < 100; i++) {
                tag.Add($"SuperVault_{i}", SuperVault[i]);
            }
            tag["SuperVaultPos"] = UISystem.JuVaultUIGUI.MainPanel.GetDimensions().Position();
        }

        public override void LoadData(TagCompound tag) {
            ReforgeItemPrefix = tag.GetInt("ReforgeItemPrefix");
            for (int i = 0; i < SuperVault.Length; i++) {
                if (tag.ContainsKey($"SuperVault_{i}")) {
                    SuperVault[i] = tag.Get<Item>($"SuperVault_{i}");
                }
            }
            tag.TryGet("SuperVaultPos", out SuperVaultPos);
        }
    }
}
