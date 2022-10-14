using ImproveGame.Common.ModHooks;
using ImproveGame.Interface.BannerChestUI;
using ImproveGame.Interface.Common;
using System.Collections.Generic;
using Terraria.ModLoader.IO;
using Terraria.UI.Chat;

namespace ImproveGame.Content.Items
{
    public class BannerChest : ModItem, IItemOverrideLeftClick
    {
        public override bool IsLoadingEnabled(Mod mod) => Config.LoadModItems;

        public List<Item> storedBanners = new();

        // 克隆内容不克隆引用
        public override ModItem Clone(Item newEntity)
        {
            BannerChest bag = base.Clone(newEntity) as BannerChest;
            bag.storedBanners = new(storedBanners); // 创建一个新的集合，依旧会拷贝 list 内的引用，但是它本身是一个新的对象。
            return bag;
        }

        public override bool CanRightClick() => storedBanners is not null;

        public override void RightClick(Player player)
        {
            //player.QuickSpawnItem(player.GetSource_OpenItem(Type), storedBanners[^1], storedBanners[^1].stack);
            //storedBanners.RemoveAt(storedBanners.Count - 1);
            UISystem.Instance.PackageGUI.Open(storedBanners, Item.Name, PackageGUI.StorageType.Banners);
        }

        public override bool ConsumeItem(Player player) => false;

        /// <summary>
        /// 只有在这些地方才可以放旗帜进去
        /// </summary>
        private static readonly List<int> availableContexts = new() {
            ItemSlot.Context.InventoryItem,
            ItemSlot.Context.ChestItem,
            114514
        };

        public bool OverrideLeftClick(Item[] inventory, int context, int slot)
        {
            // 很多的条件
            int bannerID = ItemToBanner(Main.mouseItem);
            if (ItemSlot.ShiftInUse || ItemSlot.ControlInUse || !availableContexts.Contains(context) ||
                Main.mouseItem.IsAir || !Main.mouseItem.consumable || bannerID == -1)
            {
                return false;
            }
            PutInBannerChest(storedBanners, ref Main.mouseItem);
            if (context != 114514 && Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendData(MessageID.SyncEquipment, -1, -1, null, Main.myPlayer, slot, inventory[slot].prefix);
            }
            return true;
        }

        public static bool PutInBannerChest(List<Item> storedBanners, ref Item item)
        {
            for (int i = 0; i < storedBanners.Count; i++)
            {
                if (storedBanners[i].IsAir)
                {
                    storedBanners.RemoveAt(i);
                    i--;
                    continue;
                }
                if (storedBanners[i].type == item.type && storedBanners[i].stack < storedBanners[i].maxStack && ItemLoader.CanStack(storedBanners[i], item))
                {
                    int stackAvailable = storedBanners[i].maxStack - storedBanners[i].stack;
                    int stackAddition = Math.Min(item.stack, stackAvailable);
                    item.stack -= stackAddition;
                    storedBanners[i].stack += stackAddition;
                    SoundEngine.PlaySound(SoundID.Grab);
                    Recipe.FindRecipes();
                    if (item.stack <= 0)
                        item.TurnToAir();
                }
            }
            if (!item.IsAir && storedBanners.Count < 500)
            {
                storedBanners.Add(item.Clone());
                item.TurnToAir();
                SoundEngine.PlaySound(SoundID.Grab);
            }
            return false;
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Mod == Mod.Name && line.Name == "BannerList" && Main.SettingsEnabled_OpaqueBoxBehindTooltips)
            {
                var font = FontAssets.MouseText.Value;
                var position = new Vector2(line.X, line.Y);
                var color = line.OverrideColor ?? line.Color;
                TextSnippet[] snippets = ChatManager.ParseMessage(line.Text, color).ToArray();
                ChatManager.ConvertNormalSnippets(snippets);
                ChatManager.DrawColorCodedString(Main.spriteBatch, font, snippets, position, Color.White, 0f, Vector2.Zero, Vector2.One, out _, -1);
                return false;
            }
            return base.PreDrawTooltipLine(line, ref yOffset);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (!Config.NoPlace_BUFFTile_Banner)
            {
                tooltips.Add(new(Mod, "BannerChestUseless", GetText("Tips.BannerChestUseless"))
                {
                    OverrideColor = Color.SkyBlue
                });
            }
            if (storedBanners is not null && storedBanners.Count > 0)
            {
                if (storedBanners.Count >= 500)
                {
                    tooltips.Add(new(Mod, "BannerChestCurrent", GetText("Tips.BannerChestCurrentFull"))
                    {
                        OverrideColor = Color.LightGreen
                    });
                }
                else
                {
                    tooltips.Add(new(Mod, "BannerChestCurrent", GetTextWith("Tips.BannerChestCurrent", new { StoredCount = storedBanners.Count }))
                    {
                        OverrideColor = Color.LightGreen
                    });
                }
                string cachedText = string.Empty;
                for (int i = 0; i < storedBanners.Count; i++)
                {
                    var banner = storedBanners[i];
                    string text = $"[i/s{banner.stack}:{banner.type}]";
                    cachedText += text;
                    if ((i + 1) % 20 == 0)
                    {
                        tooltips.Add(new(Mod, "BannerList", cachedText));
                        cachedText = string.Empty;
                    }
                }
                if (!string.IsNullOrEmpty(cachedText))
                {
                    tooltips.Add(new(Mod, "BannerList", cachedText));
                }
            }
            else
            {
                tooltips.Add(new(Mod, "BannerChestNone", GetText("Tips.BannerChestNone"))
                {
                    OverrideColor = Color.SkyBlue
                });
            }
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.EyeOfCthulhuBossBag);
            Item.consumable = false;
            Item.maxStack = 1;
            Item.rare = ItemRarityID.LightRed;
            Item.expert = false;
            Item.width = 48;
            Item.height = 42;
        }

        public override void SetStaticDefaults() => SacrificeTotal = 3;

        public override void LoadData(TagCompound tag)
        {
            List<Item> list = new();
            foreach (var entry in tag.GetList<TagCompound>("storedBanners"))
            {
                if (!entry.TryGet("banner", out Item banner) || banner.IsAir)
                {
                    continue;
                }
                list.Add(banner);
            }
            storedBanners = list;
        }

        public override void SaveData(TagCompound tag)
        {
            if (storedBanners is not null && storedBanners.Count != 0)
            {
                tag["storedBanners"] = storedBanners.Select(item => new TagCompound
                {
                    ["banner"] = item
                }).ToList();
            }
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write((byte)storedBanners.Count);
            foreach (var p in storedBanners)
                ItemIO.Send(p, writer, true);
        }

        public override void NetReceive(BinaryReader reader)
        {
            byte count = reader.ReadByte();
            for (int i = 0; i < count; i++)
            {
                storedBanners.Add(ItemIO.Receive(reader, true));
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.IronBar, 12)
                .AddTile(TileID.Anvils).Register();
        }
    }
}
