using ImproveGame.Common.Configs;
using ImproveGame.Interface.BannerChest;
using ImproveGame.Interface.GUI;
using ImproveGame.Interface.PlayerInfo;
using System.Reflection;

namespace ImproveGame.Interface.Common
{
    /// <summary>
    /// 用户界面
    /// </summary>
    public class UISystem : ModSystem
    {
        internal static UISystem Instance;

        #region 定义

        // CA2211
        public PlayerInfoGUI PlayerInfoGUI;
        internal static EventTrigger PlayerInfoTrigger;

        public AutofisherGUI AutofisherGUI;
        internal static UserInterface AutofisherInterface;

        public BuffTrackerGUI BuffTrackerGUI;
        internal static UserInterface BuffTrackerInterface;

        public LiquidWandGUI LiquidWandGUI;
        internal static UserInterface LiquidWandInterface;

        public ArchitectureGUI ArchitectureGUI;
        internal static UserInterface ArchitectureInterface;

        public BrustGUI BrustGUI;
        internal static UserInterface BrustInterface;

        public BigBagGUI BigBagGUI;
        internal static EventTrigger BigBagTrigger;

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

        // 药水袋 & 旗帜盒 UI
        public PackageGUI PackageGUI;
        public EventTrigger PackageTrigger;

        #endregion

        #region 卸载 & 加载

        public override void Unload()
        {
            Instance = null;

            PlayerInfoGUI = null;
            PlayerInfoTrigger = null;

            AutofisherGUI = null;
            AutofisherInterface = null;

            BuffTrackerGUI = null;
            BuffTrackerInterface = null;

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
            PackageTrigger = new EventTrigger("Inventory", 10)
            {
                CanRunFunc = () => PackageGUI.Visible
            };
            BigBagTrigger = new EventTrigger("Inventory", 0)
            {
                CanRunFunc = () => BigBagGUI.Visible
            };
            PlayerInfoTrigger = new EventTrigger("Mouse Text", 1000)
            {
                CanRunFunc = () => PlayerInfoGUI.Visible
            };
            AutofisherGUI = new AutofisherGUI();
            BuffTrackerGUI = new BuffTrackerGUI();
            LiquidWandGUI = new LiquidWandGUI();
            ArchitectureGUI = new ArchitectureGUI();
            BrustGUI = new BrustGUI();
            SpaceWandGUI = new SpaceWandGUI();
            PaintWandGUI = new PaintWandGUI();
            GrabBagInfoGUI = new GrabBagInfoGUI();
            LifeformAnalyzerGUI = new LifeformAnalyzerGUI();
            StructureGUI = new StructureGUI();
            PrefixRecallGUI = new PrefixRecallGUI();
            LoadGUI(ref AutofisherGUI, out AutofisherInterface);
            LoadGUI(ref BuffTrackerGUI, out BuffTrackerInterface);
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
            EventTrigger.UpdateUI(gameTime);

            if (AutofisherGUI.Visible)
                AutofisherInterface?.Update(gameTime);
            if (BuffTrackerGUI.Visible)
                BuffTrackerInterface?.Update(gameTime);
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
            layers.FindVanilla("Mouse Text", index =>
            {
                layers.Insert(index + 1, new LegacyGameInterfaceLayer("ImproveGame: General Inventory",
                    () => EventTrigger.DrawAllUI("Mouse Text"), InterfaceScaleType.UI));
            });

            // 诊断网络？
            layers.FindVanilla("Diagnose Net", index =>
            {
                layers.Insert(index, "Buff Tracker GUI", () =>
                {
                    if (!UIConfigs.Instance.ShowMoreData)
                        return true;

                    bool modNetShouldDraw = (bool)typeof(ModNet).GetField("ShouldDrawModNetDiagnosticsUI",
                            BindingFlags.NonPublic | BindingFlags.Static)
                        ?.GetValue(null)!;

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
                layers.Insert(index + 1, new LegacyGameInterfaceLayer("ImproveGame: General Inventory",
                    () =>
                    {
                        EventTrigger.DrawAllUI("Inventory");
                        return true;
                    }, InterfaceScaleType.UI));
                layers.Insert(index, "Buff Tracker GUI", BuffTrackerGUI, () => BuffTrackerGUI.Visible);
                layers.Insert(index, "Liquid Wand GUI", LiquidWandGUI, () => LiquidWandGUI.Visible);
                layers.Insert(index, "Architecture GUI", ArchitectureGUI, () => ArchitectureGUI.Visible);
                layers.Insert(index, "Autofisher GUI", AutofisherGUI, () => AutofisherGUI.Visible);
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