using ImproveGame.Interface.BannerChestUI;
using ImproveGame.Interface.GUI;

namespace ImproveGame.Interface.Common
{
    /// <summary>
    /// 用户界面
    /// </summary>
    public class UISystem : ModSystem
    {
        internal static UISystem Instance;

        #region 定义

        public AutofisherGUI AutofisherGUI;
        public static UserInterface AutofisherInterface;

        public BuffTrackerGUI BuffTrackerGUI;
        public static UserInterface BuffTrackerInterface;

        public LiquidWandGUI LiquidWandGUI;
        public static UserInterface LiquidWandInterface;

        public ArchitectureGUI ArchitectureGUI;
        public static UserInterface ArchitectureInterface;

        public BrustGUI BrustGUI;
        public static UserInterface BrustInterface;

        public BigBagGUI BigBagGUI;
        public static UserInterface BigBagInterface;

        public SpaceWandGUI SpaceWandGUI;
        public static UserInterface SpaceWandInterface;

        public PaintWandGUI PaintWandGUI;
        public static UserInterface PaintWandInterface;

        public GrabBagInfoGUI GrabBagInfoGUI;
        public static UserInterface GrabBagInfoInterface;

        public LifeformAnalyzerGUI LifeformAnalyzerGUI;
        public static UserInterface LifeformAnalyzerInterface;

        public StructureGUI StructureGUI;
        public static UserInterface StructureInterface;

        public PrefixRecallGUI PrefixRecallGUI;
        public static UserInterface PrefixRecallInterface;

        public PackageGUI PackageGUI;
        public static UserInterface PackageInterface { get; set; }

        #endregion

        #region 卸载 & 加载

        public override void Unload()
        {
            Instance = null;

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
            BigBagInterface = null;

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
            PackageInterface = null;
        }

        public override void Load()
        {
            Instance = this;
            if (!Main.dedServ)
            {
                AutofisherGUI = new();
                BuffTrackerGUI = new();
                LiquidWandGUI = new();
                ArchitectureGUI = new();
                BrustGUI = new();
                BigBagInterface = new();
                SpaceWandGUI = new();
                BigBagGUI = new();
                PaintWandGUI = new();
                GrabBagInfoGUI = new();
                LifeformAnalyzerGUI = new();
                StructureGUI = new();
                PrefixRecallGUI = new();
                PackageInterface = new();
                LoadGUI(ref AutofisherGUI, out AutofisherInterface);
                LoadGUI(ref BuffTrackerGUI, out BuffTrackerInterface);
                LoadGUI(ref LiquidWandGUI, out LiquidWandInterface);
                LoadGUI(ref ArchitectureGUI, out ArchitectureInterface);
                LoadGUI(ref BrustGUI, out BrustInterface);
                LoadGUI(ref SpaceWandGUI, out SpaceWandInterface);
                LoadGUI(ref PaintWandGUI, out PaintWandInterface);
                LoadGUI(ref GrabBagInfoGUI, out GrabBagInfoInterface, () => GrabBagInfoGUI.UserInterface = GrabBagInfoInterface);
                LoadGUI(ref LifeformAnalyzerGUI, out LifeformAnalyzerInterface);
                LoadGUI(ref StructureGUI, out StructureInterface);
                LoadGUI(ref PrefixRecallGUI, out PrefixRecallInterface);
            }
        }

        public static void LoadGUI<T>(ref T uiState, out UserInterface uiInterface, Action PreActive = null) where T : UIState
        {
            uiInterface = new();
            PreActive?.Invoke();
            uiState.Activate();
            uiInterface.SetState(uiState);
        }

        #endregion

        #region 更新 & 绘制

