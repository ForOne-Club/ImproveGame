using ImproveGame.Content.Items;
using ImproveGame.UI;
using ImproveGame.UI.ArchitectureUI;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public ArchitectureGUI ArchitectureGUI;
        public static UserInterface ArchitectureInterface;

        public BrustGUI BrustGUI;
        public static UserInterface BrustInterface;

        public static UserInterface userInterface;
        public static JuBigVault vaultUI;

        public override void Unload() {
            ArchitectureGUI = null;
            ArchitectureInterface = null;
            BrustGUI = null;
            BrustInterface = null;
            userInterface = null;
            vaultUI = null;
        }

        public override void Load() {
            if (!Main.dedServ) {
                vaultUI = new JuBigVault();
                vaultUI.Activate();
                userInterface = new UserInterface();
                userInterface.SetState(vaultUI);

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
            if (JuBigVault.Visible) {
                userInterface.Update(gameTime);
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
                        if (JuBigVault.Visible) {
                            vaultUI.Draw(Main.spriteBatch);
                        }
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }

            int inventoryIndex = layers.FindIndex(layer => layer.Name == "Vanilla: Inventory");
            if (inventoryIndex != -1)
                layers.Insert(inventoryIndex + 1, new LegacyGameInterfaceLayer("ImproveGame: Architecture GUI", DrawArchitectureGUI, InterfaceScaleType.UI));


            int wireIndex = layers.FindIndex(layer => layer.Name == "Vanilla: Wire Selection");
            if (wireIndex != -1)
                layers.Insert(wireIndex + 1, new LegacyGameInterfaceLayer("ImproveGame: Brust GUI", DrawBrustGUI, InterfaceScaleType.UI));
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
                    BrustGUI.Close();
                    return true;
                }
                BrustInterface.Draw(Main.spriteBatch, new GameTime());
            }
            return true;
        }
    }
}
