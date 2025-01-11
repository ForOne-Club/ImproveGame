using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;

namespace ImproveGame.UI.ModernConfig.OptionElements
{
    public abstract class OptionCollections : ModernConfigOption
    {
        protected OptionCollections(ModConfig config, string name, int reservedWidth) : base(config, name, reservedWidth)
        {
        }
    }
}
