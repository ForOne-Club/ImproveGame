using ImproveGame.QolUISystem.UIElements;

namespace ImproveGame.QolUISystem.UIStruct;

public struct MouseTargets
{
    public List<XUIElement> Left;
    public List<XUIElement> Right;
    public List<XUIElement> Middle;

    public MouseTargets()
    {
        Left = new List<XUIElement>();
        Right = new List<XUIElement>();
        Middle = new List<XUIElement>();
    }
}
