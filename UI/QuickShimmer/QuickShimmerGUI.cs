using ImproveGame.Content.Functions.AutoPiggyBank;
using ImproveGame.Core;
using ImproveGame.UI.QuickShimmer;
using ImproveGame.UIFramework;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.SUIElements;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Achievements;
using Terraria.GameInput;
using Terraria.ID;

namespace ImproveGame.UI.QuickShimmer;

[AutoCreateGUI(LayerName.Vanilla.RadialHotbars, "Quick Shimmer GUI")]
public class QuickShimmerGUI : BaseBody
{
    public static QuickShimmerGUI Instance { get; private set; }

    public QuickShimmerGUI() => Instance = this;

    public override bool IsNotSelectable => StartTimer.AnyClose;

    public override bool Enabled
    {
        get => StartTimer.Closing || _enabled;
        set => _enabled = value;
    }

    private bool _enabled;

    public override bool CanSetFocusTarget(UIElement target)
        => (target != this && MainPanel.IsMouseHovering) || MainPanel.IsLeftMousePressed;

    /// <summary>
    /// 启动关闭动画计时器
    /// </summary>
    public AnimationTimer StartTimer = new(3);

    // 主面板
    public SUIPanel MainPanel;

    // 标题面板
    private View TitlePanel;

    // 搜索到的物品和滚动条(隐藏)
    public BaseGrid LootsGrid;
    public SUIScrollBar Scrollbar;
    public UIText TipText;

    public override void OnInitialize()
    {
        void MakeSeparator()
        {
            View searchArea = new()
            {
                Height = new StyleDimension(10f, 0f),
                Width = new StyleDimension(-16f, 1f),
                HAlign = 0.5f,
                DragIgnore = true,
                RelativeMode = RelativeMode.Vertical,
                Spacing = new Vector2(0, 6)
            };
            searchArea.JoinParent(MainPanel);
            searchArea.Append(new UIHorizontalSeparator
            {
                Width = StyleDimension.FromPercent(1f),
                Color = Color.Lerp(Color.White, new Color(63, 65, 151, 255), 0.85f) * 0.9f
            });
        }

        // 主面板
        MainPanel = new SUIPanel(UIStyle.PanelBorder, UIStyle.PanelBg)
        {
            Shaded = true,
            Draggable = true,
            IsAdaptiveHeight = true
        };
        MainPanel.SetPadding(0f);
        MainPanel.SetPosPixels(410, 360)
            .SetSizePixels(404, 0)
            .JoinParent(this);

        TitlePanel = ViewHelper.CreateHead(Color.Black * 0.25f, 45f, 10f);
        TitlePanel.SetPadding(0f);
        TitlePanel.JoinParent(MainPanel);

        // 标题
        var title = new SUIText
        {
            IsLarge = true,
            UseKey = true,
            TextOrKey = "Mods.ImproveGame.UI.QuickShimmer.Title",
            TextAlign = new Vector2(0f, 0.5f),
            TextScale = 0.45f,
            Height = StyleDimension.Fill,
            Width = StyleDimension.Fill,
            DragIgnore = true,
            Left = new StyleDimension(16f, 0f)
        };
        title.JoinParent(TitlePanel);

        var cross = new SUICross
        {
            HAlign = 1f,
            Rounded = new Vector4(0f, 10f, 0f, 0f),
            CrossSize = 20f,
            CrossRounded = 4.5f * 0.85f,
            Border = 0f,
            BorderColor = Color.Transparent,
            BgColor = Color.Transparent,
        };
        cross.CrossOffset.X = 1f;
        cross.Width.Pixels = 46f;
        cross.Height.Set(0f, 1f);
        cross.OnUpdate += _ =>
        {
            cross.BgColor = cross.HoverTimer.Lerp(Color.Transparent, Color.Black * 0.25f);
        };
        cross.OnLeftMouseDown += (_, _) => Close();
        cross.JoinParent(TitlePanel);

        MakeMainSlotAndButtons();

        // 分割
        MakeSeparator();

        var itemsPanel = new View
        {
            DragIgnore = true,
            RelativeMode = RelativeMode.Vertical,
            OverflowHidden = true
        };
        itemsPanel.SetPadding(15, 0, 14, 14);
        itemsPanel.SetSize(0f, 190f, 1f, 0f);
        itemsPanel.JoinParent(MainPanel);

        // 没有物品时显示的提示，这里先Append，要用到的时候调一下Left就行
        TipText = new UIText(GetText("UI.QuickShimmer.TipText"))
        {
            Width = { Percent = 1f },
            Height = { Percent = 1f },
            TextOriginX = 0.5f,
            TextOriginY = 0.5f
        };
        itemsPanel.Append(TipText);

        LootsGrid = new BaseGrid();
        LootsGrid.SetBaseValues(-1, 8, new Vector2(4f), new Vector2(43));
        LootsGrid.JoinParent(itemsPanel);

        Scrollbar = new SUIScrollBar { HAlign = 1f };
        Scrollbar.Left.Pixels = -1;
        Scrollbar.Height.Pixels = LootsGrid.Height();
        Scrollbar.SetView(itemsPanel.GetInnerSizePixels().Y, LootsGrid.Height.Pixels);
        Scrollbar.JoinParent(this);
        RefreshGrid();
    }

