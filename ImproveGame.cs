global using ImproveGame.Common.Utils;
global using ImproveGame.Common.Utils.NetHelpers;
global using Microsoft.Xna.Framework;
global using Microsoft.Xna.Framework.Graphics;
global using ReLogic.Content;
global using System;
global using System.IO;
global using System.Linq;
global using Terraria;
global using Terraria.Audio;
global using Terraria.GameContent;
global using Terraria.GameContent.UI.Elements;
global using Terraria.ID;
global using Terraria.Localization;
global using Terraria.ModLoader;
global using Terraria.UI;
global using static ImproveGame.MyUtils;
using ImproveGame.Common.Systems;

namespace ImproveGame
{
    // 更新任务
    // Tile 工具：自动钓鱼: true, 自动采集: false, 自动挖矿: false.
    // Buff Tile 在背包也可以获得 Buff: true
    // 刷怪率 UI: true
    public class ImproveGame : Mod
    {
        // 额外BUFF槽
        public override uint ExtraPlayerBuffSlots => (uint)Config.ExtraPlayerBuffSlots;
        public static ImproveGame Instance;

        public override void Load()
        {
            // 加载前缀信息
            LoadPrefixInfo();
            // On和IL移动到了Common.Systems.MinorModifySystem.cs
            Instance = this;
        }

        public override void Unload()
        {
            Instance = null;
            Config = null;
            GC.Collect();
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI) => NetHelper.HandlePacket(reader, whoAmI);

        public override object Call(params object[] args) => ModIntegrationsSystem.Call(args);
    }
}