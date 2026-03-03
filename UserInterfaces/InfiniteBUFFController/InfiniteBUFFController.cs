using ImproveGame.Common.ModPlayers;
using ImproveGame.Content.Functions.PortableBuff;
using ImproveGame.Packets;
using SilkyUIFramework;
using SilkyUIFramework.Attributes;
using SilkyUIFramework.Elements;
using Terraria.ModLoader.UI;
using static tModPorter.ProgressUpdate;

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
    public static string GetTextValue(string key) => Language.GetTextValue($"Mods.ImproveGame.UI.InfiniteBUFFController.{key}");

    /// <summary>
    /// Buff 列表滚动容器的内容区域。
    /// </summary>
    public UIElementGroup BuffsContainer { get; private set; }

    /// <summary>
    /// 需要参与背景模糊的主要子视图。
    /// </summary>
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

        // 同步滑块默认值，并监听玩家配置变化
        if (Main.LocalPlayer.TryGetModPlayer<BattlerPlayer>(out var battlerPlayer))
        {
            Slider.Value = battlerPlayer.SpawnRateSliderValue;
            battlerPlayer.SpawnRateSliderValueChanged += (sender, value) => Slider.Value = value;
        }

        // 拖动滑块时同步刷新刷怪倍率
        Slider.Drag += (_, value) => SpawnRateSlider.Get(Main.myPlayer, value).Send(runLocally: true);

        // 生成 0~1 的刻度标签 (含首尾)
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
            var progress = i / (quantity - 1f);
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
                var text = BattlerPlayer.RemapSliderToSpawnRate(progress);
                item.Text = $"{text:0.##}";
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
            var text = BattlerPlayer.RemapSliderToSpawnRate(Slider.Value);
            UICommon.TooltipMouseText($"{text:0.##}");
        }

        Rebuild();

        if (!Main.LocalPlayer.TryGetModPlayer<BattlerPlayer>(out _)) return;
    }

    private readonly BuffTypesState _typesState = new();

    /// <summary>
    /// 按当前 BuffIds 刷新滚动容器中的 Buff 项
    /// </summary>
    private void Rebuild()
    {
        if (BuffsContainer == null) return;

        _typesState.Rebuild(FilterBox?.Text ?? string.Empty);
        if (!_typesState.ConsumeDirty()) return;

        BuffsContainer.RemoveAllChildren();

        foreach (var type in _typesState.Types)
        {
            BuffsContainer.AddChild(ButtonPool.GetOrAdd(type, key => new SUIBuffButton() { BuffType = key }));
        }
    }

    private Dictionary<int, SUIBuffButton> ButtonPool { get; } = [];
}

/// <summary>
/// 维护可无限化 Buff 类型的构建状态，负责筛选、收藏优先排序与脏标记管理。
/// </summary>
public class BuffTypesState
{
    /// <summary>
    /// 当前构建得到的 Buff 类型列表。
    /// </summary>
    private readonly List<int> _types = [];

    /// <summary>
    /// 上一次构建结果的快照，用于判断列表是否变化。
    /// </summary>
    private readonly List<int> _typesCache = [];

    /// <summary>
    /// 标记当前列表是否发生变化并需要刷新 UI。
    /// </summary>
    public bool IsDirty { get; private set; }

    /// <summary>
    /// 当前可供 UI 展示的 Buff 类型序列。
    /// </summary>
    public IEnumerable<int> Types => _types;

    /// <summary>
    /// 读取并清除脏标记。
    /// </summary>
    /// <returns>
    /// 若调用前状态为脏则返回 <see langword="true"/>，并将 <see cref="IsDirty"/> 重置为 <see langword="false"/>；
    /// 否则返回 <see langword="false"/>。
    /// </returns>
    public bool ConsumeDirty()
    {
        if (IsDirty)
        {
            IsDirty = false;
            return true;
        }

        return false;
    }

    /// <summary>
    /// 根据筛选关键字重建 Buff 类型列表，并在结果变化时设置脏标记。
    /// </summary>
    /// <param name="filterString">Buff 名称筛选关键字；为空或空白时不过滤。</param>
    public void Rebuild(string filterString)
    {
        // 先重建基础列表：所有已启用“可无限化”的 Buff。
        _types.Clear();

        for (int i = 0; i < HideBuffSystem.BuffTypesShouldHide.Length; i++)
        {
            // 仅保留已启用隐藏（可无限）效果的 Buff
            if (!HideBuffSystem.BuffTypesShouldHide[i]) continue;

            if (!string.IsNullOrWhiteSpace(filterString) &&
                !Lang.GetBuffName(i).Contains(filterString)) continue;

            _types.Add(i);
        }

        // 若无法读取玩家的收藏配置，则仅保留基础筛选结果。
        if (InfiniteBuffPlayer.TryGet(Main.LocalPlayer, out var infinitePlayer))
        {
            // 收藏 Buff 置顶，其他 Buff 维持相对顺序。
            var types = infinitePlayer.Favorites.GetBuffTypes();
            var array = _types.OrderBy(x => !types.Contains(x)).ToArray();
            _types.Clear();
            _types.AddRange(array);
        }

        // 与上次快照一致则无需刷新 UI。
        if (_typesCache.SequenceEqual(_types)) return;

        _typesCache.Clear();
        _typesCache.AddRange(_types);

        IsDirty = true;
    }
}
