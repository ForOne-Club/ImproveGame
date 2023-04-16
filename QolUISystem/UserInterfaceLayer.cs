using ImproveGame.QolUISystem.UIElements;

namespace ImproveGame.QolUISystem;

public class UserInterfaceLayer : GameInterfaceLayer
{
    public readonly XUIManager Manager;

    public UserInterfaceLayer(string name, XUIManager manager) : base("ImrpoveGame: " + name, InterfaceScaleType.UI)
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
