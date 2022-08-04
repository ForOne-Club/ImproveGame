using System.Collections.Generic;
using System.Threading;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace ImproveGame.Common.ConstructCore
{
    public class MaterialCore : ModSystem
    {
        public static Dictionary<int, int> ItemToPlaceStyle { get; private set; } = new();
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
                int tileType = FileOperator.ParseTileType(tileData.Tile);
                if (tileType is not -1 && TileID.Sets.Grass[tileType])
                {
                    tileType = TileID.Dirt;
                }
                int tileItemType = GetTileItem(tileType);
                if (tileItemType != -1)
                {
                    var tileObjectData = TileObjectData.GetTileData(tileType, ItemToPlaceStyle[tileItemType]);
                    bool isMaterialVaild;
                    if (tileObjectData is null || (tileObjectData.CoordinateFullWidth <= 18 && tileObjectData.CoordinateFullHeight <= 18))
                    {
                        isMaterialVaild = true;
                    }
                    else
                    {
                        int subX = tileData.TileFrameX % tileObjectData.CoordinateFullWidth;
                        int subY = tileData.TileFrameY % tileObjectData.CoordinateFullHeight;
                        Point16 frame = new(subX / 18, subY / 18);
                        isMaterialVaild = frame.X == tileObjectData.Origin.X && frame.Y == tileObjectData.Origin.Y;
                    }
                    if (isMaterialVaild)
                    {
                        if (!materialDictionary.ContainsKey(tileItemType))
                            materialDictionary[tileItemType] = 1;
                        else
                            materialDictionary[tileItemType]++;
                    }
                }

                int wallItemType = GetWallItem(FileOperator.ParseTileType(tileData.Wall));
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
                        int targetTile = item.createTile;
                        ItemToTile.Add(i, targetTile);
                        ItemToPlaceStyle.Add(i, item.placeStyle);
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
    }
}
