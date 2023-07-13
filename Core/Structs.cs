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
}