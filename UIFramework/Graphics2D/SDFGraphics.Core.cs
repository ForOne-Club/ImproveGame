namespace ImproveGame.UIFramework.Graphics2D;
public static partial class SDFGraphics
{
    #region 圆Circle/Round

    static void Round(float size)
    {
        const float innerShrinkage = 1;
        size += innerShrinkage * 2;
        SDF_Effect.Parameters["uSizeOver2"].SetValue(new Vector2(size) / 2f);
    }
    static void RoundBase(Vector2 pos, Vector2 percentOrigin, float size, Matrix matrix, string passName)
    {
        SetCommon(matrix);
        Round(size);
        const float innerShrinkage = 1;
        pos -= new Vector2(innerShrinkage);
        size += innerShrinkage * 2;
        SDF_Effect.CurrentTechnique.Passes[passName].Apply();
        BaseDraw(pos - percentOrigin * size, new Vector2(size));
    }
    /// <summary>
    /// 绘制一个圆，有边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="size">直径</param>
    /// <param name="background">背景色</param>
    /// <param name="border">边界宽度</param>
    /// <param name="borderColor">边界颜色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void HasBorderRound(Vector2 pos, Vector2 percentOrigin, float size, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        RoundBase(pos, percentOrigin, size, matrix, nameof(HasBorderRound));
    }
    /// <summary>
    /// 绘制一个圆，无边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="size">直径</param>
    /// <param name="background">背景色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void NoBorderRound(Vector2 pos, Vector2 percentOrigin, float size, Color background, Matrix matrix)
    {
        SetBackground(background);
        RoundBase(pos, percentOrigin, size, matrix, nameof(NoBorderRound));
    }
    /// <summary>
    /// 绘制一个圆，根据距离场使用采样色条染色
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="size">直径</param>
    /// <param name="barTexture">采样色条</param>
    /// <param name="time">时间偏移量</param>
    /// <param name="distanceScaler">距离系数</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void BarRound(Vector2 pos, Vector2 percentOrigin, float size, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        RoundBase(pos, percentOrigin, size, matrix, nameof(BarRound));
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
    /// <summary>
    /// 绘制一个曲边矩形，有边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="size">大小</param>
    /// <param name="round">拐角圆半径<br>x y z w分别对应右下 右上 左下 左上</br></param>
    /// <param name="background">背景色</param>
    /// <param name="border">边界宽度</param>
    /// <param name="borderColor">边界颜色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void HasBorderRoundedBox(Vector2 pos, Vector2 percentOrigin, Vector2 size, Vector4 round, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        RoundedBoxBase(pos, percentOrigin, size, round, matrix, nameof(HasBorderRoundedBox));
    }
    /// <summary>
    /// 绘制一个曲边矩形，无边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="size">大小</param>
    /// <param name="round">拐角圆半径<br>x y z w分别对应右下 右上 左下 左上</br></param>
    /// <param name="background">背景色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void NoBorderRoundedBox(Vector2 pos, Vector2 percentOrigin, Vector2 size, Vector4 round, Color background, Matrix matrix)
    {
        SetBackground(background);
        RoundedBoxBase(pos, percentOrigin, size, round, matrix, nameof(NoBorderRoundedBox));
    }
    /// <summary>
    /// 绘制一个曲边矩形，根据距离场使用采样色条染色
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="size">大小</param>
    /// <param name="round">拐角圆半径<br>x y z w分别对应右下 右上 左下 左上</br></param>
    /// <param name="barTexture">采样色条</param>
    /// <param name="time">时间偏移量</param>
    /// <param name="distanceScaler">距离系数</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
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
    /// <summary>
    /// 绘制一个矩形，有边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="size">大小</param>
    /// <param name="background">背景色</param>
    /// <param name="border">边界宽度</param>
    /// <param name="borderColor">边界颜色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void HasBorderBox(Vector2 pos, Vector2 percentOrigin, Vector2 size, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        BoxBase(pos, percentOrigin, size, matrix, nameof(HasBorderBox));
    }
    /// <summary>
    /// 绘制一个矩形，无边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="size">大小</param>
    /// <param name="background">背景色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void NoBorderBox(Vector2 pos, Vector2 percentOrigin, Vector2 size, Color background, Matrix matrix)
    {
        SetBackground(background);
        BoxBase(pos, percentOrigin, size, matrix, nameof(NoBorderBox));
    }
    /// <summary>
    /// 绘制一个矩形，根据距离场使用采样色条染色
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="size">大小</param>
    /// <param name="barTexture">采样色条</param>
    /// <param name="time">时间偏移量</param>
    /// <param name="distanceScaler">距离系数</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
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
    /// <summary>
    /// 绘制一个正交矩形，有边框
    /// </summary>
    /// <param name="start">起点</param>
    /// <param name="end">终点</param>
    /// <param name="width">宽度</param>
    /// <param name="background">背景色</param>
    /// <param name="border">边界宽度</param>
    /// <param name="borderColor">边界颜色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void HasBorderOrientedBox(Vector2 start, Vector2 end, float width, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        OrientedBoxBase(start, end, width, matrix, nameof(HasBorderOrientedBox));
    }
    /// <summary>
    /// 绘制一个正交矩形，无边框
    /// </summary>
    /// <param name="start">起点</param>
    /// <param name="end">终点</param>
    /// <param name="width">宽度</param>
    /// <param name="background">背景色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void NoBorderOrientedBox(Vector2 start, Vector2 end, float width, Color background, Matrix matrix)
    {
        SetBackground(background);
        OrientedBoxBase(start, end, width, matrix, nameof(NoBorderOrientedBox));
    }
    /// <summary>
    /// 绘制一个正交矩形，根据距离场使用采样色条染色
    /// </summary>
    /// <param name="start">起点</param>
    /// <param name="end">终点</param>
    /// <param name="width">宽度</param>
    /// <param name="barTexture">采样色条</param>
    /// <param name="time">时间偏移量</param>
    /// <param name="distanceScaler">距离系数</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void BarOrientedBox(Vector2 start, Vector2 end, float width, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        OrientedBoxBase(start, end, width, matrix, nameof(BarOrientedBox));
    }
    #endregion

    #region 线段Segment/Line
    static void Line(Vector2 start, Vector2 end, float width, out Vector2 pos, out Vector2 size)
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
    static void LineBase(Vector2 start, Vector2 end, float width, Matrix matrix, string passName)
    {
        SetCommon(matrix);
        Line(start, end, width, out Vector2 pos, out Vector2 size);
        ApplyPass(passName);
        BaseDraw(pos, size);
    }
    /// <summary>
    /// 绘制一个线段，有边框
    /// </summary>
    /// <param name="start">起点</param>
    /// <param name="end">终点</param>
    /// <param name="width">宽度</param>
    /// <param name="background">背景色</param>
    /// <param name="border">边界宽度</param>
    /// <param name="borderColor">边界颜色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void HasBorderLine(Vector2 start, Vector2 end, float width, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        LineBase(start, end, width, matrix, nameof(HasBorderLine));
    }
    /// <summary>
    /// 绘制一个线段，无边框
    /// </summary>
    /// <param name="start">起点</param>
    /// <param name="end">终点</param>
    /// <param name="width">宽度</param>
    /// <param name="background">背景色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void NoBorderLine(Vector2 start, Vector2 end, float width, Color background, Matrix matrix)
    {
        SetBackground(background);
        LineBase(start, end, width, matrix, nameof(NoBorderLine));
    }
    /// <summary>
    /// 绘制一个线段，根据距离场使用采样色条染色
    /// </summary>
    /// <param name="start">起点</param>
    /// <param name="end">终点</param>
    /// <param name="width">宽度</param>
    /// <param name="barTexture">采样色条</param>
    /// <param name="time">时间偏移量</param>
    /// <param name="distanceScaler">距离系数</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void BarLine(Vector2 start, Vector2 end, float width, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        LineBase(start, end, width, matrix, nameof(BarLine));
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
    /// 绘制一个菱形，有边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="size">大小</param>
    /// <param name="background">背景色</param>
    /// <param name="border">边界宽度</param>
    /// <param name="borderColor">边界颜色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void HasBorderRhombus(Vector2 pos, Vector2 percentOrigin, Vector2 size, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        RhombusBase(pos, percentOrigin, size, matrix, nameof(HasBorderRhombus));
    }
    /// <summary>
    /// 绘制一个菱形，无边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="size">大小</param>
    /// <param name="background">背景色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void NoBorderRhombus(Vector2 pos, Vector2 percentOrigin, Vector2 size, Color background, Matrix matrix)
    {
        SetBackground(background);
        RhombusBase(pos, percentOrigin, size, matrix, nameof(NoBorderRhombus));
    }
    /// <summary>
    /// 绘制一个菱形，根据距离场使用采样色条染色
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="size">大小</param>
    /// <param name="barTexture">采样色条</param>
    /// <param name="time">时间偏移量</param>
    /// <param name="distanceScaler">距离系数</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
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
    /// <summary>
    /// 绘制一个等腰梯形，有边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="size">大小</param>
    /// <param name="bottomScaler">底与高的比值</param>
    /// <param name="background">背景色</param>
    /// <param name="border">边界宽度</param>
    /// <param name="borderColor">边界颜色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void HasBorderTrapezoid(Vector2 pos, Vector2 percentOrigin, Vector2 size, float bottomScaler, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        TrapezoidBase(pos, percentOrigin, size, bottomScaler, matrix, nameof(HasBorderTrapezoid));
    }
    /// <summary>
    /// 绘制一个等腰梯形，无边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="size">大小</param>
    /// <param name="bottomScaler">底与高的比值</param>
    /// <param name="background">背景色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void NoBorderTrapezoid(Vector2 pos, Vector2 percentOrigin, Vector2 size, float bottomScaler, Color background, Matrix matrix)
    {
        SetBackground(background);
        TrapezoidBase(pos, percentOrigin, size, bottomScaler, matrix, nameof(NoBorderTrapezoid));
    }
    /// <summary>
    /// 绘制一个等腰梯形，根据距离场使用采样色条染色
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="size">大小</param>
    /// <param name="bottomScaler">底与高的比值</param>
    /// <param name="barTexture">采样色条</param>
    /// <param name="time">时间偏移量</param>
    /// <param name="distanceScaler">距离系数</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
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
    /// <summary>
    /// 绘制一个平行四边形，有边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="size">大小</param>
    /// <param name="bottomScaler">底占宽的比值，取值[-1,1]，负数表示水平翻转</param>
    /// <param name="background">背景色</param>
    /// <param name="border">边界宽度</param>
    /// <param name="borderColor">边界颜色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void HasBorderParallelogram(Vector2 pos, Vector2 percentOrigin, Vector2 size, float bottomScaler, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        ParallelogramBase(pos, percentOrigin, size, bottomScaler, matrix, nameof(HasBorderParallelogram));
    }
    /// <summary>
    /// 绘制一个平行四边形，无边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="size">大小</param>
    /// <param name="bottomScaler">底占宽的比值，取值[-1,1]，负数表示水平翻转</param>
    /// <param name="background">背景色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void NoBorderParallelogram(Vector2 pos, Vector2 percentOrigin, Vector2 size, float bottomScaler, Color background, Matrix matrix)
    {
        SetBackground(background);
        ParallelogramBase(pos, percentOrigin, size, bottomScaler, matrix, nameof(NoBorderParallelogram));
    }
    /// <summary>
    /// 绘制一个平行四边形，根据距离场使用采样色条染色
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="size">大小</param>
    /// <param name="bottomScaler">底占宽的比值，取值[-1,1]，负数表示水平翻转</param>
    /// <param name="barTexture">采样色条</param>
    /// <param name="time">时间偏移量</param>
    /// <param name="distanceScaler">距离系数</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
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
    /// <summary>
    /// 绘制一个等边三角形，有边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r">边长</param>
    /// <param name="background">背景色</param>
    /// <param name="border">边界宽度</param>
    /// <param name="borderColor">边界颜色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void HasBorderEquilateralTriangle(Vector2 pos, Vector2 percentOrigin, float r, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        EquilateralTriangleBase(pos, percentOrigin, r, matrix, nameof(HasBorderEquilateralTriangle));
    }
    /// <summary>
    /// 绘制一个等边三角形，无边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r">边长</param>
    /// <param name="background">背景色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void NoBorderEquilateralTriangle(Vector2 pos, Vector2 percentOrigin, float r, Color background, Matrix matrix)
    {
        SetBackground(background);
        EquilateralTriangleBase(pos, percentOrigin, r, matrix, nameof(NoBorderEquilateralTriangle));
    }
    /// <summary>
    /// 绘制一个等边三角形，根据距离场使用采样色条染色
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r">边长</param>
    /// <param name="barTexture">采样色条</param>
    /// <param name="time">时间偏移量</param>
    /// <param name="distanceScaler">距离系数</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
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
    /// <summary>
    /// 绘制一个等腰三角形，有边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="size">大小</param>
    /// <param name="background">背景色</param>
    /// <param name="border">边界宽度</param>
    /// <param name="borderColor">边界颜色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void HasBorderTriangleIsosceles(Vector2 pos, Vector2 percentOrigin, Vector2 size, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        TriangleIsoscelesBase(pos, percentOrigin, size, matrix, nameof(HasBorderTriangleIsosceles));
    }
    /// <summary>
    /// 绘制一个等腰三角形，无边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="size">大小</param>
    /// <param name="background">背景色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void NoBorderTriangleIsosceles(Vector2 pos, Vector2 percentOrigin, Vector2 size, Color background, Matrix matrix)
    {
        SetBackground(background);
        TriangleIsoscelesBase(pos, percentOrigin, size, matrix, nameof(NoBorderTriangleIsosceles));
    }
    /// <summary>
    /// 绘制一个等腰三角形，根据距离场使用采样色条染色
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="size">大小</param>
    /// <param name="barTexture">采样色条</param>
    /// <param name="time">时间偏移量</param>
    /// <param name="distanceScaler">距离系数</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
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
    /// <summary>
    /// 绘制一个三角形，有边框
    /// </summary>
    /// <param name="A">第一个顶点</param>
    /// <param name="B">第二个顶点</param>
    /// <param name="C">第三个顶点</param>
    /// <param name="background">背景色</param>
    /// <param name="border">边界宽度</param>
    /// <param name="borderColor">边界颜色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void HasBorderTriangle(Vector2 A, Vector2 B, Vector2 C, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        TriangleBase(A, B, C, matrix, nameof(HasBorderTriangle));
    }
    /// <summary>
    /// 绘制一个三角形，无边框
    /// </summary>
    /// <param name="A">第一个顶点</param>
    /// <param name="B">第二个顶点</param>
    /// <param name="C">第三个顶点</param>
    /// <param name="background">背景色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void NoBorderTriangle(Vector2 A, Vector2 B, Vector2 C, Color background, Matrix matrix)
    {
        SetBackground(background);
        TriangleBase(A, B, C, matrix, nameof(NoBorderTriangle));
    }
    /// <summary>
    /// 绘制一个三角形，根据距离场使用采样色条染色
    /// </summary>
    /// <param name="A">第一个顶点</param>
    /// <param name="B">第二个顶点</param>
    /// <param name="C">第三个顶点</param>
    /// <param name="barTexture">采样色条</param>
    /// <param name="time">时间偏移量</param>
    /// <param name="distanceScaler">距离系数</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
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
    /// <summary>
    /// 绘制一个胶囊，有边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="distance">圆心距</param>
    /// <param name="round1">下圆半径</param>
    /// <param name="round2">上圆半径</param>
    /// <param name="background">背景色</param>
    /// <param name="border">边界宽度</param>
    /// <param name="borderColor">边界颜色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void HasBorderUnevenCapsule(Vector2 pos, Vector2 percentOrigin, float distance, float round1, float round2, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        UnevenCapsuleBase(pos, percentOrigin, distance, round1, round2, matrix, nameof(HasBorderUnevenCapsule));
    }
    /// <summary>
    /// 绘制一个胶囊，无边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="distance">圆心距</param>
    /// <param name="round1">下圆半径</param>
    /// <param name="round2">上圆半径</param>
    /// <param name="background">背景色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void NoBorderUnevenCapsule(Vector2 pos, Vector2 percentOrigin, float distance, float round1, float round2, Color background, Matrix matrix)
    {
        SetBackground(background);
        UnevenCapsuleBase(pos, percentOrigin, distance, round1, round2, matrix, nameof(NoBorderUnevenCapsule));
    }
    /// <summary>
    /// 绘制一个胶囊，根据距离场使用采样色条染色
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="distance">圆心距</param>
    /// <param name="round1">下圆半径</param>
    /// <param name="round2">上圆半径</param>
    /// <param name="barTexture">采样色条</param>
    /// <param name="time">时间偏移量</param>
    /// <param name="distanceScaler">距离系数</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
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
    /// <summary>
    /// 绘制一个正五边形，有边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r">内切圆半径</param>
    /// <param name="background">背景色</param>
    /// <param name="border">边界宽度</param>
    /// <param name="borderColor">边界颜色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void HasBorderPentagon(Vector2 pos, Vector2 percentOrigin, float r, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        PentagonBase(pos, percentOrigin, r, matrix, nameof(HasBorderPentagon));
    }
    /// <summary>
    /// 绘制一个正五边形，无边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r">内切圆半径</param>
    /// <param name="background">背景色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void NoBorderPentagon(Vector2 pos, Vector2 percentOrigin, float r, Color background, Matrix matrix)
    {
        SetBackground(background);
        PentagonBase(pos, percentOrigin, r, matrix, nameof(NoBorderPentagon));
    }
    /// <summary>
    /// 绘制一个正五边形，根据距离场使用采样色条染色
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r">内切圆半径</param>
    /// <param name="barTexture">采样色条</param>
    /// <param name="time">时间偏移量</param>
    /// <param name="distanceScaler">距离系数</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
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
    /// <summary>
    /// 绘制一个正六边形，有边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r">内切圆半径</param>
    /// <param name="background">背景色</param>
    /// <param name="border">边界宽度</param>
    /// <param name="borderColor">边界颜色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void HasBorderHexagon(Vector2 pos, Vector2 percentOrigin, float r, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        HexagonBase(pos, percentOrigin, r, matrix, nameof(HasBorderHexagon));
    }
    /// <summary>
    /// 绘制一个正六边形，无边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r">内切圆半径</param>
    /// <param name="background">背景色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void NoBorderHexagon(Vector2 pos, Vector2 percentOrigin, float r, Color background, Matrix matrix)
    {
        SetBackground(background);
        HexagonBase(pos, percentOrigin, r, matrix, nameof(NoBorderHexagon));
    }
    /// <summary>
    /// 绘制一个正五边形，根据距离场使用采样色条染色
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r">内切圆半径</param>
    /// <param name="barTexture">采样色条</param>
    /// <param name="time">时间偏移量</param>
    /// <param name="distanceScaler">距离系数</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
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
    /// <summary>
    /// 绘制一个正八边形，有边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r">内切圆半径</param>
    /// <param name="background">背景色</param>
    /// <param name="border">边界宽度</param>
    /// <param name="borderColor">边界颜色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void HasBorderOctogon(Vector2 pos, Vector2 percentOrigin, float r, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        OctogonBase(pos, percentOrigin, r, matrix, nameof(HasBorderOctogon));
    }
    /// <summary>
    /// 绘制一个正八边形，无边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r">内切圆半径</param>
    /// <param name="background">背景色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void NoBorderOctogon(Vector2 pos, Vector2 percentOrigin, float r, Color background, Matrix matrix)
    {
        SetBackground(background);
        OctogonBase(pos, percentOrigin, r, matrix, nameof(NoBorderOctogon));
    }
    /// <summary>
    /// 绘制一个正八边形，根据距离场使用采样色条染色
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r">内切圆半径</param>
    /// <param name="barTexture">采样色条</param>
    /// <param name="time">时间偏移量</param>
    /// <param name="distanceScaler">距离系数</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
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
    /// <summary>
    /// 绘制一个正N边形，有边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r">内切圆半径</param>
    /// <param name="N">边数</param>
    /// <param name="background">背景色</param>
    /// <param name="border">边界宽度</param>
    /// <param name="borderColor">边界颜色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void HasBorderNgon(Vector2 pos, Vector2 percentOrigin, float r, float N, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        NgonBase(pos, percentOrigin, r, N, matrix, nameof(HasBorderNgon));
    }
    /// <summary>
    /// 绘制一个正N边形，无边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r">内切圆半径</param>
    /// <param name="N">边数</param>
    /// <param name="background">背景色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void NoBorderNgon(Vector2 pos, Vector2 percentOrigin, float r, float N, Color background, Matrix matrix)
    {
        SetBackground(background);
        NgonBase(pos, percentOrigin, r, N, matrix, nameof(NoBorderNgon));
    }
    /// <summary>
    /// 绘制一个正N边形，根据距离场使用采样色条染色
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r">内切圆半径</param>
    /// <param name="N">边数</param>
    /// <param name="barTexture">采样色条</param>
    /// <param name="time">时间偏移量</param>
    /// <param name="distanceScaler">距离系数</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
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
    /// <summary>
    /// 绘制一个六芒星，有边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r">外接圆半径</param>
    /// <param name="background">背景色</param>
    /// <param name="border">边界宽度</param>
    /// <param name="borderColor">边界颜色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void HasBorderHexagram(Vector2 pos, Vector2 percentOrigin, float r, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        HexagramBase(pos, percentOrigin, r, matrix, nameof(HasBorderHexagram));
    }
    /// <summary>
    /// 绘制一个六芒星，无边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r">外接圆半径</param>
    /// <param name="background">背景色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void NoBorderHexagram(Vector2 pos, Vector2 percentOrigin, float r, Color background, Matrix matrix)
    {
        SetBackground(background);
        HexagramBase(pos, percentOrigin, r, matrix, nameof(NoBorderHexagram));
    }
    /// <summary>
    /// 绘制一个六芒星，根据距离场使用采样色条染色
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r"></param>
    /// <param name="barTexture">采样色条</param>
    /// <param name="time">时间偏移量</param>
    /// <param name="distanceScaler">距离系数</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
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
    /// <summary>
    /// 绘制一个五角星，有边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r">外接圆半径</param>
    /// <param name="rf">内切圆半径与外接圆半径比值，小于1</param>
    /// <param name="background">背景色</param>
    /// <param name="border">边界宽度</param>
    /// <param name="borderColor">边界颜色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void HasBorderStar5(Vector2 pos, Vector2 percentOrigin, float r, float rf, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        Star5Base(pos, percentOrigin, r, rf, matrix, nameof(HasBorderStar5));
    }
    /// <summary>
    /// 绘制一个五角星，无边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r">外接圆半径</param>
    /// <param name="rf">内切圆半径与外接圆半径比值，小于1</param>
    /// <param name="background">背景色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void NoBorderStar5(Vector2 pos, Vector2 percentOrigin, float r, float rf, Color background, Matrix matrix)
    {
        SetBackground(background);
        Star5Base(pos, percentOrigin, r, rf, matrix, nameof(NoBorderStar5));
    }
    /// <summary>
    /// 绘制一个五角星，根据距离场使用采样色条染色
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r">外接圆半径</param>
    /// <param name="rf">内切圆半径与外接圆半径比值，小于1</param>
    /// <param name="barTexture">采样色条</param>
    /// <param name="time">时间偏移量</param>
    /// <param name="distanceScaler">距离系数</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
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
    /// <summary>
    /// 绘制一个X角星，有边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r">外接圆半径</param>
    /// <param name="N">角数</param>
    /// <param name="k">内切圆半径与外接圆半径比值，小于1</param>
    /// <param name="background">背景色</param>
    /// <param name="border">边界宽度</param>
    /// <param name="borderColor">边界颜色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void HasBorderStarX(Vector2 pos, Vector2 percentOrigin, float r, float N, float k, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        StarXBase(pos, percentOrigin, r, N, k, matrix, nameof(HasBorderStarX));
    }
    /// <summary>
    /// 绘制一个X角星，无边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r">外接圆半径</param>
    /// <param name="N">角数</param>
    /// <param name="k">内切圆半径与外接圆半径比值，小于1</param>
    /// <param name="background">背景色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void NoBorderStarX(Vector2 pos, Vector2 percentOrigin, float r, float N, float k, Color background, Matrix matrix)
    {
        SetBackground(background);
        StarXBase(pos, percentOrigin, r, N, k, matrix, nameof(NoBorderStarX));
    }
    /// <summary>
    /// 绘制一个X角星，根据距离场使用采样色条染色
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r">外接圆半径</param>
    /// <param name="N">角数</param>
    /// <param name="k">内切圆半径与外接圆半径比值，小于1</param>
    /// <param name="barTexture">采样色条</param>
    /// <param name="time">时间偏移量</param>
    /// <param name="distanceScaler">距离系数</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
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
    /// <summary>
    /// 绘制一个饼图，有边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r">半径</param>
    /// <param name="angle">开口角大小，向上开口</param>
    /// <param name="background">背景色</param>
    /// <param name="border">边界宽度</param>
    /// <param name="borderColor">边界颜色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void HasBorderPie(Vector2 pos, Vector2 percentOrigin, float r, float angle, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        PieBase(pos, percentOrigin, r, angle, matrix, nameof(HasBorderPie));
    }
    /// <summary>
    /// 绘制一个饼图，无边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r">半径</param>
    /// <param name="angle">开口角大小，向上开口</param>
    /// <param name="background">背景色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void NoBorderPie(Vector2 pos, Vector2 percentOrigin, float r, float angle, Color background, Matrix matrix)
    {
        SetBackground(background);
        PieBase(pos, percentOrigin, r, angle, matrix, nameof(NoBorderPie));
    }
    /// <summary>
    /// 绘制一个饼图，根据距离场使用采样色条染色
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r">半径</param>
    /// <param name="angle">开口角大小，向上开口</param>
    /// <param name="barTexture">采样色条</param>
    /// <param name="time">时间偏移量</param>
    /// <param name="distanceScaler">距离系数</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
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
    /// <summary>
    /// 绘制一个弓形，有边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r">完整圆半径</param>
    /// <param name="k">切除部分占半径占比，1为半圆/param>
    /// <param name="background">背景色</param>
    /// <param name="border">边界宽度</param>
    /// <param name="borderColor">边界颜色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void HasBorderCutDisk(Vector2 pos, Vector2 percentOrigin, float r, float k, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        CutDiskBase(pos, percentOrigin, r, k, matrix, nameof(HasBorderCutDisk));
    }
    /// <summary>
    /// 绘制一个弓形，无边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r">完整圆半径</param>
    /// <param name="k">切除部分占半径占比，1为半圆/param>
    /// <param name="background">背景色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void NoBorderCutDisk(Vector2 pos, Vector2 percentOrigin, float r, float k, Color background, Matrix matrix)
    {
        SetBackground(background);
        CutDiskBase(pos, percentOrigin, r, k, matrix, nameof(NoBorderCutDisk));
    }
    /// <summary>
    /// 绘制一个弓形，根据距离场使用采样色条染色
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r">完整圆半径</param>
    /// <param name="k">切除部分占半径占比，1为半圆/param>
    /// <param name="barTexture">采样色条</param>
    /// <param name="time">时间偏移量</param>
    /// <param name="distanceScaler">距离系数</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
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
    /// <summary>
    /// 绘制一个弦，有边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r">半径</param>
    /// <param name="angle">开口角大小，向上开口</param>
    /// <param name="width">线宽</param>
    /// <param name="background">背景色</param>
    /// <param name="border">边界宽度</param>
    /// <param name="borderColor">边界颜色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void HasBorderArc(Vector2 pos, Vector2 percentOrigin, float r, float angle, float width, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        ArcBase(pos, percentOrigin, r, angle, width, matrix, nameof(HasBorderArc));
    }
    /// <summary>
    /// 绘制一个弦，无边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r">半径</param>
    /// <param name="angle">开口角大小，向上开口</param>
    /// <param name="width">线宽</param>
    /// <param name="background">背景色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void NoBorderArc(Vector2 pos, Vector2 percentOrigin, float r, float angle, float width, Color background, Matrix matrix)
    {
        SetBackground(background);
        ArcBase(pos, percentOrigin, r, angle, width, matrix, nameof(NoBorderArc));
    }
    /// <summary>
    /// 绘制一个弦，根据距离场使用采样色条染色
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r">半径</param>
    /// <param name="angle">开口角大小，向上开口</param>
    /// <param name="width">线宽</param>
    /// <param name="barTexture">采样色条</param>
    /// <param name="time">时间偏移量</param>
    /// <param name="distanceScaler">距离系数</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
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
    /// <summary>
    /// 绘制一个环，有边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r">半径</param>
    /// <param name="angle">开口角大小，向上开口</param>
    /// <param name="width">线宽</param>
    /// <param name="background">背景色</param>
    /// <param name="border">边界宽度</param>
    /// <param name="borderColor">边界颜色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void HasBorderRing(Vector2 pos, Vector2 percentOrigin, float r, float angle, float width, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        RingBase(pos, percentOrigin, r, angle, width, matrix, nameof(HasBorderRing));
    }
    /// <summary>
    /// 绘制一个环，无边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r">半径</param>
    /// <param name="angle">开口角大小，向上开口</param>
    /// <param name="width">线宽</param>
    /// <param name="background">背景色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void NoBorderRing(Vector2 pos, Vector2 percentOrigin, float r, float angle, float width, Color background, Matrix matrix)
    {
        SetBackground(background);
        RingBase(pos, percentOrigin, r, angle, width, matrix, nameof(NoBorderRing));
    }
    /// <summary>
    /// 绘制一个环，根据距离场使用采样色条染色
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r">半径</param>
    /// <param name="angle">开口角大小，向上开口</param>
    /// <param name="width">线宽</param>
    /// <param name="barTexture">采样色条</param>
    /// <param name="time">时间偏移量</param>
    /// <param name="distanceScaler">距离系数</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
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
    /// <summary>
    /// 绘制一个马蹄铁，有边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r">半径</param>
    /// <param name="angle">扇环部分圆心角大小，向下开口</param>
    /// <param name="width">线宽</param>
    /// <param name="length">延长线长</param>
    /// <param name="background">背景色</param>
    /// <param name="border">边界宽度</param>
    /// <param name="borderColor">边界颜色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void HasBorderHorseshoe(Vector2 pos, Vector2 percentOrigin, float r, float angle, float width, float length, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        HorseshoeBase(pos, percentOrigin, r, angle, width, length, matrix, nameof(HasBorderHorseshoe));
    }
    /// <summary>
    /// 绘制一个马蹄铁，无边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r">半径</param>
    /// <param name="angle">扇环部分圆心角大小，向下开口</param>
    /// <param name="width">线宽</param>
    /// <param name="length">延长线长</param>
    /// <param name="background">背景色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void NoBorderHorseshoe(Vector2 pos, Vector2 percentOrigin, float r, float angle, float width, float length, Color background, Matrix matrix)
    {
        SetBackground(background);
        HorseshoeBase(pos, percentOrigin, r, angle, width, length, matrix, nameof(NoBorderHorseshoe));
    }
    /// <summary>
    /// 绘制一个马蹄铁，根据距离场使用采样色条染色
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r">半径</param>
    /// <param name="angle">扇环部分圆心角大小，向下开口</param>
    /// <param name="width">线宽</param>
    /// <param name="length">延长线长</param>
    /// <param name="barTexture">采样色条</param>
    /// <param name="time">时间偏移量</param>
    /// <param name="distanceScaler">距离系数</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
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
    /// <summary>
    /// 绘制一个鱼鳔，有边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r">圆半径</param>
    /// <param name="distance">圆心距</param>
    /// <param name="background">背景色</param>
    /// <param name="border">边界宽度</param>
    /// <param name="borderColor">边界颜色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void HasBorderVesica(Vector2 pos, Vector2 percentOrigin, float r, float distance, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        VesicaBase(pos, percentOrigin, r, distance, matrix, nameof(HasBorderVesica));
    }
    /// <summary>
    /// 绘制一个鱼鳔，无边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r">圆半径</param>
    /// <param name="distance">圆心距</param>
    /// <param name="background">背景色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void NoBorderVesica(Vector2 pos, Vector2 percentOrigin, float r, float distance, Color background, Matrix matrix)
    {
        SetBackground(background);
        VesicaBase(pos, percentOrigin, r, distance, matrix, nameof(NoBorderVesica));
    }
    /// <summary>
    /// 绘制一个鱼鳔，根据距离场使用采样色条染色
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r">圆半径</param>
    /// <param name="distance">圆心距</param>
    /// <param name="barTexture">采样色条</param>
    /// <param name="time">时间偏移量</param>
    /// <param name="distanceScaler">距离系数</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
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
    /// <summary>
    /// 绘制一个正交鱼鳔，有边框
    /// </summary>
    /// <param name="pos1">第一个圆位置</param>
    /// <param name="pos2">第二个圆位置</param>
    /// <param name="r">圆半径</param>
    /// <param name="background">背景色</param>
    /// <param name="border">边界宽度</param>
    /// <param name="borderColor">边界颜色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void HasBorderOrientedVesica(Vector2 pos1, Vector2 pos2, float r, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        OrientedVesicaBase(pos1, pos2, r, matrix, nameof(HasBorderOrientedVesica));
    }
    /// <summary>
    /// 绘制一个正交鱼鳔，无边框
    /// </summary>
    /// <param name="pos1">第一个圆位置</param>
    /// <param name="pos2">第二个圆位置</param>
    /// <param name="r">圆半径</param>
    /// <param name="background">背景色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void NoBorderOrientedVesica(Vector2 pos1, Vector2 pos2, float r, Color background, Matrix matrix)
    {
        SetBackground(background);
        OrientedVesicaBase(pos1, pos2, r, matrix, nameof(NoBorderOrientedVesica));
    }
    /// <summary>
    /// 绘制一个正交鱼鳔，根据距离场使用采样色条染色
    /// </summary>
    /// <param name="pos1">第一个圆位置</param>
    /// <param name="pos2">第二个圆位置</param>
    /// <param name="r">圆半径</param>
    /// <param name="barTexture">采样色条</param>
    /// <param name="time">时间偏移量</param>
    /// <param name="distanceScaler">距离系数</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
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
    /// <summary>
    /// 绘制一个月，有边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r1">底圆半径</param>
    /// <param name="r2">切除圆半径</param>
    /// <param name="x">切除圆的圆心横坐标</param>
    /// <param name="background">背景色</param>
    /// <param name="border">边界宽度</param>
    /// <param name="borderColor">边界颜色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void HasBorderMoon(Vector2 pos, Vector2 percentOrigin, float r1, float r2, float x, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        MoonBase(pos, percentOrigin, r1, r2, x, matrix, nameof(HasBorderMoon));

    }
    /// <summary>
    /// 绘制一个月，无边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r1">底圆半径</param>
    /// <param name="r2">切除圆半径</param>
    /// <param name="x">切除圆的圆心横坐标</param>
    /// <param name="background">背景色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void NoBorderMoon(Vector2 pos, Vector2 percentOrigin, float r1, float r2, float x, Color background, Matrix matrix)
    {
        SetBackground(background);
        MoonBase(pos, percentOrigin, r1, r2, x, matrix, nameof(NoBorderMoon));
    }
    /// <summary>
    /// 绘制一个月，根据距离场使用采样色条染色
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r1">底圆半径</param>
    /// <param name="r2">切除圆半径</param>
    /// <param name="x">切除圆的圆心横坐标</param>
    /// <param name="barTexture">采样色条</param>
    /// <param name="time">时间偏移量</param>
    /// <param name="distanceScaler">距离系数</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
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
    /// <summary>
    /// 绘制一个曲边四芒星，有边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="baseSize">基准大小</param>
    /// <param name="r">曲边圆半径</param>
    /// <param name="background">背景色</param>
    /// <param name="border">边界宽度</param>
    /// <param name="borderColor">边界颜色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void HasBorderCircleCross(Vector2 pos, Vector2 percentOrigin, Vector2 baseSize, float r, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        CircleCrossBase(pos, percentOrigin, baseSize, r, matrix, nameof(HasBorderCircleCross));
    }
    /// <summary>
    /// 绘制一个曲边四芒星，无边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="baseSize">基准大小</param>
    /// <param name="r">曲边圆半径</param>
    /// <param name="background">背景色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void NoBorderCircleCross(Vector2 pos, Vector2 percentOrigin, Vector2 baseSize, float r, Color background, Matrix matrix)
    {
        SetBackground(background);
        CircleCrossBase(pos, percentOrigin, baseSize, r, matrix, nameof(NoBorderCircleCross));
    }
    /// <summary>
    /// 绘制一个曲边四芒星，根据距离场使用采样色条染色
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="baseSize">基准大小</param>
    /// <param name="r">曲边圆半径</param>
    /// <param name="barTexture">采样色条</param>
    /// <param name="time">时间偏移量</param>
    /// <param name="distanceScaler">距离系数</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
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
    /// <summary>
    /// 绘制一个蛋，有边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="ra">上圆半径</param>
    /// <param name="rb">下圆半径</param>
    /// <param name="height">圆心距</param>
    /// <param name="background">背景色</param>
    /// <param name="border">边界宽度</param>
    /// <param name="borderColor">边界颜色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void HasBorderEgg(Vector2 pos, Vector2 percentOrigin, float ra, float rb, float height, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        EggBase(pos, percentOrigin, ra, rb, height, matrix, nameof(HasBorderEgg));
    }
    /// <summary>
    /// 绘制一个蛋，无边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="ra">上圆半径</param>
    /// <param name="rb">下圆半径</param>
    /// <param name="height">圆心距</param>
    /// <param name="background">背景色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void NoBorderEgg(Vector2 pos, Vector2 percentOrigin, float ra, float rb, float height, Color background, Matrix matrix)
    {
        SetBackground(background);
        EggBase(pos, percentOrigin, ra, rb, height, matrix, nameof(NoBorderEgg));
    }
    /// <summary>
    /// 绘制一个蛋，根据距离场使用采样色条染色
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="ra">上圆半径</param>
    /// <param name="rb">下圆半径</param>
    /// <param name="height">圆心距</param>
    /// <param name="barTexture">采样色条</param>
    /// <param name="time">时间偏移量</param>
    /// <param name="distanceScaler">距离系数</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
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
    /// <summary>
    /// 绘制一个心，有边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="sizeX">宽度</param>
    /// <param name="background">背景色</param>
    /// <param name="border">边界宽度</param>
    /// <param name="borderColor">边界颜色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void HasBorderHeart(Vector2 pos, Vector2 percentOrigin, float sizeX, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        HeartBase(pos, percentOrigin, sizeX, matrix, nameof(HasBorderHeart));
    }
    /// <summary>
    /// 绘制一个心，无边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="sizeX">宽度</param>
    /// <param name="background">背景色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void NoBorderHeart(Vector2 pos, Vector2 percentOrigin, float sizeX, Color background, Matrix matrix)
    {
        SetBackground(background);
        HeartBase(pos, percentOrigin, sizeX, matrix, nameof(NoBorderHeart));
    }
    /// <summary>
    /// 绘制一个心，根据距离场使用采样色条染色
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="sizeX">宽度</param>
    /// <param name="barTexture">采样色条</param>
    /// <param name="time">时间偏移量</param>
    /// <param name="distanceScaler">距离系数</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
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
    /// <summary>
    /// 绘制一个十字，有边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="d">基准大小，在拐角圆不相交时大小为2 * (d - r)，否则取拐角圆交出的星形作大小</param>
    /// <param name="c">十字拐点处圆心横纵坐标</param>
    /// <param name="r">小于零时为圆角半径，大于时为拐角圆半径</param>
    /// <param name="background">背景色</param>
    /// <param name="border">边界宽度</param>
    /// <param name="borderColor">边界颜色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void HasBorderPlus(Vector2 pos, Vector2 percentOrigin, float d, float c, float r, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        PlusBase(pos, percentOrigin, d, c, r, matrix, nameof(HasBorderPlus));
    }
    /// <summary>
    /// 绘制一个十字，无边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="d">基准大小，在拐角圆不相交时大小为2 * (d - r)，否则取拐角圆交出的星形作大小</param>
    /// <param name="c">十字拐点处圆心横纵坐标</param>
    /// <param name="r">小于零时为圆角半径，大于时为拐角圆半径</param>
    /// <param name="background">背景色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void NoBorderPlus(Vector2 pos, Vector2 percentOrigin, float d, float c, float r, Color background, Matrix matrix)
    {
        SetBackground(background);
        PlusBase(pos, percentOrigin, d, c, r, matrix, nameof(NoBorderPlus));
    }
    /// <summary>
    /// 绘制一个十字，根据距离场使用采样色条染色
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="d">基准大小，在拐角圆不相交时大小为2 * (d - r)，否则取拐角圆交出的星形作大小</param>
    /// <param name="c">十字拐点处圆心横纵坐标</param>
    /// <param name="r">小于零时为圆角半径，大于时为拐角圆半径</param>
    /// <param name="barTexture">采样色条</param>
    /// <param name="time">时间偏移量</param>
    /// <param name="distanceScaler">距离系数</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
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
    /// 绘制一个叉号，有边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="width">线宽</param>
    /// <param name="round">圆角半径</param>
    /// <param name="background">背景色</param>
    /// <param name="border">边界宽度</param>
    /// <param name="borderColor">边界颜色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void HasBorderCross(Vector2 pos, Vector2 percentOrigin, float width, float round, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        CrossBase(pos, percentOrigin, width, round, matrix, nameof(HasBorderCross));
    }
    /// <summary>
    /// 绘制一个叉号，无边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="width">线宽</param>
    /// <param name="round">圆角半径</param>
    /// <param name="background">背景色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void NoBorderCross(Vector2 pos, Vector2 percentOrigin, float width, float round, Color background, Matrix matrix)
    {
        SetBackground(background);
        CrossBase(pos, percentOrigin, width, round, matrix, nameof(NoBorderCross));
    }
    /// <summary>
    /// 绘制一个叉号，根据距离场使用采样色条染色
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="width">线宽</param>
    /// <param name="round">圆角半径</param>
    /// <param name="barTexture">采样色条</param>
    /// <param name="time">时间偏移量</param>
    /// <param name="distanceScaler">距离系数</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
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
    /// <summary>
    /// 绘制一个多边形，有边框
    /// </summary>
    /// <param name="vecs">多边形顶点坐标</param>
    /// <param name="background">背景色</param>
    /// <param name="border">边界宽度</param>
    /// <param name="borderColor">边界颜色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void HasBorderPolygon(IEnumerable<Vector2> vecs, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        PolygonBase(vecs, matrix, nameof(HasBorderPolygon));
    }
    /// <summary>
    /// 绘制一个多边形，无边框
    /// </summary>
    /// <param name="vecs">多边形顶点坐标</param>
    /// <param name="background">背景色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void NoBorderPolygon(IEnumerable<Vector2> vecs, Color background, Matrix matrix)
    {
        SetBackground(background);
        PolygonBase(vecs, matrix, nameof(NoBorderPolygon));
    }
    /// <summary>
    /// 绘制一个多边形，根据距离场使用采样色条染色
    /// </summary>
    /// <param name="vecs">多边形顶点坐标</param>
    /// <param name="barTexture">采样色条</param>
    /// <param name="time">时间偏移量</param>
    /// <param name="distanceScaler">距离系数</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void BarPolygon(IEnumerable<Vector2> vecs, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        PolygonBase(vecs, matrix, nameof(BarPolygon));
    }
    /// <summary>
    /// 绘制一个多边形，有边框
    /// </summary>
    /// <param name="position">多边形基准点坐标</param>
    /// <param name="offsets">相对与基准点的偏移顶点坐标</param>
    /// <param name="background">背景色</param>
    /// <param name="border">边界宽度</param>
    /// <param name="borderColor">边界颜色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void HasBorderPolygon(Vector2 position, IEnumerable<Vector2> offsets, Color background, float border, Color borderColor, Matrix matrix)
        => HasBorderPolygon(from offset in offsets select offset + position, background, border, borderColor, matrix);
    /// <summary>
    /// 绘制一个多边形，无边框
    /// </summary>
    /// <param name="position">多边形基准点坐标</param>
    /// <param name="offsets">相对与基准点的偏移顶点坐标</param>
    /// <param name="background">背景色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void NoBorderPolygon(Vector2 position, IEnumerable<Vector2> offsets, Color background, Matrix matrix)
        => NoBorderPolygon(from offset in offsets select offset + position, background, matrix);
    /// <summary>
    /// 绘制一个多边形，根据距离场使用采样色条染色
    /// </summary>
    /// <param name="position">多边形基准点坐标</param>
    /// <param name="offsets">相对与基准点的偏移顶点坐标</param>
    /// <param name="barTexture">采样色条</param>
    /// <param name="time">时间偏移量</param>
    /// <param name="distanceScaler">距离系数</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void BarPolygon(Vector2 position, IEnumerable<Vector2> offsets, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
        => BarPolygon(from offset in offsets select offset + position, barTexture, time, distanceScaler, matrix);
    /// <summary>
    /// 绘制一个多边形，有边框
    /// </summary>
    /// <param name="position">多边形基准点坐标</param>
    /// <param name="unit">单位偏移量</param>
    /// <param name="percents">基准点的偏移顶点系数</param>
    /// <param name="background">背景色</param>
    /// <param name="border">边界宽度</param>
    /// <param name="borderColor">边界颜色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void HasBorderPolygon(Vector2 position, Vector2 unit, IEnumerable<Vector2> percents, Color background, float border, Color borderColor, Matrix matrix)
        => HasBorderPolygon(position, from percent in percents select unit * percent, background, border, borderColor, matrix);
    /// <summary>
    /// 绘制一个多边形，无边框
    /// </summary>
    /// <param name="position">多边形基准点坐标</param>
    /// <param name="unit">单位偏移量</param>
    /// <param name="percents">基准点的偏移顶点系数</param>
    /// <param name="background">背景色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void NoBorderPolygon(Vector2 position, Vector2 unit, IEnumerable<Vector2> percents, Color background, Matrix matrix)
        => NoBorderPolygon(position, from percent in percents select unit * percent, background, matrix);
    /// <summary>
    /// 绘制一个多边形，根据距离场使用采样色条染色
    /// </summary>
    /// <param name="position">多边形基准点坐标</param>
    /// <param name="unit">单位偏移量</param>
    /// <param name="percents">基准点的偏移顶点系数</param>
    /// <param name="barTexture">采样色条</param>
    /// <param name="time">时间偏移量</param>
    /// <param name="distanceScaler">距离系数</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
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
    /// <summary>
    /// 绘制一个椭圆，有边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r">半长轴半短轴</param>
    /// <param name="background">背景色</param>
    /// <param name="border">边界宽度</param>
    /// <param name="borderColor">边界颜色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void HasBorderEllipse(Vector2 pos, Vector2 percentOrigin, Vector2 r, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        EllipseBase(pos, percentOrigin, r, matrix, nameof(HasBorderEllipse));
    }
    /// <summary>
    /// 绘制一个椭圆，无边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r">半长轴半短轴</param>
    /// <param name="background">背景色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void NoBorderEllipse(Vector2 pos, Vector2 percentOrigin, Vector2 r, Color background, Matrix matrix)
    {
        SetBackground(background);
        EllipseBase(pos, percentOrigin, r, matrix, nameof(NoBorderEllipse));
    }
    /// <summary>
    /// 绘制一个椭圆，根据距离场使用采样色条染色
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r">半长轴半短轴</param>
    /// <param name="barTexture">采样色条</param>
    /// <param name="time">时间偏移量</param>
    /// <param name="distanceScaler">距离系数</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
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
    /// <summary>
    /// 绘制一个抛物线，有边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="size">大小</param>
    /// <param name="background">背景色</param>
    /// <param name="border">边界宽度</param>
    /// <param name="borderColor">边界颜色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void HasBorderParabola(Vector2 pos, Vector2 percentOrigin, Vector2 size, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        ParabolaBase(pos, percentOrigin, size, matrix, nameof(HasBorderParabola));
    }
    /// <summary>
    /// 绘制一个抛物线，无边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="size">大小</param>
    /// <param name="background">背景色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void NoBorderParabola(Vector2 pos, Vector2 percentOrigin, Vector2 size, Color background, Matrix matrix)
    {
        SetBackground(background);
        ParabolaBase(pos, percentOrigin, size, matrix, nameof(NoBorderParabola));
    }
    /// <summary>
    /// 绘制一个抛物线，根据距离场使用采样色条染色
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="size">大小</param>
    /// <param name="barTexture">采样色条</param>
    /// <param name="time">时间偏移量</param>
    /// <param name="distanceScaler">距离系数</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
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
    /// <summary>
    /// 绘制一个抛物线段，有边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="size">大小</param>
    /// <param name="height">线段所占高度</param>
    /// <param name="background">背景色</param>
    /// <param name="border">边界宽度</param>
    /// <param name="borderColor">边界颜色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void HasBorderParabolaSegment(Vector2 pos, Vector2 percentOrigin, Vector2 size, float height, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        ParabolaSegmentBase(pos, percentOrigin, size, height, matrix, nameof(HasBorderParabolaSegment));
    }
    /// <summary>
    /// 绘制一个抛物线段，无边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="size">大小</param>
    /// <param name="height">线段所占高度</param>
    /// <param name="background">背景色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void NoBorderParabolaSegment(Vector2 pos, Vector2 percentOrigin, Vector2 size, float height, Color background, Matrix matrix)
    {
        SetBackground(background);
        ParabolaSegmentBase(pos, percentOrigin, size, height, matrix, nameof(NoBorderParabolaSegment));
    }
    /// <summary>
    /// 绘制一个抛物线段，根据距离场使用采样色条染色
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="size">大小</param>
    /// <param name="height">线段所占高度</param>
    /// <param name="barTexture">采样色条</param>
    /// <param name="time">时间偏移量</param>
    /// <param name="distanceScaler">距离系数</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
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
    /// <summary>
    /// 绘制一个二阶贝塞尔曲线，有边框
    /// </summary>
    /// <param name="start">起点</param>
    /// <param name="control">控制点</param>
    /// <param name="end">终点</param>
    /// <param name="width">线宽</param>
    /// <param name="background">背景色</param>
    /// <param name="border">边界宽度</param>
    /// <param name="borderColor">边界颜色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void HasBorderQuadraticBezier(Vector2 start, Vector2 control, Vector2 end, float width, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        QuadraticBezierBase(start, control, end, width, matrix, nameof(HasBorderQuadraticBezier));
    }
    /// <summary>
    /// 绘制一个二阶贝塞尔曲线，无边框
    /// </summary>
    /// <param name="start">起点</param>
    /// <param name="control">控制点</param>
    /// <param name="end">终点</param>
    /// <param name="width">线宽</param>
    /// <param name="background">背景色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void NoBorderQuadraticBezier(Vector2 start, Vector2 control, Vector2 end, float width, Color background, Matrix matrix)
    {
        SetBackground(background);
        QuadraticBezierBase(start, control, end, width, matrix, nameof(NoBorderQuadraticBezier));
    }
    /// <summary>
    /// 绘制一个二阶贝塞尔曲线，根据距离场使用采样色条染色
    /// </summary>
    /// <param name="start">起点</param>
    /// <param name="control">控制点</param>
    /// <param name="end">终点</param>
    /// <param name="width">线宽</param>
    /// <param name="barTexture">采样色条</param>
    /// <param name="time">时间偏移量</param>
    /// <param name="distanceScaler">距离系数</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
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
    /// <summary>
    /// 绘制一个圆头十字，有边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="s">基准半边长</param>
    /// <param name="k">控制点坐标参量<br>起点(s,0)，终点(0,s)，控制点横纵坐标s * (0.5 - sqrt(2) * k )</br><br>实际由四段抛物线段拼合，再圆角处理</br></param>
    /// <param name="r">圆角半径</param>
    /// <param name="background">背景色</param>
    /// <param name="border">边界宽度</param>
    /// <param name="borderColor">边界颜色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void HasBorderBlobbyCross(Vector2 pos, Vector2 percentOrigin, float s, float k, float r, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        BlobbyCrossBase(pos, percentOrigin, s, k, r, matrix, nameof(HasBorderBlobbyCross));
    }
    /// <summary>
    /// 绘制一个圆头十字，无边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="s">基准半边长</param>
    /// <param name="k">控制点坐标参量<br>起点(s,0)，终点(0,s)，控制点横纵坐标s * (0.5 - sqrt(2) * k )</br><br>实际由四段抛物线段拼合，再圆角处理</br></param>
    /// <param name="r">圆角半径</param>
    /// <param name="background">背景色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void NoBorderBlobbyCross(Vector2 pos, Vector2 percentOrigin, float s, float k, float r, Color background, Matrix matrix)
    {
        SetBackground(background);
        BlobbyCrossBase(pos, percentOrigin, s, k, r, matrix, nameof(NoBorderBlobbyCross));
    }
    /// <summary>
    /// 绘制一个圆头十字，根据距离场使用采样色条染色
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="s">基准半边长</param>
    /// <param name="k">控制点坐标参量<br>起点(s,0)，终点(0,s)，控制点横纵坐标s * (0.5 - sqrt(2) * k )</br><br>实际由四段抛物线段拼合，再圆角处理</br></param>
    /// <param name="r">圆角半径</param>
    /// <param name="barTexture">采样色条</param>
    /// <param name="time">时间偏移量</param>
    /// <param name="distanceScaler">距离系数</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
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
    /// <summary>
    /// 绘制一个隧道，有边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="wh">半圆半径和矩形高</param>
    /// <param name="background">背景色</param>
    /// <param name="border">边界宽度</param>
    /// <param name="borderColor">边界颜色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void HasBorderTunnel(Vector2 pos, Vector2 percentOrigin, Vector2 wh, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        TunnelBase(pos, percentOrigin, wh, matrix, nameof(HasBorderTunnel));
    }
    /// <summary>
    /// 绘制一个隧道，无边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="wh">半圆半径和矩形高</param>
    /// <param name="background">背景色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void NoBorderTunnel(Vector2 pos, Vector2 percentOrigin, Vector2 wh, Color background, Matrix matrix)
    {
        SetBackground(background);
        TunnelBase(pos, percentOrigin, wh, matrix, nameof(NoBorderTunnel));
    }
    /// <summary>
    /// 绘制一个隧道，根据距离场使用采样色条染色
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="wh">半圆半径和矩形高</param>
    /// <param name="barTexture">采样色条</param>
    /// <param name="time">时间偏移量</param>
    /// <param name="distanceScaler">距离系数</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
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
    /// <summary>
    /// 绘制一个楼梯，有边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="unit">单级步长</param>
    /// <param name="count">阶数</param>
    /// <param name="background">背景色</param>
    /// <param name="border">边界宽度</param>
    /// <param name="borderColor">边界颜色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void HasBorderStairs(Vector2 pos, Vector2 percentOrigin, Vector2 unit, float count, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        StairsBase(pos, percentOrigin, unit, count, matrix, nameof(HasBorderStairs));
    }
    /// <summary>
    /// 绘制一个楼梯，无边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="unit">单级步长</param>
    /// <param name="count">阶数</param>
    /// <param name="background">背景色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void NoBorderStairs(Vector2 pos, Vector2 percentOrigin, Vector2 unit, float count, Color background, Matrix matrix)
    {
        SetBackground(background);
        StairsBase(pos, percentOrigin, unit, count, matrix, nameof(NoBorderStairs));
    }
    /// <summary>
    /// 绘制一个楼梯，根据距离场使用采样色条染色
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="unit">单级步长</param>
    /// <param name="count">阶数</param>
    /// <param name="barTexture">采样色条</param>
    /// <param name="time">时间偏移量</param>
    /// <param name="distanceScaler">距离系数</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
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
    /// <summary>
    /// 绘制一个四次方圆，有边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r">半径</param>
    /// <param name="background">背景色</param>
    /// <param name="border">边界宽度</param>
    /// <param name="borderColor">边界颜色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void HasBorderQuadraticCircle(Vector2 pos, Vector2 percentOrigin, float r, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        QuadraticCircleBase(pos, percentOrigin, r, matrix, nameof(HasBorderQuadraticCircle));
    }
    /// <summary>
    /// 绘制一个四次方圆，无边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r">半径</param>
    /// <param name="background">背景色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void NoBorderQuadraticCircle(Vector2 pos, Vector2 percentOrigin, float r, Color background, Matrix matrix)
    {
        SetBackground(background);
        QuadraticCircleBase(pos, percentOrigin, r, matrix, nameof(NoBorderQuadraticCircle));
    }
    /// <summary>
    /// 绘制一个四次方圆，根据距离场使用采样色条染色
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="r">半径</param>
    /// <param name="barTexture">采样色条</param>
    /// <param name="time">时间偏移量</param>
    /// <param name="distanceScaler">距离系数</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
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
    /// <summary>
    /// 绘制一个双曲线，有边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="size">大小</param>
    /// <param name="background">背景色</param>
    /// <param name="border">边界宽度</param>
    /// <param name="borderColor">边界颜色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void HasBorderHyperbola(Vector2 pos, Vector2 percentOrigin, Vector2 size, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        HyperbolaBase(pos, percentOrigin, size, matrix, nameof(HasBorderHyperbola));
    }
    /// <summary>
    /// 绘制一个双曲线，无边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="size">大小</param>
    /// <param name="background">背景色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void NoBorderHyperbola(Vector2 pos, Vector2 percentOrigin, Vector2 size, Color background, Matrix matrix)
    {
        SetBackground(background);
        HyperbolaBase(pos, percentOrigin, size, matrix, nameof(NoBorderHyperbola));
    }
    /// <summary>
    /// 绘制一个双曲线，根据距离场使用采样色条染色
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="size">大小</param>
    /// <param name="barTexture">采样色条</param>
    /// <param name="time">时间偏移量</param>
    /// <param name="distanceScaler">距离系数</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
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
    /// <summary>
    /// 绘制一个酷酷S，有边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="height">高度</param>
    /// <param name="background">背景色</param>
    /// <param name="border">边界宽度</param>
    /// <param name="borderColor">边界颜色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void HasBorderCoolS(Vector2 pos, Vector2 percentOrigin, float height, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        CoolSBase(pos, percentOrigin, height, matrix, nameof(HasBorderCoolS));
    }
    /// <summary>
    /// 绘制一个酷酷S，无边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="height">高度</param>
    /// <param name="background">背景色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void NoBorderCoolS(Vector2 pos, Vector2 percentOrigin, float height, Color background, Matrix matrix)
    {
        SetBackground(background);
        CoolSBase(pos, percentOrigin, height, matrix, nameof(NoBorderCoolS));
    }
    /// <summary>
    /// 绘制一个酷酷S，根据距离场使用采样色条染色
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="height">高度</param>
    /// <param name="barTexture">采样色条</param>
    /// <param name="time">时间偏移量</param>
    /// <param name="distanceScaler">距离系数</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
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
    /// <summary>
    /// 绘制一个圆波，有边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="width">绘制宽度</param>
    /// <param name="k">圆心角占5/6整圆的比例，从0到1</param>
    /// <param name="r">半径</param>
    /// <param name="lineWidth">线宽</param>
    /// <param name="background">背景色</param>
    /// <param name="border">边界宽度</param>
    /// <param name="borderColor">边界颜色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void HasBorderCircleWave(Vector2 pos, Vector2 percentOrigin, float width, float k, float r, float lineWidth, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        CircleWaveBase(pos, percentOrigin, width, k, r, lineWidth, matrix, nameof(HasBorderCircleWave));
    }
    /// <summary>
    /// 绘制一个圆波，无边框
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="width">绘制宽度</param>
    /// <param name="k">圆心角占5/6整圆的比例，从0到1</param>
    /// <param name="r">半径</param>
    /// <param name="lineWidth">线宽</param>
    /// <param name="background">背景色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void NoBorderCircleWave(Vector2 pos, Vector2 percentOrigin, float width, float k, float r, float lineWidth, Color background, Matrix matrix)
    {
        SetBackground(background);
        CircleWaveBase(pos, percentOrigin, width, k, r, lineWidth, matrix, nameof(NoBorderCircleWave));
    }
    /// <summary>
    /// 绘制一个圆波，根据距离场使用采样色条染色
    /// </summary>
    /// <param name="pos">锚点位置</param>
    /// <param name="percentOrigin">相对中心位置<br>0到1从左上到右下</br></param>
    /// <param name="width">绘制宽度</param>
    /// <param name="k">圆心角占5/6整圆的比例，从0到1</param>
    /// <param name="r">半径</param>
    /// <param name="lineWidth">线宽</param>
    /// <param name="barTexture">采样色条</param>
    /// <param name="time">时间偏移量</param>
    /// <param name="distanceScaler">距离系数</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
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
    /// <summary>
    /// 绘制一个连段二阶贝塞尔曲线，有边框
    /// </summary>
    /// <param name="vecs">点列</param>
    /// <param name="background">背景色</param>
    /// <param name="border">边界宽度</param>
    /// <param name="borderColor">边界颜色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void HasBorderChainedQuadraticBezier(IEnumerable<Vector2> vecs, Color background, float border, Color borderColor, Matrix matrix)
    {
        SetBackground(background);
        SetBorder(border, borderColor);
        ChainedQuadraticBezierBase(vecs, matrix, nameof(HasBorderChainedQuadraticBezier));
    }
    /// <summary>
    /// 绘制一个连段二阶贝塞尔曲线，无边框
    /// </summary>
    /// <param name="vecs">点列</param>
    /// <param name="background">背景色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void NoBorderChainedQuadraticBezier(IEnumerable<Vector2> vecs, Color background, Matrix matrix)
    {
        SetBackground(background);
        ChainedQuadraticBezierBase(vecs, matrix, nameof(NoBorderChainedQuadraticBezier));
    }
    /// <summary>
    /// 绘制一个连段二阶贝塞尔曲线，根据距离场使用采样色条染色
    /// </summary>
    /// <param name="vecs">点列</param>
    /// <param name="barTexture">采样色条</param>
    /// <param name="time">时间偏移量</param>
    /// <param name="distanceScaler">距离系数</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void BarChainedQuadraticBezier(IEnumerable<Vector2> vecs, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
    {
        SetBar(barTexture, time, distanceScaler);
        ChainedQuadraticBezierBase(vecs, matrix, nameof(BarChainedQuadraticBezier));
    }
    /// <summary>
    /// 绘制一个连段二阶贝塞尔曲线，有边框
    /// </summary>
    /// <param name="position">基准点坐标</param>
    /// <param name="offsets">偏移点列</param>
    /// <param name="background">背景色</param>
    /// <param name="border">边界宽度</param>
    /// <param name="borderColor">边界颜色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void HasBorderChainedQuadraticBezier(Vector2 position, IEnumerable<Vector2> offsets, Color background, float border, Color borderColor, Matrix matrix)
        => HasBorderChainedQuadraticBezier(from offset in offsets select offset + position, background, border, borderColor, matrix);
    /// <summary>
    /// 绘制一个连段二阶贝塞尔曲线，无边框
    /// </summary>
    /// <param name="position">基准点坐标</param>
    /// <param name="offsets">偏移点列</param>
    /// <param name="background">背景色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void NoBorderChainedQuadraticBezier(Vector2 position, IEnumerable<Vector2> offsets, Color background, Matrix matrix)
        => NoBorderChainedQuadraticBezier(from offset in offsets select offset + position, background, matrix);
    /// <summary>
    /// 绘制一个连段二阶贝塞尔曲线，根据距离场使用采样色条染色
    /// </summary>
    /// <param name="position">基准点坐标</param>
    /// <param name="offsets">偏移点列</param>
    /// <param name="barTexture">采样色条</param>
    /// <param name="time">时间偏移量</param>
    /// <param name="distanceScaler">距离系数</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void BarChainedQuadraticBezier(Vector2 position, IEnumerable<Vector2> offsets, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
        => BarChainedQuadraticBezier(from offset in offsets select offset + position, barTexture, time, distanceScaler, matrix);
    /// <summary>
    /// 绘制一个连段二阶贝塞尔曲线，有边框
    /// </summary>
    /// <param name="position">基准点坐标</param>
    /// <param name="unit">单位偏移量</param>
    /// <param name="percents">基准点的偏移顶点系数</param>
    /// <param name="background">背景色</param>
    /// <param name="border">边界宽度</param>
    /// <param name="borderColor">边界颜色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void HasBorderChainedQuadraticBezier(Vector2 position, Vector2 unit, IEnumerable<Vector2> percents, Color background, float border, Color borderColor, Matrix matrix)
        => HasBorderChainedQuadraticBezier(position, from percent in percents select unit * percent, background, border, borderColor, matrix);
    /// <summary>
    /// 绘制一个连段二阶贝塞尔曲线，无边框
    /// </summary>
    /// <param name="position">基准点坐标</param>
    /// <param name="unit">单位偏移量</param>
    /// <param name="percents">基准点的偏移顶点系数</param>
    /// <param name="background">背景色</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void NoBorderChainedQuadraticBezier(Vector2 position, Vector2 unit, IEnumerable<Vector2> percents, Color background, Matrix matrix)
        => NoBorderChainedQuadraticBezier(position, from percent in percents select unit * percent, background, matrix);
    /// <summary>
    /// 绘制一个连段二阶贝塞尔曲线，根据距离场使用采样色条染色
    /// </summary>
    /// <param name="position">基准点坐标</param>
    /// <param name="unit">单位偏移量</param>
    /// <param name="percents">基准点的偏移顶点系数</param>
    /// <param name="barTexture">采样色条</param>
    /// <param name="time">时间偏移量</param>
    /// <param name="distanceScaler">距离系数</param>
    /// <param name="matrix">视角矩阵<br>使用<see cref="GetMatrix(bool)"/>来获取UI层(true)或者世界层(false)矩阵</br></param>
    public static void BarChainedQuadraticBezier(Vector2 position, Vector2 unit, IEnumerable<Vector2> percents, Texture2D barTexture, float time, float distanceScaler, Matrix matrix)
        => BarChainedQuadraticBezier(position, from percent in percents select unit * percent, barTexture, time, distanceScaler, matrix);
    #endregion
}
