namespace ImproveGame.QolUISystem.UIStruct;

public struct Spacing
{
    public float Top;
    public float Right;
    public float Bottom;
    public float Left;

    public Spacing(float all)
    {
        Top = all;
        Right = all;
        Bottom = all;
        Left = all;
    }

    public Spacing(float top, float right, float bottom, float left)
    {
        Top = top;
        Right = right;
        Bottom = bottom;
        Left = left;
    }

    public void SetValue(float all)
    {
        Top = all;
        Right = all;
        Bottom = all;
        Left = all;
    }

    public void SetValue(float top, float right, float bottom, float left)
    {
        Top = top;
        Right = right;
        Bottom = bottom;
        Left = left;
    }

    /// <summary>
    /// 从上开始，顺时针旋转
    /// X, Y, Z, W → Top, Right, Bottom, Left
    /// </summary>
    /// <returns></returns>
    public Vector4 ToVector4() => new Vector4(Top, Right, Bottom, Left);
}
