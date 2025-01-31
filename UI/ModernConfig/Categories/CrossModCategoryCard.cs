using FullSerializer;
using ImproveGame.UI.ModernConfig.OptionElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using tModPorter;

namespace ImproveGame.UI.ModernConfig.Categories
{
    public class CrossModCategoryCard : Category
    {
        public override void AddOptions(ConfigOptionsPanel panel)
        {
            foreach (var pair in options)
                AddOptionToPanel(pair.Key, pair.Value, panel);
        }
        public static void AddOptionToPanel(PropertyFieldWrapper variable, ModConfig config, ConfigOptionsPanel panel)
        {
            Type type = variable.Type;
            if (type == typeof(bool))
                panel.AddToggle(config, variable);
            else if (OptionSlider.SupportedTypes.Contains(type))
                panel.AddValueSlider(config, variable);
            else if (type.IsEnum)
                panel.AddEnum(config, variable);
            else if (type == typeof(string))
            {
                if (ConfigManager.GetCustomAttributeFromMemberThenMemberType<OptionStringsAttribute>(variable, null, null) != null)
                    panel.AddEnum(config, variable);
                else
                    panel.AddEditableText(config, variable);

            }
            else if (type.IsArray)
                panel.AddArray(config, variable);
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                panel.AddList(config, variable);
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(HashSet<>))
                panel.AddHashSet(config, variable);
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                panel.AddDictionary(config, variable);
                //panel.AddNotSupportText(config, variable);

            else if (type.IsSubclassOf(typeof(EntityDefinition)))
            {
                panel.AddNotSupportText(config, variable);

            }
            else
                panel.AddObject(config, variable);
            //panel.AddNotSupportText(config, variable);
        }
        public override int ItemIconId => iconID;
        readonly int iconID;
        readonly Func<Texture2D> GetIconTex;
        readonly Func<string> GetLabel;
        readonly Func<string> GetTooltip;

        public override Texture2D GetIcon() => GetIconTex?.Invoke() ?? base.GetIcon();
        public override string Label => GetLabel?.Invoke() ?? base.Label;
        public override string Tooltip => GetTooltip?.Invoke() ?? base.Tooltip;

        readonly List<KeyValuePair<PropertyFieldWrapper, ModConfig>> options;
        public CrossModCategoryCard(List<KeyValuePair<PropertyFieldWrapper, ModConfig>> variables, int itemIconID = 0, Func<Texture2D> getIconTexture = null, Func<string> getLabel = null, Func<string> getTooltip = null)
        {
            options = variables;
            iconID = itemIconID;
            GetIconTex = getIconTexture;
            GetLabel = getLabel;
            GetTooltip = getTooltip;
        }
    }
}
