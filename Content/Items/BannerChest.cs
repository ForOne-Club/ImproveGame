using ImproveGame.Common.ModHooks;
using ImproveGame.Interface.Common;
using ImproveGame.Interface.GUI.BannerChest;
using Terraria.ModLoader.IO;
using Terraria.UI.Chat;

namespace ImproveGame.Content.Items
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class BannerChest : ModItem, IItemOverrideLeftClick, IItemOverrideHover, IPackageItem
    {
        public override bool IsLoadingEnabled(Mod mod) => Config.LoadModItems.BannerChest;
        public List<Item> StoredBanners = new List<Item>();
        public bool AutoStorage { get; set; }
        public bool AutoSort { get; set; }

        // 克隆内容不克隆引用
        public override ModItem Clone(Item newEntity)
        {
            BannerChest bag = base.Clone(newEntity) as BannerChest;
            bag.StoredBanners = new List<Item>(StoredBanners);
            // 创建一个新的集合，依旧会拷贝 list 内的引用，但是它本身是一个新的对象。
            return bag;
        }

        public override bool CanRightClick() => StoredBanners is not null;

        public override void RightClick(Player player)
        {
            if (PackageGUI.Visible && PackageGUI.StorageType == StorageType.Banners)
                UISystem.Instance.PackageGUI.Close();
            else
                UISystem.Instance.PackageGUI.Open(StoredBanners, Item.Name, StorageType.Banners, this);

            //player.QuickSpawnItem(player.GetSource_OpenItem(Type), storedBanners[^1], storedBanners[^1].stack);
            //storedBanners.RemoveAt(storedBanners.Count - 1);
        }

        public override bool ConsumeItem(Player player) => false;

        /// <summary>
        /// 只有在这些地方才可以放旗帜进去
        /// </summary>
        private static readonly List<int> AvailableContexts = new()
        {
            ItemSlot.Context.InventoryItem,
            ItemSlot.Context.ChestItem,
            114514
        };

        public bool OverrideLeftClick(Item[] inventory, int context, int slot)
        {
            // 很多的条件
            int bannerID = ItemToBanner(Main.mouseItem);
            if (ItemSlot.ShiftInUse || ItemSlot.ControlInUse || !AvailableContexts.Contains(context) ||
                Main.mouseItem.IsAir || !Main.mouseItem.consumable || bannerID == -1)
            {
                return false;
            }

            PutInPackage(ref Main.mouseItem);
            if (context != 114514 && Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendData(MessageID.SyncEquipment, -1, -1, null, Main.myPlayer, slot, inventory[slot].prefix);
            }

            return true;
        }

        public void PutInPackage(ref Item item)
        {
            for (int i = 0; i < StoredBanners.Count; i++)
            {
                if (StoredBanners[i].IsAir)
                {
                    StoredBanners.RemoveAt(i--);
                    continue;
                }

                if (StoredBanners[i].type != item.type ||
                    StoredBanners[i].stack >= StoredBanners[i].maxStack ||
                    !ItemLoader.CanStack(StoredBanners[i], item))
                {
                    continue;
                }

                int stackAvailable = StoredBanners[i].maxStack - StoredBanners[i].stack;
                int stackAddition = Math.Min(item.stack, stackAvailable);
                item.stack -= stackAddition;
                StoredBanners[i].stack += stackAddition;
                SoundEngine.PlaySound(SoundID.Grab);
                if (item.stack <= 0)
                {
                    item.TurnToAir();
                }
            }

            if (!item.IsAir && StoredBanners.Count < 500)
            {
                StoredBanners.Add(item.Clone());
                item.TurnToAir();
                SoundEngine.PlaySound(SoundID.Grab);
            }

            // 依照type对物品进行排序
            if (AutoSort)
            {
                Sort();
            }
        }

        public void Sort()
        {
            StoredBanners.Sort((a, b) =>
            {
                return a.type.CompareTo(b.type) + (a.stack > b.stack ? 1 : (a.stack == b.stack ? 0 : -1)) * 10;
            });
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
                ChatManager.DrawColorCodedString(Main.spriteBatch, font, snippets, position, Color.White, 0f,
                    Vector2.Zero, Vector2.One, out _, -1);
                return false;
            }

            return base.PreDrawTooltipLine(line, ref yOffset);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            // 决定文本显示的是“开启”还是“关闭”
            if (_itemInInventory)
            {
                string text = (PackageGUI.Visible && PackageGUI.StorageType is StorageType.Banners) ?
                    GetTextWith($"Tips.MouseMiddleClose", new { ItemName = Item.Name }) :
                    GetTextWith($"Tips.MouseMiddleOpen", new { ItemName = Item.Name });
                tooltips.Add(new TooltipLine(Mod, "CreateWand", text) { OverrideColor = Color.LightGreen });
            }

            _itemInInventory = false;

            if (!Config.NoPlace_BUFFTile_Banner)
            {
                tooltips.Add(new TooltipLine(Mod, "BannerChestUseless", GetText("Tips.BannerChestUseless"))
                {
                    OverrideColor = Color.SkyBlue
                });
            }

            if (StoredBanners is not null && StoredBanners.Count > 0)
            {
                string storeText = StoredBanners.Count >= 500
                    ? GetText("Tips.BannerChestCurrentFull")
                    : GetTextWith("Tips.BannerChestCurrent", new { StoredCount = StoredBanners.Count });
                tooltips.Add(new TooltipLine(Mod, "BannerChestCurrent", storeText)
                {
                    OverrideColor = Color.LightGreen
                });

                string cachedText = string.Empty;
                for (int i = 0; i < StoredBanners.Count; i++)
                {
                    var banner = StoredBanners[i];
                    string text = $"[i/s{banner.stack}:{banner.type}]";
                    cachedText += text;
                    if ((i + 1) % 20 != 0)
                    {
                        continue;
                    }

                    tooltips.Add(new TooltipLine(Mod, "BannerList", cachedText));
                    cachedText = string.Empty;
                }

                if (!string.IsNullOrEmpty(cachedText))
                {
                    tooltips.Add(new TooltipLine(Mod, "BannerList", cachedText));
                }
            }
            else
            {
                tooltips.Add(new TooltipLine(Mod, "BannerChestNone", GetText("Tips.BannerChestNone"))
                {
                    OverrideColor = Color.SkyBlue
                });
            }
        }

        public override void SetDefaults()
        {
            Item.SetBaseValues(48, 42, ItemRarityID.Red, Item.sellPrice(0, 0, 50));
            Item.expert = false;
        }

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 3;

        public override void LoadData(TagCompound tag)
        {
            (this as IPackageItem).ILoadData(tag);
            StoredBanners = tag.Get<List<Item>>("banners");
            StoredBanners ??= new List<Item>();

            // 旧版迁移
            if (!tag.ContainsKey("storedBanners"))
                return;

            List<Item> list = new();
            foreach (var entry in tag.GetList<TagCompound>("storedBanners"))
            {
                if (!entry.TryGet("banner", out Item banner) || banner.IsAir)
                {
                    continue;
                }

                list.Add(banner);
            }

            StoredBanners = list;
        }

        public override void SaveData(TagCompound tag)
        {
            (this as IPackageItem).ISaveData(tag);
            tag["banners"] = StoredBanners;
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(StoredBanners.ToArray());
        }

        public override void NetReceive(BinaryReader reader)
        {
            StoredBanners = new List<Item>(reader.ReadItemArray());
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.IronBar, 12)
                .AddTile(TileID.Anvils).Register();
        }

        private bool _itemInInventory;

        public bool OverrideHover(Item[] inventory, int context, int slot)
        {
            if (context != ItemSlot.Context.InventoryItem)
            {
                return false;
            }

            _itemInInventory = true;

            // MouseMiddleRelease 鼠标按键松开, 但是会比 MouseMiddle 晚一帧才变 <br/>
            // 也就是说如果这一帧按下了中键 MouseMiddle 和 MouseMiddleRelease 都是 true
            if (Main.mouseMiddle && Main.mouseMiddleRelease)
            {
                RightClick(Main.LocalPlayer);
            }

            return false;
        }
    }
}