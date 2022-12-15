namespace ImproveGame.Interface.Common
{
    public class UIColor
    {
        public static ModColors Default = new ModColors();
    }

    public class ModColors
    {
        public Color PanelBorder = new(18, 18, 38);
        public Color PanelBackground = new(44, 57, 105, 160);

        public Color SlotFavoritedBorder = new(233, 176, 0, 200);
        public Color SlotNoFavoritedBorder = new(18, 18, 38, 200);
        public Color SlotFavoritedBackground = new(83, 88, 151, 200);
        public Color SlotNoFavoritedBackground = new(63, 65, 151, 200);

        public Color ButtonBackground = new(54, 56, 130);

        public Color TitleBackground = new(35, 40, 83);

        public Color ScrollBarBackground = new(44, 57, 105);

        // 边框
        public Color CheckBoxBorder = new(21, 15, 56);
        public Color CheckBoxBorderHover = new(233, 176, 0, 200);
        // 圆
        public Color CheckBoxRound = new(21, 15, 56);
        public Color CheckBoxRoundHover = new(233, 176, 0, 200);
        // 背景色
        public Color CheckBoxBackground = new(44, 57, 105, 160);
        public Color CheckBoxBackgroundHover = new(34, 47, 95, 160);

        public Color Fork = new(200, 40, 40);
    }

    public class ModColors2 : ModColors
    {
        public ModColors2()
        {
            PanelBorder = new(27, 50, 57, 210);
            PanelBackground = new(11, 14, 15, 127);

            SlotFavoritedBorder = new(230, 230, 230, 200);
            SlotNoFavoritedBorder = new(27, 50, 57, 210);
            SlotFavoritedBackground = new(8, 25, 30, 100);
            SlotNoFavoritedBackground = new(11, 14, 15, 127);

            ButtonBackground = new(17, 40, 47);

            TitleBackground = new(8, 25, 30);

            // 滚动条
            ScrollBarBackground = new(11, 14, 15, 127);

            // 边框
            CheckBoxBorder = new(27, 50, 57, 210);
            CheckBoxBorderHover = new(230, 230, 230, 200);
            // 开关中的圆形
            CheckBoxRound = new(27, 50, 57, 210);
            CheckBoxRoundHover = new(230, 230, 230, 200);
            // 背景色
            CheckBoxBackground = new(11, 14, 15, 127);
            CheckBoxBackgroundHover = new(11, 14, 15, 127);

            Fork = new(200, 40, 40);
        }
    }
}
