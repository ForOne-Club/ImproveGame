using ImproveGame.QolUISystem.UIElements;

namespace ImproveGame.QolUISystem;

public class UserInterfaceLayer : GameInterfaceLayer
{
    public readonly UIManager Manager;

    public UserInterfaceLayer(string name, UIManager manager) : base("ImrpoveGame: " + name, InterfaceScaleType.UI)
    {
        Manager = manager;
    }

    public override bool DrawSelf()
    {
        Manager.PreDraw();
        Manager.Draw(default);
        return true;
    }
}
