using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;

namespace ImproveGame.UI.ModernConfig.OptionElements
{
    public class OptionColor : ModernConfigOption
    {
        public OptionColor(ModConfig config, PropertyFieldWrapper propertyFieldWrapper) : base(config, propertyFieldWrapper, 70)
        {
        }
        public OptionColor(ModConfig config, string name) : base(config, name, 70)
        {
            
        }
        protected override void OnBind(ModConfig config, string optionName, int reservedWidth)
        {
            base.OnBind(config, optionName, reservedWidth);
        }
    }
}
