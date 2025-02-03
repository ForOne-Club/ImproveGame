using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Graphics2D;
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
            TextOrKey = "Mods.ImproveGame.ModernConfig.AboutPage_ModConfig.About",
            //TextOrKey = "你好！这个页面是由错数螺线(LogSpiral)脑子一热试着做的！\n因为Cyril做的ModernConfig实在是太【顺滑口感】了\n如果出了什么问题【请求原谅】，我是说【私信轰炸】来催促他【摧毁漏洞】",
            UseKey = true,
            TextAlign = new Vector2(0f),
            IsWrapped = true,
            Width = { Precent = 1f },
            TextScale = 1f,
            RelativeMode = RelativeMode.Vertical
        };
        text.RecalculateText();
        text.SetInnerPixels(new Vector2(0f, text.TextSize.Y));

        var koishi = new SUIDrawingImage(() => SDFGraphics.FumoFumoKoishi(text.GetDimensions().Position() + new Vector2(400,680))) { Width = new(0,1f),Height = new(300,0),RelativeMode = RelativeMode.Vertical};//new Vector2(770,180)
        panel.AddToOptionsDirect(text);
        panel.AddToOptionsDirect(koishi);

        var v = new View()
        {
            Height =new(300,0),
            Width =new(300,0),
            RelativeMode = RelativeMode.Vertical
        };
        panel.AddToOptionsDirect(v);


    }
    //public override string Label => "关于(";
    //public override string Tooltip => "通用版ModernConfig PrPrPrPrPr";
}
