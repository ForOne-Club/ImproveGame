using ImproveGame.Common.Configs;
using ImproveGame.UI;
using ImproveGame.UI.Autofisher;
using ImproveGame.UI.ExtremeStorage;
using ImproveGame.UI.GrabBagInfo;
using ImproveGame.UI.ItemSearcher;
using ImproveGame.UI.OpenBag;
using ImproveGame.UI.SpaceWand;
using ImproveGame.UI.WorldFeature;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.Common.Extensions;
using ImproveGame.UIFramework.SUIElements;
using System.Reflection;

namespace ImproveGame.UIFramework;

/// <summary>
/// 用户界面
/// </summary>
public class UISystem : ModSystem
{
    public static UISystem Instance { get; private set; }
    public UISystem() => Instance = this;

    /// <summary>
    /// 鼠标是否悬停于可编辑文本上
    /// </summary>
    public static bool IsHoveringOnEditableText;

    /// <summary>
    /// 当前焦点的可编辑文本，此字段便于界面上所有UI与一个可编辑文本的交互
    /// </summary>
    public static SUIEditableText FocusedEditableText;

    #region 定义

    // 检测主题是否变化，变化了就重设UI，Config的OnChanged无法确定是哪个变量发生了变化，所以放这里了
    private ThemeType _themeLastTick;
    private bool _acrylicVfxLastTick;

    public static UserInterface BrustInterface { get; private set; }
    public BurstGUI BurstGUI;

    public static UserInterface SpaceWandInterface { get; private set; }
    public SpaceWandGUI SpaceWandGUI;

    public static UserInterface PaintWandInterface { get; private set; }
    public PaintWandGUI PaintWandGUI;

    // 生命体检测仪筛选
    public LifeformAnalyzerGUI LifeformAnalyzerGUI;
    public EventTrigger LifeformAnalyzerTrigger;

    // Buff 追踪站
    public BuffTrackerGUI BuffTrackerGUI;
    public EventTrigger BuffTrackerTrigger;

    // 液体法杖
    public LiquidWandGUI LiquidWandGUI;
    public EventTrigger LiquidWandTrigger;

    // 建筑法杖
    public ArchitectureGUI ArchitectureGUI;
    public EventTrigger ArchitectureTrigger;

    // 构造法杖
    public StructureGUI StructureGUI;
    public EventTrigger StructureTrigger;

    // 侧栏 UI
    public AutofisherGUI AutofisherGUI;
    public ExtremeStorageGUI ExtremeStorageGUI;
    public PrefixRecallGUI PrefixRecallGUI;
    public SidedEventTrigger SidedEventTrigger;
    #endregion

    #region 声明
    /// <summary>
    /// 所有继承自 BaseBody 的类
    /// </summary>
    public List<Type> AutoCreateBodyTypes { get; init; } = [];
    /// <summary>
    /// 根据继承自 BaseBody 的类获取对应的 AutoCreateGUI 特性
    /// </summary>
    public Dictionary<Type, AutoCreateGUIAttribute> AutoCreateGUIAttributes { get; init; } = [];
    /// <summary>
    /// 获取 BaseBody 对应的事件触发器
    /// </summary>
    public Dictionary<Type, EventTrigger> EventTriggerInstances { get; init; } = [];
    /// <summary>
    /// BaseBody 实例
    /// </summary>
    public Dictionary<Type, BaseBody> BaseBodyInstances { get; init; } = [];
    #endregion

    #region 卸载 & 加载
    private void LoadGUIInfo()
    {
        Type[] types = Assembly.GetExecutingAssembly().GetTypes();

        foreach (Type type in types)
        {
            if (type.IsSubclassOf(typeof(BaseBody)))
            {
                var autoCreate = type.GetCustomAttribute<AutoCreateGUIAttribute>();

                if (autoCreate != null)
                {
                    AutoCreateBodyTypes.Add(type);
                    AutoCreateGUIAttributes[type] = autoCreate;
                    EventTriggerInstances[type] = new EventTrigger(autoCreate.LayerName, autoCreate.OwnName).Register();
                    EventTriggerInstances[type].Priority = autoCreate.Priority;
                    BaseBodyInstances[type] = null;
                }
            }
        }
    }

