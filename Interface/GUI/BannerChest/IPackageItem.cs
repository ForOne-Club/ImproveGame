using Terraria.ModLoader.IO;

namespace ImproveGame.Interface.GUI.BannerChest
{
    public interface IPackageItem
    {
        public bool AutoStorage { get; set; }
        public bool AutoSort { get; set; }
        public void Sort();
        public void PutInPackage(ref Item item);
        public void ISaveData(TagCompound tag)
        {
            tag["autoStorage"] = AutoStorage;
            tag["autoSort"] = AutoSort;
        }
        public void ILoadData(TagCompound tag)
        {
            if (tag.TryGet("autoStorage", out bool autoStorage))
            {
                AutoStorage = autoStorage;
            }
            if (tag.TryGet("autoSort", out bool autoSort))
            {
                AutoSort = autoSort;
            }
        }
    }
}
