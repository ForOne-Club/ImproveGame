using ImproveGame.Common.GlobalItems;
using ImproveGame.Common.Players;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ImproveGame
{
    // 更新任务
    // 神庙电池（现版本右键使用会消耗），松露虫，光女召唤物下版本更新加入不消耗 ×
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
    }
}