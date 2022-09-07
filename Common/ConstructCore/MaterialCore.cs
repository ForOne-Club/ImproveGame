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
        // 反表
        public static Dictionary<int, List<int>> TileToItem { get; private set; } = new();
        public static Dictionary<int, List<int>> WallToItem { get; private set; } = new();
        internal static bool FinishSetup { get; private set; } = false;

        public static Dictionary<int, int> CountMaterials(QoLStructure structure)
        {
            List<TileDefinition> data = structure.StructureDatas;

            if (data is null || data.Count is 0)
            {
                // 此处应有Logger.Warn
                return new();
            }

            // Key: ItemID, Value: 堆叠
            var materialDictionary = new Dictionary<int, int>();

            foreach (var tileData in data)
            {
                int tileType = structure.ParseTileType(tileData);
                if (tileType is not -1)
                {
                    if (TileID.Sets.Grass[tileType])
                        tileType = TileID.Dirt;

                    int tileItemType;
                    var tileObjectData = TileObjectData.GetTileData(tileType, 0);
                    bool isMaterialVaild;
                    if (tileObjectData is null || (tileObjectData.CoordinateFullWidth <= 18 && tileObjectData.CoordinateFullHeight <= 18))
                    {
                        tileItemType = GetTileItem(tileType, tileData.TileFrameX, tileData.TileFrameY);
                        isMaterialVaild = true;
                    }
                    else
                    {
                        int subX = (tileData.TileFrameX / tileObjectData.CoordinateFullWidth) * tileObjectData.CoordinateFullWidth;
                        int subY = (tileData.TileFrameY / tileObjectData.CoordinateFullHeight) * tileObjectData.CoordinateFullHeight;
                        tileItemType = GetTileItem(tileType, subX, subY);

                        subX = tileData.TileFrameX % tileObjectData.CoordinateFullWidth;
                        subY = tileData.TileFrameY % tileObjectData.CoordinateFullHeight;
                        Point16 frame = new(subX / 18, subY / 18);
                        isMaterialVaild = frame.X == tileObjectData.Origin.X && frame.Y == tileObjectData.Origin.Y;
                    }

                    if (isMaterialVaild && tileItemType is not -1)
                        PlusMaterial(tileItemType);
                }

                int wallItemType = GetWallItem(structure.ParseWallType(tileData));
                if (wallItemType != -1)
                    PlusMaterial(wallItemType);

                PlusMaterial(ItemID.Wire, tileData.RedWire.ToInt() + tileData.GreenWire.ToInt() + tileData.BlueWire.ToInt() + tileData.YellowWire.ToInt());
                if (tileData.HasActuator)
                    PlusMaterial(ItemID.Actuator);

                void PlusMaterial(int itemID, int amount = 1)
                {
                    if (!materialDictionary.ContainsKey(itemID))
                        materialDictionary[itemID] = amount;
                    else
                        materialDictionary[itemID] += amount;
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
                        if (TileToItem.ContainsKey(targetTile))
                        {
                            TileToItem[targetTile].Add(i);
                        }
                        else
                        {
                            TileToItem.Add(targetTile, new() { i });
                        }
                    }
                    if (item.createWall != -1)
                    {
                        ItemToWall.Add(i, item.createWall);
                        if (WallToItem.ContainsKey(item.createWall))
                        {
                            WallToItem[item.createWall].Add(i);
                        }
                        else
                        {
                            WallToItem.Add(item.createWall, new() { i });
                        }
                    }
                }
                FinishSetup = true;
            });
            setupThread.Start();
        }
    }
}
