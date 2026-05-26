using SilkyUIFramework.Elements;

namespace ImproveGame.UserInterfaces.CreateWand;

public class ConstructStructureCard : UIElementGroup
{
    protected override object CommandParameter => InnerText.Text;
    public UITextView InnerText { get; }
    public ConstructStructureCard()
    {
        InnerText = new()
        {
            TextAlign = new(0, 0.5f)
        };
        AddChild(InnerText);
    }
    protected override void UpdateStatus(GameTime gameTime)
    {
        base.UpdateStatus(gameTime);
        BackgroundColor = Color.Black * HoverTimer.Lerp(0.25f, 0.1f);
    }
    public override void OnLeftMouseClick(SilkyUIFramework.UIMouseEvent evt)
    {
        base.OnLeftMouseClick(evt);
        SoundEngine.PlaySound(SoundID.ResearchComplete);
    }
}
public class ConstructStructureCardTemplate : ISourcedUIViewTemplate
{
    public static ConstructStructureCardTemplate Instance { get; } = new();
    UIView ISourcedUIViewTemplate.ConstructFromSource(object sourceData)
    {
        if (sourceData is not string path)
            throw new ArgumentException($"The type of source should be string, but this sourceData is {sourceData.GetType()}");
        ConstructStructureCard fileCard = new()
        {
            Width = new(0, 1),
            FitHeight = true,
            Padding = new(4),
            Margin = new(1),
            BorderRadius = new(8),
            BackgroundColor = Color.Black * .25f
        };
        fileCard.InnerText.Text = Path.GetFileName(path);
        fileCard.Bind(nameof(CreateWandViewModel.RegisterFromQotStructureCommand), nameof(ConstructStructureCard.Command));
        return fileCard;
    }
}