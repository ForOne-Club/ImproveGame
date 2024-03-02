namespace ImproveGame.Core;

public struct PortableBuffTile
{
    public PortableBuffTile(ushort tileID, int style, int buffID)
    {
        TileID = tileID;
        Style = style;
        BuffID = buffID;
    }

    public ushort TileID { get; }
    public int Style { get; }
    public int BuffID { get; }

    public override bool Equals(object obj) => obj is PortableBuffTile its && its.TileID == TileID &&
                                               its.Style == Style && its.BuffID == BuffID;

    public override int GetHashCode() => (TileID, Style, BuffID).GetHashCode();

    public override string ToString() => $"TileID: {TileID}, Style: {Style}, BuffID: {BuffID}";
}