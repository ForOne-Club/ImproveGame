//本文件中的形状绘制全部改自iq大佬的SDF https://iquilezles.org/articles/distfunctions2d/

namespace ImproveGame.UIFramework.Graphics2D;
public static class SDFGraphics
{
    const float root2Over2 = 1.414213562373f / 2f;

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


        //DEBUG用代码
        if (DrawFrame)
            for (int n = 0; n < triangles.Length / 3; n++)
            {
                DrawLine(triangles[3 * n].Pos, triangles[3 * n + 1].Pos, 1);
                DrawLine(triangles[3 * n + 1].Pos, triangles[3 * n + 2].Pos, 1);
                DrawLine(triangles[3 * n + 2].Pos, triangles[3 * n].Pos, 1);
            }
    }
    static void DrawLine(Vector2 start, Vector2 end, float LineWidth)
    {
        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, (start + end) * .5f, new Rectangle(0, 0, 1, 1), Color.White, (start - end).ToRotation(), new Vector2(.5f), new Vector2(Vector2.Distance(start, end), LineWidth), 0, 0);
    }
    static bool DrawFrame => true;
    public static void Gallery(Vector2 position, Vector2 unitSize, Color backgroundColor, float border, Color borderColor, Matrix matrix)
    {
        Vector2 start = position;
        Vector2 noBorderOffset = Vector2.UnitY * 120;

        HasBorderRound(position, unitSize.X, backgroundColor, border, borderColor, matrix);
        NoBorderRound(position + noBorderOffset, unitSize.X, backgroundColor, matrix);
        position += Vector2.UnitX * 60;


        HasBorderRoundedBox(position, unitSize, new Vector4(1, 4, 8, 16), backgroundColor, border, borderColor, matrix);
        NoBorderRoundedBox(position + noBorderOffset, unitSize, new Vector4(1, 4, 8, 16), backgroundColor, matrix);
        position += Vector2.UnitX * 60;


        HasBorderBox(position, unitSize, backgroundColor, border, borderColor, matrix);
        NoBorderBox(position + noBorderOffset, unitSize, backgroundColor, matrix);
        position += Vector2.UnitX * 60;


        HasBorderOrientedBox(position, position + unitSize, 20, backgroundColor, border, borderColor, matrix);
        NoBorderOrientedBox(position + noBorderOffset, position + noBorderOffset + unitSize, 20, backgroundColor, matrix);
        position += Vector2.UnitX * 60;


        HasBorderLine(position, position + unitSize, 10, backgroundColor, border, borderColor, matrix);
        NoBorderLine(position + noBorderOffset, position + noBorderOffset + unitSize, 10, backgroundColor, matrix);
        position += Vector2.UnitX * 60;


        HasBorderRhombus(position, unitSize, backgroundColor, border, borderColor, matrix);
        NoBorderRhombus(position + noBorderOffset, unitSize, backgroundColor, matrix);
        position += Vector2.UnitX * 60;


        HasBorderTrapezoid(position, unitSize, 2f, backgroundColor, border, borderColor, matrix);
        NoBorderTrapezoid(position + noBorderOffset, unitSize, 2f, backgroundColor, matrix);
        position += Vector2.UnitX * 60;


        HasBorderParallelogram(position, unitSize, 0.5f, backgroundColor, border, borderColor, matrix);
        NoBorderParallelogram(position + noBorderOffset, unitSize, 0.5f, backgroundColor, matrix);
        position += Vector2.UnitX * 60;


        HasBorderEquilateralTriangle(position, unitSize.X, backgroundColor, border, borderColor, matrix);
        NoBorderEquilateralTriangle(position + noBorderOffset, unitSize.X, backgroundColor, matrix);
        position += Vector2.UnitX * 60;


        HasBorderTriangleIsosceles(position, unitSize, backgroundColor, border, borderColor, matrix);
        NoBorderTriangleIsosceles(position + noBorderOffset, unitSize, backgroundColor, matrix);
        position += Vector2.UnitX * 60;


        HasBorderTriangle(position, position + unitSize * new Vector2(1.0f, 0.5f), position + unitSize * new Vector2(0.5f, 1.0f), backgroundColor, border, borderColor, matrix);
        NoBorderTriangle(position + noBorderOffset, position + noBorderOffset + unitSize * new Vector2(1.0f, 0.5f), position + noBorderOffset + unitSize * new Vector2(0.5f, 1.0f), backgroundColor, matrix);
        position += Vector2.UnitX * 60;


        HasBorderUnevenCapsule(position, unitSize.Y, unitSize.X * .5f, unitSize.X * .25f, backgroundColor, border, borderColor, matrix);
        NoBorderUnevenCapsule(position + noBorderOffset, unitSize.Y, unitSize.X * .5f, unitSize.X * .25f, backgroundColor, matrix);
        position += Vector2.UnitX * 60;


        HasBorderPentagon(position, unitSize.X * .5f, backgroundColor, border, borderColor, matrix);
        NoBorderPentagon(position + noBorderOffset, unitSize.X * .5f, backgroundColor, matrix);
        position += Vector2.UnitX * 60;


        HasBorderHexagon(position, unitSize.X * .5f, backgroundColor, border, borderColor, matrix);
        NoBorderHexagon(position + noBorderOffset, unitSize.X * .5f, backgroundColor, matrix);
        position += Vector2.UnitX * 60;


        HasBorderOctogon(position, unitSize.X * .5f, backgroundColor, border, borderColor, matrix);
        NoBorderOctogon(position + noBorderOffset, unitSize.X * .5f, backgroundColor, matrix);
        position += Vector2.UnitX * 60;


        HasBorderNgon(position, unitSize.X * .5f, 3 + (int)Main.GlobalTimeWrappedHourly % 8, backgroundColor, border, borderColor, matrix);
        NoBorderNgon(position + noBorderOffset, unitSize.X * .5f, 3 + (int)Main.GlobalTimeWrappedHourly % 8, backgroundColor, matrix);
        position += Vector2.UnitX * 80;


        HasBorderHexagram(position, unitSize.X * .5f, backgroundColor, border, borderColor, matrix);
        NoBorderHexagram(position + noBorderOffset, unitSize.X * .5f, backgroundColor, matrix);
        position += Vector2.UnitX * 60;


        HasBorderStar5(position, unitSize.X * .5f, 0.5f, backgroundColor, border, borderColor, matrix);
        NoBorderStar5(position + noBorderOffset, unitSize.X * .5f, 0.5f, backgroundColor, matrix);
        position += Vector2.UnitX * 60;


        HasBorderStarX(position, unitSize.X * .5f, 3 + (int)Main.GlobalTimeWrappedHourly % 8, 0.5f, backgroundColor, border, borderColor, matrix);
        NoBorderStarX(position + noBorderOffset, unitSize.X * .5f, 3 + (int)Main.GlobalTimeWrappedHourly % 8, 0.5f, backgroundColor, matrix);
        position += Vector2.UnitX * 80;

        //float theta = MathHelper.Pi * (0.5f + 0.5f * MathF.Cos(Main.GlobalTimeWrappedHourly));// 
        float theta = MathHelper.Pi * 2 / 3f;
        HasBorderPie(position, unitSize.X * .5f, theta, backgroundColor, border, borderColor, matrix);
        NoBorderPie(position + noBorderOffset, unitSize.X * .5f, theta, backgroundColor, matrix);
        position += Vector2.UnitX * 60;

        //float k = MathF.Cos(Main.GlobalTimeWrappedHourly);
        float k = 0.5f;
        HasBorderCutDisk(position, unitSize.X * .5f, k, backgroundColor, border, borderColor, matrix);
        NoBorderCutDisk(position + noBorderOffset, unitSize.X * .5f, k, backgroundColor, matrix);
        position += Vector2.UnitX * 60;

        HasBorderArc(position, unitSize.X * .5f, theta, 4, backgroundColor, border, borderColor, matrix);
        NoBorderArc(position + noBorderOffset, unitSize.X * .5f, theta, 4, backgroundColor, matrix);
        position += Vector2.UnitX * 60;


        HasBorderRing(position, unitSize.X * .5f, theta, 7, backgroundColor, border, borderColor, matrix);
        NoBorderRing(position + noBorderOffset, unitSize.X * .5f, theta, 7, backgroundColor, matrix);
        position += Vector2.UnitX * 60;

        HasBorderHorseshoe(position, unitSize.X * .5f, theta, 7, 36, backgroundColor, border, borderColor, matrix);
        NoBorderHorseshoe(position + noBorderOffset, unitSize.X * .5f, theta, 7, 36, backgroundColor, matrix);
        position += Vector2.UnitX * 120;

        HasBorderVesica(position, unitSize.X, unitSize.X * .5f, backgroundColor, border, borderColor, matrix);
        NoBorderVesica(position + noBorderOffset, unitSize.X, unitSize.X * .5f, backgroundColor, matrix);
        position += Vector2.UnitX * 80;

        HasBorderOrientedVesica(position, position + unitSize * .5f, unitSize.Length() * .5f, backgroundColor, border, borderColor, matrix);
        NoBorderOrientedVesica(position + noBorderOffset, position + noBorderOffset + unitSize * .5f, unitSize.Length() * .5f, backgroundColor, matrix);
        //HasBorderRound(position - new Vector2(unitSize.Length()) * .5f , unitSize.Length(), Color.Transparent, border, Color.Cyan with { A = 0}, matrix);
        //HasBorderRound(position - new Vector2(unitSize.Length()) * .5f + unitSize * .5f, unitSize.Length(), Color.Transparent, border, Color.Cyan with { A = 0}, matrix);
        position += Vector2.UnitX * 60;

        HasBorderMoon(position, unitSize.X * .5f, unitSize.X * .45f, unitSize.X * .2f, backgroundColor, border, borderColor, matrix);
        NoBorderMoon(position + noBorderOffset, unitSize.X * .5f, unitSize.X * .45f, unitSize.X * .2f, backgroundColor, matrix);
        position += Vector2.UnitX * 60;

        HasBorderCircleCross(position, new Vector2(unitSize.Y, unitSize.X) * .75f, 4, backgroundColor, border, borderColor, matrix);
        NoBorderCircleCross(position + noBorderOffset, new Vector2(unitSize.Y, unitSize.X) * .75f, 4, backgroundColor, matrix);
        position += Vector2.UnitX * 80;


        HasBorderEgg(position, unitSize.X * .5f, unitSize.X * .25f, unitSize.Y * .5f, backgroundColor, border, borderColor, matrix);
        NoBorderEgg(position + noBorderOffset, unitSize.X * .5f, unitSize.X * .25f, unitSize.Y * .5f, backgroundColor, matrix);
        position += Vector2.UnitX * 60;

        HasBorderHeart(position, unitSize.X, backgroundColor, border, borderColor, matrix);
        NoBorderHeart(position + noBorderOffset, unitSize.X, backgroundColor, matrix);
        position += Vector2.UnitX * 60;


        HasBorderPlus(position, unitSize.X * .75f, unitSize.X * .25f, unitSize.X * .15f, backgroundColor, border, borderColor, matrix);
        NoBorderPlus(position + noBorderOffset, unitSize.X * .75f, unitSize.X * .25f, unitSize.X * .15f, backgroundColor, matrix);
        position += Vector2.UnitX * 60;

        HasBorderCross(position, unitSize.X, 6, backgroundColor, border, borderColor, matrix);
        NoBorderCross(position + noBorderOffset, unitSize.X, 6, backgroundColor, matrix);
        position += Vector2.UnitX * 60;

        HasBorderPolygon(position, unitSize, [Vector2.Zero, new(0.5f, 1.0f), new(0.8f, 0f), new(0f, 0.6f), new(1, 0.8f)], backgroundColor, border, borderColor, matrix);
        NoBorderPolygon(position + noBorderOffset, unitSize, [Vector2.Zero, new(0.5f, 1.0f), new(0.8f, 0f), new(0f, 0.6f), new(1, 0.8f)], backgroundColor, matrix);
        position += Vector2.UnitX * 60;

        HasBorderEllipse(position, unitSize * .5f, backgroundColor, border, borderColor, matrix);
        NoBorderEllipse(position + noBorderOffset, unitSize * .5f, backgroundColor, matrix);

        position = start + noBorderOffset * 2;

        HasBorderParabola(position, unitSize, backgroundColor, border, borderColor, matrix);
        NoBorderParabola(position + noBorderOffset, unitSize, backgroundColor, border, borderColor, matrix);
        position += Vector2.UnitX * 60;

        HasBorderParabolaSegment(position, unitSize, unitSize.Y * .5f, backgroundColor, border, borderColor, matrix);
        NoBorderParabolaSegment(position + noBorderOffset, unitSize, unitSize.Y * .5f, backgroundColor, matrix);
        position += Vector2.UnitX * 60;

        HasBorderQuadraticBezier(position, position + unitSize * new Vector2(2f, 0.5f), position + unitSize * new Vector2(0.5f, 1f), 4f, backgroundColor, border, borderColor, matrix);
        NoBorderQuadraticBezier(position + noBorderOffset, position + unitSize * new Vector2(2f, 0.5f) + noBorderOffset, position + unitSize * new Vector2(0.5f, 1f) + noBorderOffset, 4f, backgroundColor, matrix);
        position += Vector2.UnitX * 60;

        HasBorderBlobbyCross(position, unitSize.X * .5f, 0.7f, 8f, backgroundColor, border, borderColor, matrix);
        NoBorderBlobbyCross(position + noBorderOffset, unitSize.X * .5f, 0.7f, 8f, backgroundColor, matrix);
        position += Vector2.UnitX * 80;

        HasBorderTunnel(position, unitSize * .5f, backgroundColor, border, borderColor, matrix);
        NoBorderTunnel(position + noBorderOffset, unitSize * .5f, backgroundColor, matrix);
        position += Vector2.UnitX * 60;

        HasBorderStairs(position, unitSize * .2f, 5, backgroundColor, border, borderColor, matrix);
        NoBorderStairs(position + noBorderOffset, unitSize * .2f, 5, backgroundColor, matrix);
        position += Vector2.UnitX * 60;

        HasBorderQuadraticCircle(position, unitSize.X * .5f, backgroundColor, border, borderColor, matrix);
        NoBorderQuadraticCircle(position + noBorderOffset, unitSize.X * .5f, backgroundColor, matrix);
        position += Vector2.UnitX * 60;


        HasBorderHyperbola(position, new Vector2(unitSize.X * 1.1f, unitSize.X), backgroundColor, border, borderColor, matrix);
        NoBorderHyperbola(position + noBorderOffset, new Vector2(unitSize.X * 1.1f, unitSize.X), backgroundColor, matrix);
        position += Vector2.UnitX * 80;

        HasBorderCoolS(position, unitSize.Y, backgroundColor, border, borderColor, matrix);
        NoBorderCoolS(position + noBorderOffset, unitSize.Y, backgroundColor, matrix);
        position += Vector2.UnitX * 60;

        HasBorderCircleWave(position, unitSize.Y * 2, 0.75f, unitSize.X * .25f, 4, backgroundColor, border, borderColor, matrix);
        NoBorderCircleWave(position + noBorderOffset, unitSize.Y * 2, 0.75f, unitSize.X * .25f, 4, backgroundColor, matrix);
    }

    #region 圆Circle/Round
    static void Circle(Vector2 pos, float size, Color background, Matrix matrix)
    {
        const float innerShrinkage = 1;
        pos -= new Vector2(innerShrinkage);
        size += innerShrinkage * 2;
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uTransform"].SetValue(matrix);
        effect.Parameters["uSizeOver2"].SetValue(new Vector2(size) / 2f);
        effect.Parameters["uBackgroundColor"].SetValue(background.ToVector4());
        effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);

    }
    public static void HasBorderRound(Vector2 pos, float size, Color background, float border, Color borderColor, Matrix matrix)
    {
        Circle(pos, size, background, matrix);
        const float innerShrinkage = 1;
        pos -= new Vector2(innerShrinkage);
        size += innerShrinkage * 2;
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uBorder"].SetValue(border);
        effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        effect.CurrentTechnique.Passes["HasBorderCircle"].Apply();
        BaseDraw(pos, new Vector2(size));
    }
    public static void NoBorderRound(Vector2 pos, float size, Color background, Matrix matrix)
    {
        Circle(pos, size, background, matrix);
        const float innerShrinkage = 1;
        pos -= new Vector2(innerShrinkage);
        size += innerShrinkage * 2;
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.CurrentTechnique.Passes["NoBorderCircle"].Apply();
        BaseDraw(pos, new Vector2(size));
    }
    #endregion

    #region 曲边矩形RoundedBox
    static void RoundedBox(Vector2 size, Vector4 round, Color background, Matrix matrix)
    {
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uTransform"].SetValue(matrix);
        effect.Parameters["uBackgroundColor"].SetValue(background.ToVector4());
        effect.Parameters["uSizeOver2"].SetValue(size / 2f);
        effect.Parameters["uRound"].SetValue(round);
        effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);
    }
    public static void HasBorderRoundedBox(Vector2 pos, Vector2 size, Vector4 round, Color background, float border, Color borderColor, Matrix matrix)
    {
        RoundedBox(size, round, background, matrix);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uBorder"].SetValue(border);
        effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        effect.CurrentTechnique.Passes[nameof(HasBorderRoundedBox)].Apply();
        BaseDraw(pos, size);
    }
    public static void NoBorderRoundedBox(Vector2 pos, Vector2 size, Vector4 round, Color background, Matrix matrix)
    {
        RoundedBox(size, round, background, matrix);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.CurrentTechnique.Passes[nameof(NoBorderRoundedBox)].Apply();
        BaseDraw(pos, size);
    }
    #endregion

    #region 矩形Box
    static void Box(Vector2 size, Color background, Matrix matrix)
    {
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uTransform"].SetValue(matrix);
        effect.Parameters["uBackgroundColor"].SetValue(background.ToVector4());
        effect.Parameters["uSizeOver2"].SetValue(size / 2f);
        effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);
    }
    public static void HasBorderBox(Vector2 pos, Vector2 size, Color background, float border, Color borderColor, Matrix matrix)
    {
        Box(size, background, matrix);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uBorder"].SetValue(border);
        effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        effect.CurrentTechnique.Passes[nameof(HasBorderBox)].Apply();
        BaseDraw(pos, size);
    }
    public static void NoBorderBox(Vector2 pos, Vector2 size, Color background, Matrix matrix)
    {
        Box(size, background, matrix);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.CurrentTechnique.Passes[nameof(NoBorderBox)].Apply();
        BaseDraw(pos, size);
    }
    #endregion

    #region 正交矩形OrientedBox
    static void OrientedBox(Vector2 start, Vector2 end, float width, Color background, Matrix matrix, out Vector2 pos, out Vector2 size)
    {
        Vector2 normal = (start - end).SafeNormalize(default);
        normal = new Vector2(-normal.Y, normal.X);
        width /= 2;

        Vector2 topLeft = start + width * normal;
        Vector2 bottomRight = topLeft;

        for (int n = 1; n < 4; n++)
        {
            Vector2 cur = (n / 2 == 0 ? start : end) + width * (n % 2 == 0 ? 1 : -1) * normal;
            topLeft = Vector2.Min(topLeft, cur);
            bottomRight = Vector2.Max(bottomRight, cur);
        }

        start -= topLeft;
        end -= topLeft;

        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uTransform"].SetValue(matrix);
        effect.Parameters["uStart"].SetValue(start);
        effect.Parameters["uEnd"].SetValue(end);
        effect.Parameters["uLineWidth"].SetValue(width * 2);
        //effect.Parameters["uSizeOver2"].SetValue(size / 2f);
        effect.Parameters["uBackgroundColor"].SetValue(background.ToVector4());
        effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);

        pos = topLeft;
        size = bottomRight - topLeft;
    }
    public static void HasBorderOrientedBox(Vector2 start, Vector2 end, float width, Color background, float border, Color borderColor, Matrix matrix)
    {
        OrientedBox(start, end, width, background, matrix, out Vector2 pos, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uBorder"].SetValue(border);
        effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        effect.CurrentTechnique.Passes[nameof(HasBorderOrientedBox)].Apply();
        BaseDraw(pos, size);
    }
    public static void NoBorderOrientedBox(Vector2 start, Vector2 end, float width, Color background, Matrix matrix)
    {
        OrientedBox(start, end, width, background, matrix, out Vector2 pos, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.CurrentTechnique.Passes[nameof(NoBorderOrientedBox)].Apply();
        BaseDraw(pos, size);
    }
    #endregion

    #region 线段Segment/Line
    static void Segment(Vector2 start, Vector2 end, float width, Color background, Matrix matrix, out Vector2 pos, out Vector2 size)
    {
        Vector2 min = Vector2.Min(start, end);
        Vector2 max = Vector2.Max(start, end);
        pos = min - new Vector2(width);
        start -= pos;
        end -= pos;
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uTransform"].SetValue(matrix);
        effect.Parameters["uStart"].SetValue(start);
        effect.Parameters["uEnd"].SetValue(end);
        effect.Parameters["uLineWidth"].SetValue(width);
        effect.Parameters["uBackgroundColor"].SetValue(background.ToVector4());
        effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);

        size = max - min + new Vector2(width * 2);

    }
    /// <summary>
    /// 绘制一条线，有边框
    /// </summary>
    public static void HasBorderLine(Vector2 start, Vector2 end, float width, Color background, float border, Color borderColor, Matrix matrix)
    {
        Segment(start, end, width, background, matrix, out Vector2 pos, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uBorder"].SetValue(border);
        effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        effect.CurrentTechnique.Passes["HasBorderSegment"].Apply();
        BaseDraw(pos, size);
    }
    /// <summary>
    /// 绘制一条线，无边框
    /// </summary>
    public static void NoBorderLine(Vector2 start, Vector2 end, float width, Color background, Matrix matrix)
    {
        Segment(start, end, width, background, matrix, out Vector2 pos, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.CurrentTechnique.Passes["NoBorderSegment"].Apply();
        BaseDraw(pos, size);
    }
    #endregion

    #region 菱形Rhombus
    static void Rhombus(Vector2 size, Color background, Matrix matrix)
    {
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uTransform"].SetValue(matrix);
        effect.Parameters["uBackgroundColor"].SetValue(background.ToVector4());
        effect.Parameters["uSizeOver2"].SetValue(size * .5f);
        effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);
    }
    /// <summary>
    /// 绘制菱形
    /// </summary>
    public static void HasBorderRhombus(Vector2 pos, Vector2 size, Color background, float border, Color borderColor, Matrix matrix)
    {
        Rhombus(size, background, matrix);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uBorder"].SetValue(border);
        effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        effect.CurrentTechnique.Passes[nameof(HasBorderRhombus)].Apply();
        BaseDraw(pos, size);
    }

    /// <summary>
    /// 绘制菱形
    /// </summary>
    public static void NoBorderRhombus(Vector2 pos, Vector2 size, Color background, Matrix matrix)
    {
        Rhombus(size, background, matrix);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.CurrentTechnique.Passes[nameof(NoBorderRhombus)].Apply();
        BaseDraw(pos, size);
    }
    #endregion

    #region 梯形Trapezoid
    static void Trapezoid(Vector2 size, float bottomScaler, Color background, Matrix matrix)
    {
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uTransform"].SetValue(matrix);
        effect.Parameters["uBackgroundColor"].SetValue(background.ToVector4());
        effect.Parameters["uBottomScaler"].SetValue(bottomScaler);
        effect.Parameters["uSizeOver2"].SetValue(size * .5f);
        effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);
    }
    public static void HasBorderTrapezoid(Vector2 pos, Vector2 size, float bottomScaler, Color background, float border, Color borderColor, Matrix matrix)
    {
        Trapezoid(size, bottomScaler, background, matrix);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uBorder"].SetValue(border);
        effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        effect.CurrentTechnique.Passes[nameof(HasBorderTrapezoid)].Apply();
        BaseDraw(pos, size);
    }
    public static void NoBorderTrapezoid(Vector2 pos, Vector2 size, float bottomScaler, Color background, Matrix matrix)
    {
        Trapezoid(size, bottomScaler, background, matrix);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.CurrentTechnique.Passes[nameof(NoBorderTrapezoid)].Apply();
        BaseDraw(pos, size);
    }
    #endregion

    #region 平行四边形Parallelogram
    static void Parallelogram(Vector2 size, float bottomScaler, Color background, Matrix matrix)
    {
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uTransform"].SetValue(matrix);
        effect.Parameters["uBackgroundColor"].SetValue(background.ToVector4());
        effect.Parameters["uBottomScaler"].SetValue(bottomScaler);
        effect.Parameters["uSizeOver2"].SetValue(size * .5f);
        effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);
    }
    public static void HasBorderParallelogram(Vector2 pos, Vector2 size, float bottomScaler, Color background, float border, Color borderColor, Matrix matrix)
    {
        Parallelogram(size, bottomScaler, background, matrix);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uBorder"].SetValue(border);
        effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        effect.CurrentTechnique.Passes[nameof(HasBorderParallelogram)].Apply();
        BaseDraw(pos, size);
    }
    public static void NoBorderParallelogram(Vector2 pos, Vector2 size, float bottomScaler, Color background, Matrix matrix)
    {
        Parallelogram(size, bottomScaler, background, matrix);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.CurrentTechnique.Passes[nameof(NoBorderParallelogram)].Apply();
        BaseDraw(pos, size);
    }
    #endregion

    #region 等边三角形EquilateralTriangle
    static void EquilateralTriangle(float r, Color background, Matrix matrix, out Vector2 size)
    {
        Effect effect = ModAsset.SDFGraphics.Value;
        const float sqrt3 = 1.732050807f;
        size = new Vector2(1, sqrt3 * .5f) * r;
        effect.Parameters["uTransform"].SetValue(matrix);
        effect.Parameters["uBackgroundColor"].SetValue(background.ToVector4());
        effect.Parameters["uSizeOver2"].SetValue(size * .5f);
        effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);
    }
    public static void HasBorderEquilateralTriangle(Vector2 pos, float r, Color background, float border, Color borderColor, Matrix matrix)
    {
        EquilateralTriangle(r, background, matrix, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uBorder"].SetValue(border);
        effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        effect.CurrentTechnique.Passes[nameof(HasBorderEquilateralTriangle)].Apply();
        BaseDraw(pos, size);
    }
    public static void NoBorderEquilateralTriangle(Vector2 pos, float r, Color background, Matrix matrix)
    {
        EquilateralTriangle(r, background, matrix, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.CurrentTechnique.Passes[nameof(NoBorderEquilateralTriangle)].Apply();
        BaseDraw(pos, size);
    }
    #endregion

    #region 等腰三角形TriangleIsosceles
    static void TriangleIsosceles(Vector2 size, Color background, Matrix matrix)
    {
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uTransform"].SetValue(matrix);
        effect.Parameters["uBackgroundColor"].SetValue(background.ToVector4());
        effect.Parameters["uSizeOver2"].SetValue(size * .5f);//
        effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);
    }
    public static void HasBorderTriangleIsosceles(Vector2 pos, Vector2 size, Color background, float border, Color borderColor, Matrix matrix)
    {
        TriangleIsosceles(size, background, matrix);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uBorder"].SetValue(border);
        effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        effect.CurrentTechnique.Passes[nameof(HasBorderTriangleIsosceles)].Apply();
        BaseDraw(pos, size);
    }
    public static void NoBorderTriangleIsosceles(Vector2 pos, Vector2 size, Color background, Matrix matrix)
    {
        TriangleIsosceles(size, background, matrix);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.CurrentTechnique.Passes[nameof(NoBorderTriangleIsosceles)].Apply();
        BaseDraw(pos, size);
    }

    #endregion

    #region 三角形Triangle
    static void Triangle(Vector2 A, Vector2 B, Vector2 C, Color background, Matrix matrix, out Vector2 pos, out Vector2 size)
    {
        Vector2 min = Vector2.Min(A, Vector2.Min(B, C));
        Vector2 max = Vector2.Max(A, Vector2.Max(B, C));
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uTransform"].SetValue(matrix);
        effect.Parameters["uStart"].SetValue(A - min);
        effect.Parameters["uEnd"].SetValue(B - min);
        effect.Parameters["uAnother"].SetValue(C - min);
        effect.Parameters["uBackgroundColor"].SetValue(background.ToVector4());
        effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);
        size = max - min;
        pos = min;
    }
    public static void HasBorderTriangle(Vector2 A, Vector2 B, Vector2 C, Color background, float border, Color borderColor, Matrix matrix)
    {
        Triangle(A, B, C, background, matrix, out Vector2 pos, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uBorder"].SetValue(border);
        effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        effect.CurrentTechnique.Passes[nameof(HasBorderTriangle)].Apply();
        BaseDraw(pos, size);
    }
    public static void NoBorderTriangle(Vector2 A, Vector2 B, Vector2 C, Color background, Matrix matrix)
    {
        Triangle(A, B, C, background, matrix, out Vector2 pos, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.CurrentTechnique.Passes[nameof(NoBorderTriangle)].Apply();
        BaseDraw(pos, size);
    }
    #endregion

    #region 胶囊UnevenCapsule
    static void UnevenCapsule(float distance, float round1, float round2, Color background, Matrix matrix, out Vector2 size)
    {
        float w = Math.Max(round1, round2) * 2;
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uTransform"].SetValue(matrix);
        effect.Parameters["uSizeOver2"].SetValue(new Vector2(w * .5f, distance));
        effect.Parameters["uRound"].SetValue(new Vector4(round1, round2, 0, 0));
        effect.Parameters["uBackgroundColor"].SetValue(background.ToVector4());
        effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);
        size = new Vector2(w, distance + round1 + round2);
    }
    public static void HasBorderUnevenCapsule(Vector2 pos, float distance, float round1, float round2, Color background, float border, Color borderColor, Matrix matrix)
    {
        UnevenCapsule(distance, round1, round2, background, matrix, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uBorder"].SetValue(border);
        effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        effect.CurrentTechnique.Passes[nameof(HasBorderUnevenCapsule)].Apply();
        BaseDraw(pos, size);
    }
    public static void NoBorderUnevenCapsule(Vector2 pos, float distance, float round1, float round2, Color background, Matrix matrix)
    {
        UnevenCapsule(distance, round1, round2, background, matrix, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.CurrentTechnique.Passes[nameof(NoBorderUnevenCapsule)].Apply();
        BaseDraw(pos, size);
    }

    #endregion

    #region 正五边形Pentagon
    static void Pentagon(float r, Color background, Matrix matrix, out Vector2 size)
    {
        Effect effect = ModAsset.SDFGraphics.Value;
        const float c = 0.80902f; // cos(pi / 5)
        const float k = 1.17557f; // cos(pi / 10)/cos(pi / 5)
        effect.Parameters["uTransform"].SetValue(matrix);
        effect.Parameters["uBackgroundColor"].SetValue(background.ToVector4());
        effect.Parameters["uSizeOver2"].SetValue(new Vector2(r * k, r));
        effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);
        size = new Vector2(2 * r * k, (1 + 1 / c) * r);
    }
    public static void HasBorderPentagon(Vector2 pos, float r, Color background, float border, Color borderColor, Matrix matrix)
    {
        Pentagon(r, background, matrix, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uBorder"].SetValue(border);
        effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        effect.CurrentTechnique.Passes[nameof(HasBorderPentagon)].Apply();
        BaseDraw(pos, size);
    }
    public static void NoBorderPentagon(Vector2 pos, float r, Color background, Matrix matrix)
    {
        Pentagon(r, background, matrix, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.CurrentTechnique.Passes[nameof(NoBorderPentagon)].Apply();
        BaseDraw(pos, size);
    }
    #endregion

    #region 正六边形Hexagon
    static void Hexagon(float r, Color background, Matrix matrix, out Vector2 size)
    {
        Effect effect = ModAsset.SDFGraphics.Value;
        const float c = 1.1547f;//1 / cos(pi / 6)
        size = new Vector2(c, 1) * r;
        effect.Parameters["uTransform"].SetValue(matrix);
        effect.Parameters["uBackgroundColor"].SetValue(background.ToVector4());
        effect.Parameters["uSizeOver2"].SetValue(size);
        effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);
        size *= 2;
    }
    public static void HasBorderHexagon(Vector2 pos, float r, Color background, float border, Color borderColor, Matrix matrix)
    {
        Hexagon(r, background, matrix, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uBorder"].SetValue(border);
        effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        effect.CurrentTechnique.Passes[nameof(HasBorderHexagon)].Apply();
        BaseDraw(pos, size);
    }
    public static void NoBorderHexagon(Vector2 pos, float r, Color background, Matrix matrix)
    {
        Hexagon(r, background, matrix, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.CurrentTechnique.Passes[nameof(NoBorderHexagon)].Apply();
        BaseDraw(pos, size);
    }
    #endregion

    #region 正八边形Octogon
    static void Octogon(float r, Color background, Matrix matrix, out Vector2 size)
    {
        Effect effect = ModAsset.SDFGraphics.Value;
        size = new Vector2(r);
        effect.Parameters["uTransform"].SetValue(matrix);
        effect.Parameters["uBackgroundColor"].SetValue(background.ToVector4());
        effect.Parameters["uSizeOver2"].SetValue(size);
        effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);
        size *= 2;
    }
    public static void HasBorderOctogon(Vector2 pos, float r, Color background, float border, Color borderColor, Matrix matrix)
    {
        Octogon(r, background, matrix, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uBorder"].SetValue(border);
        effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        effect.CurrentTechnique.Passes[nameof(HasBorderOctogon)].Apply();
        BaseDraw(pos, size);
    }
    public static void NoBorderOctogon(Vector2 pos, float r, Color background, Matrix matrix)
    {
        Octogon(r, background, matrix, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.CurrentTechnique.Passes[nameof(NoBorderOctogon)].Apply();
        BaseDraw(pos, size);
    }
    #endregion

    #region 正N边形Ngon
    static void Ngon(float r, float N, Color background, Matrix matrix, out Vector2 size)
    {
        Effect effect = ModAsset.SDFGraphics.Value;
        float angle = MathHelper.Pi / N;
        float l = r / MathF.Cos(angle);
        float s = MathF.Sin(angle * (2 * ((int)N / 4) + 1));
        size = new Vector2(2 * l * s, (int)N % 2 == 0 ? (2 * r) : (r + l));
        effect.Parameters["uTransform"].SetValue(matrix);
        effect.Parameters["uBackgroundColor"].SetValue(background.ToVector4());
        effect.Parameters["uRound"].SetValue(new Vector4(N));
        effect.Parameters["uSizeOver2"].SetValue(new Vector2(size.X * .5f, r));
        effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);
    }
    public static void HasBorderNgon(Vector2 pos, float r, float N, Color background, float border, Color borderColor, Matrix matrix)
    {
        Ngon(r, N, background, matrix, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uBorder"].SetValue(border);
        effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        effect.CurrentTechnique.Passes[nameof(HasBorderNgon)].Apply();
        BaseDraw(pos, size);
    }
    public static void NoBorderNgon(Vector2 pos, float r, float N, Color background, Matrix matrix)
    {
        Ngon(r, N, background, matrix, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.CurrentTechnique.Passes[nameof(NoBorderNgon)].Apply();
        BaseDraw(pos, size);
    }
    #endregion

    #region 六芒星Hexagram
    static void Hexagram(float r, Color background, Matrix matrix, out Vector2 size)
    {
        Effect effect = ModAsset.SDFGraphics.Value;
        size = new Vector2(1.73205f, 2) * r;
        effect.Parameters["uTransform"].SetValue(matrix);
        effect.Parameters["uBackgroundColor"].SetValue(background.ToVector4());
        effect.Parameters["uSizeOver2"].SetValue(size * .5f);
        effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);
    }
    public static void HasBorderHexagram(Vector2 pos, float r, Color background, float border, Color borderColor, Matrix matrix)
    {
        Hexagram(r, background, matrix, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uBorder"].SetValue(border);
        effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        effect.CurrentTechnique.Passes[nameof(HasBorderHexagram)].Apply();
        BaseDraw(pos, size);
    }
    public static void NoBorderHexagram(Vector2 pos, float r, Color background, Matrix matrix)
    {
        Hexagram(r, background, matrix, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.CurrentTechnique.Passes[nameof(NoBorderHexagram)].Apply();
        BaseDraw(pos, size);
    }
    #endregion

    #region 五角星Star5
    static void Star5(float r, float rf, Color background, Matrix matrix, out Vector2 size)
    {
        Effect effect = ModAsset.SDFGraphics.Value;
        const float c = 0.80902f; // cos(pi / 5)
        const float c2 = 0.95106f; // cos(pi / 10)
        effect.Parameters["uTransform"].SetValue(matrix);
        effect.Parameters["uBackgroundColor"].SetValue(background.ToVector4());
        effect.Parameters["uSizeOver2"].SetValue(new Vector2(r * c2, r));
        effect.Parameters["uRound"].SetValue(new Vector4(rf));
        effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);
        size = new Vector2(2 * r * c2, (1 + 1 * c) * r);
    }
    public static void HasBorderStar5(Vector2 pos, float r, float rf, Color background, float border, Color borderColor, Matrix matrix)
    {
        Star5(r, rf, background, matrix, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uBorder"].SetValue(border);
        effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        effect.CurrentTechnique.Passes[nameof(HasBorderStar5)].Apply();
        BaseDraw(pos, size);
    }
    public static void NoBorderStar5(Vector2 pos, float r, float rf, Color background, Matrix matrix)
    {
        Star5(r, rf, background, matrix, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.CurrentTechnique.Passes[nameof(NoBorderStar5)].Apply();
        BaseDraw(pos, size);
    }
    #endregion

    #region X角星StarX
    static void StarX(float r, float N, float k, Color background, Matrix matrix, out Vector2 size)
    {
        Effect effect = ModAsset.SDFGraphics.Value;
        float angle = MathHelper.Pi / N;
        float l = r / MathF.Cos(angle);
        float s = MathF.Sin(angle * (2 * ((int)N / 4) + 1));
        size = new Vector2(2 * l * s, (int)N % 2 == 0 ? (2 * r) : (r + l));
        effect.Parameters["uTransform"].SetValue(matrix);
        effect.Parameters["uBackgroundColor"].SetValue(background.ToVector4());
        effect.Parameters["uRound"].SetValue(new Vector4(N, k, 0, 0));
        effect.Parameters["uSizeOver2"].SetValue(size * .5f);
        effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);
    }
    public static void HasBorderStarX(Vector2 pos, float r, float N, float k, Color background, float border, Color borderColor, Matrix matrix)
    {
        StarX(r, N, k, background, matrix, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uBorder"].SetValue(border);
        effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        effect.CurrentTechnique.Passes[nameof(HasBorderStarX)].Apply();
        BaseDraw(pos, size);
    }
    public static void NoBorderStarX(Vector2 pos, float r, float N, float k, Color background, Matrix matrix)
    {
        StarX(r, N, k, background, matrix, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.CurrentTechnique.Passes[nameof(NoBorderStarX)].Apply();
        BaseDraw(pos, size);
    }
    #endregion

    #region 饼图Pie
    static void Pie(float r, float angle, Color background, Matrix matrix, out Vector2 size)
    {
        float s = MathF.Sin(angle);
        float c = MathF.Cos(angle);
        size = new Vector2((c < 0 ? 1 : s) * r * 2, (c < 0 ? 1 - c : 1) * r);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uTransform"].SetValue(matrix);
        effect.Parameters["uBackgroundColor"].SetValue(background.ToVector4());
        effect.Parameters["uRound"].SetValue(new Vector3(s, c, r));
        effect.Parameters["uSizeOver2"].SetValue(size * .5f);
        effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);
    }
    public static void HasBorderPie(Vector2 pos, float r, float angle, Color background, float border, Color borderColor, Matrix matrix)
    {
        Pie(r, angle, background, matrix, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uBorder"].SetValue(border);
        effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        effect.CurrentTechnique.Passes[nameof(HasBorderPie)].Apply();
        BaseDraw(pos, size);
    }
    public static void NoBorderPie(Vector2 pos, float r, float angle, Color background, Matrix matrix)
    {
        Pie(r, angle, background, matrix, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.CurrentTechnique.Passes[nameof(NoBorderPie)].Apply();
        BaseDraw(pos, size);
    }
    #endregion

    #region 弓形CutDisk
    static void CutDisk(float r, float k, Color background, Matrix matrix, out Vector2 size)
    {
        size = new Vector2((k > 0 ? 2 : MathF.Sqrt(1 - k * k) * 2), 1 + k) * r;
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uTransform"].SetValue(matrix);
        effect.Parameters["uBackgroundColor"].SetValue(background.ToVector4());
        effect.Parameters["uRound"].SetValue(new Vector2(k, r));
        effect.Parameters["uSizeOver2"].SetValue(size * .5f);
        effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);
    }

    public static void HasBorderCutDisk(Vector2 pos, float r, float k, Color background, float border, Color borderColor, Matrix matrix)
    {
        CutDisk(r, k, background, matrix, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uBorder"].SetValue(border);
        effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        effect.CurrentTechnique.Passes[nameof(HasBorderCutDisk)].Apply();
        BaseDraw(pos, size);
    }
    public static void NoBorderCutDisk(Vector2 pos, float r, float k, Color background, Matrix matrix)
    {
        CutDisk(r, k, background, matrix, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.CurrentTechnique.Passes[nameof(NoBorderCutDisk)].Apply();
        BaseDraw(pos, size);
    }
    #endregion

    #region 弦Arc
    static void Arc(float r, float angle, float width, Color background, Matrix matrix, out Vector2 size)
    {
        float s = MathF.Sin(angle);
        float c = MathF.Cos(angle);
        size = new Vector2((c < 0 ? 1 : s) * r * 2, (1 - c) * r);
        size += new Vector2(width * 2);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uTransform"].SetValue(matrix);
        effect.Parameters["uBackgroundColor"].SetValue(background.ToVector4());
        effect.Parameters["uRound"].SetValue(new Vector4(s, c, r, width));
        effect.Parameters["uSizeOver2"].SetValue(size * .5f);
        effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);
    }
    public static void HasBorderArc(Vector2 pos, float r, float angle, float width, Color background, float border, Color borderColor, Matrix matrix)
    {
        Arc(r, angle, width, background, matrix, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uBorder"].SetValue(border);
        effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        effect.CurrentTechnique.Passes[nameof(HasBorderArc)].Apply();
        BaseDraw(pos, size);
    }
    public static void NoBorderArc(Vector2 pos, float r, float angle, float width, Color background, Matrix matrix)
    {
        Arc(r, angle, width, background, matrix, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.CurrentTechnique.Passes[nameof(NoBorderArc)].Apply();
        BaseDraw(pos, size);
    }
    #endregion

    #region 环Ring
    static void Ring(float r, float angle, float width, Color background, Matrix matrix, out Vector2 size)
    {
        float s = MathF.Sin(angle);
        float c = MathF.Cos(angle);
        size = new Vector2((c < 0 ? 1 : s) * 2 * (r + width * .5f), r + width * .5f - (r - MathF.Sign(c) * width * .5f) * c);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uTransform"].SetValue(matrix);
        effect.Parameters["uBackgroundColor"].SetValue(background.ToVector4());
        effect.Parameters["uRound"].SetValue(new Vector4(r, width, c, s));
        effect.Parameters["uSizeOver2"].SetValue(size * .5f);
        effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);
    }
    public static void HasBorderRing(Vector2 pos, float r, float angle, float width, Color background, float border, Color borderColor, Matrix matrix)
    {
        Ring(r, angle, width, background, matrix, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uBorder"].SetValue(border);
        effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        effect.CurrentTechnique.Passes[nameof(HasBorderRing)].Apply();
        BaseDraw(pos, size);
    }
    public static void NoBorderRing(Vector2 pos, float r, float angle, float width, Color background, Matrix matrix)
    {
        Ring(r, angle, width, background, matrix, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.CurrentTechnique.Passes[nameof(NoBorderRing)].Apply();
        BaseDraw(pos, size);
    }
    #endregion

    #region 马蹄铁Horseshoe
    static void Horseshoe(float r, float angle, float width, float length, Color background, Matrix matrix, out Vector2 size)
    {
        float s = MathF.Sin(angle);
        float c = MathF.Cos(angle);

        float sizeX = c > 0 ? (2 * r + width) : (s * (2 * r + width) - c * length * 2);
        float L = c > 0 ? MathF.Min(length, MathF.Tan(angle) * (r + width * .5f)) : length;
        float top = c * (r + MathF.Sign(c) * 0.5f * width) + s * L;
        float bottom = -r - 0.5f * width;
        size = new Vector2(sizeX, top - bottom);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uTransform"].SetValue(matrix);
        effect.Parameters["uBackgroundColor"].SetValue(background.ToVector4());
        effect.Parameters["uRound"].SetValue(new Vector4(r, length, c, s));
        effect.Parameters["uLineWidth"].SetValue(width * .5f);
        effect.Parameters["uSizeOver2"].SetValue(new Vector2(sizeX * .5f, -bottom));
        effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);
    }

    public static void HasBorderHorseshoe(Vector2 pos, float r, float angle, float width, float length, Color background, float border, Color borderColor, Matrix matrix)
    {
        Horseshoe(r, angle, width, length, background, matrix, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uBorder"].SetValue(border);
        effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        effect.CurrentTechnique.Passes[nameof(HasBorderHorseshoe)].Apply();
        BaseDraw(pos, size);
    }
    public static void NoBorderHorseshoe(Vector2 pos, float r, float angle, float width, float length, Color background, Matrix matrix)
    {
        Horseshoe(r, angle, width, length, background, matrix, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.CurrentTechnique.Passes[nameof(NoBorderHorseshoe)].Apply();
        BaseDraw(pos, size);
    }
    #endregion

    #region 鱼鳔Vesica
    static void Vesica(float r, float distance, Color background, Matrix matrix, out Vector2 size)
    {
        size = new Vector2(r - distance, distance > 0 ? MathF.Sqrt(r * r - distance * distance) : r);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uTransform"].SetValue(matrix);
        effect.Parameters["uBackgroundColor"].SetValue(background.ToVector4());
        effect.Parameters["uRound"].SetValue(new Vector2(r, distance));
        effect.Parameters["uSizeOver2"].SetValue(size);
        effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);
        size *= 2;
    }
    public static void HasBorderVesica(Vector2 pos, float r, float distance, Color background, float border, Color borderColor, Matrix matrix)
    {
        Vesica(r, distance, background, matrix, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uBorder"].SetValue(border);
        effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        effect.CurrentTechnique.Passes[nameof(HasBorderVesica)].Apply();
        BaseDraw(pos, size);
    }
    public static void NoBorderVesica(Vector2 pos, float r, float distance, Color background, Matrix matrix)
    {
        Vesica(r, distance, background, matrix, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.CurrentTechnique.Passes[nameof(NoBorderVesica)].Apply();
        BaseDraw(pos, size);
    }
    #endregion

    #region 正交鱼鳔OrientedVesica
    static void OrientedVesica(Vector2 pos1, Vector2 pos2, float r, Color background, Matrix matrix, out Vector2 pos, out Vector2 size)
    {
        Vector2 C = (pos1 + pos2) * .5f;
        Vector2 D = pos1 - pos2;
        float k = MathF.Sqrt(r * r / Vector2.DistanceSquared(pos1, pos2) * 4 - 1);
        Vector2 N = new Vector2(-D.Y, D.X) * k;
        Vector2 P1 = C + N * .5f;
        Vector2 P2 = C - N * .5f;

        D = new Vector2(MathF.Abs(D.X), MathF.Abs(D.Y));
        N = new Vector2(MathF.Abs(N.X), MathF.Abs(N.Y));

        float sizeX = D.Y < D.X * k ? (2 * r - D.X) : N.X;
        float sizeY = D.X < D.Y * k ? (2 * r - D.Y) : N.Y;

        size = new Vector2(sizeX, sizeY);
        pos = C - size * .5f;

        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uTransform"].SetValue(matrix);
        effect.Parameters["uBackgroundColor"].SetValue(background.ToVector4());
        effect.Parameters["uRound"].SetValue(r - D.Length() * .5f);
        effect.Parameters["uStart"].SetValue(P1 - pos);
        effect.Parameters["uEnd"].SetValue(P2 - pos);
        effect.Parameters["uSizeOver2"].SetValue(size * .5f);
        effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);

    }
    public static void HasBorderOrientedVesica(Vector2 pos1, Vector2 pos2, float r, Color background, float border, Color borderColor, Matrix matrix)
    {
        OrientedVesica(pos1, pos2, r, background, matrix, out Vector2 pos, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uBorder"].SetValue(border);
        effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        effect.CurrentTechnique.Passes[nameof(HasBorderOrientedVesica)].Apply();
        BaseDraw(pos, size);
    }
    public static void NoBorderOrientedVesica(Vector2 pos1, Vector2 pos2, float r, Color background, Matrix matrix)
    {
        OrientedVesica(pos1, pos2, r, background, matrix, out Vector2 pos, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.CurrentTechnique.Passes[nameof(NoBorderOrientedVesica)].Apply();
        BaseDraw(pos, size);
    }
    #endregion

    #region 月Moon
    static void Moon(float r1, float r2, float x, Color background, Matrix matrix, out Vector2 size)
    {
        float coordX = x == 0 ? float.NaN : (x * x - r2 * r2 + r1 * r1) / (2 * x);

        float offset = float.IsNaN(coordX) || MathF.Abs(coordX) > r1 ? r1 : MathF.Abs(coordX);
        size = new Vector2(offset + r1, 2 * r1);

        Vector2 orig = coordX < 0 ? new Vector2(-coordX, r1) : new Vector2(r1);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uTransform"].SetValue(matrix);
        effect.Parameters["uBackgroundColor"].SetValue(background.ToVector4());
        effect.Parameters["uRound"].SetValue(new Vector3(r1, r2, x));
        effect.Parameters["uSizeOver2"].SetValue(orig);
        effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);
    }
    public static void HasBorderMoon(Vector2 pos, float r1, float r2, float x, Color background, float border, Color borderColor, Matrix matrix)
    {
        Moon(r1, r2, x, background, matrix, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uBorder"].SetValue(border);
        effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        effect.CurrentTechnique.Passes[nameof(HasBorderMoon)].Apply();
        BaseDraw(pos, size);

    }
    public static void NoBorderMoon(Vector2 pos, float r1, float r2, float x, Color background, Matrix matrix)
    {
        Moon(r1, r2, x, background, matrix, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.CurrentTechnique.Passes[nameof(NoBorderMoon)].Apply();
        BaseDraw(pos, size);
    }
    #endregion

    #region 曲边四芒星CircleCross
    static void CircleCross(Vector2 baseSize, float r, Color background, Matrix matrix, out Vector2 size)
    {
        size = baseSize + new Vector2(2 * r);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uTransform"].SetValue(matrix);
        effect.Parameters["uBackgroundColor"].SetValue(background.ToVector4());
        effect.Parameters["uRound"].SetValue(new Vector3(r, baseSize.Y / baseSize.X, baseSize.X * 0.5f));//baseSize.Y / baseSize.X
        effect.Parameters["uSizeOver2"].SetValue(size * .5f);
        effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);
    }


    public static void HasBorderCircleCross(Vector2 pos, Vector2 baseSize, float r, Color background, float border, Color borderColor, Matrix matrix)
    {
        CircleCross(baseSize, r, background, matrix, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uBorder"].SetValue(border);
        effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        effect.CurrentTechnique.Passes[nameof(HasBorderCircleCross)].Apply();
        BaseDraw(pos, size);
    }
    public static void NoBorderCircleCross(Vector2 pos, Vector2 baseSize, float r, Color background, Matrix matrix)
    {
        CircleCross(baseSize, r, background, matrix, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.CurrentTechnique.Passes[nameof(NoBorderCircleCross)].Apply();
        BaseDraw(pos, size);
    }
    #endregion

    #region 蛋Egg
    static void Egg(float ra, float rb, float height, Color background, Matrix matrix, out Vector2 size)
    {
        size = new Vector2(ra * 2, ra + rb + height);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uTransform"].SetValue(matrix);
        effect.Parameters["uBackgroundColor"].SetValue(background.ToVector4());
        effect.Parameters["uRound"].SetValue(new Vector3(ra, rb, height));//baseSize.Y / baseSize.X
        effect.Parameters["uSizeOver2"].SetValue(new Vector2(ra));
        effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);
    }
    public static void HasBorderEgg(Vector2 pos, float ra, float rb, float height, Color background, float border, Color borderColor, Matrix matrix)
    {
        Egg(ra, rb, height, background, matrix, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uBorder"].SetValue(border);
        effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        effect.CurrentTechnique.Passes[nameof(HasBorderEgg)].Apply();
        BaseDraw(pos, size);
    }
    public static void NoBorderEgg(Vector2 pos, float ra, float rb, float height, Color background, Matrix matrix)
    {
        Egg(ra, rb, height, background, matrix, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.CurrentTechnique.Passes[nameof(NoBorderEgg)].Apply();
        BaseDraw(pos, size);
    }
    #endregion

    #region 心Heart
    static void Heart(float sizeX, Color background, Matrix matrix, out Vector2 size)
    {
        const float k = (0.75f + root2Over2 * .5f) / (0.5f + root2Over2);
        size = new Vector2(1, k) * sizeX;
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uTransform"].SetValue(matrix);
        effect.Parameters["uBackgroundColor"].SetValue(background.ToVector4());
        effect.Parameters["uSizeOver2"].SetValue(new Vector2(sizeX * .5f, 0));
        effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);
    }
    public static void HasBorderHeart(Vector2 pos, float sizeX, Color background, float border, Color borderColor, Matrix matrix)
    {
        Heart(sizeX, background, matrix, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uBorder"].SetValue(border);
        effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        effect.CurrentTechnique.Passes[nameof(HasBorderHeart)].Apply();
        BaseDraw(pos, size);
    }
    public static void NoBorderHeart(Vector2 pos, float sizeX, Color background, Matrix matrix)
    {
        Heart(sizeX, background, matrix, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.CurrentTechnique.Passes[nameof(NoBorderHeart)].Apply();
        BaseDraw(pos, size);
    }
    #endregion

    #region 十字Cross/Plus
    static void Plus(float d, float c, float r, Color background, Matrix matrix, out Vector2 size)
    {
        float s = c > r ? d - r : (c - MathF.Sqrt(r * r - c * c));
        size = new Vector2(2 * s);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uTransform"].SetValue(matrix);
        effect.Parameters["uBackgroundColor"].SetValue(background.ToVector4());
        effect.Parameters["uSizeOver2"].SetValue(new Vector2(s));
        effect.Parameters["uRound"].SetValue(new Vector3(d, c, r));
        effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);
    }
    public static void HasBorderPlus(Vector2 pos, float d, float c, float r, Color background, float border, Color borderColor, Matrix matrix)
    {
        Plus(d, c, r, background, matrix, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uBorder"].SetValue(border);
        effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        effect.CurrentTechnique.Passes[nameof(HasBorderPlus)].Apply();
        BaseDraw(pos, size);
    }
    public static void NoBorderPlus(Vector2 pos, float d, float c, float r, Color background, Matrix matrix)
    {
        Plus(d, c, r, background, matrix, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.CurrentTechnique.Passes[nameof(NoBorderPlus)].Apply();
        BaseDraw(pos, size);
    }
    #endregion

    #region 叉号RoundedX/Cross
    static void Cross(Vector2 pos, float width, float round, Color backgroundColor, Matrix matrix, out Vector2 size)
    {
        const float innerShrinkage = 1;
        size = new Vector2(width + round * 2);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uTransform"].SetValue(matrix);
        effect.Parameters["uSizeOver2"].SetValue(size * .5f);
        effect.Parameters["uRound"].SetValue(new Vector2(round, width));
        effect.Parameters["uBackgroundColor"].SetValue(backgroundColor.ToVector4());
        effect.Parameters["uInnerShrinkage"].SetValue(innerShrinkage);
        effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);
    }
    /// <summary>
    /// 绘制叉号
    /// </summary>
    public static void HasBorderCross(Vector2 pos, float width, float round, Color backgroundColor, float border, Color borderColor, Matrix matrix)
    {
        width *= .5f;
        Cross(pos, width, round, backgroundColor, matrix, out Vector2 size);
        const float innerShrinkage = 1;
        pos -= new Vector2(innerShrinkage);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uBorder"].SetValue(border);
        effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        effect.CurrentTechnique.Passes[nameof(HasBorderCross)].Apply();
        BaseDraw(pos + new Vector2(width * .5f - round), size);
    }

    /// <summary>
    /// 绘制叉号
    /// </summary>
    public static void NoBorderCross(Vector2 pos, float width, float round, Color backgroundColor, Matrix matrix)
    {
        width *= .5f;
        Cross(pos, width, round, backgroundColor, matrix, out Vector2 size);
        const float innerShrinkage = 1;
        pos -= new Vector2(innerShrinkage);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.CurrentTechnique.Passes[nameof(NoBorderCross)].Apply();
        BaseDraw(pos + new Vector2(width * .5f - round), size);

    }
    #endregion

    #region 多边形Polygon
    static void Polygon(Vector2[] vecs, Color background, Matrix matrix, out Vector2 pos, out Vector2 size)
    {
        Vector2 topLeft = vecs[0];
        Vector2 bottomRight = vecs[0];

        for (int n = 1; n < vecs.Length; n++)
        {
            topLeft = Vector2.Min(topLeft, vecs[n]);
            bottomRight = Vector2.Max(bottomRight, vecs[n]);
        }
        pos = topLeft;
        size = bottomRight - topLeft;

        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uTransform"].SetValue(matrix);
        effect.Parameters["uSizeOver2"].SetValue(size * .5f);
        effect.Parameters["uVectors"].SetValue((from v in vecs select v - topLeft).ToArray());
        effect.Parameters["uCurrentPointCount"].SetValue(vecs.Length);
        effect.Parameters["uBackgroundColor"].SetValue(background.ToVector4());
        effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);
    }


    public static void HasBorderPolygon(IEnumerable<Vector2> vecs, Color background, float border, Color borderColor, Matrix matrix)
    {
        Polygon(vecs.ToArray(), background, matrix, out Vector2 pos, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uBorder"].SetValue(border);
        effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        effect.CurrentTechnique.Passes[nameof(HasBorderPolygon)].Apply();
        BaseDraw(pos, size);
    }

    public static void NoBorderPolygon(IEnumerable<Vector2> vecs, Color background, Matrix matrix)
    {
        Polygon(vecs.ToArray(), background, matrix, out Vector2 pos, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.CurrentTechnique.Passes[nameof(NoBorderPolygon)].Apply();
        BaseDraw(pos, size);
    }

    public static void HasBorderPolygon(Vector2 position, IEnumerable<Vector2> offsets, Color background, float border, Color borderColor, Matrix matrix)
        => HasBorderPolygon(from offset in offsets select offset + position, background, border, borderColor, matrix);

    public static void NoBorderPolygon(Vector2 position, IEnumerable<Vector2> offsets, Color background, Matrix matrix)
        => NoBorderPolygon(from offset in offsets select offset + position, background, matrix);

    public static void HasBorderPolygon(Vector2 position, Vector2 unit, IEnumerable<Vector2> percents, Color background, float border, Color borderColor, Matrix matrix)
        => HasBorderPolygon(position, from percent in percents select unit * percent, background, border, borderColor, matrix);

    public static void NoBorderPolygon(Vector2 position, Vector2 unit, IEnumerable<Vector2> percents, Color background, Matrix matrix)
        => NoBorderPolygon(position, from percent in percents select unit * percent, background, matrix);

    #endregion

    #region 椭圆Ellipse
    static void Ellipse(Vector2 r, Color background, Matrix matrix)
    {
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uTransform"].SetValue(matrix);
        effect.Parameters["uSizeOver2"].SetValue(r);
        effect.Parameters["uBackgroundColor"].SetValue(background.ToVector4());
        effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);
    }
    public static void HasBorderEllipse(Vector2 pos, Vector2 r, Color background, float border, Color borderColor, Matrix matrix)
    {
        Ellipse(r, background, matrix);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uBorder"].SetValue(border);
        effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        effect.CurrentTechnique.Passes[nameof(HasBorderEllipse)].Apply();
        BaseDraw(pos, 2 * r);
    }
    public static void NoBorderEllipse(Vector2 pos, Vector2 r, Color background, Matrix matrix)
    {
        Ellipse(r, background, matrix);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.CurrentTechnique.Passes[nameof(NoBorderEllipse)].Apply();
        BaseDraw(pos, 2 * r);
    }
    #endregion

    #region 抛物线Parabola
    static void Parabola(Vector2 size, Color background, Matrix matrix)
    {
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uTransform"].SetValue(matrix);
        effect.Parameters["uSizeOver2"].SetValue(new Vector2(size.X * .5f, 0));
        effect.Parameters["uRound"].SetValue(4 * size.Y / (size.X * size.X));
        effect.Parameters["uBackgroundColor"].SetValue(background.ToVector4());
        effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);
    }
    public static void HasBorderParabola(Vector2 pos, Vector2 size, Color background, float border, Color borderColor, Matrix matrix)
    {
        Parabola(size, background, matrix);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uBorder"].SetValue(border);
        effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        effect.CurrentTechnique.Passes[nameof(HasBorderParabola)].Apply();
        BaseDraw(pos, size);
    }
    public static void NoBorderParabola(Vector2 pos, Vector2 size, Color background, float border, Color borderColor, Matrix matrix)
    {
        Parabola(size, background, matrix);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uBorder"].SetValue(border);
        effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        effect.CurrentTechnique.Passes[nameof(NoBorderParabola)].Apply();
        BaseDraw(pos, size);
    }
    #endregion

    #region 抛物线段ParabolaSegment
    static void ParabolaSegment(Vector2 size, float height, Color background, Matrix matrix)
    {
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uTransform"].SetValue(matrix);
        effect.Parameters["uSizeOver2"].SetValue(new Vector2(size.X * .5f, size.Y - height));
        effect.Parameters["uRound"].SetValue(new Vector2(0.5f * size.X * MathF.Sqrt(height / size.Y), height));
        effect.Parameters["uBackgroundColor"].SetValue(background.ToVector4());
        effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);
    }
    public static void HasBorderParabolaSegment(Vector2 pos, Vector2 size, float height, Color background, float border, Color borderColor, Matrix matrix)
    {
        ParabolaSegment(size, height, background, matrix);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uBorder"].SetValue(border);
        effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        effect.CurrentTechnique.Passes[nameof(HasBorderParabolaSegment)].Apply();
        BaseDraw(pos, size);
    }
    public static void NoBorderParabolaSegment(Vector2 pos, Vector2 size, float height, Color background, Matrix matrix)
    {
        ParabolaSegment(size, height, background, matrix);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.CurrentTechnique.Passes[nameof(NoBorderParabolaSegment)].Apply();
        BaseDraw(pos, size);
    }
    #endregion

    #region 二阶贝塞尔曲线QuadraticBezier
    static void QuadraticBezier(Vector2 start, Vector2 control, Vector2 end, float width, Color background, Matrix matrix, out Vector2 pos, out Vector2 size)
    {
        Vector2 topLeft = start;
        Vector2 bottomRight = start;
        topLeft = Vector2.Min(topLeft, end);
        bottomRight = Vector2.Max(bottomRight, end);
        float t = (start.X - control.X) / (start.X + end.X - 2 * control.X);
        if (t * (1 - t) > 0)
        {
            Vector2 boundary = (1 - t) * (1 - t) * start + 2 * t * (1 - t) * control + t * t * end;
            topLeft = Vector2.Min(topLeft, boundary);
            bottomRight = Vector2.Max(bottomRight, boundary);
        }
        t = (start.Y - control.Y) / (start.Y + end.Y - 2 * control.Y);
        if (t * (1 - t) > 0)
        {
            Vector2 boundary = (1 - t) * (1 - t) * start + 2 * t * (1 - t) * control + t * t * end;
            topLeft = Vector2.Min(topLeft, boundary);
            bottomRight = Vector2.Max(bottomRight, boundary);
        }

        pos = topLeft - new Vector2(width);
        size = bottomRight - topLeft + new Vector2(width * 2);

        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uTransform"].SetValue(matrix);
        effect.Parameters["uBackgroundColor"].SetValue(background.ToVector4());
        effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);
        effect.Parameters["uStart"].SetValue(start - pos);
        effect.Parameters["uAnother"].SetValue(control - pos);
        effect.Parameters["uEnd"].SetValue(end - pos);
        effect.Parameters["uLineWidth"].SetValue(width);

    }
    public static void HasBorderQuadraticBezier(Vector2 start, Vector2 control, Vector2 end, float width, Color background, float border, Color borderColor, Matrix matrix)
    {
        QuadraticBezier(start, control, end, width, background, matrix, out Vector2 pos, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uBorder"].SetValue(border);
        effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        effect.CurrentTechnique.Passes[nameof(HasBorderQuadraticBezier)].Apply();
        BaseDraw(pos, size);
    }
    public static void NoBorderQuadraticBezier(Vector2 start, Vector2 control, Vector2 end, float width, Color background, Matrix matrix)
    {
        QuadraticBezier(start, control, end, width, background, matrix, out Vector2 pos, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.CurrentTechnique.Passes[nameof(NoBorderQuadraticBezier)].Apply();
        BaseDraw(pos, size);
    }
    #endregion

    #region 圆头十字BlobbyCross
    static void BlobbyCross(float s, float k, float r, Color background, Matrix matrix, out Vector2 size)
    {
        float u = r > 0 ? (s + r) : k switch
        {
            > root2Over2 * .5f => root2Over2 / k - 1,
            < root2Over2 * -.5f => -(2 * k * (k - root2Over2) + .25f) / (4 * k * root2Over2),
            _ => 1,
        } * s;


        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uTransform"].SetValue(matrix);
        effect.Parameters["uBackgroundColor"].SetValue(background.ToVector4());
        effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);
        effect.Parameters["uRound"].SetValue(new Vector3(s, k, r));
        effect.Parameters["uSizeOver2"].SetValue(new Vector2(u));


        size = new Vector2(u * 2);
    }
    public static void HasBorderBlobbyCross(Vector2 pos, float s, float k, float r, Color background, float border, Color borderColor, Matrix matrix)
    {
        BlobbyCross(s, k, r, background, matrix, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uBorder"].SetValue(border);
        effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        effect.CurrentTechnique.Passes[nameof(HasBorderBlobbyCross)].Apply();
        BaseDraw(pos, size);
    }
    public static void NoBorderBlobbyCross(Vector2 pos, float s, float k, float r, Color background, Matrix matrix)
    {
        BlobbyCross(s, k, r, background, matrix, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.CurrentTechnique.Passes[nameof(NoBorderBlobbyCross)].Apply();
        BaseDraw(pos, size);
    }
    #endregion

    #region 隧道Tunnel
    static void Tunnel(Vector2 wh, Color background, Matrix matrix, out Vector2 size)
    {
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uTransform"].SetValue(matrix);
        effect.Parameters["uBackgroundColor"].SetValue(background.ToVector4());
        effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);
        effect.Parameters["uSizeOver2"].SetValue(wh);

        size = new Vector2(2 * wh.X, wh.X + wh.Y);
    }
    public static void HasBorderTunnel(Vector2 pos, Vector2 wh, Color background, float border, Color borderColor, Matrix matrix)
    {
        Tunnel(wh, background, matrix, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uBorder"].SetValue(border);
        effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        effect.CurrentTechnique.Passes[nameof(HasBorderTunnel)].Apply();
        BaseDraw(pos, size);
    }
    public static void NoBorderTunnel(Vector2 pos, Vector2 wh, Color background, Matrix matrix)
    {
        Tunnel(wh, background, matrix, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.CurrentTechnique.Passes[nameof(NoBorderTunnel)].Apply();
        BaseDraw(pos, size);
    }
    #endregion

    #region 楼梯Stairs
    static void Stairs(Vector2 unit, float count, Color background, Matrix matrix, out Vector2 size)
    {
        size = unit * count;
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uTransform"].SetValue(matrix);
        effect.Parameters["uBackgroundColor"].SetValue(background.ToVector4());
        effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);
        effect.Parameters["uRound"].SetValue(count);
        effect.Parameters["uSizeOver2"].SetValue(unit);
    }
    public static void HasBorderStairs(Vector2 pos, Vector2 unit, float count, Color background, float border, Color borderColor, Matrix matrix)
    {
        Stairs(unit, count, background, matrix, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uBorder"].SetValue(border);
        effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        effect.CurrentTechnique.Passes[nameof(HasBorderStairs)].Apply();
        BaseDraw(pos, size);
    }
    public static void NoBorderStairs(Vector2 pos, Vector2 unit, float count, Color background, Matrix matrix)
    {
        Stairs(unit, count, background, matrix, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.CurrentTechnique.Passes[nameof(NoBorderStairs)].Apply();
        BaseDraw(pos, size);
    }
    #endregion

    #region 四次方圆QuadraticCircle
    static void QuadraticCircle(float r, Color background, Matrix matrix)
    {
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uTransform"].SetValue(matrix);
        effect.Parameters["uBackgroundColor"].SetValue(background.ToVector4());
        effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);
        effect.Parameters["uSizeOver2"].SetValue(new Vector2(r));
    }

    public static void HasBorderQuadraticCircle(Vector2 pos, float r, Color background, float border, Color borderColor, Matrix matrix)
    {
        QuadraticCircle(r, background, matrix);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uBorder"].SetValue(border);
        effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        effect.CurrentTechnique.Passes[nameof(HasBorderQuadraticCircle)].Apply();
        BaseDraw(pos, new Vector2(r * 2));
    }
    public static void NoBorderQuadraticCircle(Vector2 pos, float r, Color background, Matrix matrix)
    {
        QuadraticCircle(r, background, matrix);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.CurrentTechnique.Passes[nameof(NoBorderQuadraticCircle)].Apply();
        BaseDraw(pos, new Vector2(r * 2));
    }
    #endregion

    #region 双曲线Hyperbola
    static void Hyperbola(Vector2 size, Color background, Matrix matrix)
    {
        float k = (size.X * size.X - size.Y * size.Y) * .125f;//MathF.Sqrt((size.X * size.X - size.Y * size.Y) * .25f - 1);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uTransform"].SetValue(matrix);
        effect.Parameters["uBackgroundColor"].SetValue(background.ToVector4());
        effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);
        effect.Parameters["uSizeOver2"].SetValue(size * .5f);
        effect.Parameters["uRound"].SetValue(k);
    }
    public static void HasBorderHyperbola(Vector2 pos, Vector2 size, Color background, float border, Color borderColor, Matrix matrix)
    {
        Hyperbola(size, background, matrix);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uBorder"].SetValue(border);
        effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        effect.CurrentTechnique.Passes[nameof(HasBorderHyperbola)].Apply();
        BaseDraw(pos, size);
    }
    public static void NoBorderHyperbola(Vector2 pos, Vector2 size, Color background, Matrix matrix)
    {
        Hyperbola(size, background, matrix);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.CurrentTechnique.Passes[nameof(NoBorderHyperbola)].Apply();
        BaseDraw(pos, size);
    }
    #endregion

    #region 酷酷S CoolS
    static void CoolS(float height, Color background, Matrix matrix)
    {

        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uTransform"].SetValue(matrix);
        effect.Parameters["uBackgroundColor"].SetValue(background.ToVector4());
        effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);
        effect.Parameters["uSizeOver2"].SetValue(new Vector2(.2f, .5f) * height);
    }
    public static void HasBorderCoolS(Vector2 pos, float height, Color background, float border, Color borderColor, Matrix matrix)
    {
        CoolS(height, background, matrix);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uBorder"].SetValue(border);
        effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        effect.CurrentTechnique.Passes[nameof(HasBorderCoolS)].Apply();
        BaseDraw(pos, new Vector2(.4f, 1f) * height);
    }
    public static void NoBorderCoolS(Vector2 pos, float height, Color background, Matrix matrix)
    {
        CoolS(height, background, matrix);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.CurrentTechnique.Passes[nameof(NoBorderCoolS)].Apply();
        BaseDraw(pos, new Vector2(.4f, 1f) * height);
    }
    #endregion

    #region 圆波CircleWave
    static void CircleWave(float width, float k, float r, float lineWidth, Color background, Matrix matrix, out Vector2 size)
    {
        float height = (1 - MathF.Cos(5 / 6.0f * k * MathHelper.Pi)) * 2 * r;
        size = new Vector2(width, height) + new Vector2(2 * lineWidth);

        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uTransform"].SetValue(matrix);
        effect.Parameters["uBackgroundColor"].SetValue(background.ToVector4());
        effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);
        effect.Parameters["uSizeOver2"].SetValue(new Vector2(width * .5f + lineWidth, height - r + lineWidth));
        effect.Parameters["uRound"].SetValue(new Vector2(k,r));

        effect.Parameters["uLineWidth"].SetValue(lineWidth);
    }

    public static void HasBorderCircleWave(Vector2 pos, float width, float k, float r, float lineWidth, Color background, float border, Color borderColor, Matrix matrix)
    {
        CircleWave(width, k, r, lineWidth, background, matrix, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.Parameters["uBorder"].SetValue(border);
        effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        effect.CurrentTechnique.Passes[nameof(HasBorderCircleWave)].Apply();
        BaseDraw(pos, size);
    }

    public static void NoBorderCircleWave(Vector2 pos, float width, float k, float r, float lineWidth, Color background, Matrix matrix)
    {
        CircleWave(width, k, r, lineWidth, background, matrix, out Vector2 size);
        Effect effect = ModAsset.SDFGraphics.Value;
        effect.CurrentTechnique.Passes[nameof(NoBorderCircleWave)].Apply();
        BaseDraw(pos, size);
    }

    #endregion
}
