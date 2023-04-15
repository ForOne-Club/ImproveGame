using Terraria.DataStructures;

namespace ImproveGame.Common.Utils.Extensions
{
    public static class PlayerExtensions
    {
        public static void GetItem(IEntitySource source, Item item, bool toMouse)
        {
            Player player = Main.LocalPlayer;
            GetItem(player, source, item, toMouse);
        }
        
        public static void GetItem(this Player player, IEntitySource source, Item item, bool toMouse)
        {
            if (toMouse && Main.playerInventory && Main.mouseItem.IsAir)
            {
                Main.mouseItem = item;
                return;
            }

            if (toMouse && Main.playerInventory && Main.mouseItem.type == item.type)
            {
                int total = Main.mouseItem.stack + item.stack;
                if (total > Main.mouseItem.maxStack)
                    total = Main.mouseItem.maxStack;
                int difference = total - Main.mouseItem.stack;
                Main.mouseItem.stack = total;
                item.stack -= difference;
            }

            if (item.IsAir)
                return;

            item = player.GetItem(Main.myPlayer, item, GetItemSettings.InventoryEntityToPlayerInventorySettings);
            if (item.IsAir)
                return;

            if (Main.mouseItem.IsAir)
            {
                Main.mouseItem = item;
                return;
            }

            player.QuickSpawnItem(source, item, item.stack);
        }
    }
}