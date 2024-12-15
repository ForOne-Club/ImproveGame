using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.SUIElements;
using Newtonsoft.Json;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;

namespace ImproveGame.UI.ModernConfig.Categories
{
    public class SingleConfigCategory(ModConfig config) : Category
    {
        public override int ItemIconId => ItemID.WireKite;
        public override void AddOptions(ConfigOptionsPanel panel)
        {
            //panel.ShouldHideSearchBar = true;

            foreach (PropertyFieldWrapper variable in ConfigManager.GetFieldsAndProperties(config))
            {
                if (variable.IsProperty && variable.Name == "Mode")
                    continue;

                if (Attribute.IsDefined(variable.MemberInfo, typeof(JsonIgnoreAttribute)) && !Attribute.IsDefined(variable.MemberInfo, typeof(ShowDespiteJsonIgnoreAttribute)))
                    continue;
                string name = variable.Name;
                Type type = variable.Type;
                if (type == typeof(bool))
                    panel.AddToggle(config, name);
                else if (type == typeof(int) || type == typeof(float) || type == typeof(double))
                    panel.AddValueSlider(config, name);
                else if (type.IsEnum)
                    panel.AddEnum(config, name);
                else
                    panel.AddNotSupportText(config, variable);
                //{
                //    var text = new SUIText
                //    {
                //        TextOrKey = $"属性 {ConfigHelper.GetLabel(config,variable.Name)}({variable.Name}) \n类型 {variable.Type} 不受支持, 请前往普通模组配置修改",
                //        UseKey = true,
                //        TextAlign = new Vector2(0f),
                //        IsWrapped = true,
                //        Width = { Precent = 1f },
                //        TextScale = 1.1f,
                //        RelativeMode = RelativeMode.Vertical
                //    };
                //    panel.AddToOptionsDirect(text);
                //    text.RecalculateText();
                //    text.SetInnerPixels(new Vector2(0f, text.TextSize.Y));
                //}
            }
        }
        public override string Label => config.DisplayName.Value;
        public override string Tooltip => "螺线反射生成的配置界面\n目前只支持布尔 整数 单双精度浮点 枚举";
    }
}
