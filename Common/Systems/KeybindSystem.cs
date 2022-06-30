using Terraria.ModLoader;

namespace ImproveGame.Common.Systems
{
    /// <summary>
    /// 快捷键
    /// </summary>
    public class KeybindSystem : ModSystem
    {
        public static ModKeybind SuperVaultKeybind { get; private set; }

        public override void Load()
        {
            SuperVaultKeybind = KeybindLoader.RegisterKeybind(Mod, "大背包快捷键 Huge Inventory Keybind", "I");
        }

        public override void Unload()
        {
            SuperVaultKeybind = null;
        }
    }
}
