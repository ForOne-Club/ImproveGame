using ImproveGame.Common.Configs;

namespace ImproveGame.Interface.Common
{
    internal static class UIColor
    {
        public static Color
            PanelBorder,
            PanelBorderLight,
            PanelBg,
            PanelBgLight,
            PanelBgLightHover;

        public static float
            ShadowThicknessThinner,
            ShadowThicknessThinnerer,
            ShadowThickness;

        public static Color
            ItemSlotBorderFav,
            ItemSlotBorder,
            ItemSlotBgFav,
            ItemSlotBg;

        public static float
            ItemSlotBorderSize,
            ItemSlotBorderRound;

        public static Color
            ButtonBg,
            ButtonBgHover;

        public static Color
            TitleBg,
            TitleBg2;

        public static Color
            ScrollBarBorder,
            ScrollBarBg,
            ScrollBarInnerBg,
            ScrollBarInnerBgHover;

        public static float
            ScrollBarRoundMultiplier; // 一个0-1的乘数，数值越小越方

        public static Color
            SearchBarBorder,
            SearchBarBorderSelected,
            SearchBarBg;

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

        public static float
            CrossBorderSize,
            CrossThickness;

        public static Color
            PackgeBg,
            PackgeBorder,
            PackgeGridBg,
            PackgeGridBorder;

        static UIColor()
        {
            SetUIColors(UIConfigs.Instance.ThemeType);
        }

        public static void SetUIColors(ThemeType theme)
        {
            switch (theme)
            {
                case ThemeType.Blue:
                    Blue();
                    break;
                case ThemeType.Stormdark:
                    Stormdark();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(theme), theme, null);
            }
        }

        private static void Blue()
        {
            PanelBorder = new Color(20, 25, 60);
            PanelBorderLight = new Color(89, 116, 213);
            PanelBg = new Color(44, 55, 105, 180);
            PanelBgLight = new Color(55, 69, 126);
            PanelBgLightHover = new Color(66, 85, 157);
            ShadowThicknessThinner = 18;
            ShadowThicknessThinnerer = 12;
            ShadowThickness = 26;

            ItemSlotBorderFav = new Color(233, 176, 0, 180);
            ItemSlotBorder = new Color(18, 18, 38, 180);
            ItemSlotBgFav = new Color(83, 88, 151, 180);
            ItemSlotBg = new Color(63, 65, 151, 180);
            ItemSlotBorderSize = 2f;
            ItemSlotBorderRound = 12f;

            ButtonBg = new Color(55, 55, 130);
            ButtonBgHover = new Color(66, 66, 143);

            TitleBg = new Color(35, 40, 83);
            TitleBg2 = new Color(37, 46, 92);

            ScrollBarBorder = new Color(20, 25, 60);
            ScrollBarBg = new Color(44, 57, 105);
            ScrollBarInnerBg = Color.White;
            ScrollBarInnerBgHover = new Color(220, 220, 220);
            ScrollBarRoundMultiplier = 0.5f;

            SearchBarBorder = new Color(35, 40, 83);
            SearchBarBg = new Color(35, 40, 83);
            SearchBarBorderSelected = Main.OurFavoriteColor;

            // 边框
            SwitchBorder = new Color(20, 25, 60);
            SwitchBorderHover = new Color(233, 176, 0);
            // 圆
            SwitchRound = new Color(20, 25, 60);
            SwitchRoundHover = new Color(233, 176, 0);
            // 背景色
            SwitchBg = new Color(20, 25, 60, 127);
            SwitchBgHover = new Color(72, 63, 63, 127);

            Cross = new Color(200, 40, 40);
            CrossBorderSize = 2f;
            CrossThickness = 4.6f;

            PackgeBg = new Color(59, 67, 139, 160);
            PackgeBorder = new Color(22, 25, 55);
            PackgeGridBg = new Color(29, 33, 70, 160);
            PackgeGridBorder = new Color(22, 25, 55, 160);
        }

        private static void Stormdark()
        {
            PanelBorder = new Color(41, 73, 84, 170);
            PanelBorderLight = new Color(104, 185, 212, 170);
            PanelBg = new Color(13, 16, 17, 220);
            PanelBgLight = new Color(23, 29, 28, 220);
            PanelBgLightHover = new Color(30, 38, 36, 220);
            ShadowThicknessThinner = 12;
            ShadowThicknessThinnerer = 8;
            ShadowThickness = 20;

            ItemSlotBorderFav = new Color(255, 255, 255, 180);
            ItemSlotBorder = new Color(32, 59, 69, 180);
            ItemSlotBgFav = new Color(8, 24, 31, 180);
            ItemSlotBg = new Color(13, 16, 17, 180);
            ItemSlotBorderSize = 1f;
            ItemSlotBorderRound = 6f;

            ButtonBg = new Color(13, 16, 17);
            ButtonBgHover = new Color(23, 29, 28);

            TitleBg = new Color(18, 24, 22, 200);
            TitleBg2 = new Color(7, 22, 26, 200);

            ScrollBarBorder = new Color(26, 40, 61);
            ScrollBarBg = new Color(13, 21, 39);
            ScrollBarInnerBg = new Color(198, 200, 204);
            ScrollBarInnerBgHover = new Color(231, 232, 234);
            ScrollBarRoundMultiplier = 0.3f;

            SearchBarBorder = new Color(25, 29, 60);
            SearchBarBg = new Color(8, 11, 26);
            SearchBarBorderSelected = new Color(255, 213, 0);

            // 边框
            SwitchBorder = new Color(41, 73, 84, 170);
            SwitchBorderHover = new Color(30, 53, 58);
            // 圆
            SwitchRound = new Color(21, 39, 42);
            SwitchRoundHover = new Color(255, 255, 255);
            // 背景色
            SwitchBg = new Color(41, 73, 84, 127);
            SwitchBgHover = new Color(30, 53, 58, 127);

            Cross = new Color(200, 40, 40);
            CrossBorderSize = 1f;
            CrossThickness = 3.6f;
        }
    }
}