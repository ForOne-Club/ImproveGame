using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace ImproveGame.Common.GlobalItems
{
    public class ItemVar : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public int recastCount;

        // 克隆
        public override GlobalItem Clone(Item item, Item itemClone)
        {
            ItemVar clone = (ItemVar)MemberwiseClone();
            return clone;
        }

        // 保存数据
        public override void SaveData(Item item, TagCompound tag)
        {
            tag.Add("recastCount", item.GetGlobalItem<ItemVar>().recastCount);
        }

        // 加载数据
        public override void LoadData(Item item, TagCompound tag)
        {
            if (tag.ContainsKey("recastCount"))
                item.GetGlobalItem<ItemVar>().recastCount = tag.GetInt("recastCount");
        }
    }
}
