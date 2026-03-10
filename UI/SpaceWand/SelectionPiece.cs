using ImproveGame.UIFramework;

namespace ImproveGame.UI.SpaceWand;

public record SelectionPiece(string HoverText, Asset<Texture2D> HoverImage, Asset<Texture2D> MainImage, Func<bool> Selected)
{
    public AnimationTimer HoverTimer = new(3);
    public AnimationTimer SelectedTimer = new(3);

    public void Update()
    {
        HoverTimer.Update();
        SelectedTimer.Update();
    }

    public void MouseOver()
    {
        HoverTimer.OpenAndResetTimer();
    }

    public void MouseOut()
    {
        HoverTimer.CloseAndResetTimer();
    }

    public Color GetColor()
    {
        if (Selected())
            SelectedTimer.Open();
        else
            SelectedTimer.Close();
        return Color.Lerp(Color.Gray, Color.White, SelectedTimer.Schedule);
    }

    public void DrawSelf(SpriteBatch sb, SelectionButton parent)
    {
        CalculatedStyle dimensions = parent.GetDimensions();
        Vector2 position = dimensions.Position() + parent.GetSize() / 2f;
        Color color = GetColor() * parent.Opacity;

        sb.Draw(MainImage.Value, position, null, color, 0, MainImage.Size() / 2f, 1f, 0, 0f);
        sb.Draw(HoverImage.Value, position, null, color * 1.4f * HoverTimer.Schedule, 0, parent.GetSize() / 2f, 1f, 0, 0f);
    }
}