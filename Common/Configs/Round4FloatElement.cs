using Terraria.ModLoader.Config.UI;

namespace ImproveGame.Common.Configs;

public class Round4FloatElement : FloatElement
{
    public override void SetValue(object value) => base.SetValue(MathF.Round((float)value, 4));
}
