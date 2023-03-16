namespace ImproveGame.VertexTypes;

/// <summary>
/// VertexPositionCoordRounded
/// </summary>
public struct VertexPosCoordRounded : IVertexType
{
    private static readonly VertexDeclaration _vertexDeclaration = new(new VertexElement[3]
    {
                new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
                new VertexElement(8, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                new VertexElement(16, VertexElementFormat.Single, VertexElementUsage.Color, 0)
    });

    public VertexDeclaration VertexDeclaration => _vertexDeclaration;

    public Vector2 Position;
    public Vector2 Coord;
    public float Rounded;

    public VertexPosCoordRounded(Vector2 position, Vector2 coord, float rounded)
    {
        Position = position;
        Coord = coord;
        Rounded = rounded;
    }

    public VertexPosCoordRounded(float positionX, float positionY, float coordX, float coordY, float rounded)
    {
        Position.X = positionX;
        Position.Y = positionY;
        Coord.X = coordX;
        Coord.Y = coordY;
        Rounded = rounded;
    }
}