using ImproveGame.Common.GlobalItems;
using ImproveGame.Interface.Common;
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
        private readonly int[] oldSuperVaultStack = new int[100]; // 上一帧的SuperVault的stack情况
        public readonly Item[] SuperVault = new Item[100];
        public bool SuperVaultVisable;
        public Vector2 SuperVaultPos;

        /// <summary>
        /// 记录ID
        /// </summary>
        public List<int> InfBuffDisabledVanilla;

        /// <summary>
        /// 格式：Mod内部名/Buff类名
        /// </summary>
        public List<string> InfBuffDisabledMod;

        public override void ResetEffects()
        {
            for (int i = 0; i < SuperVault.Length; i++)
            {
                if (SuperVault[i] is null)
                    SuperVault[i] = new Item();
            }
        }

        public override void LoadData(TagCompound tag)
        {
            // 哥布林重铸栏内属于 Mod 的数据无法保存，这个是专门用于保存这个无法保存到 Item 上的数据。
            tag.TryGet("ReforgeItemPrefix", out ReforgeItemPrefix);

            // 大背包 Item 数据，选择了错误的保存方式，但是现在没办法改了。。。
            for (int i = 0; i < SuperVault.Length; i++)
            {
                if (!tag.TryGet($"SuperVault_{i}", out SuperVault[i]))
                    SuperVault[i] = new Item();
            }

            // 大背包的位置
            tag.TryGet("SuperVaultPos", out SuperVaultPos);

            // 原版 Buff 禁用列表
            if (!tag.TryGet("InfBuffDisabledVanilla", out InfBuffDisabledVanilla))
                InfBuffDisabledVanilla = new();

            // MOD Buff 禁用列表
            if (!tag.TryGet("InfBuffDisabledMod", out InfBuffDisabledMod))
                InfBuffDisabledMod = new();
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

            tag["SuperVaultPos"] = UISystem.Instance.BigBagGUI.MainPanel.GetDimensions().Position();

            tag["InfBuffDisabledVanilla"] = InfBuffDisabledVanilla;
            tag["InfBuffDisabledMod"] = InfBuffDisabledMod;
        }

        public override void PostUpdate()
        {
            if (Main.myPlayer != Player.whoAmI)
                return;

            // 侦测stack，如果有变化就发包
            for (int i = 0; i < 100; i++)
            {
                if (SuperVault[i] is null)
                {
                    SuperVault[i] = new();
                    continue;
                }
                if (SuperVault[i].stack != oldSuperVaultStack[i])
                {
                    Recipe.FindRecipes(); // 刷新配方
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        NetBigBag.SendSlot(i, SuperVault[i], Main.myPlayer, -1, -1);
                    }
                }
                oldSuperVaultStack[i] = SuperVault[i].stack;
                if (SuperVault[i].IsAir)
                    oldSuperVaultStack[i] = 0;
            }
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            // 按照Example的写法 - 直接写就完了！
            NetBigBag.SendAllSlot(this, toWho, fromWho);
        }
    }
}
