using ImproveGame.Common.GlobalItems;
using ImproveGame.Common.Systems;
using ImproveGame.Content.UI;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace ImproveGame.Common.Players
{
    public class DataPlayer : ModPlayer
    {
        // 保存的物品前缀，哥布林重铸栏
        public int ReforgeItemPrefix = 0;
        public Item[] SuperVault;
        public Vector2 SuperVaultOffset;
        public bool SuperVaultVisable;

        /*public override ModPlayer Clone(Player newEntity)
        {
            DataPlayer dataPlayer = (DataPlayer)base.Clone(newEntity);
            return dataPlayer;
        }*/

        /// <summary>
        /// 初始化数据
        /// </summary>
        public override void Initialize()
        {
            SuperVault = new Item[100];
            for (int i = 0; i < SuperVault.Length; i++)
            {
                SuperVault[i] = new Item();
            }
            SuperVaultOffset = Vector2.Zero;
        }

        /// <summary>
        /// 进入地图时候
        /// </summary>
        /// <param name="player"></param>
        public override void OnEnterWorld(Terraria.Player player)
        {
            if (!Main.dedServ && Main.myPlayer == player.whoAmI)
            {
                UISystem.vaultUI.MyInitialize(SuperVault, SuperVaultOffset, SuperVaultVisable);
            }
        }

        public override void SaveData(TagCompound tag)
        {
            if (Main.reforgeItem.type > ItemID.None)
            {
                tag.Add("ReforgeItemPrefix", Main.reforgeItem.GetGlobalItem<GlobalItemData>().recastCount);
            }
            for (int i = 0; i < 100; i++)
            {
                tag.Add($"SuperVault_{i}", SuperVault[i]);
            }
            tag.Add("SuperVaultOffset", VaultUI.position);
            tag.Add("SuperVaultVisable", VaultUI._visible);
        }

        public override void LoadData(TagCompound tag)
        {
            ReforgeItemPrefix = tag.GetInt("ReforgeItemPrefix");
            for (int i = 0; i < SuperVault.Length; i++)
            {
                if (tag.ContainsKey($"SuperVault_{i}"))
                {
                    SuperVault[i] = tag.Get<Item>($"SuperVault_{i}");
                }
            }
            if (tag.ContainsKey("SuperVaultOffset"))
            {
                SuperVaultOffset = tag.Get<Vector2>($"SuperVaultOffset");
            }
            if (tag.ContainsKey("SuperVaultVisible"))
            {
                SuperVaultVisable = tag.Get<bool>($"SuperVaultVisable");
            }
        }
    }
}
