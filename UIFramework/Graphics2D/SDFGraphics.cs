using Terraria.ID;

namespace ImproveGame.UIFramework.Graphics2D;

public static class SDFGraphics
{
    private struct VertexPosCoord(Vector2 pos, Vector2 coord) : IVertexType
    {
        private static readonly VertexDeclaration _vertexDeclaration = new(
        [
            new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
            new VertexElement(8, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
        ]);

        public Vector2 Pos = pos;
        public Vector2 Coord = coord;

        public readonly VertexDeclaration VertexDeclaration => _vertexDeclaration;
    }

    private static void GetSDFVertexInfo(ref List<VertexPosCoord> vertices, Vector2 pos, Vector2 size)
    {
        vertices.Add(new VertexPosCoord(pos, new Vector2(0, 0)));
        vertices.Add(new VertexPosCoord(pos + new Vector2(size.X, 0), new Vector2(size.X, 0)));
        vertices.Add(new VertexPosCoord(pos + new Vector2(0, size.Y), new Vector2(0, size.Y)));
        vertices.Add(new VertexPosCoord(pos + new Vector2(0, size.Y), new Vector2(0, size.Y)));
        vertices.Add(new VertexPosCoord(pos + new Vector2(size.X, 0), new Vector2(size.X, 0)));
        vertices.Add(new VertexPosCoord(pos + size, size));
    }

    private static void BaseDraw(Vector2 pos, Vector2 size)
    {
        List<VertexPosCoord> vertices = [];
        GetSDFVertexInfo(ref vertices, pos, size);
        VertexPosCoord[] triangles = [.. vertices];
        Main.graphics.GraphicsDevice.DrawUserPrimitives(0, triangles, 0, triangles.Length / 3);
        Main.spriteBatch.spriteEffectPass.Apply();
    }

    /// <summary>
    /// 绘制叉号
    /// </summary>
    public static void HasBorderCross(Vector2 pos, float size, float round, Color backgroundColor, float border,
        Color borderColor, bool ui = true)
    {
        const float innerShrinkage = 1;
        pos -= new Vector2(innerShrinkage);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uTransform"].SetValue(GetMatrix(ui));
        effect.Parameters["uSizeOver2"].SetValue(new Vector2(size / 2f + innerShrinkage));
        effect.Parameters["uBorder"].SetValue(border);
        effect.Parameters["uRound"].SetValue(round + innerShrinkage / 2f);
        effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        effect.Parameters["uBackgroundColor"].SetValue(backgroundColor.ToVector4());
        effect.Parameters["uInnerShrinkage"].SetValue(innerShrinkage);
        const float root2Over2 = 1.414213562373f / 2f;
        effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);
        effect.CurrentTechnique.Passes["HasBorderCross"].Apply();
        BaseDraw(pos, new Vector2(size + innerShrinkage * 2));
    }

    public static void HasBorderRound(Vector2 pos, float size, Color background, float border, Color borderColor,
        bool ui = true)
    {
        const float innerShrinkage = 1;
        pos -= new Vector2(innerShrinkage);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uTransform"].SetValue(GetMatrix(ui));
        effect.Parameters["uSizeOver2"].SetValue(new Vector2(size + innerShrinkage * 2) / 2f);
        effect.Parameters["uBackgroundColor"].SetValue(background.ToVector4());
        effect.Parameters["uBorder"].SetValue(border);
        effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        effect.Parameters["uInnerShrinkage"].SetValue(innerShrinkage);
        const float root2Over2 = 1.414213562373f / 2f;
        effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);
        effect.CurrentTechnique.Passes["HasBorderRound"].Apply();
        BaseDraw(pos, new Vector2(size + innerShrinkage * 2));
    }

    public static void NoBorderRound(Vector2 pos, float size, Color background, bool ui = true)
    {
        const float innerShrinkage = 1;
        pos -= new Vector2(innerShrinkage);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uTransform"].SetValue(GetMatrix(ui));
        effect.Parameters["uSizeOver2"].SetValue(new Vector2(size + innerShrinkage * 2) / 2f);
        effect.Parameters["uBackgroundColor"].SetValue(background.ToVector4());
        effect.Parameters["uInnerShrinkage"].SetValue(innerShrinkage);
        const float root2Over2 = 1.414213562373f / 2f;
        effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);
        effect.CurrentTechnique.Passes["NoBorderRound"].Apply();
        BaseDraw(pos, new Vector2(size + innerShrinkage * 2));
    }

    /// <summary>
    /// 绘制一条线，无边框
    /// </summary>
    public static void NoBorderLine(Vector2 start, Vector2 end, float width, Color background, bool ui = true)
    {
        Vector2 min = Vector2.Min(start, end);
        Vector2 max = Vector2.Max(start, end);
        Vector2 size = max - min + new Vector2(width * 2);

        start += new Vector2(width) - min;
        end += new Vector2(width) - min;
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uTransform"].SetValue(GetMatrix(ui));
        effect.Parameters["uStart"].SetValue(start);
        effect.Parameters["uEnd"].SetValue(end);
        effect.Parameters["uLineWidth"].SetValue(width);
        effect.Parameters["uBackgroundColor"].SetValue(background.ToVector4());
        effect.CurrentTechnique.Passes["NoBorderLine"].Apply();
        BaseDraw(min - new Vector2(width), size);
    }

    /// <summary>
    /// 绘制一条线，有边框
    /// </summary>
    public static void HasBorderLine(Vector2 start, Vector2 end, float width, Color background, float border, Color borderColor, bool ui = true)
    {
        Vector2 min = Vector2.Min(start, end);
        Vector2 max = Vector2.Max(start, end);
        Vector2 size = max - min + new Vector2(width * 2);

        start += new Vector2(width) - min;
        end += new Vector2(width) - min;
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uTransform"].SetValue(GetMatrix(ui));
        effect.Parameters["uStart"].SetValue(start);
        effect.Parameters["uEnd"].SetValue(end);
        effect.Parameters["uLineWidth"].SetValue(width);
        effect.Parameters["uBackgroundColor"].SetValue(background.ToVector4());
        effect.Parameters["uBorder"].SetValue(border);
        effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        effect.CurrentTechnique.Passes["HasBorderLine"].Apply();
        BaseDraw(min - new Vector2(width), size);
    }
}
