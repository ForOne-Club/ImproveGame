using ImproveGame.Entitys;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static ImproveGame.Entitys.TileData;
using static ImproveGame.MyUtils;

namespace ImproveGame.Content.Items
{
    public class CreateWand : ModItem
    {
        public override bool IsLoadingEnabled(Mod mod) => Config().LoadModItems;
        public override bool AltFunctionUse(Player player) => true;

        private static readonly Texture2D[] jianYu = new[] { GetTexture("JianYu").Value, GetTexture("JianYu2").Value, GetTexture("JianYu3").Value };
        private static readonly Texture2D[] jianYu_PreView = new[] { GetTexture("JianYu_PreView").Value, GetTexture("JianYu_PreView2").Value, GetTexture("JianYu_PreView3").Value };
        private static readonly Color[][] colors = new[] { GetColors(jianYu[0]), GetColors(jianYu[1]), GetColors(jianYu[2]) };

        private static int Index = 0;

        private static Texture2D JianYu { get { return jianYu[Index]; } }
        private static Texture2D JianYu_PreView { get { return jianYu_PreView[Index]; } }
        private static Color[] Colors { get { return colors[Index]; } }

        // 切换样式
        private static void ToggleStyle()
        {
            Index++;
            if (Index >= jianYu.Length)
            {
                Index = 0;
            }
        }

        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.width = 42;
            Item.mana = 20;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(0, 1, 0, 0);
        }

        public override void HoldItem(Player player)
        {
            if (player.itemAnimation == 0)
                Item.mana = 40;
            if (!Main.dedServ && Main.myPlayer == player.whoAmI)
            {
                Point point = Main.MouseWorld.ToTileCoordinates() - (JianYu.Size() / 2f).ToPoint(); // 鼠标位置
                Box box = DrawSystem.boxs[Box.NewBox(new Rectangle(point.X, point.Y, JianYu.Width, JianYu.Height),
                    Color.Yellow * 0f, Color.Yellow * 0f)];
                box.PreView = JianYu_PreView;
            }
        }

        public override bool CanUseItem(Player player)
        {
            if (!Main.dedServ && Main.myPlayer == player.whoAmI)
            {
                if (player.altFunctionUse == 0)
                {
                    Item.mana = 40;
                    Point position = Main.MouseWorld.ToTileCoordinates() - (JianYu.Size() / 2f).ToPoint();

                    List<TileData> tileDatas = new();
                    SoundEngine.PlaySound(SoundID.Item14, Main.MouseWorld);

                    for (int i = 0; i < Colors.Length; i++) // 不会放置椅子和工作台
                    {
                        int x = position.X + i % JianYu.Width; // 物块在图片中的 X 坐标
                        int y = position.Y + i / JianYu.Width; // Y 坐标

                        TryKillTile(x, y, player, player.GetBestPickaxe());
                        BongBong(new Vector2(x, y) * 16f, 16, 16);

                        TileSort tileSort = Color2TileSort(Colors[i]);
                        // 墙体
                        if (tileSort != TileSort.Solid && tileSort != TileSort.Platform &&
                            tileSort != TileSort.NoWall) // 平台或实体块位置不放置
                        {
                            MyUtils.ConsumeItem(player, (item) =>
                            {
                                if (item.createWall > -1)
                                {
                                    WorldGen.KillWall(x, y);
                                    if (Main.tile[x, y].WallType == 0)
                                    {
                                        WorldGen.PlaceWall(x, y, item.createWall, true);
                                        return true;
                                    }
                                }
                                return false;
                            });
                        }

                        if (tileSort != TileSort.None)// 物块
                        {
                            if (tileSort == TileSort.Torch || tileSort == TileSort.Chair ||
                                tileSort == TileSort.WorkBenche || tileSort == TileSort.Table ||
                                tileSort == TileSort.Bed) // 火把，椅子，工作台，桌子，床
                            {
                                tileDatas.Add(new(tileSort, x, y));
                            }
                            else
                            {
                                if (!Main.tile[x, y].HasTile) // 考虑到玩家和位置重叠导致不强制放置就放不出来的问题
                                {
                                    MyUtils.ConsumeItem(player, (item) =>
                                    {
                                        if (item.createTile > -1 &&
                                        ((tileSort == TileSort.Solid && Main.tileSolid[item.createTile] && !TileID.Sets.Platforms[item.createTile]) ||
                                        (tileSort == TileSort.Platform && TileID.Sets.Platforms[item.createTile])) &&
                                        WorldGen.PlaceTile(x, y, item.createTile, true, true, player.whoAmI, item.placeStyle))
                                        {
                                            return true;
                                        }
                                        return false;
                                    });
                                }
                            }
                        }
                    }

                    for (int i = 0; i < tileDatas.Count; i++) // 火把，椅子，工作台，桌子，床
                    {
                        MyUtils.ConsumeItem(player, (item) =>
                        {
                            if (!Main.tile[tileDatas[i].x, tileDatas[i].y].HasTile && item.createTile > -1 &&
                            ((tileDatas[i].tileSort == TileSort.Torch && TileID.Sets.Torch[item.createTile]) ||
                            (tileDatas[i].tileSort == TileSort.WorkBenche && item.createTile == TileID.WorkBenches) ||
                            (tileDatas[i].tileSort == TileSort.Chair && item.createTile == TileID.Chairs) ||
                            (tileDatas[i].tileSort == TileSort.Table && item.createTile == TileID.Tables) ||
                            (tileDatas[i].tileSort == TileSort.Bed && item.createTile == TileID.Beds)) &&
                            WorldGen.PlaceTile(tileDatas[i].x, tileDatas[i].y, item.createTile, true, false, player.whoAmI, item.placeStyle))
                            {
                                if (item.createTile == TileID.Chairs)
                                {
                                    Main.tile[tileDatas[i].x, tileDatas[i].y].TileFrameX += 18;
                                    Main.tile[tileDatas[i].x, tileDatas[i].y - 1].TileFrameX += 18;
                                }
                                return true;
                            }
                            return false;
                        });
                    }
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        NetMessage.SendTileSquare(player.whoAmI, position.X, position.Y, JianYu.Width, JianYu.Height);
                }
                else if (player.altFunctionUse == 2)
                {
                    Item.mana = 0;
                    ToggleStyle();
                }
            }
            return true;
        }

