using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace ImproveGame.Common.GlobalItems
{
    public class GlobalItemData : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public int recastCount;
        public bool ShouldHaveInvGlowForBanner = false;
        public bool InventoryGlow;

        // 保存数据
        public override void SaveData(Item item, TagCompound tag)
        {
            if (recastCount > 0)
                tag.Add("recastCount", recastCount);
        }

        // 加载数据
        public override void LoadData(Item item, TagCompound tag)
        {
            recastCount = tag.GetInt("recastCount"); // 根据官方Wiki若找不到会赋初值0
        }
    }
}
