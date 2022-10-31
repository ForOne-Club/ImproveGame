global using ImproveGame.Common.Utils;
global using Microsoft.Xna.Framework;
global using Microsoft.Xna.Framework.Graphics;
global using NetSimplified;
global using ReLogic.Content;
global using System;
global using System.Collections.Generic;
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
global using TrUtils = Terraria.Utils;
using ImproveGame.Common;
using ImproveGame.Common.Systems;
using Terraria.UI.Chat;

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
            Instance = this;
            
            ChatManager.Register<BgItemTagHandler>("bgitem");
            ChatManager.Register<CenteredItemTagHandler>("centeritem");
        }

        public override void Unload()
        {
            Instance = null;
            Config = null;
            GC.Collect();
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            //NetHelper.HandlePacket(reader, whoAmI);
            NetModule.ReceiveModule(reader, whoAmI);
        }

        public override object Call(params object[] args) => ModIntegrationsSystem.Call(args);
    }
}