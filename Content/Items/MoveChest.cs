using Terraria.ModLoader.IO;

namespace ImproveGame.Content.Items
{
    // 好像有 BUG：使用魔杖拿起箱子 -> 保存退出 -> 再放置 -> 再用法杖拿起刚放置的箱子 -> 卡死
    internal class MoveChest : ModItem
    {
        protected override bool CloneNewInstances => true;
        public override bool IsCloneable => true;
        public override ModItem Clone(Item newEntity)
        {
            var clone = base.Clone(newEntity) as MoveChest;
            clone.items = null;
            clone.hasChest = false;
            clone.modChestName = null;
            clone.chestType = 0;
            return base.Clone(newEntity);
        }
        public Item[] items = null;
        public bool hasChest = false;
        public ushort chestType = 0;
        public string modChestName = null;

        public override void SetDefaults()
        {
            Item.width = 1;
            Item.height = 1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.autoReuse = true;
        }

        public override bool CanUseItem(Player player)
        {
            var coord = Main.MouseWorld.ToTileCoordinates();
            if (hasChest)
            {
                int index = WorldGen.PlaceChest(coord.X, coord.Y, chestType);
                if(index == -1)
                {
                    //多检测一格的判定，看起来稍微聪明点
                    if((index = WorldGen.PlaceChest(coord.X - 1, coord.Y, chestType)) == -1)
                    {
                        return false;
                    }
                }
                SoundEngine.PlaySound(SoundID.Dig, player.position);
                var chest = Main.chest[index];
                hasChest = false;
                chest.item = items;
                items = null;
                chestType = 0;
                modChestName = null;
                return true;
            }

            var tile = Main.tile[coord.X, coord.Y];
            
            if (TileID.Sets.BasicChest[tile.TileType])
            {
                for (int i = 0; i < Main.chest.Length; i++)
                {
                    var chest = Main.chest[i];
                    if(chest == null)
                    {
                        continue;
                    }
                    Rectangle chestRange = new Rectangle(chest.x, chest.y, 2, 2);
                    if (chestRange.Contains(coord))
                    {
                        coord = new Point(chest.x, chest.y);
                        items = new Item[chest.item.Length];
                        chest.item.CopyTo(items, 0);
                        Main.chest[i] = null;
                        chestType = tile.TileType;
                        if (chestType >= TileID.Count)
                        {
                            modChestName = ModContent.GetModTile(chestType).FullName;
                        }
                        hasChest = true;

                        SoundEngine.PlaySound(SoundID.Dig, player.position);
                        Main.tile[coord.X , coord.Y].ClearTile();
                        Main.tile[coord.X , coord.Y + 1].ClearTile();
                        Main.tile[coord.X + 1, coord.Y].ClearTile();
                        Main.tile[coord.X + 1, coord.Y + 1].ClearTile();
                        return true;
                    }
                }

            }
            return false;
        }

        public override void SaveData(TagCompound tag)
        {
            if(!hasChest)
            {
                return;
            }

            tag.Add("chest", (short)chestType);
            if(chestType >= TileID.Count)
            {
                tag.Add("mod", modChestName);
            }
            for (int i = 0; i < items.Length; i++)
            {
                tag.Add(i.ToString(), ItemIO.Save(items[i]));
            }
        }
        public override void LoadData(TagCompound tag)
        {
            if(tag.Count == 0)
            {
                return;
            }

            hasChest = true;
            chestType = (ushort)tag.GetAsShort("chest");
            if(chestType >= TileID.Count)
            {
                modChestName = tag.GetString("mod");
                if(ModContent.TryFind<ModTile>(modChestName, out var tile))
                {
                    chestType = tile.Type;
                }
                else
                {
                    chestType = TileID.Containers;
                }
            }
            items = new Item[Chest.maxItems];
            for (int i = 0; i < items.Length; i++)
            {
                items[i] = ItemIO.Load(tag.GetCompound(i.ToString()));
            }
        }
    }
}
