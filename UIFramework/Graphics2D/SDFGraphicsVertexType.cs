namespace ImproveGame.UIFramework.Graphics2D;

public struct SDFGraphicsVertexType(Vector2 position, Vector2 coord, float cornerRadius) : IVertexType
{
    public Vector2 Position = position;
    public Vector2 Coord = coord;
    public float CornerRadius = cornerRadius;

    private static readonly VertexDeclaration VertexDeclaration = new (
        [
            new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
            new VertexElement(8, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(16, VertexElementFormat.Single, VertexElementUsage.Color, 0)
        ]);

    readonly VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;
}

public static class SDFGraphicsVertexTypeExtensions
{
    public static void AddRectangle(this List<SDFGraphicsVertexType> vertices, Vector2 pos, Vector2 size, Vector2 coordQ1, Vector2 coordQ2, float rounded)
    {
        SDFGraphicsVertexType[] thisVertices =
        [
            new SDFGraphicsVertexType(pos, coordQ1, rounded),
            new SDFGraphicsVertexType(pos + new Vector2(size.X, 0f), new Vector2(coordQ2.X, coordQ1.Y), rounded),
            new SDFGraphicsVertexType(pos + new Vector2(0f, size.Y), new Vector2(coordQ1.X, coordQ2.Y), rounded),
            new SDFGraphicsVertexType(pos + size, coordQ2, rounded),
        ];

        vertices.Add(thisVertices[0]);
        vertices.Add(thisVertices[1]);
        vertices.Add(thisVertices[2]);

        vertices.Add(thisVertices[2]);
        vertices.Add(thisVertices[1]);
        vertices.Add(thisVertices[3]);
    }
}