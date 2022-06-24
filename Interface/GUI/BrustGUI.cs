﻿using ImproveGame.Common.Systems;
using ImproveGame.Interface.UIElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ImproveGame.Interface.GUI
{
    public class BrustGUI : UIState
    {
        public static bool Visible { get; private set; }

        private static Asset<Texture2D> fixedModeButton;
        private static Asset<Texture2D> freeModeButton;

        private static ModImageButton modeButton;
        private static ModImageButton tileButton;
        private static ModImageButton wallButton;

        public override void OnInitialize() {
            base.OnInitialize();

            fixedModeButton = ModContent.Request<Texture2D>("ImproveGame/Assets/Images/UI/Brust/FixedMode");
            freeModeButton = ModContent.Request<Texture2D>("ImproveGame/Assets/Images/UI/Brust/FreeMode");
            Asset<Texture2D> hoverImage = ModContent.Request<Texture2D>("ImproveGame/Assets/Images/UI/Brust/Hover");
            Asset<Texture2D> backgroundImage = ModContent.Request<Texture2D>("ImproveGame/Assets/Images/UI/Brust/Background");
            var inactiveColor = new Color(120, 120, 120);
            var activeColor = Color.White;

            modeButton = new ModImageButton(
                fixedModeButton,
                activeColor: activeColor, inactiveColor: inactiveColor);
            modeButton.SetHoverImage(hoverImage);
            modeButton.SetBackgroundImage(backgroundImage);
            modeButton.Width.Set(40, 0f);
            modeButton.Height.Set(40, 0f);
            modeButton.OnRightMouseDown += RightClickClose;
            modeButton.DrawColor += () => Color.White;
            modeButton.OnMouseDown += SwitchMode;
            Append(modeButton);

            tileButton = new ModImageButton(
                ModContent.Request<Texture2D>("ImproveGame/Assets/Images/UI/Brust/TileMode"),
                activeColor: activeColor, inactiveColor: inactiveColor);
            tileButton.SetHoverImage(hoverImage);
            tileButton.SetBackgroundImage(backgroundImage);
            tileButton.Width.Set(40, 0f);
            tileButton.Height.Set(40, 0f);
            tileButton.OnRightMouseDown += RightClickClose;
            tileButton.DrawColor += () => BrustWandSystem.TileMode ? Color.White : inactiveColor;
            tileButton.OnMouseDown += (UIMouseEvent _, UIElement _) => BrustWandSystem.TileMode = !BrustWandSystem.TileMode;
            Append(tileButton);

            wallButton = new ModImageButton(
                ModContent.Request<Texture2D>("ImproveGame/Assets/Images/UI/Brust/WallMode"),
                activeColor: activeColor, inactiveColor: inactiveColor);
            wallButton.SetHoverImage(hoverImage);
            wallButton.SetBackgroundImage(backgroundImage);
            wallButton.Width.Set(40, 0f);
            wallButton.Height.Set(40, 0f);
            wallButton.OnRightMouseDown += RightClickClose;
            wallButton.DrawColor += () => BrustWandSystem.WallMode ? Color.White : inactiveColor;
            wallButton.OnMouseDown += (UIMouseEvent _, UIElement _) => BrustWandSystem.WallMode = !BrustWandSystem.WallMode;
            Append(wallButton);
        }

        /// <summary>
        /// 右键点击按钮也会关闭，就跟蓝图一样
        /// </summary>
        private void RightClickClose(UIMouseEvent evt, UIElement listeningElement) {
            Close();
        }

        private void SwitchMode(UIMouseEvent evt, UIElement listeningElement) {
            BrustWandSystem.FixedMode = !BrustWandSystem.FixedMode;
            modeButton.SetImage(BrustWandSystem.FixedMode ? fixedModeButton : freeModeButton);
        }

        /// <summary>
        /// 打开GUI界面
        /// </summary>
        public static void Open() {
            modeButton.SetCenter(Main.mouseX, Main.mouseY);
            tileButton.SetCenter(Main.mouseX - 40, Main.mouseY - 40);
            wallButton.SetCenter(Main.mouseX + 30, Main.mouseY - 40);
            modeButton.SetImage(BrustWandSystem.FixedMode ? fixedModeButton : freeModeButton);
            Visible = true;
        }

        /// <summary>
        /// 关闭GUI界面
        /// </summary>
        public static void Close() {
            Visible = false;
            Main.blockInput = false;
        }
    }
}