    public override void Load()
    {
        if (Main.dedServ)
        {
            return;
        }

        LoadGUIInfo();

        // UserInterface 之 EventTrigger 版
        LiquidWandTrigger = new EventTrigger("Radial Hotbars", "Liquid Wand").Register();
        ArchitectureTrigger = new EventTrigger("Radial Hotbars", "Architecture").Register();
        StructureTrigger = new EventTrigger("Radial Hotbars", "Structure").Register();
        LifeformAnalyzerTrigger = new EventTrigger("Radial Hotbars", "Lifeform Analyzer").Register();
        BuffTrackerTrigger = new EventTrigger("Radial Hotbars", "Buff Tracker GUI").Register();

        SidedEventTrigger = new SidedEventTrigger();
        SidedEventTrigger.Register();

        BurstGUI = new BurstGUI();
        SpaceWandGUI = new SpaceWandGUI();
        PaintWandGUI = new PaintWandGUI();

        LoadGUI(ref BurstGUI, out var ui); BrustInterface = ui;
        LoadGUI(ref SpaceWandGUI, out ui); SpaceWandInterface = ui;
        LoadGUI(ref PaintWandGUI, out ui); PaintWandInterface = ui;
    }

    private static void LoadGUI<T>(ref T uiState, out UserInterface ui, Action action = null) where T : UIState
    {
        ui = new UserInterface();
        action?.Invoke();
        ui.SetState(uiState);
    }

    public override void Unload()
    {
        BrustInterface = null;
        SpaceWandInterface = null;
        PaintWandInterface = null;
    }
    #endregion

    #region 更新

    public override void UpdateUI(GameTime gameTime)
    {
        if (Main.ingameOptionsWindow || Main.InGameUI.IsVisible)
            return;

        IsHoveringOnEditableText = false;

        // 特殊处理
        PrefixRecallGUI?.TrackDisplayment();

        // 可以看到，它执行的是最早的。
        EventTriggerManager.UpdateUI(gameTime);

        if (BurstGUI.Visible)
            BrustInterface?.Update(gameTime);
        if (SpaceWandGUI.Visible)
            SpaceWandInterface?.Update(gameTime);
        if (PaintWandGUI.Visible)
            PaintWandInterface?.Update(gameTime);

        if (_themeLastTick != UIConfigs.Instance.ThemeType || _acrylicVfxLastTick != GlassVfxEnabled)
        {
            UIStyle.SetUIColors(UIConfigs.Instance.ThemeType);
            if (GlassVfxAvailable)
                UIStyle.AcrylicRedesign();
            UIPlayer.InitUI();
        }

        _themeLastTick = UIConfigs.Instance.ThemeType;
        _acrylicVfxLastTick = GlassVfxEnabled;
    }

    #endregion

    #region 绘制

    // UserInterface 的 Draw() 里面的 Use() 好像会一直 Recalculate()。没看懂为啥。
    // 多个 UserInterface 实例都在 Draw() 的时候导致 UserInterface.ActiveInstance 变更。
    // UserInterface.ActiveInstance 每次变更都会使 Recalculate() 执行一次。
    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        // 如果 Insert 是按照向前插入的逻辑，越早插入越晚绘制，也就是越靠近顶层。
        EventTriggerManager.ModifyInterfaceLayers(layers);

        // 诊断网络？
        layers.FindVanilla("Diagnose Net", index =>
        {
            layers.Insert(index, "Buff Tracker GUI", () =>
            {
                if (!UIConfigs.Instance.ShowMoreData)
                {
                    return true;
                }

                bool modNetShouldDraw = ModNet.ShouldDrawModNetDiagnosticsUI;

                if (modNetShouldDraw)
                {
                    NetModuleLoader.NetModuleDiagnosticsUI.Draw(Main.spriteBatch, 640, 110);
                }

                return true;
            });
        });

        // 精密线控仪
        layers.FindVanilla("Wire Selection", index =>
        {
            layers.Insert(index, "SpaceWand GUI", SpaceWandGUI, () => SpaceWandGUI.Visible);
            layers.Insert(index, "Burst GUI", BurstGUI, () => BurstGUI.Visible);
            layers.Insert(index, "Paint GUI", PaintWandGUI, () => PaintWandGUI.Visible);
        });
    }

    #endregion
    
    #region 获取

    public static bool TryGetBaseBody<T>(out T body) where T : BaseBody
    {
        if (Instance?.BaseBodyInstances is null)
        {
            body = null;
            return false;
        }

        bool success = Instance.BaseBodyInstances.TryGetValue(typeof(T), out var baseBody);
        body = baseBody as T;
        return success;
    }
    
    #endregion

    public override void PreSaveAndQuit()
    {
        UIPlayer.ShouldShowUI = false;
    }
}