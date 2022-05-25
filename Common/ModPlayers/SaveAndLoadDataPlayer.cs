using ImproveGame.Common.GlobalItems;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace ImproveGame.Common.GlobalPlayers
{
    public class SaveAndLoadDataPlayer : ModPlayer
    {
        // 保存的物品前缀，哥布林重铸栏
        public int ReforgeItemPrefix = 0;
        public override void SaveData(TagCompound tag)
        {
            if (Main.reforgeItem.type > ItemID.None)
            {
                tag.Add("ReforgeItemPrefix", Main.reforgeItem.GetGlobalItem<ItemVar>().recastCount);
            }
        }

        public override void LoadData(TagCompound tag)
        {
            ReforgeItemPrefix = tag.GetInt("ReforgeItemPrefix");
        }
    }
}
