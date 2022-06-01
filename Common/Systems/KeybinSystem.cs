using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace ImproveGame.Common.Systems
{
    /// <summary>
    /// 快捷键
    /// </summary>
    public class KeybinSystem : ModSystem
    {
        public static ModKeybind RandomBuffKeybind { get; private set; }

        public override void Load()
        {
            RandomBuffKeybind = KeybindLoader.RegisterKeybind(Mod, MyUtils.GetText("Keybin.SuperVault"), "I");
        }

        public override void Unload()
        {
            RandomBuffKeybind = null;
        }
    }
}
