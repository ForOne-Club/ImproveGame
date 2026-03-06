using ImproveGame.Content.Functions.PortableBuff;
using ImproveGame.Packets;
using SilkyUIFramework;
using SilkyUIFramework.Attributes;
using SilkyUIFramework.Elements;
using SilkyUIFramework.Extensions;
using Terraria.ModLoader.UI;

namespace ImproveGame.IndependentModules.InfiniteBuff.UserInterface;

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
    public SUIScrollContainer ScrollContainer { get; private set; }

    /// <summary>
    /// 需要参与背景模糊的主要子视图。
    /// </summary>
    public override IEnumerable<UIView> BlurElements => [MainContainer, SliderContainer];

    protected override void OnInitialize()
    {
        InitializeComponent();

        MainContainer.BorderColor = SUIColor.Border;
        MainContainer.BackgroundColor = SUIColor.Background * 0.75f;

        SliderContainer.BorderColor = SUIColor.Border;
        SliderContainer.BackgroundColor = SUIColor.Background * 0.75f;

        ScrollContainer = ScrollView.Container;

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
        if (Main.LocalPlayer.TryGetModPlayer<BattlerModPlayer>(out var battler))
        {
            Slider.Value = battler.SpawnRateSliderValue;
            battler.SpawnRateSliderValueChanged += (sender, value) => Slider.Value = value;
        }

        // 拖动滑块时同步刷新刷怪倍率
        Slider.Drag += (_, value) =>
        {
            SpawnRateSlider.Get(Main.myPlayer, value).Send(runLocally: true);
            Main.NewText($"{value}");
            Console.WriteLine(value);
        };

        // 生成 0~1 的刻度标签 (含首尾)
        RefreshScaleMarks(5);
    }

    public override bool ContainsPoint(Vector2 point)
    {
        if (SliderContainer is null) return base.ContainsPoint(point);

        return SliderContainer.Bounds.Contains(point) || base.ContainsPoint(point);
    }

    /// <summary>
    /// 根据给定数量重建滑块下方刻度标签。
    /// </summary>
    /// <param name="quantity">刻度数量，建议大于等于 2。</param>
    private void RefreshScaleMarks(int quantity)
    {
        if (!Main.LocalPlayer.TryGetModPlayer<BattlerModPlayer>(out var battler)) return;

        MarkerContainer.RemoveAllChildren();

        var half = quantity / 2.0f;

        for (int i = 0; i < quantity; i++)
        {
            var progress = i / (quantity - 1f);
            var item = new SUIProgressMarker()
            {
                Progress = progress
            }.Join(MarkerContainer);

            item.LeftMouseDown += (_, _) => battler.SpawnRateSliderValue = progress;
        }
    }

    /// <summary>
    /// 每帧更新：显示滑块提示并刷新 Buff 列表展示。
    /// </summary>
    protected override void UpdateStatus(GameTime gameTime)
    {
        base.UpdateStatus(gameTime);

        UpdateBuffsContainer();

        // 拖动条悬浮提示
        if (Slider.Thumb.IsMouseHovering || Slider.Thumb.LeftMousePressed)
        {
            UICommon.TooltipMouseText($"{InfiniteBuffHelper.RemapSliderToSpawnRate(Slider.Value):0.##}");
        }

        // 没有激活组合时不显示控制器
        if (!Main.LocalPlayer.TryGetModPlayer<BattlerModPlayer>(out var battler)) return;
        SliderContainer.Invalid = !battler.MeetsActivationConditions(HideBuffSystem.HideFlags);
    }

    private readonly BuffTypesState _typesState = new();

    /// <summary>
    /// 按当前 BuffIds 刷新滚动容器中的 Buff 项
    /// </summary>
    private void UpdateBuffsContainer()
    {
        if (ScrollContainer is null) return;

        var filterString = FilterBox?.Text ?? string.Empty;
        _typesState.Rebuild(filterString);
        if (!_typesState.ConsumeDirty()) return;

        ScrollContainer.RemoveAllChildren();

        foreach (var type in _typesState.Types)
        {
            ScrollContainer.AddChild(ButtonPool.GetOrAdd(type, key => new SUIBuffButton() { BuffType = key }));
        }
    }

    private readonly Dictionary<int, SUIBuffButton> ButtonPool = [];
}

/// <summary>
/// 标记
/// </summary>
public class SUIProgressMarker : UITextView
{
    public SUIProgressMarker()
    {
        TextScale = 0.8f;
        TextAlign = new(0.5f);
        FitWidth = false;
        Width = new Dimension(35);
    }

    /// <summary>
    /// 代表刻度值，修改随之改变位置和文本
    /// </summary>
    public required float Progress
    {
        get; set
        {
            field = value;
            Text = $"{InfiniteBuffHelper.RemapSliderToSpawnRate(field, 1):0.##}";
            Left = new Anchor(0f, field - 0.5f, 0.5f - field);
        }
    }

    protected override void UpdateStatus(GameTime gameTime)
    {
        base.UpdateStatus(gameTime);

        Text = $"{InfiniteBuffHelper.RemapSliderToSpawnRate(Progress, 1):0.##}";
        TextBorderColor = HoverTimer.Lerp(Color.Black, SUIColor.Highlight);
    }
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

        for (int i = 0; i < HideBuffSystem.HideFlags.Length; i++)
        {
            // 仅保留已启用隐藏（可无限）效果的 Buff
            if (!HideBuffSystem.HideFlags[i]) continue;

            if (!string.IsNullOrWhiteSpace(filterString) &&
                !Lang.GetBuffName(i).Contains(filterString)) continue;

            _types.Add(i);
        }

        // 若无法读取玩家的收藏配置，则仅保留基础筛选结果。
        if (Main.LocalPlayer.TryGetModPlayer<InfiniteBuffPlayer>(out var infinitePlayer))
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
