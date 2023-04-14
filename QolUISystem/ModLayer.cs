using ImproveGame.QolUISystem.UIElements;

namespace ImproveGame.QolUISystem;

public class ModLayer : GameInterfaceLayer
{
    public readonly XUIManager Manager;

    public ModLayer(string name, XUIManager manager) : base("ImrpoveGame: " + name, InterfaceScaleType.UI)
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
