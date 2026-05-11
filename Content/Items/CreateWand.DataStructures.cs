using ImproveGame.Content.Functions.Construction;
using Terraria.DataStructures;
using Terraria.ObjectData;

namespace ImproveGame.Content.Items;

public partial class CreateWand
{
    public enum TileSort : byte
    {
        None,
        Block,
        Platform,
        Workbench,
        Table,
        Chair,
        Door,
        Chest,
        Bed,
        Bookcase,
        Bathtub,
        Candelabra,
        Candle,
        Chandelier,
        Clock,
        Dresser,
        Lamp,
        Lantern,
        Piano,
        Sink,
        Sofa,
        Toilet,
        Torch,
        Campfire
    }
    public struct TileInfo(TileSort sort, bool hasWall, bool flip)
    {
        public TileSort Sort { get; set; } = sort;
        public bool HasWall { get; set; } = hasWall;
        public bool Flip { get; set; } = flip;
        public static TileInfo FromColor(Color color)
        {
            int index = (color.R + 1) / 64 * 5 + (color.G + 1) / 64;
            if (index is > 23 or < 0) index = 0;
            TileSort sort = (TileSort)index;
            bool hasWall = color.B > 127;
            bool flip = color.A < 128;
            return new TileInfo(sort, hasWall, flip);
        }
        public readonly Color ToColor()
        {
            int index = (int)Sort;
            return new Color(index < 5 ? 0 : (index / 5 * 64 - 1), index == 0 ? 0 : (index % 5 * 64 - 1), HasWall ? 255 : 0, Flip ? 127 : 255);
        }
    }
    private record TileData(TileInfo Info, int X, int Y);
    public class BuildingData
    {
        public int Width { get; }
        public int Height { get; }
        private TileInfo[] _tileInfos;
        public IReadOnlyList<TileInfo> TileInfos => _tileInfos;

        private readonly int[] _materialConsume = new int[24];
        public IReadOnlyList<int> MaterialConsume => _materialConsume;

        private void BuildConsume()
        {
            foreach (var info in _tileInfos)
            {
                if (info.HasWall)
                    _materialConsume[^1]++;
                int index = (int)info.Sort - 1;
                if (index < 0) continue;
                _materialConsume[index]++;
            }
        }

        private BuildingData(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public static BuildingData FromDataMap(Texture2D dataMap)
        {
            int width = dataMap.Width;
            int height = dataMap.Height;
            int length = width * height;
            var data = new BuildingData(dataMap.Width, dataMap.Height);
            var infos = data._tileInfos = new TileInfo[length];
            Color[] colors = new Color[length];
            dataMap.GetData(colors);
            for (int n = 0; n < length; n++)
                infos[n] = TileInfo.FromColor(colors[n]);
            data.BuildConsume();
            return data;
        }

        public static BuildingData FromQotStructure(QoLStructure structure)
        {
            int width = structure.Width + 1;
            int height = structure.Height + 1;
            int length = width * height;
            var data = new BuildingData(width, height);
            var infos = data._tileInfos = new TileInfo[length];
            var structureDatas = structure.StructureDatas;
            var entries = structure.entries;
            for (int n = 0; n < length; n++)
            {
                TileInfo info = new();
                var definition = structureDatas[n];

                var wall = structure.ParseWallType(definition);
                if (wall != -1)
                    info.HasWall = true;

                var tile = structure.ParseTileType(definition);

                if (tile != -1)
                {
                    if (CreateWandHelper.IsTileBlock(tile))
                    {
                        info.Sort = TileSort.Block;
                    }
                    else if (CreateWandHelper.IsTilePlatform(tile))
                    {
                        info.Sort = TileSort.Platform;
                    }
                    else
                    {
                        TileSort tempsort = TileSort.None;
                        var objData = TileObjectData.GetTileData(tile, 0);
                        if (objData != null)
                        {
                            int coordX = definition.TileFrameX % objData.CoordinateFullWidth / (objData.CoordinateWidth + objData.CoordinatePadding);
                            int coordY = -1;
                            var heights = objData.CoordinateHeights;
                            var hlengh = heights.Length;
                            int frY = definition.TileFrameY % objData.CoordinateFullHeight;
                            for (int k = 0; k < hlengh; k++)
                            {
                                if (frY == 0)
                                {
                                    coordY = k;
                                    break;
                                }
                                frY -= heights[k] + objData.CoordinatePadding;
                            }
                            Point16 origin = objData.Origin;
                            Point16 coord = new(coordX, coordY);
                            switch (tile)
                            {
                                case TileID.Candelabras:
                                case TileID.Sinks:
                                    origin = new(origin.X + 1, origin.Y);
                                    break;
                                case TileID.ClosedDoor:
                                    origin = new(origin.X, origin.Y + 2);
                                    break;
                            }
                            if (coord == origin)
                            {
                                for (int k = 2; k < 24; k++)
                                {
                                    var checker = CreateWandHelper.CheckersForTile[k];
                                    if (checker != null && checker.Invoke(tile))
                                        tempsort = (TileSort)(k + 1);
                                }
                                info.Sort = tempsort;
                                if (tempsort is TileSort.None or TileSort.Bed or TileSort.Bathtub)
                                {
                                    int styleX = definition.TileFrameX / objData.CoordinateFullWidth;
                                    bool flip = styleX % 2 == 1;
                                    int styleY = definition.TileFrameY / objData.CoordinateFullHeight;
                                    int placeStyle = styleY; // 这玩意对于椅子和马桶就直接用styleY就行了，通用的计算鬼知道
                                    if (CreateWandHelper.IsTileChair(tile, placeStyle))
                                        info.Sort = TileSort.Chair;
                                    else if (CreateWandHelper.IsTileToilet(tile, placeStyle))
                                        info.Sort = TileSort.Toilet;

                                    info.Flip = flip;
                                }
                            }
                        }
                    }
                }
                infos[n % height * width + n / height] = info;
            }



            data.BuildConsume();
            return data;

        }


        public void SaveAsDatamap(string path)
        {
            Main.RunOnMainThread(() =>
            {
                using Texture2D result = new(Main.instance.GraphicsDevice, Width, Height);
                int length = Width * Height;
                Color[] colors = new Color[length];
                for (int n = 0; n < length; n++)
                    colors[n] = TileInfos[n].ToColor();
                result.SetData(colors);
                using FileStream fs = new FileStream(path, FileMode.Create);
                result.SaveAsPng(fs, Width, Height);
            });
        }
    }
    private readonly struct BuildingLoadData
    {
        public required string FilePath { get; init; }
        public required Texture2D DataMap { get; init; }
        public readonly bool IsStructureFile { get; init; }
        public bool IsDataMap => DataMap != null;
        public static BuildingLoadData FromDataMap(Texture2D dataMap) => new() { FilePath = null, DataMap = dataMap, IsStructureFile = false };
        public static BuildingLoadData FromFilePath(string filePath) => new() { FilePath = filePath, DataMap = null, IsStructureFile = false };
        public static BuildingLoadData FromStrcutre(string filePath) => new() { FilePath = filePath, DataMap = null, IsStructureFile = true };
    }
}