        public override void UpdateUI(GameTime gameTime)
        {
            if (AutofisherGUI.Visible)
                AutofisherInterface?.Update(gameTime);
            if (BuffTrackerGUI.Visible)
                BuffTrackerInterface?.Update(gameTime);
            if (LiquidWandGUI.Visible)
                LiquidWandInterface?.Update(gameTime);
            if (PackageGUI.Visible)
                PackageInterface?.Update(gameTime);
            if (BigBagGUI.Visible)
                BigBagInterface?.Update(gameTime);
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

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int inventoryIndex = layers.FindIndex(layer => layer.Name == "Vanilla: Inventory");
            if (inventoryIndex != -1)
            {
                layers.Insert(inventoryIndex + 1, new LegacyGameInterfaceLayer("ImproveGame: Buff Tracker GUI", DrawBuffTrackerGUI, InterfaceScaleType.UI));
                layers.Insert(inventoryIndex + 1, new LegacyGameInterfaceLayer("ImproveGame: Liquid Wand GUI", DrawLiquidWandGUI, InterfaceScaleType.UI));
                layers.Insert(inventoryIndex + 1, new LegacyGameInterfaceLayer("ImproveGame: Architecture GUI", DrawArchitectureGUI, InterfaceScaleType.UI));
                layers.Insert(inventoryIndex + 1, new LegacyGameInterfaceLayer("ImproveGame: Autofisher GUI", DrawAutofishGUI, InterfaceScaleType.UI));

                layers.Insert(inventoryIndex + 1, new LegacyGameInterfaceLayer("ImproveGame: BigBag GUI",
                    () =>
                    {
                        if (BigBagGUI.Visible)
                            BigBagGUI.Draw(Main.spriteBatch);
                        /*SpriteBatch sb = Main.spriteBatch;
                        Vector2 size = new(100);
                        Effect effect = ModAssets.BezierCurves.Value;
                        effect.Parameters["size"].SetValue(size);
                        effect.Parameters["Pn"].SetValue(new Vector2[] { new(10), new(90, 10), new(10, 90), new(90) });
                        sb.ReBegin(effect, Main.UIScaleMatrix);
                        sb.Draw(new Texture2D(Main.graphics.GraphicsDevice, 1, 1), Main.MouseScreen - size - new Vector2(10), null, Color.White, 0, new(0), size, 0, 1f);
                        sb.ReBegin(null, Main.UIScaleMatrix);*/
                        return true;
                    }, InterfaceScaleType.UI));

                layers.Insert(inventoryIndex + 1, new LegacyGameInterfaceLayer("ImproveGame: Package GUI",
                    () =>
                    {
                        if (PackageGUI.Visible)
                            PackageGUI.Draw(Main.spriteBatch);
                        return true;
                    }, InterfaceScaleType.UI));

                layers.Insert(inventoryIndex + 1, new LegacyGameInterfaceLayer("ImproveGame: Grab Bag Info GUI",
                    () => { if (GrabBagInfoGUI.Visible) GrabBagInfoGUI.Draw(Main.spriteBatch); return true; }, InterfaceScaleType.UI));
                layers.Insert(inventoryIndex + 1, new LegacyGameInterfaceLayer("ImproveGame: Lifeform Analyzer GUI",
                    () => { if (LifeformAnalyzerGUI.Visible) LifeformAnalyzerGUI.Draw(Main.spriteBatch); return true; }, InterfaceScaleType.UI));
                layers.Insert(inventoryIndex + 1, new LegacyGameInterfaceLayer("ImproveGame: Structure GUI", () =>
                {
                    if (StructureGUI.Visible)
                        StructureGUI.Draw(Main.spriteBatch);
                    return true;
                }, InterfaceScaleType.UI));
                layers.Insert(inventoryIndex + 1, new LegacyGameInterfaceLayer("ImproveGame: Prefix Recall GUI", () =>
                {
                    if (PrefixRecallGUI.Visible) PrefixRecallGUI.Draw(Main.spriteBatch); return true;
                }, InterfaceScaleType.UI));
            }

            int wireIndex = layers.FindIndex(layer => layer.Name == "Vanilla: Wire Selection");
            if (wireIndex != -1)
            {
                layers.Insert(wireIndex + 1, new LegacyGameInterfaceLayer("ImproveGame: SpaceWand GUI", () =>
                    {
                        if (SpaceWandGUI.Visible) SpaceWandGUI.Draw(Main.spriteBatch);
                        return true;
                    }, InterfaceScaleType.UI));
                layers.Insert(wireIndex + 1, new LegacyGameInterfaceLayer("ImproveGame: Brust GUI", DrawBrustGUI, InterfaceScaleType.UI));
                layers.Insert(wireIndex + 1, new LegacyGameInterfaceLayer("ImproveGame: Paint GUI", DrawPaintGUI, InterfaceScaleType.UI));
            }
        }

        private static bool DrawAutofishGUI()
        {
            if (AutofisherGUI.Visible)
                AutofisherInterface.Draw(Main.spriteBatch, new GameTime());
            return true;
        }

        private static bool DrawBuffTrackerGUI()
        {
            if (BuffTrackerGUI.Visible)
                BuffTrackerInterface.Draw(Main.spriteBatch, new GameTime());
            return true;
        }

        private static bool DrawLiquidWandGUI()
        {
            Player player = Main.LocalPlayer;
            if (LiquidWandGUI.Visible && Main.playerInventory && player.HeldItem is not null)
            {
                LiquidWandInterface.Draw(Main.spriteBatch, new GameTime());
            }
            return true;
        }

        private static bool DrawArchitectureGUI()
        {
            Player player = Main.LocalPlayer;
            if (ArchitectureGUI.Visible && Main.playerInventory && player.HeldItem is not null)
            {
                ArchitectureInterface.Draw(Main.spriteBatch, new GameTime());
            }
            return true;
        }

        private bool DrawBrustGUI()
        {
            if (BrustGUI.Visible)
            {
                BrustInterface.Draw(Main.spriteBatch, new GameTime());
            }
            return true;
        }

        private static bool DrawPaintGUI()
        {
            if (PaintWandGUI.Visible)
            {
                PaintWandInterface.Draw(Main.spriteBatch, new GameTime());
            }
            return true;
        }

        #endregion
    }
}
