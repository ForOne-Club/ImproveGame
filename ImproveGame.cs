global using ImproveGame.Common.Utils;
global using ImproveGame.Common.Utils.NetHelpers;
global using Microsoft.Xna.Framework;
global using System;
global using Terraria;
global using Terraria.Audio;
global using Terraria.ID;
global using Terraria.ModLoader;
global using Terraria.UI;
using System.IO;

namespace ImproveGame
{
    // 更新任务
    // Tile 工具：自动钓鱼，自动采集，自动挖矿
    // Buff Tile 在背包也可以获得 Buff （已完成）
    // 刷怪率 UI
    public class ImproveGame : Mod
    {
        // 额外BUFF槽
        public override uint ExtraPlayerBuffSlots => (uint)MyUtils.Config.ExtraPlayerBuffSlots;
        public static ImproveGame Instance;

        public override void Load() {
            // 加载前缀信息
            MyUtils.LoadPrefixInfo();
            // On和IL移动到了Common.Systems.MinorModifySystem.cs
            Instance = this;
        }

        public override void Unload() {
            Instance = null;
            MyUtils.Config = null;
            GC.Collect();
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI) {
            NetHelper.HandlePacket(reader, whoAmI);
        }
    }
}