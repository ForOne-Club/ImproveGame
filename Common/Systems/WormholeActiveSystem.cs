namespace ImproveGame.Common.Systems
{
    public class WormholeActiveSystem : ModSystem
    {
        public override void Load()
        {
            On_Player.HasItem_int_ItemArray += Player_HasItem;
            On_Player.HasUnityPotion += Player_HasUnityPotion; // 完全重写
            On_Player.TakeUnityPotion += Player_TakeUnityPotion; // 完全重写
        }

        private bool Player_HasItem(On_Player.orig_HasItem_int_ItemArray orig, Player self, int type, Item[] collection)
        {
            if (type == ItemID.WormholePotion && Config.NoConsume_Potion && HasItem(GetAllInventoryItemsList(self, true).ToArray(), -1, type))
            {
                return true;
            }

            return orig.Invoke(self, type, collection);
        }

        private bool Player_HasUnityPotion(Terraria.On_Player.orig_HasUnityPotion orig, Player player)
        {
            var items = GetAllInventoryItemsList(player, false);
            foreach (var item in from i in items where i.type == ItemID.WormholePotion && i.stack > 0 select i)
            {
                return true;
            }
            return false;
        }

        private void Player_TakeUnityPotion(Terraria.On_Player.orig_TakeUnityPotion orig, Player player)
        {
            var itemsArray = GetAllInventoryItems(player, false);
            for (int a = 0; a < itemsArray.Count; a++)
            {
                for (int i = 0; i < itemsArray[a].Length; i++)
                {
                    var item = itemsArray[a][i];
                    if (item.type == ItemID.WormholePotion && item.stack > 0)
                    {
                        if (ItemLoader.ConsumeItem(item, player))
                            item.stack--;
                        if (item.stack <= 0)
                            item.SetDefaults();
                        break;
                    }
                }
            }
        }
    }
}
