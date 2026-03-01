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
/// 无限增益控制器 UI，负责：
/// 1) 展示与筛选可无限化的 Buff；
/// 2) 维护星标 Buff 集合；
/// 3) 调整并同步刷怪倍率滑块。
/// </summary>
[RegisterUI]
public partial class InfiniteBUFFController : BaseBody
{
    /// <summary>
    /// 读取当前界面使用的本地化文本。
    /// </summary>
    /// <param name="key">本地化键名后缀。</param>
    /// <returns>对应的本地化字符串。</returns>
    public static string GetTextValue(string key) => Language.GetTextValue($"Mods.ImproveGame.UI.InfiniteBUFFController.{key}");

    /// <summary>
    /// Buff 列表滚动容器的内容区域。
    /// </summary>
    public UIElementGroup BuffsContainer { get; private set; }

    /// <summary>
    /// 需要参与背景模糊的主要子视图。
    /// </summary>
    public override IEnumerable<UIView> BlurElements => [BuffContainer, SliderContainer];

    /// <summary>
    /// 初始化界面元素、事件绑定与默认显示状态。
    /// </summary>
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

        // 同步滑块默认值，并监听玩家配置变化
        if (Main.LocalPlayer.TryGetModPlayer<BattlerPlayer>(out var battlerPlayer))
        {
            Slider.Value = battlerPlayer.SpawnRateSliderValue;
            battlerPlayer.SpawnRateSliderValueChanged += (sender, value) => Slider.Value = value;
        }

        // 拖动滑块时同步刷新刷怪倍率
        Slider.Drag += (_, value) => SpawnRateSlider.Get(Main.myPlayer, value).Send(runLocally: true);

        // 生成 0~1 的刻度标签（含首尾）
        RefreshScaleMarks(5);
    }

    /// <summary>
    /// 根据给定数量重建滑块下方刻度标签。
    /// </summary>
    /// <param name="quantity">刻度数量，建议大于等于 2。</param>
    private void RefreshScaleMarks(int quantity)
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
                // 点击刻度可直接跳转到对应倍率
                battlerPlayer.SpawnRateSliderValue = progress;
            };

            item.OnUpdateStatus += delegate
            {
                item.TextBorderColor = item.HoverTimer.Lerp(Color.Black, SUIColor.Highlight);
            };

            ScaleMarks.AddChild(item);
        }
    }

    /// <summary>
    /// 每帧更新：显示滑块提示并刷新 Buff 列表展示。
    /// </summary>
    protected override void UpdateStatus(GameTime gameTime)
    {
        base.UpdateStatus(gameTime);

        if (Slider.Thumb.IsMouseHovering || Slider.Thumb.LeftMousePressed)
        {
            UICommon.TooltipMouseText($"{Slider.Value:0.##}");
        }

        RefreshBuffsList();

        if (!Main.LocalPlayer.TryGetModPlayer<BattlerPlayer>(out _)) return;
    }

    /// <summary>
    /// Buff 项对象池，避免频繁创建 UI 元素。
    /// </summary>
    private readonly Dictionary<int, SUIBuffItem> _pool = [];

    /// <summary>
    /// 当前筛选后需要显示的 Buff ID 列表
    /// </summary>
    public static List<int> BuffIds { get; } = [];

    /// <summary>
    /// 玩家收藏（星标）的 Buff ID 列表
    /// </summary>
    public static List<int> StarBuffIds { get; } = [];

    /// <summary>
    /// 星标 Buff 名称集合（用于快速匹配）
    /// </summary>
    public static HashSet<string> StarBuffNames { get; } = [];

    /// <summary>
    /// 按当前 BuffIds 刷新滚动容器中的 Buff 项
    /// </summary>
    private void RefreshBuffsList()
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
    /// 按启用状态与搜索条件重建 BuffIds
    /// </summary>
    private void UpdateBuffIds()
    {
        BuffIds.Clear();

        var filterString = FilterBox?.Text ?? string.Empty;

        for (int i = 0; i < HideBuffSystem.BuffTypesShouldHide.Length; i++)
        {
            // 仅保留已启用隐藏（可无限）效果的 Buff
            if (!HideBuffSystem.BuffTypesShouldHide[i]) continue;
            // 若有搜索词，则按 Buff 名称过滤
            if (!string.IsNullOrWhiteSpace(filterString) && !Lang.GetBuffName(i).Contains(filterString)) continue;

            BuffIds.Add(i);
        }
    }
}

#endif
