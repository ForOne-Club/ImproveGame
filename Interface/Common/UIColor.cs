using ImproveGame.Common.Configs;

namespace ImproveGame.Interface.Common
{
    public class UIColor : ModSystem
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
            if (newThemeStyle != oldThemeStyle)
            {
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
        }

        internal static Color PanelBorder;
        internal static Color PanelBg;

        internal static Color ItemSlotBorderFav;
        internal static Color ItemSlotBorder;
        internal static Color ItemSlotBgFav;
        internal static Color ItemSlotBG;

        internal static Color ButtonBg;

        internal static Color TitleBg;
        internal static Color TitleBg2;

        internal static Color ScrollBarBorder;
        internal static Color ScrollBarBg;
        internal static Color ScrollBarInnerBg;
        internal static Color ScrollBarInnerBgHover;

        // 开关 UI 的边框
        internal static Color SwitchBorder;
        internal static Color SwitchBorderHover;
        // 开关 UI 的背景
        internal static Color SwitchBg;
        internal static Color SwitchBgHover;
        // 开关 UI 中的圆
        internal static Color SwitchRound;
        internal static Color SwitchRoundHover;

        internal static Color Cross;

        internal static Color PackgeBg;
        internal static Color PackgeBorder;
        internal static Color PackgeGridBg;
        internal static Color PackgeGridBorder;

        public static void ModifyColor1()
        {
            PanelBorder = new Color(20, 25, 60);
            PanelBg = new Color(44, 55, 105, 160);

            ItemSlotBorderFav = new Color(233, 176, 0, 200);
            ItemSlotBorder = new Color(18, 18, 38, 200);
            ItemSlotBgFav = new Color(83, 88, 151, 200);
            ItemSlotBG = new Color(63, 65, 151, 200);

            ButtonBg = new Color(54, 56, 130);

            TitleBg = new Color(35, 40, 83);
            TitleBg2 = new Color(43, 56, 101);

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

            ItemSlotBorderFav = new Color(230, 230, 230, 200);
            ItemSlotBorder = new Color(27, 50, 57, 210);
            ItemSlotBgFav = new Color(8, 25, 30, 100);
            ItemSlotBG = new Color(11, 14, 15, 127);

            ButtonBg = new Color(17, 40, 47);

            TitleBg = new Color(8, 25, 30);

            // 滚动条
            ScrollBarBg = new Color(11, 14, 15, 127);

            // 边框
            SwitchBorder = new Color(27, 50, 57, 210);
            SwitchBorderHover = new Color(230, 230, 230, 200);
            // 开关中的圆形
            SwitchRound = new Color(27, 50, 57, 210);
            SwitchRoundHover = new Color(230, 230, 230, 200);
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