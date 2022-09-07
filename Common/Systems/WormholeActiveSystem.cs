namespace ImproveGame.Common.Systems
{
    public class WormholeActiveSystem : ModSystem
    {
        public override void Load()
        {
            On.Terraria.Player.HasItem += Player_HasItem;
            On.Terraria.Player.HasUnityPotion += Player_HasUnityPotion; // 完全重写
            On.Terraria.Player.TakeUnityPotion += Player_TakeUnityPotion; // 完全重写
        }

        private bool Player_HasItem(On.Terraria.Player.orig_HasItem orig, Player player, int type)
        {
            if (type == ItemID.WormholePotion && Config.NoConsume_Potion && HasItem(GetAllInventoryItemsList(player, true).ToArray(), -1, type))
            {
                return true;
            }
            return orig.Invoke(player, type);
        }

        private bool Player_HasUnityPotion(On.Terraria.Player.orig_HasUnityPotion orig, Player player)
        {
            var items = GetAllInventoryItemsList(player, false);
            foreach (var item in from i in items where i.type == ItemID.WormholePotion && i.stack > 0 select i)
            {
                return true;
            }
            return false;
        }

        private void Player_TakeUnityPotion(On.Terraria.Player.orig_TakeUnityPotion orig, Player player)
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
