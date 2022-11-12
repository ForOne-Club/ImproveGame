using ImproveGame.Common.GlobalItems;
using ImproveGame.Common.Packets;
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
        public Item[] SuperVault = new Item[100];
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

        public override void Initialize()
        {
            SuperVault = new Item[100];
            for (int i = 0; i < SuperVault.Length; i++)
            {
                SuperVault[i] ??= new Item();
            }
        }

        public override void LoadData(TagCompound tag)
        {
            // 大背包 Item 数据以及旧版兼容
            if (tag.ContainsKey("SuperVault"))
                SuperVault = tag.Get<Item[]>("SuperVault");
            for (int i = 0; i < SuperVault.Length; i++)
                if (tag.ContainsKey($"SuperVault_{i}"))
                    SuperVault[i] = tag.Get<Item>($"SuperVault_{i}");

            // 原版 Buff 禁用列表
            InfBuffDisabledVanilla = tag.Get<List<int>>("InfBuffDisabledVanilla") ?? new();
            // MOD Buff 禁用列表
            InfBuffDisabledMod = tag.Get<List<string>>("InfBuffDisabledMod") ?? new();
        }

        public override void SaveData(TagCompound tag)
        {
            tag["SuperVault"] = SuperVault;
            tag["InfBuffDisabledVanilla"] = InfBuffDisabledVanilla;
            tag["InfBuffDisabledMod"] = InfBuffDisabledMod;
        }

        public override void PostUpdate()
        {
            if (Main.myPlayer != Player.whoAmI)
                return;

            bool RefreshRecipes = false;
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
                    RefreshRecipes = true;
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        var packet = BigBagSlotPacket.Get(this, i);
                        packet.Send(runLocally: false);
                    }
                }
                oldSuperVaultStack[i] = SuperVault[i].stack;
                if (SuperVault[i].IsAir)
                    oldSuperVaultStack[i] = 0;
            }
            if (RefreshRecipes) Recipe.FindRecipes();
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            // 按照Example的写法 - 直接写就完了！
            var packet = BigBagAllSlotsPacket.Get(this);
            packet.Send(toWho, fromWho, false);
        }
    }
}
