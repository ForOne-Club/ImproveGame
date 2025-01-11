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
        public OptionObject(ModConfig config, PropertyFieldWrapper propertyFieldWrapper) : base(config, propertyFieldWrapper, 70)
        {
        }
        public OptionObject(ModConfig config, string name) : base(config, name, 70)
        {

        }
        protected override void OnBind2()
        {
            base.OnBind2();
            int order = 0;
            object data = VariableInfo.GetValue(Item);
            if (data == null)
                return;
            foreach (PropertyFieldWrapper variable in ConfigManager.GetFieldsAndProperties(data))
            {
                if (Attribute.IsDefined(variable.MemberInfo, typeof(JsonIgnoreAttribute)))
                    continue;

                int top = 0;

                //UIModConfig.HandleHeader(dataList, ref top, ref order, variable);

                var wrapped = WrapIt(this, ref top, Config, variable, data, order++);
            }
            Height.Set(46 * (1 + Children.Count()), 0);
            Recalculate();
        }
        protected override void OnBind(ModConfig config, string optionName, int reservedWidth)
        {
            base.OnBind(config, optionName, reservedWidth);


        }
    }
}
