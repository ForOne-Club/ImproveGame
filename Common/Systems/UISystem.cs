﻿using ImproveGame.Common.Animations;
using ImproveGame.Content.Items;
using ImproveGame.Interface.GUI;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ImproveGame.Common.Systems
{
    /// <summary>
    /// 用户界面
    /// </summary>
    public class UISystem : ModSystem
    {
        internal static UISystem Instance;

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

        public StructureGUI StructureGUI;
        public static UserInterface StructureInterface;

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

            StructureGUI = null;
            StructureInterface = null;
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
                SpaceWandGUI = new();
                BigBagGUI = new();
                PaintWandGUI = new();
                GrabBagInfoGUI = new();
                StructureGUI = new();
                LoadGUI(ref AutofisherGUI, out AutofisherInterface);
                LoadGUI(ref BuffTrackerGUI, out BuffTrackerInterface);
                LoadGUI(ref LiquidWandGUI, out LiquidWandInterface);
                LoadGUI(ref ArchitectureGUI, out ArchitectureInterface);
                LoadGUI(ref BrustGUI, out BrustInterface);
                LoadGUI(ref SpaceWandGUI, out SpaceWandInterface);
                LoadGUI(ref BigBagGUI, out BigBagInterface, () => BigBagGUI.UserInterface = BigBagInterface);
                LoadGUI(ref PaintWandGUI, out PaintWandInterface);
                LoadGUI(ref GrabBagInfoGUI, out GrabBagInfoInterface, () => GrabBagInfoGUI.UserInterface = GrabBagInfoInterface);
                LoadGUI(ref StructureGUI, out StructureInterface, () => StructureGUI.UserInterface = StructureInterface);
            }
        }

        public static void LoadGUI<T>(ref T uiState, out UserInterface uiInterface, Action PreActive = null) where T : UIState
        {
            uiInterface = new();
            PreActive?.Invoke();
            uiState.Activate();
            uiInterface.SetState(uiState);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (AutofisherGUI.Visible)
            {
                AutofisherInterface?.Update(gameTime);
            }
            if (BuffTrackerGUI.Visible)
            {
                BuffTrackerInterface?.Update(gameTime);
            }
            if (LiquidWandGUI.Visible)
            {
                LiquidWandInterface?.Update(gameTime);
            }
            if (BigBagGUI.Visible)
            {
                BigBagInterface?.Update(gameTime);
            }
            if (ArchitectureGUI.Visible)
            {
                ArchitectureInterface?.Update(gameTime);
            }
            if (BrustGUI.Visible)
            {
                BrustInterface?.Update(gameTime);
            }
            if (SpaceWandGUI.Visible)
            {
                SpaceWandInterface?.Update(gameTime);
            }
            if (PaintWandGUI.Visible)
            {
                PaintWandInterface?.Update(gameTime);
            }
            if (GrabBagInfoGUI.Visible)
            {
                GrabBagInfoInterface?.Update(gameTime);
            }
            if (StructureGUI.Visible)
            {
                StructureInterface?.Update(gameTime);
            }
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
                layers.Insert(inventoryIndex + 1, new LegacyGameInterfaceLayer("ImproveGame: SpaceWand GUI",
                    () => { if (BigBagGUI.Visible) BigBagGUI.Draw(Main.spriteBatch); return true; }, InterfaceScaleType.UI));
                layers.Insert(inventoryIndex + 1, new LegacyGameInterfaceLayer("ImproveGame: Grab Bag Info GUI",
                    () => { if (GrabBagInfoGUI.Visible) GrabBagInfoGUI.Draw(Main.spriteBatch); return true; }, InterfaceScaleType.UI));
            }

            int wireIndex = layers.FindIndex(layer => layer.Name == "Vanilla: Wire Selection");
            if (wireIndex != -1)
            {
                layers.Insert(wireIndex + 1, new LegacyGameInterfaceLayer("ImproveGame: SpaceWand GUI", () =>
                    {
                        if (SpaceWandGUI.Visible)
                        {
                            SpaceWandGUI.Draw(Main.spriteBatch);
                        }
                        return true;
                    }, InterfaceScaleType.UI));
                layers.Insert(wireIndex + 1, new LegacyGameInterfaceLayer("ImproveGame: Brust GUI", DrawBrustGUI, InterfaceScaleType.UI));
                layers.Insert(wireIndex + 1, new LegacyGameInterfaceLayer("ImproveGame: Paint GUI", DrawPaintGUI, InterfaceScaleType.UI));
            }

            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(wireIndex + 1, new LegacyGameInterfaceLayer("ImproveGame: Structure GUI", () =>
                {
                    if (StructureGUI.Visible)
                    {
                        StructureGUI.Draw(Main.spriteBatch);
                    }
                    return true;
                }, InterfaceScaleType.UI));
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
    }
}
