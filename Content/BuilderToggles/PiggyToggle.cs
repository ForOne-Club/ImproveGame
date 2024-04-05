using ImproveGame.Content.Functions.AutoPiggyBank;
using Terraria.DataStructures;

namespace ImproveGame.Content.BuilderToggles;

public class PiggyToggle : BuilderToggle
{
    public override string HoverTexture => Texture;

    public static LocalizedText OnText { get; private set; }
    public static LocalizedText OffText { get; private set; }

    public override bool Active() => Main.LocalPlayer.TryGetModPlayer<AutoMoneyPlayerListener>(out var listener) &&
                                     listener.AutoSaveUnlocked;

    public override Position OrderPosition => new After(TorchBiome);

    public override int NumberOfStates => 2;

    public static bool AutoSaveEnabled
    {
        get
        {
            var instance = ModContent.GetInstance<PiggyToggle>();
            return instance.Active() && instance.CurrentState is 0;
        }
    }

    public override void SetStaticDefaults()
    {
        OnText = this.GetLocalization(nameof(OnText));
        OffText = this.GetLocalization(nameof(OffText));
    }

    public override bool OnLeftClick(ref SoundStyle? sound)
    {
        sound = SoundID.Item59;
        return base.OnLeftClick(ref sound);
    }

    public override string DisplayValue()
    {
        return CurrentState == 0 ? OnText.Value : OffText.Value;
    }

    public override bool Draw(SpriteBatch spriteBatch, ref BuilderToggleDrawParams drawParams)
    {
        drawParams.Frame = drawParams.Texture.Frame(2, 2, CurrentState % 2);
        return true;
    }

    public override bool DrawHover(SpriteBatch spriteBatch, ref BuilderToggleDrawParams drawParams)
    {
        drawParams.Frame = drawParams.Texture.Frame(2, 2, CurrentState % 2, 1);
        return true;
    }
}