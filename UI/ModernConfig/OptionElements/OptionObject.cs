using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config.UI;
using Terraria.ModLoader.Config;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Newtonsoft.Json;

namespace ImproveGame.UI.ModernConfig.OptionElements
{
    public class OptionObject:ModernConfigOption
    {
        protected override void OnBind()
        {
            object data = GetValue();
            if (data == null)
                return;
            float subHeight = 0;
            foreach (PropertyFieldWrapper variable in ConfigManager.GetFieldsAndProperties(data))
            {
                if (Attribute.IsDefined(variable.MemberInfo, typeof(JsonIgnoreAttribute)))
                    continue;
                var wrapped = WrapIt(this, Config, variable, data,owner:this);
                subHeight += wrapped.Height.Pixels;
            }

            Height.Set(60 + subHeight, 0);
            Recalculate();
        }
    }
}
