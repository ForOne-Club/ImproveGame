namespace ImproveGame.QolUISystem.UIStruct;

public struct MouseKey
{
    public bool Left;
    public bool Right;
    public bool Middle;

    public MouseKey(bool left, bool right, bool middle)
    {
        Left = left;
        Right = right;
        Middle = middle;
    }

    public void SetValue(bool left, bool right, bool middle)
    {
        Left = left;
        Right = right;
        Middle = middle;
    }
}