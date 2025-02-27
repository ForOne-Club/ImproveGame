using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.SUIElements;
using Newtonsoft.Json;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;

namespace ImproveGame.UI.ModernConfig.Categories
{
    public class SingleConfigCategory(ModConfig config) : Category
    {
        public override int ItemIconId => config.Mode == ConfigScope.ServerSide ? ItemID.CellPhone : ItemID.WireKite;
        public override void AddOptions(ConfigOptionsPanel panel)
        {
            //panel.ShouldHideSearchBar = true;

            foreach (PropertyFieldWrapper variable in ConfigManager.GetFieldsAndProperties(config))
            {
                if (variable.IsProperty && variable.Name == "Mode")
                    continue;

                if (Attribute.IsDefined(variable.MemberInfo, typeof(JsonIgnoreAttribute)) && !Attribute.IsDefined(variable.MemberInfo, typeof(ShowDespiteJsonIgnoreAttribute)))
                    continue;
                CrossModCategoryCard.AddOptionToPanel(variable, config, panel);
            }
        }
        public override string Label => config.DisplayName.Value;
    }
}
