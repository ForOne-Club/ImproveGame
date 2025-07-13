using ImproveGame.Content.Functions.AutoPiggyBank;
using ImproveGame.UI.AutoPiggyBank;
using Terraria.DataStructures;

namespace ImproveGame.Content.BuilderToggles;

public enum PiggyToggleState
{
    Coin,
    All,
    Off,
}

public class PiggyToggle : BuilderToggle
{
    public override string HoverTexture => Texture;

    public static LocalizedText AllOnText { get; private set; }
    public static LocalizedText VanillaOnText { get; private set; }
    public static LocalizedText OffText { get; private set; }
    public static LocalizedText RightFilter { get; private set; }

    public override bool Active() => Main.LocalPlayer.TryGetModPlayer<AutoMoneyPlayerListener>(out var listener) &&
                                     listener.AutoSaveUnlocked;

    public override Position OrderPosition => new After(TorchBiome);

    public override int NumberOfStates => 3;

    public static PiggyToggleState AutoSaveEnabled
    {
        get
        {
            var instance = ModContent.GetInstance<PiggyToggle>();
            if (instance is null || !instance.Active())
            {
                return PiggyToggleState.Off;
            }

            return instance.CurrentState switch
            {
                0 => PiggyToggleState.Coin,
                1 => PiggyToggleState.All,
                _ => PiggyToggleState.Off,
            };
        }
    }

    public override void SetStaticDefaults()
    {
        AllOnText = this.GetLocalization(nameof(AllOnText));
        VanillaOnText = this.GetLocalization(nameof(VanillaOnText));
        OffText = this.GetLocalization(nameof(OffText));
        RightFilter = this.GetLocalization(nameof(RightFilter));
    }

    public override bool OnLeftClick(ref SoundStyle? sound)
    {
        sound = SoundID.Item59;
        return base.OnLeftClick(ref sound);
    }

    public override void OnRightClick()
    {
        PiggyFilterUI.Instance.Enabled = !PiggyFilterUI.Instance.Enabled;
        SoundEngine.PlaySound(PiggyFilterUI.Instance.Enabled ? SoundID.MenuOpen : SoundID.MenuClose);
        base.OnRightClick();
    }

    public override string DisplayValue()
    {
        return CurrentState switch
        {
            0 => VanillaOnText.Value,
            1 => AllOnText.Value,
            _ => OffText.Value,
        } + '\n' + RightFilter.Value;
    }

    public override bool Draw(SpriteBatch spriteBatch, ref BuilderToggleDrawParams drawParams)
    {
        drawParams.Frame = drawParams.Texture.Frame(3, 2, CurrentState % 3);
        return true;
    }

    public override bool DrawHover(SpriteBatch spriteBatch, ref BuilderToggleDrawParams drawParams)
    {
        drawParams.Frame = drawParams.Texture.Frame(3, 2, CurrentState % 3, 1);
        return true;
    }
}