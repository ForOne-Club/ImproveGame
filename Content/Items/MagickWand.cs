using ImproveGame.Entitys;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace ImproveGame.Content.Items
{
    public class MagickWand : ModItem
    {
        public override bool IsLoadingEnabled(Mod mod) => MyUtils.Config().LoadModItems;

        public override bool AltFunctionUse(Player player) => true;

        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
        }

        protected Point killSizeMax;
        protected Point extraRange;
        protected Point killSize;
        protected Point start;
        protected Point end;
        protected Rectangle TileRect => new((int)MathF.Min(start.X, end.X), (int)MathF.Min(start.Y, end.Y), (int)MathF.Abs(start.X - end.X) + 1, (int)MathF.Abs(start.Y - end.Y) + 1);
        private enum State { Fixed, Free };
        private State state;

        private void ToggleStyle()
        {
            state++;
            if (state > State.Free)
            {
                state = State.Fixed;
            }
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.rare = ItemRarityID.Lime;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.useAnimation = 18;
            Item.useTime = 18;
            Item.UseSound = SoundID.Item1;
            Item.value = Item.sellPrice(0, 1, 0, 0);

            killSizeMax = new(20, 10);
            killSize = new(5, 3);
            extraRange = new(5, 3);
        }

        public override bool CanUseItem(Player player)
        {
            _AllowKillTile = true;
            MyUtils.ItemRotation(player);
            if (player.altFunctionUse == 0)
            {
                if (state == State.Fixed)
                {
                    Item.useAnimation = (int)(18 * player.pickSpeed);
                    Item.useTime = (int)(18 * player.pickSpeed);
                    if (player.whoAmI == Main.myPlayer)
                    {
                        Rectangle rect = GetKillRect(player);
                        SoundEngine.PlaySound(SoundID.Item14, Main.MouseWorld);
                        MyUtils.ForechTile(rect, (i, j) =>
                        {
                            if (Main.tile[i, j].WallType > 0 && player.TileReplacementEnabled)
                            {
                                if (player.statMana < 1)
                                    player.QuickMana();
                                if (player.statMana >= 1)
                                {
                                    WorldGen.KillWall(i, j);
                                    if (Main.netMode == NetmodeID.MultiplayerClient)
                                        NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 2, i, j);
                                    player.statMana -= 1;
                                }
                            }
                            if (player.statMana < 2)
                                player.QuickMana();
                            if (player.statMana >= 2)
                            {
                                if (Main.tile[i, j].HasTile && MyUtils.TryKillTile(i, j, player))
                                {
                                    player.statMana -= 2;
                                }
                            }
                            MyUtils.BongBong(new Vector2(i, j) * 16f, 16, 16);
                        });
                    }
                }
                else if (state == State.Free)
                {
                    Item.useAnimation = 18;
                    Item.useTime = 18;
                    start = Main.MouseWorld.ToTileCoordinates();
                }
            }
            else if (player.altFunctionUse == 2)
            {
                Item.useAnimation = 18;
                Item.useTime = 18;
                ToggleStyle();
            }
            return base.CanUseItem(player);
        }

        private bool _AllowKillTile;
        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 0 && state == State.Free && !Main.dedServ && player.whoAmI == Main.myPlayer)
            {
                if (Main.mouseRight && _AllowKillTile)
                {
                    _AllowKillTile = false;
                }
                end = MyUtils.LimitRect(start, Main.MouseWorld.ToTileCoordinates(), killSizeMax.X, killSizeMax.Y);
                Color color;
                if (_AllowKillTile)
                    color = new(255, 0, 0);
                else
                    color = Color.GreenYellow;
                Box box = DrawSystem.boxs[Box.NewBox(start, end, color * 0.35f, color)];
                box.ShowWidth = true;
                box.ShowHeight = true;
                if (Main.mouseLeft)
                {
                    player.itemAnimation = 8;
                    MyUtils.ItemRotation(player);
                }
                else
                {
                    player.itemAnimation = 0;
                    if (_AllowKillTile)
                    {
                        Rectangle tileRect = TileRect;
                        int minI = tileRect.X;
                        int maxI = tileRect.X + tileRect.Width - 1;
                        int minJ = tileRect.Y;
                        int maxJ = tileRect.Y + tileRect.Height - 1;
                        for (int j = minJ; j <= maxJ; j++)
                        {
                            for (int i = minI; i <= maxI; i++)
                            {
                                SoundEngine.PlaySound(SoundID.Item14, Main.MouseWorld);
                                MyUtils.BongBong(new Vector2(i, j) * 16f, 16, 16);
                                if (Main.tile[i, j].WallType > 0 && player.TileReplacementEnabled)
                                {
                                    WorldGen.KillWall(i, j);
                                    if (Main.netMode == NetmodeID.MultiplayerClient)
                                        NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 2, i, j);
                                    player.statMana -= 1;
                                    if (player.statMana < 1)
                                    {
                                        player.QuickMana();
                                        if (player.statMana < 1)
                                        {
                                            goto superBreak;
                                        }
                                    }
                                }
                                if (player.statMana < 2)
                                    player.QuickMana();
                                if (player.statMana >= 2)
                                {
                                    if (Main.tile[i, j].HasTile && MyUtils.TryKillTile(i, j, player))
                                    {
                                        player.statMana -= 2;
                                    }
                                }
                            }
                        }
                    superBreak:;
                    }
                }
            }
            return base.UseItem(player);
        }

        public override void HoldItem(Player player)
        {
            if (state == State.Fixed && !Main.dedServ && Main.myPlayer == player.whoAmI)
                Box.NewBox(GetKillRect(player), Color.Red * 0.35f, Color.Red);
        }

        protected Rectangle GetKillRect(Player player)
        {
            Rectangle rect = new();
            Point playerCenter = player.Center.ToTileCoordinates();
            Point mousePosition = Main.MouseWorld.ToTileCoordinates();
            mousePosition = MyUtils.LimitRect(playerCenter, mousePosition, Player.tileRangeX + extraRange.X, Player.tileRangeY + extraRange.Y);
            rect.X = mousePosition.X - killSize.X / 2;
            rect.Y = mousePosition.Y - killSize.Y / 2;
            rect.Width = killSize.X;
            rect.Height = killSize.Y;
            return rect;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.Wood, 18)
                .AddIngredient(ItemID.JungleSpores, 6)
                .AddIngredient(ItemID.Ruby, 1)
                .AddTile(TileID.WorkBenches).Register();
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey("state"))
            {
                state = (State)tag.GetInt("state");
            }
        }

        public override void SaveData(TagCompound tag)
        {
            tag.Add("state", (int)state);
        }
    }
}