        private static readonly Dictionary<TileSort, int> MaterialConsume = new()
                {
                    { TileSort.None, 0 },
                    { TileSort.Solid, 0 },
                    { TileSort.Wall, 0 },
                    { TileSort.Platform, 0 },
                    { TileSort.Torch, 0 },
                    { TileSort.Chair, 0 },
                    { TileSort.Table, 0 },
                    { TileSort.WorkBenche, 0 },
                    { TileSort.Bed, 0 },
                    { TileSort.NoWall, 0 }
                };

        // 计算消耗
        private static void CalculateConsume()
        {
            foreach (var item in MaterialConsume)
            {
                MaterialConsume[item.Key] = 0;
            }
            TileSort tileSort;
            for (int i = 0; i < Colors.Length; i++)
            {
                tileSort = Color2TileSort(Colors[i]);
                MaterialConsume[tileSort]++;
                if (tileSort != TileSort.Solid && tileSort != TileSort.Platform && tileSort != TileSort.NoWall)
                {
                    MaterialConsume[TileSort.Wall]++;
                }
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            CalculateConsume();
            tooltips.Add(new(Mod, "MaterialConsume", $"[c/ffff00:所需材料列表：]"));
            foreach (var item in MaterialConsume)
            {
                if (item.Key != TileSort.None && item.Value > 0)
                    tooltips.Add(new(Mod, $"{item.Key}", $"[c/ffff00:{GetText($"TileSort.{item.Key}")}: {MaterialConsume[item.Key]}]"));
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.Wood, 24)
                .AddIngredient(ItemID.FallenStar, 8)
                .AddIngredient(ItemID.GoldBar, 6)
                .Register();
        }

        /// <summary>
        /// 颜色对应的物块类型
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private static TileSort Color2TileSort(Color color)
        {
            if (color == Color.Red)
            {
                return TileSort.Solid; // 实体块
            }
            else if (color == Color.Black)
            {
                return TileSort.Platform; // 平台
            }
            else if (color == Color.White)
            {
                return TileSort.Torch; // 火把
            }
            else if (color == Color.Yellow)
            {
                return TileSort.Chair; // 椅子
            }
            else if (color == Pink)
            {
                return TileSort.Table; // 桌子
            }
            else if (color == Color.Blue)
            {
                return TileSort.WorkBenche; // 工作台
            }
            else if (color == ZiSe)
            {
                return TileSort.Bed; // 床
            }
            else if (color == QingSe)
            {
                return TileSort.NoWall; // 禁止放置墙体
            }
            else
            {
                return TileSort.None; // 没有任何
            }
        }

        private static readonly Color ZiSe = new(127, 0, 255);
        private static readonly Color QingSe = new(0, 255, 255);
        private static readonly Color Pink = new(255, 0, 255);
        /*private readonly JudgeItem judgeSolid = new((item) => { return item.createTile > -1 && Main.tileSolid[item.createTile]; });
        private readonly JudgeItem judgeChairs = new((item) => { return item.createTile == TileID.Chairs; });
        private readonly JudgeItem judgeWorkBenches = new((item) => { return item.createTile == TileID.WorkBenches; });
        private readonly JudgeItem judgePlatform = new((item) => { return item.createTile > -1 && TileID.Sets.Platforms[item.createTile]; });
        private readonly JudgeItem judgeTorch = new((item) => { return item.createTile > -1 && TileID.Sets.Torch[item.createTile]; });
        private readonly JudgeItem judgeWall = new((item) => { return item.createWall > -1; });*/
    }
}
