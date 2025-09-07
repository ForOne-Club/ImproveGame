#if false

using SilkyUIFramework;
using SilkyUIFramework.Attributes;
using SilkyUIFramework.BasicElements;

namespace ImproveGame.UserInterfaces.Global;

[RegisterGlobalUI("PromptUI", 0)]
public partial class PromptUI : BasicBody
{
    protected override void OnInitialize()
    {
        InitializeComponent();

        Title.UseDeathText();

        BorderColor = SUIColor.Border;
        BackgroundColor = SUIColor.Background * 0.75f;

        BorderRadius = new Vector4(4);
    }

    protected override void UpdateStatus(GameTime gameTime)
    {
        base.UpdateStatus(gameTime);
    }
}

#endif