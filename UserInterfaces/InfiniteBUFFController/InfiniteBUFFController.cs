#if DEBUG && true

using ImproveGame.Common.ModPlayers;
using ImproveGame.Content.Functions.PortableBuff;
using ImproveGame.Packets;
using SilkyUIFramework;
using SilkyUIFramework.Attributes;
using SilkyUIFramework.Elements;
using Terraria.ModLoader.UI;

namespace ImproveGame.UserInterfaces.InfiniteBUFFController;

/// <summary>
/// 无限增益控制器
/// </summary>
[RegisterUI]
public partial class InfiniteBUFFController : BaseBody
{
    public static string GetTextValue(string key) => Language.GetTextValue($"Mods.ImproveGame.UI.InfiniteBUFFController.{key}");

    public UIElementGroup BuffsContainer { get; private set; }
    public override IEnumerable<UIView> BlurElements => [BuffContainer, SliderContainer];

    protected override void OnInitialize()
    {
        InitializeComponent();

        BorderColor = Color.Transparent;
        BackgroundColor = Color.Transparent;

        BuffContainer.Border = 2f;
        BuffContainer.BorderColor = SUIColor.Border;
        BuffContainer.BackgroundColor = SUIColor.Background * 0.75f;

        SliderContainer.Border = 2f;
        SliderContainer.BorderColor = SUIColor.Border;
        SliderContainer.BackgroundColor = SUIColor.Background * 0.75f;

        BuffsContainer = ScrollView.Container;

        Header.ControlTarget = this;

        Title.Text = GetTextValue("DisplayName");
        Title.UseDeathText();

        var searchCancel = Main.Assets.Request<Texture2D>("Images/UI/SearchCancel");

        Close.Texture2D = searchCancel;
        Close.LeftMouseDown += delegate { Enabled = false; };
        Close.OnUpdateStatus += delegate
        {
            Close.ImageColor = Color.White * Close.HoverTimer.Lerp(0.5f, 1f);
        };

        FilterBox.Placeholder = GetTextValue("Placeholder");

        ClearEditText.Texture2D = searchCancel;
        ClearEditText.OnUpdateStatus += delegate
        {
            ClearEditText.ImageColor = Color.White * ClearEditText.HoverTimer.Lerp(0.5f, 1f);
        };

        ClearEditText.LeftMouseDown += delegate { FilterBox.Text = ""; };

        SliderTitle.Text = GetTextValue("EnemySpawnRate");

        // 订阅事件
        if (Main.LocalPlayer.TryGetModPlayer<BattlerPlayer>(out var battlerPlayer))
        {
            Slider.Value = battlerPlayer.SpawnRateSliderValue;
            battlerPlayer.SpawnRateSliderValueChanged += (sender, value) => Slider.Value = value;
        }

        // 发送数据
        Slider.Drag += (_, value) => SpawnRateSlider.Get(Main.myPlayer, value).Send(runLocally: true);

        UpdateScaleMarks(5);
    }

    /// <summary>
    /// 更新刻度
    /// </summary>
    /// <param name="quantity">刻度线数量</param>
    private void UpdateScaleMarks(int quantity)
    {
        if (!Main.LocalPlayer.TryGetModPlayer<BattlerPlayer>(out var battlerPlayer)) return;

        ScaleMarks.RemoveAllChildren();

        var half = quantity / 2.0f;

        for (int i = 0; i < quantity; i++)
        {
            var progress = i / ((float)quantity - 1f);
            var item = new UITextView
            {
                TextScale = 0.8f,
                TextAlign = new(0.5f),
                FitWidth = false,
                Width = new Dimension(35),
                Text = $"{progress:0.##}",

                Left = new Anchor(0f, progress - 0.5f, 0.5f - progress),
            };

            item.LeftMouseDown += delegate
            {
                battlerPlayer.SpawnRateSliderValue = progress;
            };

            item.OnUpdateStatus += delegate
            {
                item.TextBorderColor = item.HoverTimer.Lerp(Color.Black, SUIColor.Highlight);
            };

            ScaleMarks.AddChild(item);
        }
    }

    protected override void UpdateStatus(GameTime gameTime)
    {
        base.UpdateStatus(gameTime);

        if (Slider.Thumb.IsMouseHovering || Slider.Thumb.LeftMousePressed)
        {
            UICommon.TooltipMouseText($"{Slider.Value:0.##}");
        }

        UpdateBuffsList();

        if (!Main.LocalPlayer.TryGetModPlayer<BattlerPlayer>(out _)) return;
    }

    private readonly Dictionary<int, SUIBuffItem> _pool = [];

    /// <summary>
    /// 显示的 Buff Ids
    /// </summary>
    public static List<int> BuffIds { get; } = [];

    /// <summary>
    /// 收藏的 BuffIds
    /// </summary>
    public static List<int> StarBuffIds { get; } = [];

    /// <summary>
    /// 星标 Buff Name 们
    /// </summary>
    public static HashSet<string> StarBuffNames { get; } = [];

    /// <summary>
    /// 更新 Buff 列表 (UI)
    /// </summary>
    private void UpdateBuffsList()
    {
        if (BuffsContainer == null) return;
        BuffsContainer.RemoveAllChildren();

        UpdateBuffIds();

        foreach (var type in BuffIds)
        {
            BuffsContainer.AddChild(_pool.GetOrAdd(type, key => new SUIBuffItem(key)));
        }
    }

    /// <summary>
    /// 更新 Buff Type 列表 (数据)
    /// </summary>
    private void UpdateBuffIds()
    {
        BuffIds.Clear();

        var filterString = FilterBox?.Text ?? string.Empty;

        for (int i = 0; i < HideBuffSystem.BuffTypesShouldHide.Length; i++)
        {
            // 筛选未启用的
            if (!HideBuffSystem.BuffTypesShouldHide[i]) continue;
            // 筛选输入框过滤的
            if (!string.IsNullOrWhiteSpace(filterString) && !Lang.GetBuffName(i).Contains(filterString)) continue;

            BuffIds.Add(i);
        }
    }
}

#endif