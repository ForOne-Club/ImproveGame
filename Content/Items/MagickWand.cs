using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
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

        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Item.useTurn = true;
            Item.width = 28;
            Item.height = 28;
            Item.rare = ItemRarityID.Lime;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = true;
            Item.useAnimation = 12;
            Item.useTime = 12;
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
        protected bool BeginDownRight = true;

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

        public override void HoldItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                // 开启UI显示
                TileDraw.MagiskTileColor = new Color(1f, 0.9f, 0.1f, 1f);
                player.GetModPlayer<Common.ModPlayers.ImprovePlayer>().MagiskKillTiles = OpenUI;
                TileDraw.MagiskTilesRec = GetMagiskRectangle(player);
                if (Main.mouseRight && BeginDownRight)
                {
                    BeginDownRight = false;
                    OpenUI = !OpenUI;
                }
                if (!Main.mouseRight && !BeginDownRight)
                {
                    BeginDownRight = true;
                }
            }
        }

        public override bool? UseItem(Player player)
        {
            if (player.itemAnimation == player.itemAnimationMax)
            {
                if (player.whoAmI == Main.myPlayer)
                {
                    MyUtils.KillTiles(player, TileDraw.MagiskTilesRec);
                }
            }
            return true;
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
