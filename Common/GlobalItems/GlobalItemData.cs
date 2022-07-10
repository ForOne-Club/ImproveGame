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

        // 克隆
        public override GlobalItem Clone(Item item, Item itemClone)
        {
            return base.Clone(item, itemClone);
        }

        // 保存数据
        public override void SaveData(Item item, TagCompound tag)
        {
            tag.Add("recastCount", item.GetGlobalItem<GlobalItemData>().recastCount);
        }

        // 加载数据
        public override void LoadData(Item item, TagCompound tag)
        {
            if (tag.ContainsKey("recastCount"))
                item.GetGlobalItem<GlobalItemData>().recastCount = tag.GetInt("recastCount");
        }
    }
}
