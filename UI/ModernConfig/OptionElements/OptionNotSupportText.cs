using ImproveGame.Helpers;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.SUIElements;

namespace ImproveGame.UI.ModernConfig.OptionElements
{
    /// <summary>
    /// 单纯显示一行文字
    /// 嗯？你问我为什么不用SUIText 因为设置和文本混杂在一起的时候用那个会被刷掉，动_allOptions感觉会出更多很麻烦的东西
    /// </summary>
    internal class OptionNotSupportText : ModernConfigOption
    {
        //private static Mod _modInMemory;
        //private static Category _categoryInMemory;

        protected override void OnBind()
        {
            CheckExternalModify = false;
            var variable = VariableInfo;
            //TODO 本地化+对多级嵌套的正确支持


            var text = new SUIText
            {
                TextOrKey = Language.GetTextValue("Mods.ImproveGame.ModernConfig.NotSupportText", [ConfigHelper.GetLabel(Config, variable.Name), variable.Name, variable.Type]),
                TextAlign = new Vector2(0f),
                IsWrapped = true,
                Width = { Precent = 1f },
                TextScale = 1.1f,
                RelativeMode = RelativeMode.Vertical
            };
            text.JoinParent(this);
            text.RecalculateText();
            text.SetInnerPixels(new Vector2(0f, text.TextSize.Y));
            text.OnRecalculateText += delegate
            {
                Height.Set(text.TextSize.Y + 60, 0);
                text.SetInnerPixels(new Vector2(0f, text.TextSize.Y));
                Parent?.Recalculate();
            };
        }

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            base.LeftMouseDown(evt);
            SoundEngine.PlaySound(SoundID.MenuTick);
            _modInMemory = ModernConfigUI.Instance.currentMod;
            _categoryInMemory = ConfigOptionsPanel.CurrentCategory;
            Config.Open(() =>
            {
                ModernConfigUI.Instance.Open(_modInMemory);
                ConfigOptionsPanel.CurrentCategory = _categoryInMemory;
            }, OptionName);
        }
    }
}
