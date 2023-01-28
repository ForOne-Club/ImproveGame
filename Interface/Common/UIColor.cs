using ImproveGame.Common.Configs;

namespace ImproveGame.Interface.Common
{
    internal class UIColor : ModSystem
    {
        // 部分单词拼写较长，使用缩写
        // Background Bg
        // Favorite Fav
        public override void Load()
        {
            if (Main.dedServ)
                return;
            oldThemeStyle = 0;
            ModifyColor1();
        }

        private int oldThemeStyle;
        public override void UpdateUI(GameTime gameTime)
        {
            if (Main.dedServ)
                return;
            int newThemeStyle = UIConfigs.Instance.ThemeStyle;
            if (newThemeStyle == oldThemeStyle)
            {
                return;
            }

            oldThemeStyle = newThemeStyle;
            switch (newThemeStyle)
            {
                case 0:
                    ModifyColor1();
                    break;
                case 1:
                    ModifyColor2();
                    break;
            }
        }

        public static Color
            PanelBorder,
            PanelBg;

        public static Color
            ItemSlotBorderFav,
            ItemSlotBorder,
            ItemSlotBgFav,
            ItemSlotBg;

        public static Color ButtonBg;

        public static Color
            TitleBg,
            TitleBg2;

        public static Color
            ScrollBarBorder,
            ScrollBarBg,
            ScrollBarInnerBg,
            ScrollBarInnerBgHover;

        // 开关 UI 的边框
        public static Color
            SwitchBorder,
            SwitchBorderHover;

        // 开关 UI 的背景
        public static Color
            SwitchBg,
            SwitchBgHover;

        // 开关 UI 中的圆
        public static Color
            SwitchRound,
            SwitchRoundHover;

        public static Color Cross;

        public static Color
            PackgeBg,
            PackgeBorder,
            PackgeGridBg,
            PackgeGridBorder;

        public static void ModifyColor1()
        {
            PanelBorder = new Color(20, 25, 60);
            PanelBg = new Color(44, 55, 105, 180);

            ItemSlotBorderFav = new Color(233, 176, 0, 180);
            ItemSlotBorder = new Color(18, 18, 38, 180);
            ItemSlotBgFav = new Color(83, 88, 151, 180);
            ItemSlotBg = new Color(63, 65, 151, 180);

            ButtonBg = new Color(54, 56, 130);

            TitleBg = new Color(35, 40, 83);
            TitleBg2 = new Color(37, 46, 92);

            ScrollBarBorder = new Color(20, 25, 60);
            ScrollBarBg = new Color(44, 57, 105);
            ScrollBarInnerBg = Color.White;
            ScrollBarInnerBgHover = new Color(220, 220, 220);

            // 边框
            SwitchBorder = new Color(22, 25, 55);
            SwitchBorderHover = new Color(233, 176, 0);
            // 圆
            SwitchRound = new Color(22, 25, 55);
            SwitchRoundHover = new Color(233, 176, 0);
            // 背景色
            SwitchBg = new Color(22, 25, 55, 127);
            SwitchBgHover = new Color(72, 63, 63, 127);

            Cross = new Color(200, 40, 40);

            PackgeBg = new Color(59, 67, 139, 160);
            PackgeBorder = new Color(22, 25, 55);
            PackgeGridBg = new Color(29, 33, 70, 160);
            PackgeGridBorder = new Color(22, 25, 55, 160);
        }

        public static void ModifyColor2()
        {
            PanelBorder = new Color(27, 50, 57, 210);
            PanelBg = new Color(11, 14, 15, 127);

            ItemSlotBorderFav = new Color(230, 230, 230, 180);
            ItemSlotBorder = new Color(27, 50, 57, 210);
            ItemSlotBgFav = new Color(8, 25, 30, 100);
            ItemSlotBg = new Color(11, 14, 15, 127);

            ButtonBg = new Color(17, 40, 47);

            TitleBg = new Color(8, 25, 30);

            // 滚动条
            ScrollBarBg = new Color(11, 14, 15, 127);

            // 边框
            SwitchBorder = new Color(27, 50, 57, 210);
            SwitchBorderHover = new Color(230, 230, 230, 180);
            // 开关中的圆形
            SwitchRound = new Color(27, 50, 57, 210);
            SwitchRoundHover = new Color(230, 230, 230, 180);
            // 背景色
            SwitchBg = new Color(11, 14, 15, 127);
            SwitchBgHover = new Color(11, 14, 15, 127);

            Cross = new Color(200, 40, 40);

            PackgeBg = new Color(11, 14, 15, 127);
            PackgeBorder = new Color(27, 50, 57, 210);
            PackgeGridBg = new Color(11, 14, 15, 127);
            PackgeGridBorder = new Color(27, 50, 57, 210);
        }
    }
}