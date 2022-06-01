using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ImproveGame.Content.Items
{
    public class MagickWand : ModItem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return MyUtils.Config().LoadModItems;
        }

        public enum UseState
        {
            directly,
            choose
        }

        public override void SetDefaults()
        {
            Item.useTurn = true;
            Item.width = 28;
            Item.height = 28;
            Item.rare = ItemRarityID.Lime;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.mana = 10;
            Item.UseSound = SoundID.Item1;
            Item.value = Item.sellPrice(0, 1, 0, 0);

            KillTilesOffsetX = -2;
            KillTilesOffsetY = -1;
            KillTilesWidth = 5;
            KillTilesHeight = 3;
            RangeX = 5;
            RangeY = 4;
            OpenUI = true;
        }

        protected int KillTilesOffsetX;
        protected int KillTilesOffsetY;
        protected int KillTilesWidth;
        protected int KillTilesHeight;
        protected int RangeX;
        protected int RangeY;
        protected bool OpenUI;
        protected bool rightMouseDown = true;

        protected Rectangle GetMagiskRectangle(Player player)
        {
            Rectangle r = new Rectangle();
            // X
            if (MathF.Abs(player.Center.X - Main.MouseWorld.X) > RangeX * 16f + Player.tileRangeX * 16)
            {
                r.X = player.Center.ToTileCoordinates().X + (Main.MouseWorld.X > player.Center.X ? RangeX + Player.tileRangeX : -(RangeX + Player.tileRangeX));
            }
            else
            {
                r.X = Main.MouseWorld.ToTileCoordinates().X;
            }
            // Y
            if (MathF.Abs(player.Center.Y - Main.MouseWorld.Y) > RangeY * 16f + Player.tileRangeY * 16)
            {
                r.Y = player.Center.ToTileCoordinates().Y + (Main.MouseWorld.Y > player.Center.Y ? RangeY + Player.tileRangeY : -(RangeY + Player.tileRangeY));
            }
            else
            {
                r.Y = Main.MouseWorld.ToTileCoordinates().Y;
            }
            r.X += KillTilesOffsetX;
            r.Y += KillTilesOffsetY;
            r.Width = KillTilesWidth;
            r.Height = KillTilesHeight;
            return r;
        }
        public UseState useState;
        public bool _flag = true;

        public override void HoldItem(Player player)
        {
            if (!Main.dedServ && player.whoAmI == Main.myPlayer)
            {
                if (_flag && Main.mouseRight)
                {
                    _flag = false;
                    HoldItem_RightMouseDown(player);
                }
                else if (!_flag && !Main.mouseRight)
                {
                    _flag = true;
                    HoldItem_RightMouseUp(player);
                }
                HoldItem_Update(player);
            }
        }

        public void HoldItem_RightMouseDown(Player player)
        {

        }

        public void HoldItem_RightMouseUp(Player player) { }

        public void HoldItem_Update(Player player)
        {
            _overrideUseItem--;
            if (useState == UseState.directly)
            {
                // 开启UI显示
                TileDraw.allowDrawBorderRect = OpenUI;
                TileDraw.tileColor = Color.Red;
                TileDraw.tileRect = GetMagiskRectangle(player);
                if (Main.mouseRight && rightMouseDown)
                {
                    rightMouseDown = false;
                    OpenUI = !OpenUI;
                }
                if (!Main.mouseRight && !rightMouseDown)
                {
                    rightMouseDown = true;
                }
            }
            else if (useState == UseState.choose)
            {

            }
        }

        private float _overrideUseItem;
        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                if (useState == UseState.directly)
                {
                    if (_overrideUseItem <= 0 && Main.mouseLeft)
                    {
                        _overrideUseItem = Item.useTime * player.pickSpeed;
                        MyUtils.KillTiles(player, TileDraw.tileRect);
                    }
                }
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(75, 10)
                .AddIngredient(323, 5)
                // .AddIngredient(331, 5)
                .AddIngredient(178, 1)
                .AddRecipeGroup(RecipeGroupID.Wood, 10)
                .AddTile(18).Register();
        }
    }
}
