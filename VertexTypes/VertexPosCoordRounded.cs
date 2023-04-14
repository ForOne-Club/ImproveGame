namespace ImproveGame.VertexTypes;

public struct VertexPositionCoordRounded : IVertexType
{
    public Vector2 Position;
    public Vector2 Coord;
    public float Rounded;

    public static readonly VertexDeclaration VertexDeclaration;

    VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

    static VertexPositionCoordRounded()
    {
        VertexDeclaration = new VertexDeclaration(new VertexElement[]
        {
            new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
            new VertexElement(8, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(16, VertexElementFormat.Single, VertexElementUsage.Color, 0)
        });
    }

    public VertexPositionCoordRounded(Vector2 position, Vector2 coord, float rounded)
    {
        Position = position;
        Coord = coord;
        Rounded = rounded;
    }

    public VertexPositionCoordRounded(float positionX, float positionY, float coordX, float coordY, float rounded)
    {
        Position.X = positionX;
        Position.Y = positionY;
        Coord.X = coordX;
        Coord.Y = coordY;
        Rounded = rounded;
    }
}

public static class VertexPositionCoordRoundedExtensions
{
    public static void AddRectangle(this List<VertexPositionCoordRounded> vertices, Vector2 pos, Vector2 size, Vector2 coordQ1, Vector2 coordQ2, float rounded)
    {
        VertexPositionCoordRounded[] thisVertices = new VertexPositionCoordRounded[4]
        {
            new VertexPositionCoordRounded(pos, coordQ1, rounded),
            new VertexPositionCoordRounded(pos + new Vector2(size.X, 0f), new Vector2(coordQ2.X, coordQ1.Y), rounded),
            new VertexPositionCoordRounded(pos + new Vector2(0f, size.Y), new Vector2(coordQ1.X, coordQ2.Y), rounded),
            new VertexPositionCoordRounded(pos + size, coordQ2, rounded),
        };

        vertices.Add(thisVertices[0]);
        vertices.Add(thisVertices[1]);
        vertices.Add(thisVertices[2]);

        vertices.Add(thisVertices[2]);
        vertices.Add(thisVertices[1]);
        vertices.Add(thisVertices[3]);
    }
}