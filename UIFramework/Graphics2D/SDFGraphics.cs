//本文件中的形状绘制全部改自iq大佬的SDF https://iquilezles.org/articles/distfunctions2d/

using Terraria.ModLoader.IO;

namespace ImproveGame.UIFramework.Graphics2D;
public static class SDFGraphics
{
    const float root2Over2 = 1.414213562373f / 2f;
    static Effect SDF_Effect => ModAsset.SDFGraphics.Value;
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
    static void SetBackground(Color background) => SDF_Effect.Parameters["uBackgroundColor"].SetValue(background.ToVector4());
    static void SetBorder(float border, Color borderColor)
    {
        Effect effect = SDF_Effect;
        effect.Parameters["uBorder"].SetValue(border);
        effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
    }
    static void SetCommon(Matrix matrix)
    {
        Effect effect = SDF_Effect;
        effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);
        effect.Parameters["uTransform"].SetValue(matrix);

    }
    static void SetBar(Texture2D barTexture, float time, float distanceScaler)
    {
        Effect effect = SDF_Effect;
        effect.Parameters["uTime"].SetValue(time);
        effect.Parameters["uValueScaler"].SetValue(distanceScaler);
        Main.instance.GraphicsDevice.Textures[0] = barTexture;
        Main.instance.GraphicsDevice.SamplerStates[0] = SamplerState.AnisotropicWrap;
    }
    static void ApplyPass(string passName) => SDF_Effect.CurrentTechnique.Passes[passName].Apply();
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
    static bool DrawFrame => false;

    public static void FumoFumoKoishi(Vector2 position)
    {
        //NoBorderBox(default, default, new Vector2(10000, 10000), Color.White, GetMatrix(true));
        Matrix matrix = GetMatrix(true);
        float border = 2f;
        Color borderColor = Color.Black;

        #region 帽子
        Color hatColor = new Color(38, 52, 89);
        Color beltColor = new Color(244, 233, 167);
        HasBorderEllipse(position + new Vector2(20, -45), new Vector2(.5f), new Vector2(130, 100), hatColor, border, borderColor, matrix);

        HasBorderEllipse(position + new Vector2(100, -75), new(.5f), new Vector2(40, 20), beltColor, border, borderColor, matrix);
        HasBorderChainedQuadraticBezier(position + new Vector2(20, 10), [new(150, -110), new(185, -130), new(200, -120), new(190, -100), new(205, -80), new(220, -60), new(210, -30), new(195, -20), new(160, -30)], beltColor, border, borderColor, matrix);
        HasBorderChainedQuadraticBezier(position + new Vector2(20, 10), [new(110, -90), new(120, -140), new(150, -150), new(140, -120), new(150, -110), new(160, -100), new(170, -70), new(180, -40), new(150, -50)], beltColor, border, borderColor, matrix);
        HasBorderEllipse(position + new Vector2(20, -5), new Vector2(.5f), new Vector2(170, 100), hatColor, border, borderColor, matrix);

        #endregion

        #region 裙子
        Color clothColor = new Color(0, 132, 134);

        HasBorderQuadraticCircle(position + new Vector2(20, 260) + new Vector2(-50,-10), new Vector2(.5f), 30, new Color(252, 244, 241), border, borderColor, matrix);
        HasBorderQuadraticCircle(position + new Vector2(20, 260) + new Vector2(50, -10), new Vector2(.5f), 30, new Color(252, 244, 241), border, borderColor, matrix);

        HasBorderChainedQuadraticBezier(position + new Vector2(20,190), [new(-60,0),new(-70,45),new(-80,40),new(0,100),new(80,40), new(70, 45), new(60, 0)], clothColor, border, borderColor, matrix);

        NoBorderRing(position + new Vector2(20, 230), new Vector2(.5f), 80, MathHelper.Pi / 3, 4, Color.White, matrix);

        HasBorderQuadraticCircle(position + new Vector2(20, 260) + new Vector2(-60,0), new Vector2(.5f), 30, hatColor, border, borderColor, matrix);
        HasBorderQuadraticCircle(position + new Vector2(20, 260) + new Vector2(60,0), new Vector2(.5f), 30, hatColor, border, borderColor, matrix);
        #endregion

        #region 衣服
        //Color clothColor = new Color(0, 132, 134);
        Color lightHat = new(74, 81, 120);
        HasBorderRound(position + new Vector2(-70, 228), new Vector2(.5f), 50, new Color(252, 244, 241), border, borderColor, matrix);
        HasBorderRound(position + new Vector2(110, 228), new Vector2(.5f), 50, new Color(252, 244, 241), border, borderColor, matrix);
        HasBorderQuadraticCircle(position + new Vector2(20, 140), new(.5f), 70, beltColor, border, borderColor, matrix);

        HasBorderChainedQuadraticBezier(position + new Vector2(-100, 208), [default, new(5, 10), new(-10, 20), new(25, 30), new(60, 20), new(50, 10), new(60, 0)], lightHat, border, borderColor, matrix);
        HasBorderChainedQuadraticBezier(position + new Vector2(140, 208), [default, new(-5, 10), new(10, 20), new(-25, 30), new(-60, 20), new(-50, 10), new(-60, 0)], lightHat, border, borderColor, matrix);
        HasBorderChainedQuadraticBezier(position + new Vector2(-40, 88), [default, new(-80, 60), new(-60, 120), new(-35, 140), new(0, 120), new(20, 90), new(20, 60)], beltColor, border, borderColor, matrix);
        HasBorderChainedQuadraticBezier(position + new Vector2(80, 88), [default, new(80, 60), new(60, 120), new(35, 140), new(0, 120), new(-20, 90), new(-20, 60)], beltColor, border, borderColor, matrix);


        HasBorderPolygon(position + new Vector2(20, 100), [new(-45, -9), new(-75, -15), new(-50, 20), new(-40, 10)], lightHat, border, borderColor, matrix);
        HasBorderPolygon(position + new Vector2(20, 100), [new(-45, -9), new(75, -15), new(50, 20), new(40, 10)], lightHat, border, borderColor, matrix);
        HasBorderTriangle(position + new Vector2(20, 100), position + new Vector2(-20, 110), position + new Vector2(-40, 88), clothColor, border, borderColor, matrix);
        HasBorderTriangle(position + new Vector2(20, 100), position + new Vector2(60, 110), position + new Vector2(80, 88), clothColor, border, borderColor, matrix);

        //水晶
        HasBorderRhombus(position + new Vector2(20, 120), new Vector2(.5f), new Vector2(20, 40), new Color(170, 233, 246), border, borderColor, matrix);
        HasBorderRhombus(position + new Vector2(20, 160), new Vector2(.5f), new Vector2(15, 30), new Color(170, 233, 246), border, borderColor, matrix);
        #endregion

        #region 脸
        HasBorderQuadraticCircle(position + new Vector2(20, 0), new(.5f), 100, new Color(252, 244, 241), border, borderColor, matrix);
        //HasBorderEllipse(position + new Vector2(20,10), new Vector2(.5f), new Vector2(110, 90), new Color(252, 244, 241), border, borderColor, matrix);
        #endregion

        #region 腮红
        NoBorderEllipse(position + new Vector2(-50, 50), new Vector2(.5f), new Vector2(15, 10), new Color(253, 190, 181), matrix);
        NoBorderEllipse(position + new Vector2(90, 50), new Vector2(.5f), new Vector2(15, 10), new Color(253, 190, 181), matrix);
        #endregion

        #region 头发
        Color hairColor = new Color(218, 241, 235);
        Color hairColor_Dark = new Color(177, 215, 226);
        //中
        HasBorderChainedQuadraticBezier(position + new Vector2(20, 10), [new(-45, -60), new(-30, 0), new(5, 10), new(25, -50), new(10, -70)], hairColor, border, borderColor, matrix);

        //左下
        HasBorderChainedQuadraticBezier(position + new Vector2(20, 10), [new(-100, 60), new(-95, 75), new(-100, 90), new(-120, 80), new(-120, 50), new(-135, 65), new(-125, 80), new(-160, 65), new(-110, -10)], hairColor_Dark, border, borderColor, matrix);

        //左下-正下
        HasBorderChainedQuadraticBezier(position + new Vector2(20, 10), [new(-95, 15), new(-85, 50), new(-50, 60), new(-75, 70), new(-90, 65), new(-95, 75), new(-80, 80), new(-130, 65), new(-110, 10)], hairColor, border, borderColor, matrix);

        //左上
        HasBorderChainedQuadraticBezier(position + new Vector2(20, 10), [new(-45, -60), new(-85, -20), new(-90, 20), new(-115, 20), new(-120, -20)], hairColor, border, borderColor, matrix);
        HasBorderChainedQuadraticBezier(position + new Vector2(20, 10), [new(-120, -20), new(-130, -10), new(-140, -15), new(-120, -30), new(-120, -50), new(-120, -80), new(-90, -100)], hairColor, border, borderColor, matrix);

        //右下

        HasBorderChainedQuadraticBezier(position + new Vector2(20, 10), [new(80, 70), new(95, 77), new(90, 80), new(110, 70), new(110, 65), new(112, 70), new(120, 75), new(145, 60), new(130, 45)], hairColor_Dark, border, borderColor, matrix);
        HasBorderChainedQuadraticBezier(position + new Vector2(20, 10), [new(130, 45), new(145, 40), new(150, 30), new(135, 20), new(130, 5), new(125, -10), new(105, -65)], hairColor_Dark, border, borderColor, matrix);
        NoBorderPolygon(position + new Vector2(20, 10), [new(80, 70), new(132, 47), new(105, -65)], hairColor_Dark, matrix);
        //右下-前
        HasBorderChainedQuadraticBezier(position + new Vector2(20, 10), [new(90, -15), new(95, 40), new(70, 75), new(90, 70), new(100, 50), new(105, 65), new(125, 60), new(110, 20), new(115, -25)], hairColor, border, borderColor, matrix);

        //右上
        HasBorderChainedQuadraticBezier(position + new Vector2(20, 10), [new(10, -70), new(50, -20), new(110, -5), new(130, -50), new(90, -90), new(0, -120), new(-90, -100)], hairColor, border, borderColor, matrix);

        //填充
        NoBorderPolygon(position + new Vector2(20, 10), [new(-45, -60), new(10, -70), new(-90, -101), new(-122, -18)], hairColor, matrix);
        #endregion

        #region 帽子前沿
        HasBorderChainedQuadraticBezier(position + new Vector2(20, 10), [new(-130, -80), new(15, -160), new(140, -70), new(-30, -140), new(-130, -80)], hatColor, border, borderColor, matrix);

        #endregion

        #region 眼睛
        NoBorderEllipse(position + new Vector2(-50, 20), new Vector2(.5f), new Vector2(10, 20), Color.White, matrix);
        NoBorderEllipse(position + new Vector2(90, 20), new Vector2(.5f), new Vector2(10, 20), Color.White, matrix);
        HasBorderTunnel(position + new Vector2(-50, 0), default, new Vector2(20, 30), new Color(54, 181, 190), border, borderColor, matrix);
        HasBorderTunnel(position + new Vector2(50, 0), default, new Vector2(20, 30), new Color(54, 181, 190), border, borderColor, matrix);
        HasBorderRound(position + new Vector2(-50, 5), default, 20f, Color.White, border, borderColor, matrix);
        HasBorderRound(position + new Vector2(50, 5), default, 20f, Color.White, border, borderColor, matrix);
        #endregion

        #region 眉毛
        NoBorderQuadraticBezier(position + new Vector2(-60, 10), position + new Vector2(-30, -5), position + new Vector2(-15, 5), 4, borderColor, matrix);
        NoBorderQuadraticBezier(position + new Vector2(100, 10), position + new Vector2(70, -5), position + new Vector2(55, 5), 4, borderColor, matrix);

        NoBorderQuadraticBezier(position + new Vector2(-40, -8), position + new Vector2(-30, -12), position + new Vector2(-15, -5), border * .5f, borderColor, matrix);
        NoBorderQuadraticBezier(position + new Vector2(80, -8), position + new Vector2(70, -12), position + new Vector2(55, -5), border * .5f, borderColor, matrix);

        NoBorderQuadraticBezier(position + new Vector2(-55, -30), position + new Vector2(-40, -37), position + new Vector2(-25, -30), border * .5f, borderColor, matrix);
        NoBorderQuadraticBezier(position + new Vector2(95, -30), position + new Vector2(80, -37), position + new Vector2(65, -30), border * .5f, borderColor, matrix);

        #endregion

        #region 小嘴
        NoBorderArc(position + new Vector2(0, 60), default, 20, MathHelper.Pi / 4f, border * .5f, borderColor, matrix);
        #endregion
    }

    public static void Gallery(Vector2 position, Vector2 percentOrigin, Vector2 unitSize, Color backgroundColor, float border, Color borderColor, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        NoBorderBox(default, default, new Vector2(10000, 10000), Color.Black, GetMatrix(true));

        Vector2 start = position;
        Vector2 stepY = Vector2.UnitY * 2 * unitSize;
        Vector2 stepX = Vector2.UnitX * 1.5f * unitSize;

        #region 圆Circle/Round
        HasBorderRound(position, percentOrigin, unitSize.X, backgroundColor, border, borderColor, matrix);
        NoBorderRound(position + stepY, percentOrigin, unitSize.X, backgroundColor, matrix);
        BarRound(position + stepY * 2, percentOrigin, unitSize.X, barTexture, time, distanceScaler, matrix);
        position += stepX;
        #endregion

        #region 曲边矩形RoundedBox
        HasBorderRoundedBox(position, percentOrigin, unitSize, new Vector4(1, 4, 8, 16), backgroundColor, border, borderColor, matrix);
        NoBorderRoundedBox(position + stepY, percentOrigin, unitSize, new Vector4(1, 4, 8, 16), backgroundColor, matrix);
        BarRoundedBox(position + stepY * 2, percentOrigin, unitSize, new Vector4(1, 4, 8, 16), barTexture, time, distanceScaler, matrix);
        position += stepX;
        #endregion

        #region 矩形Box
        HasBorderBox(position, percentOrigin, unitSize, backgroundColor, border, borderColor, matrix);
        NoBorderBox(position + stepY, percentOrigin, unitSize, backgroundColor, matrix);
        BarBox(position + stepY * 2, percentOrigin, unitSize, barTexture, time, distanceScaler, matrix);
        position += stepX;
        #endregion

        #region 正交矩形OrientedBox
        HasBorderOrientedBox(position, position + unitSize, 20, backgroundColor, border, borderColor, matrix);
        NoBorderOrientedBox(position + stepY, position + stepY + unitSize, 20, backgroundColor, matrix);
        BarOrientedBox(position + stepY * 2, position + stepY * 2 + unitSize, 20, barTexture, time, distanceScaler, matrix);
        position += stepX;
        #endregion

        #region 线段Segment/Line
        HasBorderLine(position, position + unitSize, 10, backgroundColor, border, borderColor, matrix);
        NoBorderLine(position + stepY, position + stepY + unitSize, 10, backgroundColor, matrix);
        BarLine(position + stepY * 2, position + stepY * 2 + unitSize, 10, barTexture, time, distanceScaler, matrix);
        position += stepX;
        #endregion

        #region 菱形Rhombus
        HasBorderRhombus(position, percentOrigin, unitSize, backgroundColor, border, borderColor, matrix);
        NoBorderRhombus(position + stepY, percentOrigin, unitSize, backgroundColor, matrix);
        BarRhombus(position + stepY * 2, percentOrigin, unitSize, barTexture, time, distanceScaler, matrix);
        position += stepX;
        #endregion

        #region 梯形Trapezoid
        HasBorderTrapezoid(position, percentOrigin, unitSize, 2f, backgroundColor, border, borderColor, matrix);
        NoBorderTrapezoid(position + stepY, percentOrigin, unitSize, 2f, backgroundColor, matrix);
        BarTrapezoid(position + stepY * 2, percentOrigin, unitSize, 2f, barTexture, time, distanceScaler, matrix);
        position += stepX;
        #endregion

        #region 平行四边形Parallelogram
        HasBorderParallelogram(position, percentOrigin, unitSize, 0.5f, backgroundColor, border, borderColor, matrix);
        NoBorderParallelogram(position + stepY, percentOrigin, unitSize, 0.5f, backgroundColor, matrix);
        BarParallelogram(position + stepY * 2, percentOrigin, unitSize, 0.5f, barTexture, time, distanceScaler, matrix);
        position += stepX;
        #endregion

        #region 等边三角形EquilateralTriangle
        HasBorderEquilateralTriangle(position, percentOrigin, unitSize.X, backgroundColor, border, borderColor, matrix);
        NoBorderEquilateralTriangle(position + stepY, percentOrigin, unitSize.X, backgroundColor, matrix);
        BarEquilateralTriangle(position + stepY * 2, percentOrigin, unitSize.X, barTexture, time, distanceScaler, matrix);
        position += stepX;
        #endregion

        #region 等腰三角形TriangleIsosceles
        HasBorderTriangleIsosceles(position, percentOrigin, unitSize, backgroundColor, border, borderColor, matrix);
        NoBorderTriangleIsosceles(position + stepY, percentOrigin, unitSize, backgroundColor, matrix);
        BarTriangleIsosceles(position + stepY * 2, percentOrigin, unitSize, barTexture, time, distanceScaler, matrix);
        position += stepX;
        #endregion

        #region 三角形Triangle
        HasBorderTriangle(position, position + unitSize * new Vector2(1.0f, 0.5f), position + unitSize * new Vector2(0.5f, 1.0f), backgroundColor, border, borderColor, matrix);
        NoBorderTriangle(position + stepY, position + stepY + unitSize * new Vector2(1.0f, 0.5f), position + stepY + unitSize * new Vector2(0.5f, 1.0f), backgroundColor, matrix);
        BarTriangle(position + stepY * 2, position + stepY * 2 + unitSize * new Vector2(1.0f, 0.5f), position + stepY * 2 + unitSize * new Vector2(0.5f, 1.0f), barTexture, time, distanceScaler, matrix);
        position += stepX;
        #endregion

        #region 胶囊UnevenCapsule
        HasBorderUnevenCapsule(position, percentOrigin, unitSize.Y, unitSize.X * .5f, unitSize.X * .25f, backgroundColor, border, borderColor, matrix);
        NoBorderUnevenCapsule(position + stepY, percentOrigin, unitSize.Y, unitSize.X * .5f, unitSize.X * .25f, backgroundColor, matrix);
        BarUnevenCapsule(position + stepY * 2, percentOrigin, unitSize.Y, unitSize.X * .5f, unitSize.X * .25f, barTexture, time, distanceScaler, matrix);
        position += stepX;
        #endregion

        #region 正五边形Pentagon
        HasBorderPentagon(position, percentOrigin, unitSize.X * .5f, backgroundColor, border, borderColor, matrix);
        NoBorderPentagon(position + stepY, percentOrigin, unitSize.X * .5f, backgroundColor, matrix);
        BarPentagon(position + stepY * 2, percentOrigin, unitSize.X * .5f, barTexture, time, distanceScaler, matrix);
        position += stepX;
        #endregion

        #region 正六边形Hexagon
        HasBorderHexagon(position, percentOrigin, unitSize.X * .5f, backgroundColor, border, borderColor, matrix);
        NoBorderHexagon(position + stepY, percentOrigin, unitSize.X * .5f, backgroundColor, matrix);
        BarHexagon(position + stepY * 2, percentOrigin, unitSize.X * .5f, barTexture, time, distanceScaler, matrix);
        position += stepX;
        #endregion

        #region 正八边形Octogon
        HasBorderOctogon(position, percentOrigin, unitSize.X * .5f, backgroundColor, border, borderColor, matrix);
        NoBorderOctogon(position + stepY, percentOrigin, unitSize.X * .5f, backgroundColor, matrix);
        BarOctogon(position + stepY * 2, percentOrigin, unitSize.X * .5f, barTexture, time, distanceScaler, matrix);
        position += stepX;
        #endregion

        #region 正N边形Ngon
        HasBorderNgon(position, percentOrigin, unitSize.X * .5f, 3 + (int)Main.GlobalTimeWrappedHourly % 8, backgroundColor, border, borderColor, matrix);
        NoBorderNgon(position + stepY, percentOrigin, unitSize.X * .5f, 3 + (int)Main.GlobalTimeWrappedHourly % 8, backgroundColor, matrix);
        BarNgon(position + stepY * 2, percentOrigin, unitSize.X * .5f, 3 + (int)Main.GlobalTimeWrappedHourly % 8, barTexture, time, distanceScaler, matrix);
        position += stepX * (4 / 3f);
        #endregion

        #region 六芒星Hexagram
        HasBorderHexagram(position, percentOrigin, unitSize.X * .5f, backgroundColor, border, borderColor, matrix);
        NoBorderHexagram(position + stepY, percentOrigin, unitSize.X * .5f, backgroundColor, matrix);
        BarHexagram(position + stepY * 2, percentOrigin, unitSize.X * .5f, barTexture, time, distanceScaler, matrix);
        position += stepX;
        #endregion

        #region 五角星Star5
        HasBorderStar5(position, percentOrigin, unitSize.X * .5f, 0.5f, backgroundColor, border, borderColor, matrix);
        NoBorderStar5(position + stepY, percentOrigin, unitSize.X * .5f, 0.5f, backgroundColor, matrix);
        BarStar5(position + stepY * 2, percentOrigin, unitSize.X * .5f, 0.5f, barTexture, time, distanceScaler, matrix);
        position += stepX;
        #endregion

        #region X角星StarX
        HasBorderStarX(position, percentOrigin, unitSize.X * .5f, 3 + (int)Main.GlobalTimeWrappedHourly % 8, 0.5f, backgroundColor, border, borderColor, matrix);
        NoBorderStarX(position + stepY, percentOrigin, unitSize.X * .5f, 3 + (int)Main.GlobalTimeWrappedHourly % 8, 0.5f, backgroundColor, matrix);
        BarStarX(position + stepY * 2, percentOrigin, unitSize.X * .5f, 3 + (int)Main.GlobalTimeWrappedHourly % 8, 0.5f, barTexture, time, distanceScaler, matrix);
        position += stepX * (4 / 3f);
        #endregion

        #region 饼图Pie
        //float theta = MathHelper.Pi * (0.5f + 0.5f * MathF.Cos(Main.GlobalTimeWrappedHourly));// 
        float theta = MathHelper.Pi * 2 / 3f;
        HasBorderPie(position, percentOrigin, unitSize.X * .5f, theta, backgroundColor, border, borderColor, matrix);
        NoBorderPie(position + stepY, percentOrigin, unitSize.X * .5f, theta, backgroundColor, matrix);
        BarPie(position + stepY * 2, percentOrigin, unitSize.X * .5f, theta, barTexture, time, distanceScaler, matrix);
        position += stepX;
        #endregion

        #region 弓形CutDisk
        //float k = MathF.Cos(Main.GlobalTimeWrappedHourly);
        float k = 0.5f;
        HasBorderCutDisk(position, percentOrigin, unitSize.X * .5f, k, backgroundColor, border, borderColor, matrix);
        NoBorderCutDisk(position + stepY, percentOrigin, unitSize.X * .5f, k, backgroundColor, matrix);
        BarCutDisk(position + stepY * 2, percentOrigin, unitSize.X * .5f, k, barTexture, time, distanceScaler, matrix);
        position += stepX;
        #endregion

        #region 弦Arc
        HasBorderArc(position, percentOrigin, unitSize.X * .5f, theta, 4, backgroundColor, border, borderColor, matrix);
        NoBorderArc(position + stepY, percentOrigin, unitSize.X * .5f, theta, 4, backgroundColor, matrix);
        BarArc(position + stepY * 2, percentOrigin, unitSize.X * .5f, theta, 4, barTexture, time, distanceScaler, matrix);
        position += stepX;
        #endregion

        #region 环Ring
        HasBorderRing(position, percentOrigin, unitSize.X * .5f, theta, 7, backgroundColor, border, borderColor, matrix);
        NoBorderRing(position + stepY, percentOrigin, unitSize.X * .5f, theta, 7, backgroundColor, matrix);
        BarRing(position + stepY * 2, percentOrigin, unitSize.X * .5f, theta, 7, barTexture, time, distanceScaler, matrix);
        position += stepX;
        #endregion

        #region 马蹄铁Horseshoe
        HasBorderHorseshoe(position, percentOrigin, unitSize.X * .5f, theta, 7, 36, backgroundColor, border, borderColor, matrix);
        NoBorderHorseshoe(position + stepY, percentOrigin, unitSize.X * .5f, theta, 7, 36, backgroundColor, matrix);
        BarHorseshoe(position + stepY * 2, percentOrigin, unitSize.X * .5f, theta, 7, 36, barTexture, time, distanceScaler, matrix);
        position += stepX * 2f;
        #endregion

        #region 鱼鳔Vesica
        HasBorderVesica(position, percentOrigin, unitSize.X, unitSize.X * .5f, backgroundColor, border, borderColor, matrix);
        NoBorderVesica(position + stepY, percentOrigin, unitSize.X, unitSize.X * .5f, backgroundColor, matrix);
        BarVesica(position + stepY * 2, percentOrigin, unitSize.X, unitSize.X * .5f, barTexture, time, distanceScaler, matrix);
        position += stepX * (4 / 3f);
        #endregion

        #region 正交鱼鳔OrientedVesica
        HasBorderOrientedVesica(position, position + unitSize * .5f, unitSize.Length() * .5f, backgroundColor, border, borderColor, matrix);
        NoBorderOrientedVesica(position + stepY, position + stepY + unitSize * .5f, unitSize.Length() * .5f, backgroundColor, matrix);
        BarOrientedVesica(position + stepY * 2, position + stepY * 2 + unitSize * .5f, unitSize.Length() * .5f, barTexture, time, distanceScaler, matrix);
        //HasBorderRound(position - new Vector2(unitSize.Length()) * .5f , unitSize.Length(), Color.Transparent, border, Color.Cyan with { A = 0}, matrix);
        //HasBorderRound(position - new Vector2(unitSize.Length()) * .5f + unitSize * .5f, unitSize.Length(), Color.Transparent, border, Color.Cyan with { A = 0}, matrix);
        position += stepX;
        #endregion

        #region 月Moon
        HasBorderMoon(position, percentOrigin, unitSize.X * .5f, unitSize.X * .45f, unitSize.X * .2f, backgroundColor, border, borderColor, matrix);
        NoBorderMoon(position + stepY, percentOrigin, unitSize.X * .5f, unitSize.X * .45f, unitSize.X * .2f, backgroundColor, matrix);
        BarMoon(position + stepY * 2, percentOrigin, unitSize.X * .5f, unitSize.X * .45f, unitSize.X * .2f, barTexture, time, distanceScaler, matrix);
        position += stepX;
        #endregion

        #region 曲边四芒星CircleCross
        HasBorderCircleCross(position, percentOrigin, new Vector2(unitSize.Y, unitSize.X) * .75f, 4, backgroundColor, border, borderColor, matrix);
        NoBorderCircleCross(position + stepY, percentOrigin, new Vector2(unitSize.Y, unitSize.X) * .75f, 4, backgroundColor, matrix);
        BarCircleCross(position + stepY * 2, percentOrigin, new Vector2(unitSize.Y, unitSize.X) * .75f, 4, barTexture, time, distanceScaler, matrix);
        position += stepX * (4 / 3f);
        #endregion

        #region 蛋Egg
        HasBorderEgg(position, percentOrigin, unitSize.X * .5f, unitSize.X * .25f, unitSize.Y * .5f, backgroundColor, border, borderColor, matrix);
        NoBorderEgg(position + stepY, percentOrigin, unitSize.X * .5f, unitSize.X * .25f, unitSize.Y * .5f, backgroundColor, matrix);
        BarEgg(position + stepY * 2, percentOrigin, unitSize.X * .5f, unitSize.X * .25f, unitSize.Y * .5f, barTexture, time, distanceScaler, matrix);
        position += stepX;
        #endregion

        #region 心Heart
        HasBorderHeart(position, percentOrigin, unitSize.X, backgroundColor, border, borderColor, matrix);
        NoBorderHeart(position + stepY, percentOrigin, unitSize.X, backgroundColor, matrix);
        BarHeart(position + stepY * 2, percentOrigin, unitSize.X, barTexture, time, distanceScaler, matrix);
        position += stepX;
        #endregion

        #region 十字Cross/Plus
        HasBorderPlus(position, percentOrigin, unitSize.X * .75f, unitSize.X * .25f, unitSize.X * .15f, backgroundColor, border, borderColor, matrix);
        NoBorderPlus(position + stepY, percentOrigin, unitSize.X * .75f, unitSize.X * .25f, unitSize.X * .15f, backgroundColor, matrix);
        BarPlus(position + stepY * 2, percentOrigin, unitSize.X * .75f, unitSize.X * .25f, unitSize.X * .15f, barTexture, time, distanceScaler, matrix);
        position += stepX;
        #endregion

        #region 叉号RoundedX/Cross
        HasBorderCross(position, percentOrigin, unitSize.X, 6, backgroundColor, border, borderColor, matrix);
        NoBorderCross(position + stepY, percentOrigin, unitSize.X, 6, backgroundColor, matrix);
        BarCross(position + stepY * 2, percentOrigin, unitSize.X, 6, barTexture, time, distanceScaler, matrix);
        position += stepX;
        #endregion

        #region 多边形Polygon
        Vector2[] percents = [Vector2.Zero, new(0.5f, 1.0f), new(0.8f, 0f), new(0f, 0.6f), new(1, 0.8f)];
        HasBorderPolygon(position, unitSize, percents, backgroundColor, border, borderColor, matrix);
        NoBorderPolygon(position + stepY, unitSize, percents, backgroundColor, matrix);
        BarPolygon(position + stepY * 2, unitSize, percents, barTexture, time, distanceScaler, matrix);
        position += stepX;
        #endregion

        #region 椭圆Ellipse
        HasBorderEllipse(position, percentOrigin, unitSize * .5f, backgroundColor, border, borderColor, matrix);
        NoBorderEllipse(position + stepY, percentOrigin, unitSize * .5f, backgroundColor, matrix);
        BarEllipse(position + stepY * 2, percentOrigin, unitSize * .5f, barTexture, time, distanceScaler, matrix);
        position = start + stepY * 3;
        #endregion

        #region 抛物线Parabola
        HasBorderParabola(position, percentOrigin, unitSize, backgroundColor, border, borderColor, matrix);
        NoBorderParabola(position + stepY, percentOrigin, unitSize, backgroundColor, matrix);
        BarParabola(position + stepY * 2, percentOrigin, unitSize, barTexture, time, distanceScaler, matrix);
        position += stepX;
        #endregion

        #region 抛物线段ParabolaSegment
        HasBorderParabolaSegment(position, percentOrigin, unitSize, unitSize.Y * .5f, backgroundColor, border, borderColor, matrix);
        NoBorderParabolaSegment(position + stepY, percentOrigin, unitSize, unitSize.Y * .5f, backgroundColor, matrix);
        BarParabolaSegment(position + stepY * 2, percentOrigin, unitSize, unitSize.Y * .5f, barTexture, time, distanceScaler, matrix);
        position += stepX;
        #endregion

        #region 二阶贝塞尔曲线QuadraticBezier
        HasBorderQuadraticBezier(position, position + unitSize * new Vector2(2f, 0.5f), position + unitSize * new Vector2(0.5f, 1f), 4f, backgroundColor, border, borderColor, matrix);
        NoBorderQuadraticBezier(position + stepY, position + unitSize * new Vector2(2f, 0.5f) + stepY, position + unitSize * new Vector2(0.5f, 1f) + stepY, 4f, backgroundColor, matrix);
        BarQuadraticBezier(position + stepY * 2, position + unitSize * new Vector2(2f, 0.5f) + stepY * 2, position + unitSize * new Vector2(0.5f, 1f) + stepY * 2, 4f, barTexture, time, distanceScaler, matrix);
        position += stepX;
        #endregion

        #region 圆头十字BlobbyCross
        HasBorderBlobbyCross(position, percentOrigin, unitSize.X * .5f, 0.7f, 8f, backgroundColor, border, borderColor, matrix);
        NoBorderBlobbyCross(position + stepY, percentOrigin, unitSize.X * .5f, 0.7f, 8f, backgroundColor, matrix);
        BarBlobbyCross(position + stepY * 2, percentOrigin, unitSize.X * .5f, 0.7f, 8f, barTexture, time, distanceScaler, matrix);
        position += stepX * (4 / 3f);
        #endregion

        #region 隧道Tunnel
        HasBorderTunnel(position, percentOrigin, unitSize * .5f, backgroundColor, border, borderColor, matrix);
        NoBorderTunnel(position + stepY, percentOrigin, unitSize * .5f, backgroundColor, matrix);
        BarTunnel(position + stepY * 2, percentOrigin, unitSize * .5f, barTexture, time, distanceScaler, matrix);
        position += stepX;
        #endregion

        #region 楼梯Stairs
        HasBorderStairs(position, percentOrigin, unitSize * .2f, 5, backgroundColor, border, borderColor, matrix);
        NoBorderStairs(position + stepY, percentOrigin, unitSize * .2f, 5, backgroundColor, matrix);
        BarStairs(position + stepY * 2, percentOrigin, unitSize * .2f, 5, barTexture, time, distanceScaler, matrix);
        position += stepX;
        #endregion

        #region 四次方圆QuadraticCircle
        HasBorderQuadraticCircle(position, percentOrigin, unitSize.X * .5f, backgroundColor, border, borderColor, matrix);
        NoBorderQuadraticCircle(position + stepY, percentOrigin, unitSize.X * .5f, backgroundColor, matrix);
        BarQuadraticCircle(position + stepY * 2, percentOrigin, unitSize.X * .5f, barTexture, time, distanceScaler, matrix);
        position += stepX;
        #endregion

        #region 双曲线Hyperbola
        HasBorderHyperbola(position, percentOrigin, new Vector2(unitSize.X * 1.1f, unitSize.X), backgroundColor, border, borderColor, matrix);
        NoBorderHyperbola(position + stepY, percentOrigin, new Vector2(unitSize.X * 1.1f, unitSize.X), backgroundColor, matrix);
        BarHyperbola(position + stepY * 2, percentOrigin, new Vector2(unitSize.X * 1.1f, unitSize.X), barTexture, time, distanceScaler, matrix);
        position += stepX * (4 / 3f);
        #endregion

        #region 酷酷S CoolS
        HasBorderCoolS(position, percentOrigin, unitSize.Y, backgroundColor, border, borderColor, matrix);
        NoBorderCoolS(position + stepY, percentOrigin, unitSize.Y, backgroundColor, matrix);
        BarCoolS(position + stepY * 2f, percentOrigin, unitSize.Y, barTexture, time, distanceScaler, matrix);
        position += stepX;
        #endregion

        #region 圆波CircleWave
        HasBorderCircleWave(position, percentOrigin, unitSize.Y * 2, 0.75f, unitSize.X * .25f, 4, backgroundColor, border, borderColor, matrix);
        NoBorderCircleWave(position + stepY, percentOrigin, unitSize.Y * 2, 0.75f, unitSize.X * .25f, 4, backgroundColor, matrix);
        BarCircleWave(position + stepY * 2, percentOrigin, unitSize.Y * 2, 0.75f, unitSize.X * .25f, 4, barTexture, time, distanceScaler, matrix);
        position += stepX * 2.5f;
        #endregion

        #region 连段二阶贝塞尔曲线ChainedQuadraticBezier
        Vector2[] BPercents = [default, new(0.4f, -0.2f), new(1, .3f), new(.5f, .8f), new Vector2(0.2f, 0.6f)];
        HasBorderChainedQuadraticBezier(position, unitSize, BPercents, backgroundColor, border, borderColor, matrix);
        NoBorderChainedQuadraticBezier(position + stepY, unitSize, BPercents, backgroundColor, matrix);
        BarChainedQuadraticBezier(position + stepY * 2, unitSize, BPercents, barTexture, time, distanceScaler, matrix);
        #endregion
    }

    #region 圆Circle/Round
    static void Circle(float size)
    {
        const float innerShrinkage = 1;
        size += innerShrinkage * 2;
        SDF_Effect.Parameters["uSizeOver2"].SetValue(new Vector2(size) / 2f);
    }
    static void CircleBase(Vector2 pos, Vector2 percentOrigin, float size, Matrix matrix, string passName)
    {
        SetCommon(matrix);
        Circle(size);
        const float innerShrinkage = 1;
        pos -= new Vector2(innerShrinkage);
        size += innerShrinkage * 2;
        SDF_Effect.CurrentTechnique.Passes[passName].Apply();
        BaseDraw(pos - percentOrigin * size, new Vector2(size));
    }
    public static void HasBorderRound(Vector2 pos, Vector2 percentOrigin, float size, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        CircleBase(pos, percentOrigin, size, matrix, "HasBorderCircle");
    }
    public static void NoBorderRound(Vector2 pos, Vector2 percentOrigin, float size, Color background, Matrix matrix)
    {
        SetBackground(background);
        CircleBase(pos, percentOrigin, size, matrix, "NoBorderCircle");
    }
    public static void BarRound(Vector2 pos, Vector2 percentOrigin, float size, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        CircleBase(pos, percentOrigin, size, matrix, "BarCircle");
    }
    #endregion

    #region 曲边矩形RoundedBox
    static void RoundedBox(Vector2 size, Vector4 round)
    {
        Effect effect = SDF_Effect;
        effect.Parameters["uSizeOver2"].SetValue(size / 2f);
        effect.Parameters["uRound"].SetValue(round);
    }
    static void RoundedBoxBase(Vector2 pos, Vector2 percentOrigin, Vector2 size, Vector4 round, Matrix matrix, string passName)
    {
        SetCommon(matrix);
        RoundedBox(size, round);
        ApplyPass(passName);
        BaseDraw(pos - percentOrigin * size, size);
    }
    public static void HasBorderRoundedBox(Vector2 pos, Vector2 percentOrigin, Vector2 size, Vector4 round, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        RoundedBoxBase(pos, percentOrigin, size, round, matrix, nameof(HasBorderRoundedBox));
    }
    public static void NoBorderRoundedBox(Vector2 pos, Vector2 percentOrigin, Vector2 size, Vector4 round, Color background, Matrix matrix)
    {
        SetBackground(background);
        RoundedBoxBase(pos, percentOrigin, size, round, matrix, nameof(NoBorderRoundedBox));
    }
    public static void BarRoundedBox(Vector2 pos, Vector2 percentOrigin, Vector2 size, Vector4 round, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        RoundedBoxBase(pos, percentOrigin, size, round, matrix, nameof(BarRoundedBox));
    }
    #endregion

    #region 矩形Box
    static void Box(Vector2 size)
    {
        SDF_Effect.Parameters["uSizeOver2"].SetValue(size / 2f);
    }
    static void BoxBase(Vector2 pos, Vector2 percentOrigin, Vector2 size, Matrix matrix, string passName)
    {
        SetCommon(matrix);
        Box(size);
        ApplyPass(passName);
        BaseDraw(pos - percentOrigin * size, size);
    }
    public static void HasBorderBox(Vector2 pos, Vector2 percentOrigin, Vector2 size, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        BoxBase(pos, percentOrigin, size, matrix, nameof(HasBorderBox));
    }
    public static void NoBorderBox(Vector2 pos, Vector2 percentOrigin, Vector2 size, Color background, Matrix matrix)
    {
        SetBackground(background);
        BoxBase(pos, percentOrigin, size, matrix, nameof(NoBorderBox));
    }
    public static void BarBox(Vector2 pos, Vector2 percentOrigin, Vector2 size, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        BoxBase(pos, percentOrigin, size, matrix, nameof(BarBox));
    }
    #endregion

    #region 正交矩形OrientedBox
    static void OrientedBox(Vector2 start, Vector2 end, float width, Matrix matrix, out Vector2 pos, out Vector2 size)
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

        Effect effect = SDF_Effect;
        effect.Parameters["uStart"].SetValue(start);
        effect.Parameters["uEnd"].SetValue(end);
        effect.Parameters["uLineWidth"].SetValue(width * 2);

        pos = topLeft;
        size = bottomRight - topLeft;

        SetCommon(matrix);
    }
    static void OrientedBoxBase(Vector2 start, Vector2 end, float width, Matrix matrix, string passName)
    {
        OrientedBox(start, end, width, matrix, out Vector2 pos, out Vector2 size);
        ApplyPass(passName);
        BaseDraw(pos, size);
    }
    public static void HasBorderOrientedBox(Vector2 start, Vector2 end, float width, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        OrientedBoxBase(start, end, width, matrix, nameof(HasBorderOrientedBox));
    }
    public static void NoBorderOrientedBox(Vector2 start, Vector2 end, float width, Color background, Matrix matrix)
    {
        SetBackground(background);
        OrientedBoxBase(start, end, width, matrix, nameof(NoBorderOrientedBox));
    }
    public static void BarOrientedBox(Vector2 start, Vector2 end, float width, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        OrientedBoxBase(start, end, width, matrix, nameof(BarOrientedBox));
    }
    #endregion

    #region 线段Segment/Line
    static void Segment(Vector2 start, Vector2 end, float width, out Vector2 pos, out Vector2 size)
    {
        Vector2 min = Vector2.Min(start, end);
        Vector2 max = Vector2.Max(start, end);
        pos = min - new Vector2(width);
        start -= pos;
        end -= pos;
        Effect effect = SDF_Effect;
        effect.Parameters["uStart"].SetValue(start);
        effect.Parameters["uEnd"].SetValue(end);
        effect.Parameters["uLineWidth"].SetValue(width);
        size = max - min + new Vector2(width * 2);
    }
    static void SegmentBase(Vector2 start, Vector2 end, float width, Matrix matrix, string passName)
    {
        SetCommon(matrix);
        Segment(start, end, width, out Vector2 pos, out Vector2 size);
        ApplyPass(passName);
        BaseDraw(pos, size);
    }
    /// <summary>
    /// 绘制一条线，有边框
    /// </summary>
    public static void HasBorderLine(Vector2 start, Vector2 end, float width, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        SegmentBase(start, end, width, matrix, "HasBorderSegment");
    }
    /// <summary>
    /// 绘制一条线，无边框
    /// </summary>
    public static void NoBorderLine(Vector2 start, Vector2 end, float width, Color background, Matrix matrix)
    {
        SetBackground(background);
        SegmentBase(start, end, width, matrix, "NoBorderSegment");
    }
    public static void BarLine(Vector2 start, Vector2 end, float width, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        SegmentBase(start, end, width, matrix, "BarSegment");
    }
    #endregion

    #region 菱形Rhombus
    static void Rhombus(Vector2 size)
    {
        SDF_Effect.Parameters["uSizeOver2"].SetValue(size * .5f);
    }

    static void RhombusBase(Vector2 pos, Vector2 percentOrigin, Vector2 size, Matrix matrix, string passName)
    {
        SetCommon(matrix);
        Rhombus(size);
        ApplyPass(passName);
        BaseDraw(pos - percentOrigin * size, size);
    }
    /// <summary>
    /// 绘制菱形
    /// </summary>
    public static void HasBorderRhombus(Vector2 pos, Vector2 percentOrigin, Vector2 size, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        RhombusBase(pos, percentOrigin, size, matrix, nameof(HasBorderRhombus));
    }

    /// <summary>
    /// 绘制菱形
    /// </summary>
    public static void NoBorderRhombus(Vector2 pos, Vector2 percentOrigin, Vector2 size, Color background, Matrix matrix)
    {
        SetBackground(background);
        RhombusBase(pos, percentOrigin, size, matrix, nameof(NoBorderRhombus));
    }

    public static void BarRhombus(Vector2 pos, Vector2 percentOrigin, Vector2 size, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        RhombusBase(pos, percentOrigin, size, matrix, nameof(BarRhombus));
    }
    #endregion

    #region 梯形Trapezoid
    static void Trapezoid(Vector2 size, float bottomScaler)
    {
        Effect effect = SDF_Effect;
        effect.Parameters["uBottomScaler"].SetValue(bottomScaler);
        effect.Parameters["uSizeOver2"].SetValue(size * .5f);
    }
    static void TrapezoidBase(Vector2 pos, Vector2 percentOrigin, Vector2 size, float bottomScaler, Matrix matrix, string passName)
    {
        SetCommon(matrix);
        Trapezoid(size, bottomScaler);
        ApplyPass(passName);
        BaseDraw(pos - percentOrigin * size, size);
    }
    public static void HasBorderTrapezoid(Vector2 pos, Vector2 percentOrigin, Vector2 size, float bottomScaler, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        TrapezoidBase(pos, percentOrigin, size, bottomScaler, matrix, nameof(HasBorderTrapezoid));
    }
    public static void NoBorderTrapezoid(Vector2 pos, Vector2 percentOrigin, Vector2 size, float bottomScaler, Color background, Matrix matrix)
    {
        SetBackground(background);
        TrapezoidBase(pos, percentOrigin, size, bottomScaler, matrix, nameof(NoBorderTrapezoid));
    }
    public static void BarTrapezoid(Vector2 pos, Vector2 percentOrigin, Vector2 size, float bottomScaler, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        TrapezoidBase(pos, percentOrigin, size, bottomScaler, matrix, nameof(BarTrapezoid));
    }
    #endregion

    #region 平行四边形Parallelogram
    static void Parallelogram(Vector2 size, float bottomScaler)
    {
        Effect effect = SDF_Effect;
        effect.Parameters["uBottomScaler"].SetValue(bottomScaler);
        effect.Parameters["uSizeOver2"].SetValue(size * .5f);
    }
    static void ParallelogramBase(Vector2 pos, Vector2 percentOrigin, Vector2 size, float bottomScaler, Matrix matrix, string passName)
    {
        SetCommon(matrix);
        Parallelogram(size, bottomScaler);
        ApplyPass(passName);
        BaseDraw(pos - percentOrigin * size, size);
    }
    public static void HasBorderParallelogram(Vector2 pos, Vector2 percentOrigin, Vector2 size, float bottomScaler, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        ParallelogramBase(pos, percentOrigin, size, bottomScaler, matrix, nameof(HasBorderParallelogram));
    }
    public static void NoBorderParallelogram(Vector2 pos, Vector2 percentOrigin, Vector2 size, float bottomScaler, Color background, Matrix matrix)
    {
        SetBackground(background);
        ParallelogramBase(pos, percentOrigin, size, bottomScaler, matrix, nameof(NoBorderParallelogram));
    }
    public static void BarParallelogram(Vector2 pos, Vector2 percentOrigin, Vector2 size, float bottomScaler, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        ParallelogramBase(pos, percentOrigin, size, bottomScaler, matrix, nameof(BarParallelogram));
    }
    #endregion

    #region 等边三角形EquilateralTriangle
    static void EquilateralTriangle(float r, out Vector2 size)
    {
        const float sqrt3 = 1.732050807f;
        size = new Vector2(1, sqrt3 * .5f) * r;
        SDF_Effect.Parameters["uSizeOver2"].SetValue(size * .5f);
    }
    static void EquilateralTriangleBase(Vector2 pos, Vector2 percentOrigin, float r, Matrix matrix, string passName)
    {
        SetCommon(matrix);
        EquilateralTriangle(r, out Vector2 size);
        ApplyPass(passName);
        BaseDraw(pos - percentOrigin * size, size);
    }
    public static void HasBorderEquilateralTriangle(Vector2 pos, Vector2 percentOrigin, float r, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        EquilateralTriangleBase(pos, percentOrigin, r, matrix, nameof(HasBorderEquilateralTriangle));
    }
    public static void NoBorderEquilateralTriangle(Vector2 pos, Vector2 percentOrigin, float r, Color background, Matrix matrix)
    {
        SetBackground(background);
        EquilateralTriangleBase(pos, percentOrigin, r, matrix, nameof(NoBorderEquilateralTriangle));
    }
    public static void BarEquilateralTriangle(Vector2 pos, Vector2 percentOrigin, float r, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        EquilateralTriangleBase(pos, percentOrigin, r, matrix, nameof(BarEquilateralTriangle));
    }
    #endregion

    #region 等腰三角形TriangleIsosceles
    static void TriangleIsosceles(Vector2 size)
    {
        SDF_Effect.Parameters["uSizeOver2"].SetValue(size * .5f);
    }
    static void TriangleIsoscelesBase(Vector2 pos, Vector2 percentOrigin, Vector2 size, Matrix matrix, string passName)
    {
        SetCommon(matrix);
        TriangleIsosceles(size);
        ApplyPass(passName);
        BaseDraw(pos - percentOrigin * size, size);
    }
    public static void HasBorderTriangleIsosceles(Vector2 pos, Vector2 percentOrigin, Vector2 size, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        TriangleIsoscelesBase(pos, percentOrigin, size, matrix, nameof(HasBorderTriangleIsosceles));
    }
    public static void NoBorderTriangleIsosceles(Vector2 pos, Vector2 percentOrigin, Vector2 size, Color background, Matrix matrix)
    {
        SetBackground(background);
        TriangleIsoscelesBase(pos, percentOrigin, size, matrix, nameof(NoBorderTriangleIsosceles));
    }
    public static void BarTriangleIsosceles(Vector2 pos, Vector2 percentOrigin, Vector2 size, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        TriangleIsoscelesBase(pos, percentOrigin, size, matrix, nameof(BarTriangleIsosceles));
    }

    #endregion

    #region 三角形Triangle
    static void Triangle(Vector2 A, Vector2 B, Vector2 C, out Vector2 pos, out Vector2 size)
    {
        Vector2 min = Vector2.Min(A, Vector2.Min(B, C));
        Vector2 max = Vector2.Max(A, Vector2.Max(B, C));
        Effect effect = SDF_Effect;
        effect.Parameters["uStart"].SetValue(A - min);
        effect.Parameters["uEnd"].SetValue(B - min);
        effect.Parameters["uAnother"].SetValue(C - min);
        size = max - min;
        pos = min;
    }
    static void TriangleBase(Vector2 A, Vector2 B, Vector2 C, Matrix matrix, string passName)
    {
        SetCommon(matrix);
        Triangle(A, B, C, out Vector2 pos, out Vector2 size);
        ApplyPass(passName);
        BaseDraw(pos, size);
    }
    public static void HasBorderTriangle(Vector2 A, Vector2 B, Vector2 C, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        TriangleBase(A, B, C, matrix, nameof(HasBorderTriangle));
    }
    public static void NoBorderTriangle(Vector2 A, Vector2 B, Vector2 C, Color background, Matrix matrix)
    {
        SetBackground(background);
        TriangleBase(A, B, C, matrix, nameof(NoBorderTriangle));
    }
    public static void BarTriangle(Vector2 A, Vector2 B, Vector2 C, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        TriangleBase(A, B, C, matrix, nameof(BarTriangle));
    }
    #endregion

    #region 胶囊UnevenCapsule
    static void UnevenCapsule(float distance, float round1, float round2, out Vector2 size)
    {
        float w = Math.Max(round1, round2) * 2;
        Effect effect = SDF_Effect;
        effect.Parameters["uSizeOver2"].SetValue(new Vector2(w * .5f, distance));
        effect.Parameters["uRound"].SetValue(new Vector4(round1, round2, 0, 0));
        size = new Vector2(w, distance + round1 + round2);
    }
    static void UnevenCapsuleBase(Vector2 pos, Vector2 percentOrigin, float distance, float round1, float round2, Matrix matrix, string passName)
    {
        SetCommon(matrix);
        UnevenCapsule(distance, round1, round2, out Vector2 size);
        ApplyPass(passName);
        BaseDraw(pos - percentOrigin * size, size);
    }
    public static void HasBorderUnevenCapsule(Vector2 pos, Vector2 percentOrigin, float distance, float round1, float round2, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        UnevenCapsuleBase(pos, percentOrigin, distance, round1, round2, matrix, nameof(HasBorderUnevenCapsule));
    }
    public static void NoBorderUnevenCapsule(Vector2 pos, Vector2 percentOrigin, float distance, float round1, float round2, Color background, Matrix matrix)
    {
        SetBackground(background);
        UnevenCapsuleBase(pos, percentOrigin, distance, round1, round2, matrix, nameof(NoBorderUnevenCapsule));
    }
    public static void BarUnevenCapsule(Vector2 pos, Vector2 percentOrigin, float distance, float round1, float round2, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        UnevenCapsuleBase(pos, percentOrigin, distance, round1, round2, matrix, nameof(BarUnevenCapsule));
    }

    #endregion

    #region 正五边形Pentagon
    static void Pentagon(float r, out Vector2 size)
    {
        const float c = 0.80902f; // cos(pi / 5)
        const float k = 1.17557f; // cos(pi / 10)/cos(pi / 5)
        SDF_Effect.Parameters["uSizeOver2"].SetValue(new Vector2(r * k, r));
        size = new Vector2(2 * r * k, (1 + 1 / c) * r);
    }
    static void PentagonBase(Vector2 pos, Vector2 percentOrigin, float r, Matrix matrix, string passName)
    {
        SetCommon(matrix);
        Pentagon(r, out Vector2 size);
        ApplyPass(passName);
        BaseDraw(pos - percentOrigin * size, size);
    }
    public static void HasBorderPentagon(Vector2 pos, Vector2 percentOrigin, float r, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        PentagonBase(pos, percentOrigin, r, matrix, nameof(HasBorderPentagon));
    }
    public static void NoBorderPentagon(Vector2 pos, Vector2 percentOrigin, float r, Color background, Matrix matrix)
    {
        SetBackground(background);
        PentagonBase(pos, percentOrigin, r, matrix, nameof(NoBorderPentagon));
    }
    public static void BarPentagon(Vector2 pos, Vector2 percentOrigin, float r, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        PentagonBase(pos, percentOrigin, r, matrix, nameof(BarPentagon));
    }
    #endregion

    #region 正六边形Hexagon
    static void Hexagon(float r, out Vector2 size)
    {
        const float c = 1.1547f;//1 / cos(pi / 6)
        size = new Vector2(c, 1) * r;
        SDF_Effect.Parameters["uSizeOver2"].SetValue(size);
        size *= 2;
    }
    static void HexagonBase(Vector2 pos, Vector2 percentOrigin, float r, Matrix matrix, string passName)
    {
        SetCommon(matrix);
        Hexagon(r, out Vector2 size);
        ApplyPass(passName);
        BaseDraw(pos - percentOrigin * size, size);
    }
    public static void HasBorderHexagon(Vector2 pos, Vector2 percentOrigin, float r, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        HexagonBase(pos, percentOrigin, r, matrix, nameof(HasBorderHexagon));
    }
    public static void NoBorderHexagon(Vector2 pos, Vector2 percentOrigin, float r, Color background, Matrix matrix)
    {
        SetBackground(background);
        HexagonBase(pos, percentOrigin, r, matrix, nameof(NoBorderHexagon));
    }
    public static void BarHexagon(Vector2 pos, Vector2 percentOrigin, float r, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        HexagonBase(pos, percentOrigin, r, matrix, nameof(BarHexagon));
    }
    #endregion

    #region 正八边形Octogon
    static void Octogon(float r, out Vector2 size)
    {
        size = new Vector2(r);
        SDF_Effect.Parameters["uSizeOver2"].SetValue(size);
        size *= 2;
    }
    static void OctogonBase(Vector2 pos, Vector2 percentOrigin, float r, Matrix matrix, string passName)
    {
        SetCommon(matrix);
        Octogon(r, out Vector2 size);
        ApplyPass(passName);
        BaseDraw(pos - percentOrigin * size, size);
    }
    public static void HasBorderOctogon(Vector2 pos, Vector2 percentOrigin, float r, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        OctogonBase(pos, percentOrigin, r, matrix, nameof(HasBorderOctogon));
    }
    public static void NoBorderOctogon(Vector2 pos, Vector2 percentOrigin, float r, Color background, Matrix matrix)
    {
        SetBackground(background);
        OctogonBase(pos, percentOrigin, r, matrix, nameof(NoBorderOctogon));
    }
    public static void BarOctogon(Vector2 pos, Vector2 percentOrigin, float r, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        OctogonBase(pos, percentOrigin, r, matrix, nameof(BarOctogon));
    }
    #endregion

    #region 正N边形Ngon
    static void Ngon(float r, float N, out Vector2 size)
    {
        Effect effect = SDF_Effect;
        float angle = MathHelper.Pi / N;
        float l = r / MathF.Cos(angle);
        float s = MathF.Sin(angle * (2 * ((int)N / 4) + 1));
        size = new Vector2(2 * l * s, (int)N % 2 == 0 ? (2 * r) : (r + l));
        effect.Parameters["uRound"].SetValue(new Vector4(N));
        effect.Parameters["uSizeOver2"].SetValue(new Vector2(size.X * .5f, r));
    }
    static void NgonBase(Vector2 pos, Vector2 percentOrigin, float r, float N, Matrix matrix, string passName)
    {
        SetCommon(matrix);
        Ngon(r, N, out Vector2 size);
        ApplyPass(passName);
        BaseDraw(pos - percentOrigin * size, size);
    }
    public static void HasBorderNgon(Vector2 pos, Vector2 percentOrigin, float r, float N, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        NgonBase(pos, percentOrigin, r, N, matrix, nameof(HasBorderNgon));
    }
    public static void NoBorderNgon(Vector2 pos, Vector2 percentOrigin, float r, float N, Color background, Matrix matrix)
    {
        SetBackground(background);
        NgonBase(pos, percentOrigin, r, N, matrix, nameof(NoBorderNgon));
    }
    public static void BarNgon(Vector2 pos, Vector2 percentOrigin, float r, float N, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        NgonBase(pos, percentOrigin, r, N, matrix, nameof(BarNgon));
    }
    #endregion

    #region 六芒星Hexagram
    static void Hexagram(float r, out Vector2 size)
    {
        size = new Vector2(1.73205f, 2) * r;
        SDF_Effect.Parameters["uSizeOver2"].SetValue(size * .5f);
    }
    static void HexagramBase(Vector2 pos, Vector2 percentOrigin, float r, Matrix matrix, string passName)
    {
        SetCommon(matrix);
        Hexagram(r, out Vector2 size);
        ApplyPass(passName);
        BaseDraw(pos - percentOrigin * size, size);

    }
    public static void HasBorderHexagram(Vector2 pos, Vector2 percentOrigin, float r, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        HexagramBase(pos, percentOrigin, r, matrix, nameof(HasBorderHexagram));
    }
    public static void NoBorderHexagram(Vector2 pos, Vector2 percentOrigin, float r, Color background, Matrix matrix)
    {
        SetBackground(background);
        HexagramBase(pos, percentOrigin, r, matrix, nameof(NoBorderHexagram));
    }
    public static void BarHexagram(Vector2 pos, Vector2 percentOrigin, float r, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        HexagramBase(pos, percentOrigin, r, matrix, nameof(BarHexagram));
    }
    #endregion

    #region 五角星Star5
    static void Star5(float r, float rf, out Vector2 size)
    {
        Effect effect = SDF_Effect;
        const float c = 0.80902f; // cos(pi / 5)
        const float c2 = 0.95106f; // cos(pi / 10)
        effect.Parameters["uSizeOver2"].SetValue(new Vector2(r * c2, r));
        effect.Parameters["uRound"].SetValue(new Vector4(rf));
        size = new Vector2(2 * r * c2, (1 + 1 * c) * r);
    }
    static void Star5Base(Vector2 pos, Vector2 percentOrigin, float r, float rf, Matrix matrix, string passName)
    {
        SetCommon(matrix);
        Star5(r, rf, out Vector2 size);
        ApplyPass(passName);
        BaseDraw(pos - percentOrigin * size, size);
    }
    public static void HasBorderStar5(Vector2 pos, Vector2 percentOrigin, float r, float rf, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        Star5Base(pos, percentOrigin, r, rf, matrix, nameof(HasBorderStar5));
    }
    public static void NoBorderStar5(Vector2 pos, Vector2 percentOrigin, float r, float rf, Color background, Matrix matrix)
    {
        SetBackground(background);
        Star5Base(pos, percentOrigin, r, rf, matrix, nameof(NoBorderStar5));
    }
    public static void BarStar5(Vector2 pos, Vector2 percentOrigin, float r, float rf, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        Star5Base(pos, percentOrigin, r, rf, matrix, nameof(BarStar5));
    }
    #endregion

    #region X角星StarX
    static void StarX(float r, float N, float k, out Vector2 size)
    {
        Effect effect = SDF_Effect;
        float angle = MathHelper.Pi / N;
        float l = r / MathF.Cos(angle);
        float s = MathF.Sin(angle * (2 * ((int)N / 4) + 1));
        size = new Vector2(2 * l * s, (int)N % 2 == 0 ? (2 * r) : (r + l));
        effect.Parameters["uRound"].SetValue(new Vector4(N, k, 0, 0));
        effect.Parameters["uSizeOver2"].SetValue(size * .5f);
    }
    static void StarXBase(Vector2 pos, Vector2 percentOrigin, float r, float N, float k, Matrix matrix, string passName)
    {
        SetCommon(matrix);
        StarX(r, N, k, out Vector2 size);
        ApplyPass(passName);
        BaseDraw(pos - percentOrigin * size, size);
    }
    public static void HasBorderStarX(Vector2 pos, Vector2 percentOrigin, float r, float N, float k, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        StarXBase(pos, percentOrigin, r, N, k, matrix, nameof(HasBorderStarX));
    }
    public static void NoBorderStarX(Vector2 pos, Vector2 percentOrigin, float r, float N, float k, Color background, Matrix matrix)
    {
        SetBackground(background);
        StarXBase(pos, percentOrigin, r, N, k, matrix, nameof(NoBorderStarX));
    }
    public static void BarStarX(Vector2 pos, Vector2 percentOrigin, float r, float N, float k, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        StarXBase(pos, percentOrigin, r, N, k, matrix, nameof(BarStarX));
    }
    #endregion

    #region 饼图Pie
    static void Pie(float r, float angle, out Vector2 size)
    {
        float s = MathF.Sin(angle);
        float c = MathF.Cos(angle);
        size = new Vector2((c < 0 ? 1 : s) * r * 2, (c < 0 ? 1 - c : 1) * r);
        Effect effect = SDF_Effect;
        effect.Parameters["uRound"].SetValue(new Vector3(s, c, r));
        effect.Parameters["uSizeOver2"].SetValue(size * .5f);
    }
    static void PieBase(Vector2 pos, Vector2 percentOrigin, float r, float angle, Matrix matrix, string passName)
    {
        SetCommon(matrix);
        Pie(r, angle, out Vector2 size);
        ApplyPass(passName);
        BaseDraw(pos - percentOrigin * size, size);
    }
    public static void HasBorderPie(Vector2 pos, Vector2 percentOrigin, float r, float angle, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        PieBase(pos, percentOrigin, r, angle, matrix, nameof(HasBorderPie));
    }
    public static void NoBorderPie(Vector2 pos, Vector2 percentOrigin, float r, float angle, Color background, Matrix matrix)
    {
        SetBackground(background);
        PieBase(pos, percentOrigin, r, angle, matrix, nameof(NoBorderPie));
    }
    public static void BarPie(Vector2 pos, Vector2 percentOrigin, float r, float angle, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        PieBase(pos, percentOrigin, r, angle, matrix, nameof(BarPie));
    }
    #endregion

    #region 弓形CutDisk
    static void CutDisk(float r, float k, out Vector2 size)
    {
        size = new Vector2((k > 0 ? 2 : MathF.Sqrt(1 - k * k) * 2), 1 + k) * r;
        Effect effect = SDF_Effect;
        effect.Parameters["uRound"].SetValue(new Vector2(k, r));
        effect.Parameters["uSizeOver2"].SetValue(size * .5f);
    }
    static void CutDiskBase(Vector2 pos, Vector2 percentOrigin, float r, float k, Matrix matrix, string passName)
    {
        SetCommon(matrix);
        CutDisk(r, k, out Vector2 size);
        ApplyPass(passName);
        BaseDraw(pos - percentOrigin * size, size);
    }
    public static void HasBorderCutDisk(Vector2 pos, Vector2 percentOrigin, float r, float k, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        CutDiskBase(pos, percentOrigin, r, k, matrix, nameof(HasBorderCutDisk));
    }
    public static void NoBorderCutDisk(Vector2 pos, Vector2 percentOrigin, float r, float k, Color background, Matrix matrix)
    {
        SetBackground(background);
        CutDiskBase(pos, percentOrigin, r, k, matrix, nameof(NoBorderCutDisk));
    }
    public static void BarCutDisk(Vector2 pos, Vector2 percentOrigin, float r, float k, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        CutDiskBase(pos, percentOrigin, r, k, matrix, nameof(BarCutDisk));
    }
    #endregion

    #region 弦Arc
    static void Arc(float r, float angle, float width, out Vector2 size)
    {
        float s = MathF.Sin(angle);
        float c = MathF.Cos(angle);
        size = new Vector2((c < 0 ? 1 : s) * r * 2, (1 - c) * r);
        size += new Vector2(width * 2);
        Effect effect = SDF_Effect;
        effect.Parameters["uRound"].SetValue(new Vector4(s, c, r, width));
        effect.Parameters["uSizeOver2"].SetValue(size * .5f);
    }
    static void ArcBase(Vector2 pos, Vector2 percentOrigin, float r, float angle, float width, Matrix matrix, string passName)
    {
        SetCommon(matrix);
        Arc(r, angle, width, out Vector2 size);
        ApplyPass(passName);
        BaseDraw(pos - percentOrigin * size, size);
    }
    public static void HasBorderArc(Vector2 pos, Vector2 percentOrigin, float r, float angle, float width, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        ArcBase(pos, percentOrigin, r, angle, width, matrix, nameof(HasBorderArc));
    }
    public static void NoBorderArc(Vector2 pos, Vector2 percentOrigin, float r, float angle, float width, Color background, Matrix matrix)
    {
        SetBackground(background);
        ArcBase(pos, percentOrigin, r, angle, width, matrix, nameof(NoBorderArc));
    }
    public static void BarArc(Vector2 pos, Vector2 percentOrigin, float r, float angle, float width, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        ArcBase(pos, percentOrigin, r, angle, width, matrix, nameof(BarArc));
    }
    #endregion

    #region 环Ring
    static void Ring(float r, float angle, float width, out Vector2 size)
    {
        float s = MathF.Sin(angle);
        float c = MathF.Cos(angle);
        size = new Vector2((c < 0 ? 1 : s) * 2 * (r + width * .5f), r + width * .5f - (r - MathF.Sign(c) * width * .5f) * c);
        Effect effect = SDF_Effect;
        effect.Parameters["uRound"].SetValue(new Vector4(r, width, c, s));
        effect.Parameters["uSizeOver2"].SetValue(size * .5f);
    }
    static void RingBase(Vector2 pos, Vector2 percentOrigin, float r, float angle, float width, Matrix matrix, string passName)
    {
        SetCommon(matrix);
        Ring(r, angle, width, out Vector2 size);
        ApplyPass(passName);
        BaseDraw(pos - percentOrigin * size, size);
    }
    public static void HasBorderRing(Vector2 pos, Vector2 percentOrigin, float r, float angle, float width, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        RingBase(pos, percentOrigin, r, angle, width, matrix, nameof(HasBorderRing));
    }
    public static void NoBorderRing(Vector2 pos, Vector2 percentOrigin, float r, float angle, float width, Color background, Matrix matrix)
    {
        SetBackground(background);
        RingBase(pos, percentOrigin, r, angle, width, matrix, nameof(NoBorderRing));
    }
    public static void BarRing(Vector2 pos, Vector2 percentOrigin, float r, float angle, float width, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        RingBase(pos, percentOrigin, r, angle, width, matrix, nameof(BarRing));
    }
    #endregion

    #region 马蹄铁Horseshoe
    static void Horseshoe(float r, float angle, float width, float length, out Vector2 size)
    {
        float s = MathF.Sin(angle);
        float c = MathF.Cos(angle);

        float sizeX = c > 0 ? (2 * r + width) : (s * (2 * r + width) - c * length * 2);
        float L = c > 0 ? MathF.Min(length, MathF.Tan(angle) * (r + width * .5f)) : length;
        float top = c * (r + MathF.Sign(c) * 0.5f * width) + s * L;
        float bottom = -r - 0.5f * width;
        size = new Vector2(sizeX, top - bottom);
        Effect effect = SDF_Effect;
        effect.Parameters["uRound"].SetValue(new Vector4(r, length, c, s));
        effect.Parameters["uLineWidth"].SetValue(width * .5f);
        effect.Parameters["uSizeOver2"].SetValue(new Vector2(sizeX * .5f, -bottom));
    }
    static void HorseshoeBase(Vector2 pos, Vector2 percentOrigin, float r, float angle, float width, float length, Matrix matrix, string passName)
    {
        SetCommon(matrix);
        Horseshoe(r, angle, width, length, out Vector2 size);
        ApplyPass(passName);
        BaseDraw(pos - percentOrigin * size, size);
    }
    public static void HasBorderHorseshoe(Vector2 pos, Vector2 percentOrigin, float r, float angle, float width, float length, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        HorseshoeBase(pos, percentOrigin, r, angle, width, length, matrix, nameof(HasBorderHorseshoe));
    }
    public static void NoBorderHorseshoe(Vector2 pos, Vector2 percentOrigin, float r, float angle, float width, float length, Color background, Matrix matrix)
    {
        SetBackground(background);
        HorseshoeBase(pos, percentOrigin, r, angle, width, length, matrix, nameof(NoBorderHorseshoe));
    }
    public static void BarHorseshoe(Vector2 pos, Vector2 percentOrigin, float r, float angle, float width, float length, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        HorseshoeBase(pos, percentOrigin, r, angle, width, length, matrix, nameof(BarHorseshoe));
    }
    #endregion

    #region 鱼鳔Vesica
    static void Vesica(float r, float distance, out Vector2 size)
    {
        size = new Vector2(r - distance, distance > 0 ? MathF.Sqrt(r * r - distance * distance) : r);
        Effect effect = SDF_Effect;
        effect.Parameters["uRound"].SetValue(new Vector2(r, distance));
        effect.Parameters["uSizeOver2"].SetValue(size);
        size *= 2;
    }
    static void VesicaBase(Vector2 pos, Vector2 percentOrigin, float r, float distance, Matrix matrix, string passName)
    {
        SetCommon(matrix);
        Vesica(r, distance, out Vector2 size);
        ApplyPass(passName);
        BaseDraw(pos - percentOrigin * size, size);
    }
    public static void HasBorderVesica(Vector2 pos, Vector2 percentOrigin, float r, float distance, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        VesicaBase(pos, percentOrigin, r, distance, matrix, nameof(HasBorderVesica));
    }
    public static void NoBorderVesica(Vector2 pos, Vector2 percentOrigin, float r, float distance, Color background, Matrix matrix)
    {
        SetBackground(background);
        VesicaBase(pos, percentOrigin, r, distance, matrix, nameof(NoBorderVesica));
    }
    public static void BarVesica(Vector2 pos, Vector2 percentOrigin, float r, float distance, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        VesicaBase(pos, percentOrigin, r, distance, matrix, nameof(BarVesica));
    }
    #endregion

    #region 正交鱼鳔OrientedVesica
    static void OrientedVesica(Vector2 pos1, Vector2 pos2, float r, out Vector2 pos, out Vector2 size)
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

        Effect effect = SDF_Effect;
        effect.Parameters["uRound"].SetValue(r - D.Length() * .5f);
        effect.Parameters["uStart"].SetValue(P1 - pos);
        effect.Parameters["uEnd"].SetValue(P2 - pos);
        effect.Parameters["uSizeOver2"].SetValue(size * .5f);

    }
    static void OrientedVesicaBase(Vector2 pos1, Vector2 pos2, float r, Matrix matrix, string passName)
    {
        SetCommon(matrix);
        OrientedVesica(pos1, pos2, r, out Vector2 pos, out Vector2 size);
        ApplyPass(passName);
        BaseDraw(pos, size);
    }
    public static void HasBorderOrientedVesica(Vector2 pos1, Vector2 pos2, float r, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        OrientedVesicaBase(pos1, pos2, r, matrix, nameof(HasBorderOrientedVesica));
    }
    public static void NoBorderOrientedVesica(Vector2 pos1, Vector2 pos2, float r, Color background, Matrix matrix)
    {
        SetBackground(background);
        OrientedVesicaBase(pos1, pos2, r, matrix, nameof(NoBorderOrientedVesica));
    }
    public static void BarOrientedVesica(Vector2 pos1, Vector2 pos2, float r, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        OrientedVesicaBase(pos1, pos2, r, matrix, nameof(BarOrientedVesica));
    }
    #endregion

    #region 月Moon
    static void Moon(float r1, float r2, float x, out Vector2 size)
    {
        float coordX = x == 0 ? float.NaN : (x * x - r2 * r2 + r1 * r1) / (2 * x);

        float offset = float.IsNaN(coordX) || MathF.Abs(coordX) > r1 ? r1 : MathF.Abs(coordX);
        size = new Vector2(offset + r1, 2 * r1);

        Vector2 orig = coordX < 0 ? new Vector2(-coordX, r1) : new Vector2(r1);
        Effect effect = SDF_Effect;
        effect.Parameters["uRound"].SetValue(new Vector3(r1, r2, x));
        effect.Parameters["uSizeOver2"].SetValue(orig);
    }
    static void MoonBase(Vector2 pos, Vector2 percentOrigin, float r1, float r2, float x, Matrix matrix, string passName)
    {
        SetCommon(matrix);
        Moon(r1, r2, x, out Vector2 size);
        ApplyPass(passName);
        BaseDraw(pos - percentOrigin * size, size);
    }
    public static void HasBorderMoon(Vector2 pos, Vector2 percentOrigin, float r1, float r2, float x, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        MoonBase(pos, percentOrigin, r1, r2, x, matrix, nameof(HasBorderMoon));

    }
    public static void NoBorderMoon(Vector2 pos, Vector2 percentOrigin, float r1, float r2, float x, Color background, Matrix matrix)
    {
        SetBackground(background);
        MoonBase(pos, percentOrigin, r1, r2, x, matrix, nameof(NoBorderMoon));
    }
    public static void BarMoon(Vector2 pos, Vector2 percentOrigin, float r1, float r2, float x, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        MoonBase(pos, percentOrigin, r1, r2, x, matrix, nameof(BarMoon));
    }
    #endregion

    #region 曲边四芒星CircleCross
    static void CircleCross(Vector2 baseSize, float r, out Vector2 size)
    {
        size = baseSize + new Vector2(2 * r);
        Effect effect = SDF_Effect;
        effect.Parameters["uRound"].SetValue(new Vector3(r, baseSize.Y / baseSize.X, baseSize.X * 0.5f));//baseSize.Y / baseSize.X
        effect.Parameters["uSizeOver2"].SetValue(size * .5f);
    }
    static void CircleCrossBase(Vector2 pos, Vector2 percentOrigin, Vector2 baseSize, float r, Matrix matrix, string passName)
    {
        SetCommon(matrix);
        CircleCross(baseSize, r, out Vector2 size);
        ApplyPass(passName);
        BaseDraw(pos - percentOrigin * size, size);
    }

    public static void HasBorderCircleCross(Vector2 pos, Vector2 percentOrigin, Vector2 baseSize, float r, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        CircleCrossBase(pos, percentOrigin, baseSize, r, matrix, nameof(HasBorderCircleCross));
    }
    public static void NoBorderCircleCross(Vector2 pos, Vector2 percentOrigin, Vector2 baseSize, float r, Color background, Matrix matrix)
    {
        SetBackground(background);
        CircleCrossBase(pos, percentOrigin, baseSize, r, matrix, nameof(NoBorderCircleCross));
    }
    public static void BarCircleCross(Vector2 pos, Vector2 percentOrigin, Vector2 baseSize, float r, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        CircleCrossBase(pos, percentOrigin, baseSize, r, matrix, nameof(BarCircleCross));
    }
    #endregion

    #region 蛋Egg
    static void Egg(float ra, float rb, float height, out Vector2 size)
    {
        size = new Vector2(ra * 2, ra + rb + height);
        Effect effect = SDF_Effect;
        effect.Parameters["uRound"].SetValue(new Vector3(ra, rb, height));//baseSize.Y / baseSize.X
        effect.Parameters["uSizeOver2"].SetValue(new Vector2(ra));
    }
    static void EggBase(Vector2 pos, Vector2 percentOrigin, float ra, float rb, float height, Matrix matrix, string passName)
    {
        SetCommon(matrix);
        Egg(ra, rb, height, out Vector2 size);
        ApplyPass(passName);
        BaseDraw(pos - percentOrigin * size, size);
    }
    public static void HasBorderEgg(Vector2 pos, Vector2 percentOrigin, float ra, float rb, float height, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        EggBase(pos, percentOrigin, ra, rb, height, matrix, nameof(HasBorderEgg));
    }
    public static void NoBorderEgg(Vector2 pos, Vector2 percentOrigin, float ra, float rb, float height, Color background, Matrix matrix)
    {
        SetBackground(background);
        EggBase(pos, percentOrigin, ra, rb, height, matrix, nameof(NoBorderEgg));
    }
    public static void BarEgg(Vector2 pos, Vector2 percentOrigin, float ra, float rb, float height, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        EggBase(pos, percentOrigin, ra, rb, height, matrix, nameof(BarEgg));
    }
    #endregion

    #region 心Heart
    static void Heart(float sizeX, out Vector2 size)
    {
        const float k = (0.75f + root2Over2 * .5f) / (0.5f + root2Over2);
        size = new Vector2(1, k) * sizeX;
        SDF_Effect.Parameters["uSizeOver2"].SetValue(new Vector2(sizeX * .5f, 0));
    }
    static void HeartBase(Vector2 pos, Vector2 percentOrigin, float sizeX, Matrix matrix, string passName)
    {
        SetCommon(matrix);
        Heart(sizeX, out Vector2 size);
        ApplyPass(passName);
        BaseDraw(pos - percentOrigin * size, size);
    }
    public static void HasBorderHeart(Vector2 pos, Vector2 percentOrigin, float sizeX, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        HeartBase(pos, percentOrigin, sizeX, matrix, nameof(HasBorderHeart));
    }
    public static void NoBorderHeart(Vector2 pos, Vector2 percentOrigin, float sizeX, Color background, Matrix matrix)
    {
        SetBackground(background);
        HeartBase(pos, percentOrigin, sizeX, matrix, nameof(NoBorderHeart));
    }
    public static void BarHeart(Vector2 pos, Vector2 percentOrigin, float sizeX, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        HeartBase(pos, percentOrigin, sizeX, matrix, nameof(BarHeart));
    }
    #endregion

    #region 十字Cross/Plus
    static void Plus(float d, float c, float r, out Vector2 size)
    {
        float s = c > r ? d - r : (c - MathF.Sqrt(r * r - c * c));
        size = new Vector2(2 * s);
        Effect effect = SDF_Effect;
        effect.Parameters["uSizeOver2"].SetValue(new Vector2(s));
        effect.Parameters["uRound"].SetValue(new Vector3(d, c, r));
    }
    static void PlusBase(Vector2 pos, Vector2 percentOrigin, float d, float c, float r, Matrix matrix, string passName)
    {
        SetCommon(matrix);
        Plus(d, c, r, out Vector2 size);
        ApplyPass(passName);
        BaseDraw(pos - percentOrigin * size, size);
    }
    public static void HasBorderPlus(Vector2 pos, Vector2 percentOrigin, float d, float c, float r, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        PlusBase(pos, percentOrigin, d, c, r, matrix, nameof(HasBorderPlus));
    }
    public static void NoBorderPlus(Vector2 pos, Vector2 percentOrigin, float d, float c, float r, Color background, Matrix matrix)
    {
        SetBackground(background);
        PlusBase(pos, percentOrigin, d, c, r, matrix, nameof(NoBorderPlus));
    }
    public static void BarPlus(Vector2 pos, Vector2 percentOrigin, float d, float c, float r, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        PlusBase(pos, percentOrigin, d, c, r, matrix, nameof(BarPlus));
    }
    #endregion

    #region 叉号RoundedX/Cross
    static void Cross(float width, float round, out Vector2 size)
    {
        const float innerShrinkage = 1;
        size = new Vector2(width + round * 2);
        Effect effect = SDF_Effect;
        effect.Parameters["uSizeOver2"].SetValue(size * .5f);
        effect.Parameters["uRound"].SetValue(new Vector2(round, width));
        effect.Parameters["uInnerShrinkage"].SetValue(innerShrinkage);
    }

    static void CrossBase(Vector2 pos, Vector2 percentOrigin, float width, float round, Matrix matrix, string passName)
    {
        width *= .5f;
        SetCommon(matrix);
        Cross(width, round, out Vector2 size);
        ApplyPass(passName);
        BaseDraw(pos - percentOrigin * size + new Vector2(width * .5f - round), size);
    }

    /// <summary>
    /// 绘制叉号
    /// </summary>
    public static void HasBorderCross(Vector2 pos, Vector2 percentOrigin, float width, float round, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        CrossBase(pos, percentOrigin, width, round, matrix, nameof(HasBorderCross));
    }

    /// <summary>
    /// 绘制叉号
    /// </summary>
    public static void NoBorderCross(Vector2 pos, Vector2 percentOrigin, float width, float round, Color background, Matrix matrix)
    {
        SetBackground(background);
        CrossBase(pos, percentOrigin, width, round, matrix, nameof(NoBorderCross));
    }

    public static void BarCross(Vector2 pos, Vector2 percentOrigin, float width, float round, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        CrossBase(pos, percentOrigin, width, round, matrix, nameof(BarCross));
    }
    #endregion

    #region 多边形Polygon
    static void Polygon(Vector2[] vecs, Matrix matrix, out Vector2 pos, out Vector2 size)
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

        Effect effect = SDF_Effect;
        effect.Parameters["uSizeOver2"].SetValue(size * .5f);
        effect.Parameters["uVectors"].SetValue((from v in vecs select v - topLeft).ToArray());
        effect.Parameters["uCurrentPointCount"].SetValue(vecs.Length);
        SetCommon(matrix);
    }

    static void PolygonBase(IEnumerable<Vector2> vecs, Matrix matrix, string passName)
    {
        Polygon(vecs.ToArray(), matrix, out Vector2 pos, out Vector2 size);
        SDF_Effect.CurrentTechnique.Passes[passName].Apply();
        BaseDraw(pos, size);
    }

    public static void HasBorderPolygon(IEnumerable<Vector2> vecs, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        PolygonBase(vecs, matrix, nameof(HasBorderPolygon));
    }

    public static void NoBorderPolygon(IEnumerable<Vector2> vecs, Color background, Matrix matrix)
    {
        SetBackground(background);
        PolygonBase(vecs, matrix, nameof(NoBorderPolygon));
    }

    public static void BarPolygon(IEnumerable<Vector2> vecs, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        PolygonBase(vecs, matrix, nameof(BarPolygon));
    }

    public static void HasBorderPolygon(Vector2 position, IEnumerable<Vector2> offsets, Color background, float border, Color borderColor, Matrix matrix)
        => HasBorderPolygon(from offset in offsets select offset + position, background, border, borderColor, matrix);

    public static void NoBorderPolygon(Vector2 position, IEnumerable<Vector2> offsets, Color background, Matrix matrix)
        => NoBorderPolygon(from offset in offsets select offset + position, background, matrix);

    public static void BarPolygon(Vector2 position, IEnumerable<Vector2> offsets, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
        => BarPolygon(from offset in offsets select offset + position, barTexture, time, distanceScaler, matrix);

    public static void HasBorderPolygon(Vector2 position, Vector2 unit, IEnumerable<Vector2> percents, Color background, float border, Color borderColor, Matrix matrix)
        => HasBorderPolygon(position, from percent in percents select unit * percent, background, border, borderColor, matrix);

    public static void NoBorderPolygon(Vector2 position, Vector2 unit, IEnumerable<Vector2> percents, Color background, Matrix matrix)
        => NoBorderPolygon(position, from percent in percents select unit * percent, background, matrix);

    public static void BarPolygon(Vector2 position, Vector2 unit, IEnumerable<Vector2> percents, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
        => BarPolygon(position, from percent in percents select unit * percent, barTexture, time, distanceScaler, matrix);

    #endregion

    #region 椭圆Ellipse
    static void Ellipse(Vector2 r)
    {
        SDF_Effect.Parameters["uSizeOver2"].SetValue(r);
    }
    static void EllipseBase(Vector2 pos, Vector2 percentOrigin, Vector2 r, Matrix matrix, string passName)
    {
        SetCommon(matrix);
        Ellipse(r);
        ApplyPass(passName);
        Vector2 size = 2 * r;
        BaseDraw(pos - percentOrigin * size, size);
    }
    public static void HasBorderEllipse(Vector2 pos, Vector2 percentOrigin, Vector2 r, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        EllipseBase(pos, percentOrigin, r, matrix, nameof(HasBorderEllipse));
    }
    public static void NoBorderEllipse(Vector2 pos, Vector2 percentOrigin, Vector2 r, Color background, Matrix matrix)
    {
        SetBackground(background);
        EllipseBase(pos, percentOrigin, r, matrix, nameof(NoBorderEllipse));
    }
    public static void BarEllipse(Vector2 pos, Vector2 percentOrigin, Vector2 r, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        EllipseBase(pos, percentOrigin, r, matrix, nameof(BarEllipse));
    }
    #endregion

    #region 抛物线Parabola
    static void Parabola(Vector2 size)
    {
        Effect effect = SDF_Effect;
        effect.Parameters["uSizeOver2"].SetValue(new Vector2(size.X * .5f, 0));
        effect.Parameters["uRound"].SetValue(4 * size.Y / (size.X * size.X));
    }
    static void ParabolaBase(Vector2 pos, Vector2 percentOrigin, Vector2 size, Matrix matrix, string passName)
    {
        SetCommon(matrix);
        Parabola(size);
        ApplyPass(passName);
        BaseDraw(pos - percentOrigin * size, size);
    }
    public static void HasBorderParabola(Vector2 pos, Vector2 percentOrigin, Vector2 size, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        ParabolaBase(pos, percentOrigin, size, matrix, nameof(HasBorderParabola));
    }
    public static void NoBorderParabola(Vector2 pos, Vector2 percentOrigin, Vector2 size, Color background, Matrix matrix)
    {
        SetBackground(background);
        ParabolaBase(pos, percentOrigin, size, matrix, nameof(NoBorderParabola));
    }
    public static void BarParabola(Vector2 pos, Vector2 percentOrigin, Vector2 size, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        ParabolaBase(pos, percentOrigin, size, matrix, nameof(BarParabola));
    }
    #endregion

    #region 抛物线段ParabolaSegment
    static void ParabolaSegment(Vector2 size, float height)
    {
        Effect effect = SDF_Effect;
        effect.Parameters["uSizeOver2"].SetValue(new Vector2(size.X * .5f, size.Y - height));
        effect.Parameters["uRound"].SetValue(new Vector2(0.5f * size.X * MathF.Sqrt(height / size.Y), height));
    }
    static void ParabolaSegmentBase(Vector2 pos, Vector2 percentOrigin, Vector2 size, float height, Matrix matrix, string passName)
    {
        SetCommon(matrix);
        ParabolaSegment(size, height);
        ApplyPass(passName);
        BaseDraw(pos - percentOrigin * size, size);
    }
    public static void HasBorderParabolaSegment(Vector2 pos, Vector2 percentOrigin, Vector2 size, float height, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        ParabolaSegmentBase(pos, percentOrigin, size, height, matrix, nameof(HasBorderParabolaSegment));
    }
    public static void NoBorderParabolaSegment(Vector2 pos, Vector2 percentOrigin, Vector2 size, float height, Color background, Matrix matrix)
    {
        SetBackground(background);
        ParabolaSegmentBase(pos, percentOrigin, size, height, matrix, nameof(NoBorderParabolaSegment));
    }
    public static void BarParabolaSegment(Vector2 pos, Vector2 percentOrigin, Vector2 size, float height, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        ParabolaSegmentBase(pos, percentOrigin, size, height, matrix, nameof(BarParabolaSegment));
    }
    #endregion

    #region 二阶贝塞尔曲线QuadraticBezier
    static void QuadraticBezier(Vector2 start, Vector2 control, Vector2 end, float width, out Vector2 pos, out Vector2 size)
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

        Effect effect = SDF_Effect;
        effect.Parameters["uStart"].SetValue(start - pos);
        effect.Parameters["uAnother"].SetValue(control - pos);
        effect.Parameters["uEnd"].SetValue(end - pos);
        effect.Parameters["uLineWidth"].SetValue(width);

    }
    static void QuadraticBezierBase(Vector2 start, Vector2 control, Vector2 end, float width, Matrix matrix, string passName)
    {
        SetCommon(matrix);
        QuadraticBezier(start, control, end, width, out Vector2 pos, out Vector2 size);
        ApplyPass(passName);
        BaseDraw(pos, size);
    }
    public static void HasBorderQuadraticBezier(Vector2 start, Vector2 control, Vector2 end, float width, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        QuadraticBezierBase(start, control, end, width, matrix, nameof(HasBorderQuadraticBezier));
    }
    public static void NoBorderQuadraticBezier(Vector2 start, Vector2 control, Vector2 end, float width, Color background, Matrix matrix)
    {
        SetBackground(background);
        QuadraticBezierBase(start, control, end, width, matrix, nameof(NoBorderQuadraticBezier));
    }
    public static void BarQuadraticBezier(Vector2 start, Vector2 control, Vector2 end, float width, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        QuadraticBezierBase(start, control, end, width, matrix, nameof(BarQuadraticBezier));
    }
    #endregion

    #region 圆头十字BlobbyCross
    static void BlobbyCross(float s, float k, float r, out Vector2 size)
    {
        float u = r > 0 ? (s + r) : k switch
        {
            > root2Over2 * .5f => root2Over2 / k - 1,
            < root2Over2 * -.5f => -(2 * k * (k - root2Over2) + .25f) / (4 * k * root2Over2),
            _ => 1,
        } * s;


        Effect effect = SDF_Effect;
        effect.Parameters["uRound"].SetValue(new Vector3(s, k, r));
        effect.Parameters["uSizeOver2"].SetValue(new Vector2(u));


        size = new Vector2(u * 2);
    }
    static void BlobbyCrossBase(Vector2 pos, Vector2 percentOrigin, float s, float k, float r, Matrix matrix, string passName)
    {
        SetCommon(matrix);
        BlobbyCross(s, k, r, out Vector2 size);
        ApplyPass(passName);
        BaseDraw(pos - percentOrigin * size, size);
    }
    public static void HasBorderBlobbyCross(Vector2 pos, Vector2 percentOrigin, float s, float k, float r, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        BlobbyCrossBase(pos, percentOrigin, s, k, r, matrix, nameof(HasBorderBlobbyCross));
    }
    public static void NoBorderBlobbyCross(Vector2 pos, Vector2 percentOrigin, float s, float k, float r, Color background, Matrix matrix)
    {
        SetBackground(background);
        BlobbyCrossBase(pos, percentOrigin, s, k, r, matrix, nameof(NoBorderBlobbyCross));
    }
    public static void BarBlobbyCross(Vector2 pos, Vector2 percentOrigin, float s, float k, float r, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        BlobbyCrossBase(pos, percentOrigin, s, k, r, matrix, nameof(BarBlobbyCross));
    }
    #endregion

    #region 隧道Tunnel
    static void Tunnel(Vector2 wh, out Vector2 size)
    {
        SDF_Effect.Parameters["uSizeOver2"].SetValue(wh);
        size = new Vector2(2 * wh.X, wh.X + wh.Y);
    }
    static void TunnelBase(Vector2 pos, Vector2 percentOrigin, Vector2 wh, Matrix matrix, string passName)
    {
        SetCommon(matrix);
        Tunnel(wh, out Vector2 size);
        ApplyPass(passName);
        BaseDraw(pos - percentOrigin * size, size);
    }
    public static void HasBorderTunnel(Vector2 pos, Vector2 percentOrigin, Vector2 wh, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        TunnelBase(pos, percentOrigin, wh, matrix, nameof(HasBorderTunnel));
    }
    public static void NoBorderTunnel(Vector2 pos, Vector2 percentOrigin, Vector2 wh, Color background, Matrix matrix)
    {
        SetBackground(background);
        TunnelBase(pos, percentOrigin, wh, matrix, nameof(NoBorderTunnel));
    }
    public static void BarTunnel(Vector2 pos, Vector2 percentOrigin, Vector2 wh, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        TunnelBase(pos, percentOrigin, wh, matrix, nameof(BarTunnel));
    }
    #endregion

    #region 楼梯Stairs
    static void Stairs(Vector2 unit, float count, out Vector2 size)
    {
        size = unit * count;
        Effect effect = SDF_Effect;
        effect.Parameters["uRound"].SetValue(count);
        effect.Parameters["uSizeOver2"].SetValue(unit);
    }
    static void StairsBase(Vector2 pos, Vector2 percentOrigin, Vector2 unit, float count, Matrix matrix, string passName)
    {
        SetCommon(matrix);
        Stairs(unit, count, out Vector2 size);
        ApplyPass(passName);
        BaseDraw(pos - percentOrigin * size, size);
    }
    public static void HasBorderStairs(Vector2 pos, Vector2 percentOrigin, Vector2 unit, float count, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        StairsBase(pos, percentOrigin, unit, count, matrix, nameof(HasBorderStairs));
    }
    public static void NoBorderStairs(Vector2 pos, Vector2 percentOrigin, Vector2 unit, float count, Color background, Matrix matrix)
    {
        SetBackground(background);
        StairsBase(pos, percentOrigin, unit, count, matrix, nameof(NoBorderStairs));
    }
    public static void BarStairs(Vector2 pos, Vector2 percentOrigin, Vector2 unit, float count, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        StairsBase(pos, percentOrigin, unit, count, matrix, nameof(BarStairs));
    }
    #endregion

    #region 四次方圆QuadraticCircle
    static void QuadraticCircle(float r)
    {
        SDF_Effect.Parameters["uSizeOver2"].SetValue(new Vector2(r));
    }
    static void QuadraticCircleBase(Vector2 pos, Vector2 percentOrigin, float r, Matrix matrix, string passName)
    {
        SetCommon(matrix);
        QuadraticCircle(r);
        ApplyPass(passName);
        BaseDraw(pos - percentOrigin * r * 2, new Vector2(r * 2));
    }
    public static void HasBorderQuadraticCircle(Vector2 pos, Vector2 percentOrigin, float r, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        QuadraticCircleBase(pos, percentOrigin, r, matrix, nameof(HasBorderQuadraticCircle));
    }
    public static void NoBorderQuadraticCircle(Vector2 pos, Vector2 percentOrigin, float r, Color background, Matrix matrix)
    {
        SetBackground(background);
        QuadraticCircleBase(pos, percentOrigin, r, matrix, nameof(NoBorderQuadraticCircle));
    }
    public static void BarQuadraticCircle(Vector2 pos, Vector2 percentOrigin, float r, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        QuadraticCircleBase(pos, percentOrigin, r, matrix, nameof(BarQuadraticCircle));
    }
    #endregion

    #region 双曲线Hyperbola
    static void Hyperbola(Vector2 size)
    {
        float k = (size.X * size.X - size.Y * size.Y) * .125f;//MathF.Sqrt((size.X * size.X - size.Y * size.Y) * .25f - 1);
        Effect effect = SDF_Effect;
        effect.Parameters["uSizeOver2"].SetValue(size * .5f);
        effect.Parameters["uRound"].SetValue(k);
    }
    static void HyperbolaBase(Vector2 pos, Vector2 percentOrigin, Vector2 size, Matrix matrix, string passName)
    {
        SetCommon(matrix);
        Hyperbola(size);
        ApplyPass(passName);
        BaseDraw(pos - percentOrigin * size, size);
    }
    public static void HasBorderHyperbola(Vector2 pos, Vector2 percentOrigin, Vector2 size, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        HyperbolaBase(pos, percentOrigin, size, matrix, nameof(HasBorderHyperbola));
    }
    public static void NoBorderHyperbola(Vector2 pos, Vector2 percentOrigin, Vector2 size, Color background, Matrix matrix)
    {
        SetBackground(background);
        HyperbolaBase(pos, percentOrigin, size, matrix, nameof(NoBorderHyperbola));
    }
    public static void BarHyperbola(Vector2 pos, Vector2 percentOrigin, Vector2 size, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        HyperbolaBase(pos, percentOrigin, size, matrix, nameof(BarHyperbola));
    }
    #endregion

    #region 酷酷S CoolS
    static void CoolS(float height, Matrix matrix)
    {
        Effect effect = SDF_Effect;
        effect.Parameters["uSizeOver2"].SetValue(new Vector2(.2f, .5f) * height);
        SetCommon(matrix);
    }
    static void CoolSBase(Vector2 pos, Vector2 percentOrigin, float height, Matrix matrix, string passName)
    {
        CoolS(height, matrix);
        ApplyPass(passName);
        Vector2 size = new Vector2(.4f, 1f) * height;
        BaseDraw(pos - percentOrigin * size, size);
    }
    public static void HasBorderCoolS(Vector2 pos, Vector2 percentOrigin, float height, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        CoolSBase(pos, percentOrigin, height, matrix, nameof(HasBorderCoolS));
    }
    public static void NoBorderCoolS(Vector2 pos, Vector2 percentOrigin, float height, Color background, Matrix matrix)
    {
        SetBackground(background);
        CoolSBase(pos, percentOrigin, height, matrix, nameof(NoBorderCoolS));
    }
    public static void BarCoolS(Vector2 pos, Vector2 percentOrigin, float height, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        CoolSBase(pos, percentOrigin, height, matrix, nameof(BarCoolS));
    }
    #endregion

    #region 圆波CircleWave
    static void CircleWave(float width, float k, float r, float lineWidth, out Vector2 size)
    {
        float height = (1 - MathF.Cos(5 / 6.0f * k * MathHelper.Pi)) * 2 * r;
        size = new Vector2(width, height) + new Vector2(2 * lineWidth);

        Effect effect = SDF_Effect;
        effect.Parameters["uSizeOver2"].SetValue(new Vector2(width * .5f + lineWidth, height - r + lineWidth));
        effect.Parameters["uRound"].SetValue(new Vector2(k, r));

        effect.Parameters["uLineWidth"].SetValue(lineWidth);
    }

    static void CircleWaveBase(Vector2 pos, Vector2 percentOrigin, float width, float k, float r, float lineWidth, Matrix matrix, string passName)
    {
        SetCommon(matrix);
        CircleWave(width, k, r, lineWidth, out Vector2 size);
        ApplyPass(passName);
        BaseDraw(pos - percentOrigin * size, size);
    }
    public static void HasBorderCircleWave(Vector2 pos, Vector2 percentOrigin, float width, float k, float r, float lineWidth, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        CircleWaveBase(pos, percentOrigin, width, k, r, lineWidth, matrix, nameof(HasBorderCircleWave));
    }

    public static void NoBorderCircleWave(Vector2 pos, Vector2 percentOrigin, float width, float k, float r, float lineWidth, Color background, Matrix matrix)
    {
        SetBackground(background);
        CircleWaveBase(pos, percentOrigin, width, k, r, lineWidth, matrix, nameof(NoBorderCircleWave));
    }

    public static void BarCircleWave(Vector2 pos, Vector2 percentOrigin, float width, float k, float r, float lineWidth, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        CircleWaveBase(pos, percentOrigin, width, k, r, lineWidth, matrix, nameof(BarCircleWave));
    }
    #endregion

    #region 连段二阶贝塞尔曲线ChainedQuadraticBezier
    static void ChainedQuadraticBezier(Vector2[] vecs, out Vector2 pos, out Vector2 size)
    {
        int length = vecs.Length;
        Vector2 topLeft = vecs[0];
        Vector2 bottomRight = vecs[0];
        for (int n = 0; n < length - 2; n += 2)
        {
            Vector2 start = vecs[n];
            Vector2 control = vecs[n + 1];
            Vector2 end = vecs[n + 2];
            topLeft = Vector2.Min(topLeft, vecs[n + 2]);
            bottomRight = Vector2.Max(bottomRight, vecs[n + 2]);
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
        }
        pos = topLeft;
        size = bottomRight - topLeft;

        Effect effect = SDF_Effect;
        effect.Parameters["uCurrentPointCount"].SetValue(length);
        effect.Parameters["uBVectors"].SetValue((from v in vecs select v - topLeft).ToArray());
    }
    static void ChainedQuadraticBezierBase(IEnumerable<Vector2> vecs, Matrix matrix, string passName)
    {
        SetCommon(matrix);
        ChainedQuadraticBezier(vecs.ToArray(), out Vector2 pos, out Vector2 size);
        ApplyPass(passName);
        BaseDraw(pos, size);
    }
    public static void HasBorderChainedQuadraticBezier(IEnumerable<Vector2> vecs, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        ChainedQuadraticBezierBase(vecs, matrix, nameof(HasBorderChainedQuadraticBezier));
    }
    public static void NoBorderChainedQuadraticBezier(IEnumerable<Vector2> vecs, Color background, Matrix matrix)
    {
        SetBackground(background);
        ChainedQuadraticBezierBase(vecs, matrix, nameof(NoBorderChainedQuadraticBezier));
    }
    public static void BarChainedQuadraticBezier(IEnumerable<Vector2> vecs, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        ChainedQuadraticBezierBase(vecs, matrix, nameof(BarChainedQuadraticBezier));
    }
    public static void HasBorderChainedQuadraticBezier(Vector2 position, IEnumerable<Vector2> offsets, Color background, float border, Color borderColor, Matrix matrix)
        => HasBorderChainedQuadraticBezier(from offset in offsets select offset + position, background, border, borderColor, matrix);
    public static void NoBorderChainedQuadraticBezier(Vector2 position, IEnumerable<Vector2> offsets, Color background, Matrix matrix)
        => NoBorderChainedQuadraticBezier(from offset in offsets select offset + position, background, matrix);
    public static void BarChainedQuadraticBezier(Vector2 position, IEnumerable<Vector2> offsets, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
        => BarChainedQuadraticBezier(from offset in offsets select offset + position, barTexture, time, distanceScaler, matrix);
    public static void HasBorderChainedQuadraticBezier(Vector2 position, Vector2 unit, IEnumerable<Vector2> percents, Color background, float border, Color borderColor, Matrix matrix)
        => HasBorderChainedQuadraticBezier(position, from percent in percents select unit * percent, background, border, borderColor, matrix);
    public static void NoBorderChainedQuadraticBezier(Vector2 position, Vector2 unit, IEnumerable<Vector2> percents, Color background, Matrix matrix)
        => NoBorderChainedQuadraticBezier(position, from percent in percents select unit * percent, background, matrix);
    public static void BarChainedQuadraticBezier(Vector2 position, Vector2 unit, IEnumerable<Vector2> percents, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
        => BarChainedQuadraticBezier(position, from percent in percents select unit * percent, barTexture, time, distanceScaler, matrix);
    #endregion
}
