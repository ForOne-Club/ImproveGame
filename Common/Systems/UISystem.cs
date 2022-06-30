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
        public static UISystem Instance;

        public BuffTrackerGUI BuffTrackerGUI;
        public static UserInterface BuffTrackerInterface;

        public LiquidWandGUI LiquidWandGUI;
        public static UserInterface LiquidWandInterface;

        public ArchitectureGUI ArchitectureGUI;
        public static UserInterface ArchitectureInterface;

        public BrustGUI BrustGUI;
        public static UserInterface BrustInterface;

        public BigBagGUI JuVaultUIGUI;
        public static UserInterface JuBigVaultInterface;

        public override void Unload() {
            Instance = null;

            BuffTrackerGUI = null;
            BuffTrackerInterface = null;

            LiquidWandGUI = null;
            LiquidWandInterface = null;

            ArchitectureGUI = null;
            ArchitectureInterface = null;

            BrustGUI = null;
            BrustInterface = null;

            JuVaultUIGUI = null;
            JuBigVaultInterface = null;
        }

        public override void Load() {
            Instance = this;
            if (!Main.dedServ) {
                BuffTrackerGUI = new BuffTrackerGUI();
                BuffTrackerGUI.Activate();
                BuffTrackerInterface = new UserInterface();
                BuffTrackerInterface.SetState(BuffTrackerGUI);

                LiquidWandGUI = new LiquidWandGUI();
                LiquidWandGUI.Activate();
                LiquidWandInterface = new UserInterface();
                LiquidWandInterface.SetState(LiquidWandGUI);

                JuVaultUIGUI = new BigBagGUI();
                JuVaultUIGUI.Activate();
                JuBigVaultInterface = new UserInterface();
                JuBigVaultInterface.SetState(JuVaultUIGUI);

                ArchitectureGUI = new ArchitectureGUI();
                ArchitectureGUI.Activate();
                ArchitectureInterface = new UserInterface();
                ArchitectureInterface.SetState(ArchitectureGUI);

                BrustGUI = new BrustGUI();
                BrustGUI.Activate();
                BrustInterface = new UserInterface();
                BrustInterface.SetState(BrustGUI);
            }
        }

        public override void UpdateUI(GameTime gameTime) {
            if (BuffTrackerGUI.Visible) {
                BuffTrackerInterface.Update(gameTime);
            }
            if (LiquidWandGUI.Visible) {
                LiquidWandInterface.Update(gameTime);
            }
            if (BigBagGUI.Visible) {
                JuBigVaultInterface.Update(gameTime);
            }
            if (ArchitectureGUI.Visible) {
                ArchitectureInterface?.Update(gameTime);
            }
            if (BrustGUI.Visible) {
                BrustInterface?.Update(gameTime);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
            int MouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (MouseTextIndex != -1) {
                layers.Insert(MouseTextIndex, new LegacyGameInterfaceLayer(
                    "ImproveGame: Vault UI",
                    delegate {
                        if (BigBagGUI.Visible) {
                            JuVaultUIGUI.Draw(Main.spriteBatch);
                        }
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }

            int inventoryIndex = layers.FindIndex(layer => layer.Name == "Vanilla: Inventory");
            if (inventoryIndex != -1) {
                layers.Insert(inventoryIndex + 1, new LegacyGameInterfaceLayer("ImproveGame: Buff Tracker GUI", DrawBuffTrackerGUI, InterfaceScaleType.UI));
                layers.Insert(inventoryIndex + 1, new LegacyGameInterfaceLayer("ImproveGame: Liquid Wand GUI", DrawLiquidWandGUI, InterfaceScaleType.UI));
                layers.Insert(inventoryIndex + 1, new LegacyGameInterfaceLayer("ImproveGame: Architecture GUI", DrawArchitectureGUI, InterfaceScaleType.UI));
            }


            int wireIndex = layers.FindIndex(layer => layer.Name == "Vanilla: Wire Selection");
            if (wireIndex != -1)
                layers.Insert(wireIndex + 1, new LegacyGameInterfaceLayer("ImproveGame: Brust GUI", DrawBrustGUI, InterfaceScaleType.UI));
        }

        private static bool DrawBuffTrackerGUI() {
            if (BuffTrackerGUI.Visible) {
                BuffTrackerInterface.Draw(Main.spriteBatch, new GameTime());
            }
            return true;
        }

        private static bool DrawLiquidWandGUI() {
            Player player = Main.LocalPlayer;
            if (LiquidWandGUI.Visible && Main.playerInventory && player.HeldItem is not null) {
                LiquidWandInterface.Draw(Main.spriteBatch, new GameTime());
            }
            return true;
        }

        private static bool DrawArchitectureGUI() {
            Player player = Main.LocalPlayer;
            if (ArchitectureGUI.Visible && Main.playerInventory && player.HeldItem is not null) {
                ArchitectureInterface.Draw(Main.spriteBatch, new GameTime());
            }
            return true;
        }

        private static bool DrawBrustGUI() {
            Player player = Main.LocalPlayer;
            if (BrustGUI.Visible && player.HeldItem is not null) {
                if (player.HeldItem.ModItem is null || player.HeldItem.ModItem is not MagickWand) {
                    Instance.BrustGUI.Close();
                    return true;
                }
                BrustInterface.Draw(Main.spriteBatch, new GameTime());
            }
            return true;
        }
    }
}
