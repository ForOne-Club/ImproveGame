using ImproveGame.Common.Animations;
using ImproveGame.Common.Configs;
using ImproveGame.Interface.BannerChest;
using ImproveGame.Interface.ExtremeStorage;
using ImproveGame.Interface.GUI;
using ImproveGame.Interface.PlayerInfo;

namespace ImproveGame.Interface.Common
{
    /// <summary>
    /// 用户界面
    /// </summary>
    public class UISystem : ModSystem
    {
        internal static UISystem Instance;

        #region 定义

        public LiquidWandGUI LiquidWandGUI;
        internal static UserInterface LiquidWandInterface;

        public ArchitectureGUI ArchitectureGUI;
        internal static UserInterface ArchitectureInterface;

        public BrustGUI BrustGUI;
        internal static UserInterface BrustInterface;

        public SpaceWandGUI SpaceWandGUI;
        internal static UserInterface SpaceWandInterface;

        public PaintWandGUI PaintWandGUI;
        internal static UserInterface PaintWandInterface;

        public GrabBagInfoGUI GrabBagInfoGUI;
        internal static UserInterface GrabBagInfoInterface;

        public LifeformAnalyzerGUI LifeformAnalyzerGUI;
        internal static UserInterface LifeformAnalyzerInterface;

        public StructureGUI StructureGUI;
        internal static UserInterface StructureInterface;

        public PrefixRecallGUI PrefixRecallGUI;
        internal static UserInterface PrefixRecallInterface;

        // 玩家信息表
        public PlayerInfoGUI PlayerInfoGUI;
        public EventTrigger PlayerInfoTrigger;

        // 药水袋 & 旗帜盒 UI
        public PackageGUI PackageGUI;
        public EventTrigger PackageTrigger;

        // 大背包 UI
        public BigBagGUI BigBagGUI;
        public EventTrigger BigBagTrigger;

        // Buff 追踪站
        public BuffTrackerGUI BuffTrackerGUI;
        public EventTrigger BuffTrackerTrigger;

        // 侧栏箱子类 UI
        public AutofisherGUI AutofisherGUI;
        public ExtremeStorageGUI ExtremeStorageGUI;
        public SidedEventTrigger SidedEventTrigger;

        #endregion

        #region 卸载 & 加载

        public override void Unload()
        {
            Instance = null;

            PlayerInfoGUI = null;
            PlayerInfoTrigger = null;

            ExtremeStorageGUI = null;
            AutofisherGUI = null;
            SidedEventTrigger = null;

            BuffTrackerGUI = null;
            BuffTrackerTrigger = null;

            LiquidWandGUI = null;
            LiquidWandInterface = null;

            ArchitectureGUI = null;
            ArchitectureInterface = null;

            BrustGUI = null;
            BrustInterface = null;

            BigBagGUI = null;
            BigBagTrigger = null;

            SpaceWandGUI = null;
            SpaceWandInterface = null;

            PaintWandGUI = null;
            PaintWandInterface = null;

            GrabBagInfoGUI = null;
            GrabBagInfoInterface = null;

            LifeformAnalyzerGUI = null;
            LifeformAnalyzerInterface = null;

            StructureGUI = null;
            StructureInterface = null;

            PrefixRecallGUI = null;
            PrefixRecallInterface = null;

            PackageGUI = null;
            PackageTrigger = null;
        }

        public override void Load()
        {
            Instance = this;
            if (Main.dedServ)
            {
                return;
            }

            // UserInterface 之 EventTrigger 版
            PackageTrigger = new EventTrigger("Radial Hotbars", "Package");

            BigBagTrigger = new EventTrigger("Radial Hotbars", "BigBag");

            BuffTrackerTrigger = new EventTrigger("Radial Hotbars", "Buff Tracker GUI");

            BuffTrackerGUI = new BuffTrackerGUI();
            PlayerInfoTrigger = new EventTrigger("Radial Hotbars", "PlayerInfo");
            BuffTrackerTrigger.SetCarrier(BuffTrackerGUI);

            SidedEventTrigger = new SidedEventTrigger();

            LiquidWandGUI = new LiquidWandGUI();
            ArchitectureGUI = new ArchitectureGUI();
            BrustGUI = new BrustGUI();
            SpaceWandGUI = new SpaceWandGUI();
            PaintWandGUI = new PaintWandGUI();
            GrabBagInfoGUI = new GrabBagInfoGUI();
            LifeformAnalyzerGUI = new LifeformAnalyzerGUI();
            StructureGUI = new StructureGUI();
            PrefixRecallGUI = new PrefixRecallGUI();
            LoadGUI(ref LiquidWandGUI, out LiquidWandInterface);
            LoadGUI(ref ArchitectureGUI, out ArchitectureInterface);
            LoadGUI(ref BrustGUI, out BrustInterface);
            LoadGUI(ref SpaceWandGUI, out SpaceWandInterface);
            LoadGUI(ref PaintWandGUI, out PaintWandInterface);
            LoadGUI(ref GrabBagInfoGUI, out GrabBagInfoInterface,
                () => GrabBagInfoGUI.UserInterface = GrabBagInfoInterface);
            LoadGUI(ref LifeformAnalyzerGUI, out LifeformAnalyzerInterface);
            LoadGUI(ref StructureGUI, out StructureInterface);
            LoadGUI(ref PrefixRecallGUI, out PrefixRecallInterface);
        }

