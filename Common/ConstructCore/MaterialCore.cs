using System.Collections.Generic;
using System.Threading;
using Terraria.ModLoader.IO;

namespace ImproveGame.Common.ConstructCore
{
    public class MaterialCore : ModSystem
    {
        public static Dictionary<int, int> ItemToTile { get; private set; } = new();
        public static Dictionary<int, int> ItemToWall { get; private set; } = new();
        internal static bool FinishSetup { get; private set; } = false;

        public static Dictionary<int, int> CountMaterials(TagCompound tag)
        {
            List<TileDefinition> data = (List<TileDefinition>)tag.GetList<TileDefinition>("StructureData");

            if (data is null || data.Count is 0)
            {
                // 此处应有Logger.Warn
                return new();
            }

            // Key: ItemID, Value: 堆叠
            var materialDictionary = new Dictionary<int, int>();

            foreach (var tileData in data)
            {
                int tileItemType = GetTileItem(FileOperator.ParseTileType(tileData.Tile));
                int wallItemType = GetWallItem(FileOperator.ParseTileType(tileData.Wall));
                if (tileItemType != -1)
                {
                    if (!materialDictionary.ContainsKey(tileItemType))
                        materialDictionary[tileItemType] = 1;
                    else
                        materialDictionary[tileItemType]++;
                }
                if (wallItemType != -1)
                {
                    if (!materialDictionary.ContainsKey(wallItemType))
                        materialDictionary[wallItemType] = 1;
                    else
                        materialDictionary[wallItemType]++;
                }
            }

            return materialDictionary;
        }

        public override void PostSetupContent()
        {
            FinishSetup = false;
            var setupThread = new Thread(() =>
            {
                for (int i = 0; i < ItemLoader.ItemCount; i++)
                {
                    var item = new Item(i);
                    if (item.createTile != -1)
                    {
                        ItemToTile.Add(i, item.createTile);
                    }
                    if (item.createWall != -1)
                    {
                        ItemToWall.Add(i, item.createWall);
                    }
                }
                FinishSetup = true;
            });
            setupThread.Start();
        }

        public static int GetItemTile(int itemType)
        {
            if (FinishSetup && ItemToTile.ContainsKey(itemType))
            {
                return ItemToTile[itemType];
            }
            return -1;
        }

        public static int GetTileItem(int tileType)
        {
            if (FinishSetup && ItemToTile.ContainsValue(tileType))
            {
                return ItemToTile.FirstOrDefault(i => i.Value == tileType).Key;
            }
            return -1;
        }

        public static int GetItemWall(int itemType)
        {
            if (FinishSetup && ItemToWall.ContainsKey(itemType))
            {
                return ItemToWall[itemType];
            }
            return -1;
        }

        public static int GetWallItem(int wallType)
        {
            if (FinishSetup && ItemToWall.ContainsValue(wallType))
            {
                return ItemToWall.FirstOrDefault(i => i.Value == wallType).Key;
            }
            return -1;
        }
    }
}