    /// <summary>
    /// 放入袋子的物品框和按钮
    /// </summary>
    private void MakeMainSlotAndButtons()
    {
        var bagPanel = new View
        {
            DragIgnore = true,
            RelativeMode = RelativeMode.Vertical
        };
        bagPanel.SetPadding(6, 6, 6, 6);
        bagPanel.SetSize(0f, 56, 1f, 0f);
        bagPanel.JoinParent(MainPanel);

        var itemSlot = CreateItemSlot(20f, 6f, onItemChanged: (item, _) =>
            {
                if (Main.LocalPlayer is not null && Main.LocalPlayer.TryGetModPlayer(out ShimmerLootKeeper keeper))
                    keeper.targetItem = item;
            },
            parent: bagPanel,
            iconTextureName: "Bag",
            emptyText: () => GetText("UI.QuickShimmer.EmptyText"));
        itemSlot.OnUpdate += _ =>
        {
            if (Main.LocalPlayer is not null && Main.LocalPlayer.TryGetModPlayer(out ShimmerLootKeeper keeper))
                itemSlot.Item = keeper.targetItem;
        };

        var openButton = new SUIButton(ModAsset.Open.Value, GetText("UI.QuickShimmer.Open"))
        {
            Left = { Pixels = 80f },
            Top = { Pixels = 8f }
        };
        openButton.OnLeftMouseDown += (_, _) =>
        {
            if (CoroutineSystem.QuickShimmerRunner.Count > 0)
            {
                CoroutineSystem.QuickShimmerRunner.StopAll();
                return;
            }

            if (Main.LocalPlayer is null || !Main.LocalPlayer.TryGetModPlayer(out ShimmerLootKeeper keeper))
                return;

            // 钱币幸运直接换完
            var item = keeper.targetItem;
            int coinValue = ItemID.Sets.CoinLuckValue[item.type];
            if (coinValue > 0)
            {
                SoundEngine.PlaySound(SoundID.Item176);
                float coinLuckOld = Main.LocalPlayer.coinLuck;
                Main.LocalPlayer.AddCoinLuck(Main.LocalPlayer.Center, coinValue * item.stack);
                float coinLuckGain = Main.LocalPlayer.coinLuck - coinLuckOld;
                AddNotification(GetText("UI.QuickShimmer.CoinLuck", coinLuckGain), Color.Pink, item.type);
                item.TurnToAir();
                return;
            }

            // 否则进入正常转化
            var items = CollectHelper.GetShimmerResult(item, out int stackRequired, out int decraftingRecipeIndex);
            if (items is null)
                return;

            CoroutineSystem.QuickShimmerRunner.Run(QuickShimmerRunner(keeper, items, stackRequired,
                decraftingRecipeIndex));
        };
        openButton.OnUpdate += _ =>
        {
            openButton.Text = GetText(CoroutineSystem.QuickShimmerRunner.Count > 0
                ? "UI.QuickShimmer.Stop"
                : "UI.QuickShimmer.Open");
        };
        openButton.JoinParent(bagPanel);

        var depositButton = new SUIButton(ModAsset.Quick.Value, Lang.inter[29].Value) // 强夺全部
        {
            Left = { Pixels = openButton.Left.Pixels + openButton.Width.Pixels + 10f },
            Top = { Pixels = 8f },
            Width = { Pixels = 136f }
        };
        depositButton.OnLeftMouseDown += (_, _) =>
        {
            if (Main.LocalPlayer is null || !Main.LocalPlayer.TryGetModPlayer(out ShimmerLootKeeper keeper))
                return;

            keeper.Loots.ForEach(l => Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Loot(), l, l.stack));
            keeper.Loots.Clear();
            RefreshGrid();

            // 控制提示文本是否显示
            TipText.Left.Pixels = 0;
        };
        depositButton.JoinParent(bagPanel);
    }

    internal class EntitySource_Shimmer_QOT : IEntitySource
    {
        public string? Context { get; }
        public int Type { get; }

        public EntitySource_Shimmer_QOT(int type)
        {
            Type = type;
            Context = "context";
        }
    }

    private void GetShimmeredQOT(Item item, int consumeStep)
    {
        //SoundEngine.PlaySound(SoundID.Item176, Main.LocalPlayer.Center);
        int shimmerEquivalentType = item.GetShimmerEquivalentType();
        int decraftingRecipeIndex = ShimmerTransforms.GetDecraftingRecipeIndex(shimmerEquivalentType);
        var plr = Main.LocalPlayer;
        var source = new EntitySource_Shimmer_QOT(item.type);
        if (ItemID.Sets.CoinLuckValue[shimmerEquivalentType] > 0)
        {
            var value = consumeStep * ItemID.Sets.CoinLuckValue[shimmerEquivalentType];
            Main.LocalPlayer.AddCoinLuck(Main.LocalPlayer.Center, consumeStep);
            NetMessage.SendData(146, -1, -1, null, 1, (int)Main.LocalPlayer.Center.X, (int)Main.LocalPlayer.Center.Y,
                consumeStep);
            item.stack -= consumeStep;
            if (item.stack <= 0)
                item.TurnToAir();
        }
        else if (shimmerEquivalentType == 1326 && NPC.downedMoonlord)
        {
            plr.QuickSpawnItem(source, 5335, consumeStep);
            item.stack -= consumeStep;
            if (item.stack <= 0)
                item.TurnToAir();
        }
        else if (shimmerEquivalentType == 779 && NPC.downedMoonlord)
        {
            plr.QuickSpawnItem(source, 5134, consumeStep);
            item.stack -= consumeStep;
            if (item.stack <= 0)
                item.TurnToAir();
        }
        else if (shimmerEquivalentType == 3031 && NPC.downedMoonlord)
        {
            plr.QuickSpawnItem(source, 5364, consumeStep);
            item.stack -= consumeStep;
            if (item.stack <= 0)
                item.TurnToAir();
        }
        else if (shimmerEquivalentType == 5364 && NPC.downedMoonlord)
        {
            plr.QuickSpawnItem(source, 3031, consumeStep);
            item.stack -= consumeStep;
            if (item.stack <= 0)
                item.TurnToAir();
        }
        else if (shimmerEquivalentType == 3461)
        {
            short num5 = 3461;
            switch (Main.GetMoonPhase())
            {
                default:
                    num5 = 5406;
                    break;
                case MoonPhase.QuarterAtRight:
                    num5 = 5407;
                    break;
                case MoonPhase.HalfAtRight:
                    num5 = 5405;
                    break;
                case MoonPhase.ThreeQuartersAtRight:
                    num5 = 5404;
                    break;
                case MoonPhase.Full:
                    num5 = 5408;
                    break;
                case MoonPhase.ThreeQuartersAtLeft:
                    num5 = 5401;
                    break;
                case MoonPhase.HalfAtLeft:
                    num5 = 5403;
                    break;
                case MoonPhase.QuarterAtLeft:
                    num5 = 5402;
                    break;
            }

            plr.QuickSpawnItem(source, num5, consumeStep);
            item.stack -= consumeStep;
            if (item.stack <= 0)
                item.TurnToAir();
        }
        else if (item.createTile == 139)
        {
            plr.QuickSpawnItem(source, 576, consumeStep);
            item.stack -= consumeStep;
            if (item.stack <= 0)
                item.TurnToAir();
        }
        else if (ItemID.Sets.ShimmerTransformToItem[shimmerEquivalentType] > 0)
        {
            plr.QuickSpawnItem(source, ItemID.Sets.ShimmerTransformToItem[shimmerEquivalentType], consumeStep);
            item.stack -= consumeStep;
            if (item.stack <= 0)
                item.TurnToAir();
        }
        else if (item.type == 4986)
        {
            if (NPC.unlockedSlimeRainbowSpawn)
                return;

            NPC.unlockedSlimeRainbowSpawn = true;
            NetMessage.SendData(7);
            int num9 = NPC.NewNPC(item.GetNPCSource_FromThis(), (int)plr.Center.X + 4, (int)plr.Center.Y, 681);
            if (num9 >= 0)
            {
                NPC obj = Main.npc[num9];
                obj.velocity = plr.velocity;
                obj.netUpdate = true;
                obj.shimmerTransparency = 1f;
                NetMessage.SendData(146, -1, -1, null, 2, num9);
            }

            WorldGen.CheckAchievement_RealEstateAndTownSlimes();
            item.stack--;
            if (item.stack <= 0)
                item.TurnToAir();
        }
        else if (item.makeNPC > 0)
        {
            int num10 = 50;
            int highestNPCSlotIndexWeWillPick = 200;
            int num11 = NPC.GetAvailableAmountOfNPCsToSpawnUpToSlot(consumeStep, highestNPCSlotIndexWeWillPick);
            int cache = consumeStep;
            while (num10 > 0 && num11 > 0 && consumeStep > 0)
            {
                num10--;
                num11--;
                consumeStep--;
                int num12 = -1;
                num12 = ((NPCID.Sets.ShimmerTransformToNPC[item.makeNPC] < 0)
                    ? NPC.ReleaseNPC((int)plr.Center.X, (int)plr.Bottom.Y, item.makeNPC, item.placeStyle, Main.myPlayer)
                    : NPC.ReleaseNPC((int)plr.Center.X, (int)plr.Bottom.Y,
                        NPCID.Sets.ShimmerTransformToNPC[item.makeNPC], 0, Main.myPlayer));
                if (num12 >= 0)
                {
                    Main.npc[num12].shimmerTransparency = 1f;
                    NetMessage.SendData(146, -1, -1, null, 2, num12);
                }
            }

            cache = cache - consumeStep;
            item.stack -= cache;
            if (item.stack <= 0)
                item.TurnToAir();
        }
        else if (decraftingRecipeIndex >= 0)
        {
            int num13 = Math.Min(item.FindDecraftAmount(), consumeStep);
            Recipe recipe = Main.recipe[decraftingRecipeIndex];
            int num14 = 0;
            bool flag = recipe.requiredItem.Count > 1;
            IEnumerable<Item> enumerable = recipe.requiredItem;
            if (recipe.customShimmerResults != null)
                enumerable = recipe.customShimmerResults;

            int num15 = 0;
            foreach (Item item2 in enumerable)
            {
                if (item2.type <= 0)
                    break;

                num15++;
                int num16 = num13 * item2.stack;
                if (recipe.alchemy)
                {
                    for (int num17 = num16; num17 > 0; num17--)
                    {
                        if (Main.rand.Next(3) == 0)
                            num16--;
                    }
                }

                while (num16 > 0)
                {
                    int num18 = num16;
                    if (num18 > 9999)
                        num18 = 9999;

                    num16 -= num18;
                    int num19 = Item.NewItem(source, (int)plr.position.X, (int)plr.position.Y, plr.width, plr.height,
                        item2.type, num18);
                    Item _item = Main.item[num19];
                    _item.stack = num18;
                    _item.playerIndexTheItemIsReservedFor = Main.myPlayer;
                    NetMessage.SendData(145, -1, -1, null, num19, 1f);
                }
            }

            item.stack -= num13 * recipe.createItem.stack;
            if (item.stack <= 0)
            {
                item.stack = 0;
                item.type = 0;
            }
        }

        AchievementsHelper.NotifyProgressionEvent(27);
        if (Main.netMode == 0)
        {
            Item.ShimmerEffect(plr.Center);
        }
        else
        {
            NetMessage.SendData(146, -1, -1, null, 0, (int)plr.Center.X, (int)plr.Center.Y);
            NetMessage.SendData(145, -1, -1, null, item.whoAmI, 1f);
        }

        if (item.stack == 0)
        {
            item.makeNPC = -1;
            item.active = false;
        }
    }

    /// <summary>
    /// 协程开袋，在袋子很多的同时不卡，也有一个很好的动画效果
    /// </summary>
    private IEnumerator QuickShimmerRunner(ShimmerLootKeeper keeper, List<Item> items, int stackRequired,
        int decraftingRecipeIndex)
    {
        if (keeper is null || keeper.targetItem.IsAir)
            yield break;

        var item = keeper.targetItem;
        // 可以转化的次数
        int decraftAmount = item.stack / stackRequired;
        // 计算步长，一帧进行多少次转化
        int step = 1;
        if (decraftAmount > 100)
        {
            int counter = decraftAmount;
            while (counter > 0)
            {
                counter /= 10;
                step *= 10;
            }

            step /= 100;
        }

        // 执行Decraft的次数
        int decraftExecuted = 0;
        while (!item.IsAir && item.CanShimmer())
        {
            // 原版写法，这里仅仅是做了点小改动
            foreach (var result in items)
            {
                if (result.type <= ItemID.None)
                    break;

                var recipe = decraftingRecipeIndex >= 0 ? Main.recipe[decraftingRecipeIndex] : null;

                int resultStack = result.stack;
                if (recipe is {alchemy: true })
                {
                    for (int i = resultStack; i > 0; i--)
                    {
                        if (Main.rand.NextBool(3))
                            resultStack--;
                    }
                }

                while (resultStack > 0)
                {
                    int outputStack = resultStack;
                    if (outputStack > 9999)
                        outputStack = 9999;

                    resultStack -= outputStack;
                    keeper.AddToLoots(new Item(result.type, outputStack));
                }
            }

            item.stack -= stackRequired;
            if (item.stack <= 0) // 原版写法，我也不知道为什么不TurnToAir
            {
                item.stack = 0;
                item.type = ItemID.None;
            }
            
            RefreshGrid();
            decraftExecuted++;
            if (decraftExecuted % step == 0)
            {
                var sound = SoundID.Item176;
                sound.MaxInstances = 0; // 爽！！！！
                SoundEngine.PlaySound(sound);
                yield return 1;
            }
        }
    }

    private void RefreshGrid()
    {
        LootsGrid.RemoveAllChildren();

        if (Main.LocalPlayer is not null && Main.LocalPlayer.TryGetModPlayer(out ShimmerLootKeeper keeper))
        {
            for (var i = 0; i < keeper.Loots.Count; i++)
            {
                // 最多显示一百列，不然生成Children时太卡了
                if (i >= 8 * 100)
                {
                    var tip = new UIText(GetText("UI.QuickShimmer.NotFullyDisplayed", keeper.Loots.Count - 800))
                    {
                        Width = { Percent = 1f },
                        Height = { Pixels = 46f },
                        TextOriginX = 0.5f,
                        TextOriginY = 0.5f
                    };
                    LootsGrid.Append(tip);
                    break;
                }

                var itemSlot = new ShimmerLootItemSlot(keeper.Loots, i);
                itemSlot.JoinParent(LootsGrid);
            }

            // 控制提示文本是否显示
            if (keeper.Loots.Count is 0)
                TipText.Left.Pixels = 0;
            else
                TipText.Left.Pixels = -8888;
        }
        else
        {
            TipText.Left.Pixels = 0;
        }

        TipText.Recalculate();
        LootsGrid.CalculateAndSetSize();
        LootsGrid.CalculateAndSetChildrenPosition();
        LootsGrid.Recalculate();
        Scrollbar.SetView(LootsGrid.Parent.GetInnerSizePixels().Y, LootsGrid.Height.Pixels);
    }

    public override void ScrollWheel(UIScrollWheelEvent evt)
    {
        base.ScrollWheel(evt);

        if (!MainPanel.IsMouseHovering)
            return;
        // 下滑: -, 上滑: +
        Scrollbar.BarTopBuffer += evt.ScrollWheelValue * 0.4f;
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        if (-Scrollbar.BarTop == LootsGrid.DatumPoint.Y)
            return;

        LootsGrid.DatumPoint.Y = -Scrollbar.BarTop;
        LootsGrid.Recalculate();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        StartTimer.Update();

        if (Main.LocalPlayer is null) return;

        if (Main.LocalPlayer.TryGetModPlayer(out ShimmerLootKeeper keeper))
        {
            keeper.Loots ??= new List<Item>();
            if (keeper.Loots.RemoveAll(i => i.IsAir) > 0)
                RefreshGrid();
        }

        if (!MainPanel.IsMouseHovering)
            return;

        PlayerInput.LockVanillaMouseScroll("ImproveGame: Open Bag GUI");
        Main.LocalPlayer.mouseInterface = true;
    }

    public void Open()
    {
        SoundEngine.PlaySound(SoundID.MenuOpen);
        Enabled = true;
        StartTimer.Open();
    }

    public void Close()
    {
        SoundEngine.PlaySound(SoundID.MenuClose);
        Enabled = false;
        StartTimer.Close();
    }

    public override bool RenderTarget2DDraw => !StartTimer.Opened;
    public override float RenderTarget2DOpacity => StartTimer.Schedule;
    public override Vector2 RenderTarget2DOrigin => MainPanel.GetDimensionsCenter();
    public override Vector2 RenderTarget2DPosition => MainPanel.GetDimensionsCenter();
    public override Vector2 RenderTarget2DScale => new Vector2(0.95f + StartTimer.Lerp(0, 0.05f));
}