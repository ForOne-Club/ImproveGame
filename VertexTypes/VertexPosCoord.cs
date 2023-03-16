namespace ImproveGame.VertexTypes;

/// <summary>
/// VertexPositionCoord
/// </summary>
public struct VertexPosCoord : IVertexType
{
    private static readonly VertexDeclaration _vertexDeclaration = new VertexDeclaration(new VertexElement[2]
    {
                new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
                new VertexElement(8, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
    });

    public VertexDeclaration VertexDeclaration => _vertexDeclaration;

    public Vector2 Position;
    public Vector2 Coord;

    public VertexPosCoord(Vector2 position, Vector2 coord)
    {
        Position = position;
        Coord = coord;
    }

    public VertexPosCoord(float positionX, float positionY, float coordX, float coordY)
    {
        Position.X = positionX;
        Position.Y = positionY;
        Coord.X = coordX;
        Coord.Y = coordY;
    }
}