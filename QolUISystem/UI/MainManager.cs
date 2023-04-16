using ImproveGame.QolUISystem.UIElements;
using ImproveGame.QolUISystem.UIEnums;
using ImproveGame.QolUISystem.UIStruct;

namespace ImproveGame.QolUISystem.UI;

public class MainManager : UIManager
{
    public override void OnInitialize()
    {
        XUIPanel mainPanel = new XUIPanel()
        {
            Padding = new Spacing(20f),
            Rounded = new Vector4(20f),
            Border = 2f,
            BorderColor = Color.White,
            BgColor = Color.White * 0f,
            Draggable = true,
            Overflow = UIOverflow.Hidden
        };
        mainPanel.Position.Align = new Vector2(0.5f);
        mainPanel.Size.Pixels = new Vector2(400f, 300f);
        mainPanel.Join(this);

        XUIPanel panel_1 = new XUIPanel()
        {
            Padding = new Spacing(20f),
            Rounded = new Vector4(20f),
            Border = 2f,
            BorderColor = Color.White,
            Draggable = true,
            Overflow = UIOverflow.Hidden,
        };
        panel_1.Size.Parent = new Vector2(1.1f);
        panel_1.Join(mainPanel);

        XUIPanel panel_2 = new XUIPanel()
        {
            Padding = new Spacing(20f),
            Rounded = new Vector4(20f),
            Border = 2f,
            BorderColor = Color.White,
            Draggable = true,
            Overflow = UIOverflow.Hidden,
            IgnoresMouseInteraction = true
        };
        panel_2.Size.Parent = new Vector2(1.1f);
        panel_2.Join(panel_1);

        XUIPanel panel_3 = new XUIPanel()
        {
            Rounded = new Vector4(20f),
            Border = 2f,
            BorderColor = Color.White,
            Draggable = true,
            IgnoresMouseInteraction = true
        };
        panel_3.Size.Parent = new Vector2(1.1f);
        panel_3.Join(panel_2);
    }
}
