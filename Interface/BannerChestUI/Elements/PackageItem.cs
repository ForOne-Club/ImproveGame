using Terraria.ModLoader.IO;

namespace ImproveGame.Interface.BannerChestUI.Elements
{
    public class PackageItem : ModItem
    {
        public override bool IsLoadingEnabled(Mod mod) => false;
        public bool autoStorage;
        public bool autoSort;

        public override void LoadData(TagCompound tag)
        {
            tag.TryGet("autoStorage", out autoStorage);
            tag.TryGet("autoSort", out autoSort);
        }

        public override void SaveData(TagCompound tag)
        {
            tag["autoStorage"] = autoStorage;
            tag["autoSort"] = autoSort;
        }
    }
}
