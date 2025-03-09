using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;

namespace ImproveGame.UI.ModernConfig.OptionElements
{
    public class OptionCustomUIConfig:ModernConfigOption
    {
        protected override void OnBind()
        {
            if (customModConfigItemAttribute != null) 
            {
                var element = Activator.CreateInstance(customModConfigItemAttribute.Type) as ConfigElement;
                element.Bind(VariableInfo, Item, List, index);
                element.OnBind();
                Append(element);
            }
            base.OnBind();
        }
        CustomModConfigItemAttribute customModConfigItemAttribute;
        protected override void CheckAttributes()
        {
            customModConfigItemAttribute = GetAttribute<CustomModConfigItemAttribute>();
            
            base.CheckAttributes();
        }
    }
}
