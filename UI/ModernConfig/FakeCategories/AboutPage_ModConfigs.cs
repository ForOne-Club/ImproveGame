using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.SUIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImproveGame.UI.ModernConfig.FakeCategories;
public sealed class AboutPage_ModConfig : Category
{
    public override int ItemIconId => ItemID.GPS;

    public override void AddOptions(ConfigOptionsPanel panel)
    {
        panel.ShouldHideSearchBar = true;

        var text = new SUIText
        {
            TextOrKey = "你好！这个页面是由错数螺线(LogSpiral)脑子一热试着做的！\n因为Cyril做的ModernConfig实在是太【顺滑口感】了\n如果出了什么问题【请求原谅】，我是说【私信轰炸】来催促他【摧毁漏洞】",
            UseKey = true,
            TextAlign = new Vector2(0f),
            IsWrapped = true,
            Width = { Precent = 1f },
            TextScale = 1.1f,
            RelativeMode = RelativeMode.Vertical
        };
        panel.AddToOptionsDirect(text);
        text.RecalculateText();
        text.SetInnerPixels(new Vector2(0f, text.TextSize.Y));
    }
    public override string Label => "关于(";
    public override string Tooltip => "通用版ModernConfig PrPrPrPrPr";
}