        private static void LoadGUI<T>(ref T uiState, out UserInterface uiInterface, Action PreActive = null)
            where T : UIState
        {
            uiInterface = new UserInterface();
            PreActive?.Invoke();
            // SetState() 会执行 Activate()
            // uiState.Activate();
            uiInterface.SetState(uiState);
        }

        #endregion

        #region 更新

        public override void UpdateUI(GameTime gameTime)
        {
            // 可以看到，它执行的是最早的。
            EventTrigger.UpdateUI(gameTime);

            if (LiquidWandGUI.Visible)
                LiquidWandInterface?.Update(gameTime);
            if (ArchitectureGUI.Visible)
                ArchitectureInterface?.Update(gameTime);
            if (BrustGUI.Visible)
                BrustInterface?.Update(gameTime);
            if (SpaceWandGUI.Visible)
                SpaceWandInterface?.Update(gameTime);
            if (PaintWandGUI.Visible)
                PaintWandInterface?.Update(gameTime);
            if (GrabBagInfoGUI.Visible)
                GrabBagInfoInterface?.Update(gameTime);
            if (LifeformAnalyzerGUI.Visible)
                LifeformAnalyzerInterface?.Update(gameTime);
            if (StructureGUI.Visible)
                StructureInterface?.Update(gameTime);
            // if (PrefixRecallGUI.Visible) // 有特殊操作
            PrefixRecallInterface?.Update(gameTime);
        }

        #endregion

        #region 绘制

        // UserInterface 的 Draw() 里面的 Use() 好像会一直 Recalculate()。没看懂为啥。
        // 多个 UserInterface 实例都在 Draw() 的时候导致 UserInterface.ActiveInstance 变更。
        // UserInterface.ActiveInstance 每次变更都会使 Recalculate() 执行一次。
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            // 如果 Insert 是按照向前插入的逻辑，越早插入越晚绘制，也就是越靠近顶层。
            EventTrigger.ModifyInterfaceLayers(layers);

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

            // 背包
            layers.FindVanilla("Inventory", index =>
            {
                layers.Insert(index + 1, new LegacyGameInterfaceLayer("", () =>
                {
                    SDFRectangle.TestDraw();
                    return true;
                }, InterfaceScaleType.UI));
                layers.Insert(index, "Liquid Wand GUI", LiquidWandGUI, () => LiquidWandGUI.Visible);
                layers.Insert(index, "Architecture GUI", ArchitectureGUI, () => ArchitectureGUI.Visible);
                layers.Insert(index, "Grab Bag Info GUI", GrabBagInfoGUI, () => GrabBagInfoGUI.Visible);
                layers.Insert(index, "Lifeform Analyzer GUI", LifeformAnalyzerGUI, () => LifeformAnalyzerGUI.Visible);
                layers.Insert(index, "Structure GUI", StructureGUI, () => StructureGUI.Visible);
                layers.Insert(index, "Prefix Recall GUI", PrefixRecallGUI, () => PrefixRecallGUI.Visible);
            });

            // 精密线控仪
            layers.FindVanilla("Wire Selection", index =>
            {
                layers.Insert(index, "SpaceWand GUI", SpaceWandGUI, () => SpaceWandGUI.Visible);
                layers.Insert(index, "Brust GUI", BrustGUI, () => BrustGUI.Visible);
                layers.Insert(index, "Paint GUI", PaintWandGUI, () => PaintWandGUI.Visible);
            });
        }

        #endregion

    }
}