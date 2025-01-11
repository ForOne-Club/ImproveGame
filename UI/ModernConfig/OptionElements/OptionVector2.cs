using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config.UI;
using Terraria.ModLoader.Config;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.Graphics2D;
using ImproveGame.UIFramework.SUIElements;
using System.ComponentModel;
using System.Reflection;

namespace ImproveGame.UI.ModernConfig.OptionElements
{
    public class OptionVector2 : ModernConfigOption
    {
        public OptionVector2(ModConfig config, PropertyFieldWrapper propertyFieldWrapper) : base(config, propertyFieldWrapper, 70)
        {
        }
        public OptionVector2(ModConfig config, string name) : base(config, name, 70)
        {

        }
        protected override void OnBind(ModConfig config, string optionName, int reservedWidth)
        {
            base.OnBind(config, optionName, reservedWidth);
        }
    }
}
