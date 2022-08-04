using ImproveGame.Common.Systems;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace ImproveGame.Common.ConstructCore
{
    internal class GenerateCore
    {
        public static void GenerateFromTag(TagCompound tag, Point position)
        {
            CoroutineSystem.TileRunner.Run(Generate(tag, position));
        }

        public static IEnumerator Generate(TagCompound tag, Point position)
        {
            List<TileDefinition> data = (List<TileDefinition>)tag.GetList<TileDefinition>("StructureData");

            if (data is null || data.Count is 0)
            {
                // 此处应有Logger.Warn
                yield break;
            }

            int width = tag.GetInt("Width");
            int height = tag.GetInt("Height");

            var currentTask = CoroutineSystem.TileRunner.Run(KillTiles(position, width, height));
            while (currentTask.IsRunning)
                yield return null;
            currentTask = CoroutineSystem.TileRunner.Run(GenerateOneBlockTiles(data, position, width, height));
            while (currentTask.IsRunning)
                yield return null;
            currentTask = CoroutineSystem.TileRunner.Run(GenerateWalls(data, position, width, height));
            while (currentTask.IsRunning)
                yield return null;
            CoroutineSystem.TileRunner.Run(GenerateMultiTiles(data, position, width, height));
        }

        public static IEnumerator KillTiles(Point position, int width, int height)
        {
            for (int x = 0; x <= width; x++)
            {
                for (int y = 0; y <= height; y++)
                {
                    var placePosition = position + new Point(x, y);
                    Tile tile = Main.tile[placePosition.X, placePosition.Y];
                    if (tile.HasTile && TryKillTile(placePosition.X, placePosition.Y, Main.LocalPlayer))
                    {
                        yield return null;
                    }
                }
            }
        }

        public static IEnumerator GenerateOneBlockTiles(List<TileDefinition> data, Point position, int width, int height)
        {
            for (int x = 0; x <= width; x++)
            {
                for (int y = 0; y <= height; y++)
                {
                    int index = y + x * (height + 1);
                    TileDefinition tileData = data[index];
                    int tileType = FileOperator.ParseTileType(tileData.Tile);
                    if (tileType is not -1 && TileID.Sets.Grass[tileType])
                    {
                        tileType = TileID.Dirt;
                    }
                    int tileItemType = GetTileItem(tileType);
                    if (tileItemType != -1)
                    {
                        var tileObjectData = TileObjectData.GetTileData(tileType, MaterialCore.ItemToPlaceStyle[tileItemType]);
                        if (tileObjectData is null || (tileObjectData.CoordinateFullWidth <= 18 && tileObjectData.CoordinateFullHeight <= 18))
                        {
                            var placePosition = position + new Point(x, y);
                            PickItemInInventory(Main.LocalPlayer, (item) =>
                                item.type == tileItemType &&
                                TryPlaceTile(placePosition.X, placePosition.Y, item, Main.LocalPlayer, forced: true),
                                true, out _);
                            yield return null;
                        }
                    }
                }
            }
        }

        public static IEnumerator GenerateWalls(List<TileDefinition> data, Point position, int width, int height)
        {
            for (int x = 0; x <= width; x++)
            {
                for (int y = 0; y <= height; y++)
                {
                    int index = y + x * (height + 1);
                    var placePosition = position + new Point(x, y);
                    TileDefinition tileData = data[index];
                    int wallItemType = GetWallItem(FileOperator.ParseTileType(tileData.Wall));
                    if (wallItemType != -1)
                    {
                        PickItemInInventory(Main.LocalPlayer, (item) =>
                            item.type == wallItemType &&
                            TryPlaceWall(item, placePosition.X, placePosition.Y),
                            true, out _);
                        yield return null;
                    }
                }
            }
        }

        private static bool TryPlaceWall(Item item, int x, int y)
        {
            WorldGen.KillWall(x, y);
            if (Main.tile[x, y].WallType == 0)
            {
                WorldGen.PlaceWall(x, y, item.createWall);
                return true;
            }
            return false;
        }

        public static IEnumerator GenerateMultiTiles(List<TileDefinition> data, Point position, int width, int height)
        {
            for (int x = 0; x <= width; x++)
            {
                for (int y = 0; y <= height; y++)
                {
                    int index = y + x * (height + 1);
                    TileDefinition tileData = data[index];
                    int tileType = FileOperator.ParseTileType(tileData.Tile);
                    int tileItemType = GetTileItem(tileType);
                    if (tileItemType != -1)
                    {
                        var tileObjectData = TileObjectData.GetTileData(tileType, MaterialCore.ItemToPlaceStyle[tileItemType]);
                        if (tileObjectData is not null && (tileObjectData.CoordinateFullWidth > 18 || tileObjectData.CoordinateFullHeight > 18))
                        {
                            int subX = tileData.TileFrameX % tileObjectData.CoordinateFullWidth;
                            int subY = tileData.TileFrameY % tileObjectData.CoordinateFullHeight;
                            Point16 frame = new(subX / 18, subY / 18);
                            if (frame.X == tileObjectData.Origin.X && frame.Y == tileObjectData.Origin.Y)
                            {
                                var placePosition = position + new Point(x, y);
                                PickItemInInventory(Main.LocalPlayer, (item) =>
                                    item is not null && item.type == tileItemType &&
                                    TryPlaceTile(placePosition.X, placePosition.Y, item, Main.LocalPlayer, forced: true),
                                    true, out _);
                                yield return null;
                            }
                        }
                    }
                }
            }
        }

        public static IEnumerator SquareTiles(Point position, int width, int height)
        {
            for (int x = 0; x <= width; x++)
            {
                for (int y = 0; y <= height; y++)
                {
                    var placePosition = position + new Point(x, y);
                    WorldGen.SquareTileFrame(placePosition.X, position.Y);
                }
                yield return null;
            }
            if (Main.netMode is NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(Main.myPlayer, position.X - 1, position.Y - 1, width + 2, height + 2);
            }
        }
    }
}
