using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.SUIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;

namespace ImproveGame.UI.ModernConfig.OptionElements
{
    /// <summary>
    /// 单纯显示一行文字
    /// 嗯？你问我为什么不用SUIText 因为设置和文本混杂在一起的时候用那个会被刷掉，动_allOptions感觉会出更多很麻烦的东西
    /// </summary>
    internal class OptionNotSupportText : ModernConfigOption
    {
        public OptionNotSupportText(ModConfig config, PropertyFieldWrapper propertyFieldWrapper) : base(config, propertyFieldWrapper, 140)
        {
            //var box = new View
            //{
            //    IsAdaptiveWidth = true,
            //    HAlign = 1f,
            //    VAlign = 0.5f,
            //    Height = StyleDimension.Fill
            //};
            //box.JoinParent(this);
            var variable = propertyFieldWrapper;
            var text = new SUIText
            {
                TextOrKey = $"属性 {ConfigHelper.GetLabel(config, variable.Name)}({variable.Name}) \n类型 {variable.Type} 不受支持, 请前往普通模组配置修改",
                UseKey = true,
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
                //Recalculate();
            };

        }

    }
}
