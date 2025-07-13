namespace ImproveGame.UIFramework.Graphics2D;

public static class SDFRectangle
{
    public static bool DontDrawShadow { get; set; }
    public static EffectPass SpriteEffectPass => Main.spriteBatch.spriteEffectPass;
    public static GraphicsDevice GraphicsDevice => Main.graphics.GraphicsDevice;
    private static Effect Effect => ModAsset.SDFRectangle.Value;
    private static void Transform2SDFMatrix(ref Matrix matrix)
    {
        var device = Main.graphics.GraphicsDevice;
        var width = device.Viewport.Width;
        var height = device.Viewport.Height;

        matrix *= Matrix.CreateScale(2f / width, -2f / height, 1f);
        matrix *= Matrix.CreateTranslation(-1 + matrix.M41 * 2f / width, 1 - matrix.M42 * 2f / height, 0f);

        //matrix.M11 *= 2f / width;
        //matrix.M22 *= -2f / height;
        //matrix.M41 = -1 + matrix.M41 * 2f / width;
        //matrix.M42 = 1 - matrix.M42 * 2f / height;
    }

    public static void HasBorder(Vector2 position, Vector2 size,
        Vector4 borderRadius, Color backgroundColor, float border, Color borderColor, Matrix matrix)
    {
        Transform2SDFMatrix(ref matrix);

        float innerShrinkage = 1 / Main.UIScale;

        SetSmoothstepRange();
        SetMatrixWithBgColor(matrix, backgroundColor);

        Effect.Parameters["uBorder"].SetValue(border);
        Effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());

        Effect.CurrentTechnique.Passes["HasBorder"].Apply();
        DrawRectanglePrimitives(innerShrinkage, position, size, borderRadius);
    }

    public static void NoBorder(Vector2 position, Vector2 size,
        Vector4 borderRadius, Color backgroundColor, Matrix matrix)
    {
        Transform2SDFMatrix(ref matrix);

        float innerShrinkage = 1 / Main.UIScale;

        SetSmoothstepRange();
        SetMatrixWithBgColor(matrix, backgroundColor);

        Effect.CurrentTechnique.Passes["NoBorder"].Apply();
        DrawRectanglePrimitives(innerShrinkage, position, size, borderRadius);
    }

    public static void SampleVersion(Texture2D texture2D, Vector2 position, Vector2 size, Vector4 borderRadius, Matrix matrix)
    {
        Transform2SDFMatrix(ref matrix);

        float innerShrinkage = 1 / Main.UIScale;

        SetSmoothstepRange();
        Effect.Parameters["uTransformMatrix"].SetValue(matrix);

        var device = GraphicsDevice;
        var screenSize = new Vector2(device.Viewport.Width, device.Viewport.Height);

        Effect.CurrentTechnique.Passes["SampleVersion"].Apply();
        device.Textures[0] = texture2D;
        DrawRectanglePrimitives(innerShrinkage, position, size, borderRadius, position / screenSize, size / screenSize);
    }

    public static void Shadow(Vector2 position, Vector2 size,
        Vector4 borderRadius, Color backgroundColor, float shadowBlurSize, Matrix matrix)
    {
        if (DontDrawShadow) return;

        Transform2SDFMatrix(ref matrix);

        SetSmoothstepRange();
        SetMatrixWithBgColor(matrix, backgroundColor);
        Effect.Parameters["uShadowBlurSize"].SetValue(shadowBlurSize);

        Effect.CurrentTechnique.Passes["Shadow"].Apply();
        DrawRectanglePrimitives(0f, position, size, borderRadius);
    }

    public static void BarColor(Vector2 position, Vector2 size, Vector4 borderRadius, Texture2D texture, Vector2 direction, float time, Matrix matrix)
    {


        Transform2SDFMatrix(ref matrix);

        float innerShrinkage = 1 / Main.UIScale;

        SetSmoothstepRange();
        SetMatrixWithBgColor(matrix, default);
        Effect.Parameters["uDirection"].SetValue(direction);
        Effect.Parameters["uBarOffset"].SetValue(time);
        Main.graphics.GraphicsDevice.Textures[0] = texture;
        Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.AnisotropicWrap;
        Effect.CurrentTechnique.Passes["BarColor"].Apply();
        DrawRectanglePrimitives(innerShrinkage, position, size, borderRadius, default, size);

    }

    #region SET

    private static void SetMatrixWithBgColor(Matrix matrix, Color backgroundColor)
    {
        Effect.Parameters["uTransformMatrix"].SetValue(matrix);
        Effect.Parameters["uBackgroundColor"].SetValue(backgroundColor.ToVector4());
    }

    private static void SetSmoothstepRange()
    {
        const float root2Over2 = 1.414213562373f / 2f;
        Effect.Parameters["uSmoothstepRange"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);
    }

    #endregion

    private static readonly SDFGraphicsVertexType[] RectangleVertexData = new SDFGraphicsVertexType[16];
    private static readonly short[] IndexData = [0, 1, 2, 2, 1, 3, 4, 5, 6, 6, 5, 7, 8, 9, 10, 10, 9, 11, 12, 13, 14, 14, 13, 15];

    private static void DrawRectanglePrimitives(float innerShrinkage,
        Vector2 position, Vector2 size, Vector4 borderRadius)
    {
        position -= new Vector2(innerShrinkage);
        size += new Vector2(innerShrinkage * 2);
        borderRadius += new Vector4(innerShrinkage);

        size /= 2f;

        var vertexData = RectangleVertexData;

        vertexData.SetPosition(position, size, 0);
        vertexData.SetPosition(new(position.X + size.X, position.Y), size, 4);
        vertexData.SetPosition(new(position.X, position.Y + size.Y), size, 8);
        vertexData.SetPosition(position + size, size, 12);

        vertexData.SetDistanceFromEdge(size, borderRadius.X + innerShrinkage, 0, 1, 2, 3, 0);
        vertexData.SetDistanceFromEdge(size, borderRadius.Y + innerShrinkage, 1, 0, 3, 2, 4);
        vertexData.SetDistanceFromEdge(size, borderRadius.Z + innerShrinkage, 2, 3, 0, 1, 8);
        vertexData.SetDistanceFromEdge(size, borderRadius.W + innerShrinkage, 3, 2, 1, 0, 12);

        vertexData.SetBorderRadius(borderRadius.X, 0);
        vertexData.SetBorderRadius(borderRadius.Y, 4);
        vertexData.SetBorderRadius(borderRadius.Z, 8);
        vertexData.SetBorderRadius(borderRadius.W, 12);

        GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList,
            vertexData, 0, vertexData.Length, IndexData, 0, 8);

        SpriteEffectPass.Apply();
    }

    private static void DrawRectanglePrimitives(float innerShrinkage,
        Vector2 position, Vector2 size, Vector4 borderRadius, Vector2 textureCoordinatesPosition, Vector2 textureCoordinatesSize)
    {
        position -= new Vector2(innerShrinkage);
        size += new Vector2(innerShrinkage * 2);
        borderRadius += new Vector4(innerShrinkage);

        size /= 2f;
        textureCoordinatesSize /= 2f;

        var vertexData = RectangleVertexData;

        vertexData.SetPosition(position, size, 0);
        vertexData.SetPosition(new(position.X + size.X, position.Y), size, 4);
        vertexData.SetPosition(new(position.X, position.Y + size.Y), size, 8);
        vertexData.SetPosition(position + size, size, 12);

        vertexData.SetTextureCoordinates(textureCoordinatesPosition, textureCoordinatesSize, 0);
        vertexData.SetTextureCoordinates(new(textureCoordinatesPosition.X + textureCoordinatesSize.X, textureCoordinatesPosition.Y), textureCoordinatesSize, 4);
        vertexData.SetTextureCoordinates(new(textureCoordinatesPosition.X, textureCoordinatesPosition.Y + textureCoordinatesSize.Y), textureCoordinatesSize, 8);
        vertexData.SetTextureCoordinates(textureCoordinatesPosition + textureCoordinatesSize, textureCoordinatesSize, 12);

        vertexData.SetDistanceFromEdge(size, borderRadius.X + innerShrinkage, 0, 1, 2, 3, 0);
        vertexData.SetDistanceFromEdge(size, borderRadius.Y + innerShrinkage, 1, 0, 3, 2, 4);
        vertexData.SetDistanceFromEdge(size, borderRadius.Z + innerShrinkage, 2, 3, 0, 1, 8);
        vertexData.SetDistanceFromEdge(size, borderRadius.W + innerShrinkage, 3, 2, 1, 0, 12);

        vertexData.SetBorderRadius(borderRadius.X, 0);
        vertexData.SetBorderRadius(borderRadius.Y, 4);
        vertexData.SetBorderRadius(borderRadius.Z, 8);
        vertexData.SetBorderRadius(borderRadius.W, 12);

        GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList,
            vertexData, 0, vertexData.Length, IndexData, 0, 8);

        SpriteEffectPass.Apply();
    }

    public static void SetPosition(this SDFGraphicsVertexType[] vertexData, Vector2 position, Vector2 size, int indexOffset)
    {
        vertexData[indexOffset].Position = position;
        vertexData[indexOffset + 1].Position = new Vector2(position.X + size.X, position.Y);
        vertexData[indexOffset + 2].Position = new Vector2(position.X, position.Y + size.Y);
        vertexData[indexOffset + 3].Position = position + size;
    }

    public static void SetBorderRadius(this SDFGraphicsVertexType[] vertexData, float borderRadius, int indexOffset)
    {
        vertexData[indexOffset].BorderRadius = borderRadius;
        vertexData[indexOffset + 1].BorderRadius = borderRadius;
        vertexData[indexOffset + 2].BorderRadius = borderRadius;
        vertexData[indexOffset + 3].BorderRadius = borderRadius;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="vertexData"></param>
    /// <param name="size"></param>
    /// <param name="borderRadius"></param>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    /// <param name="d"></param>
    /// <param name="indexOffset"></param>
    public static void SetDistanceFromEdge(this SDFGraphicsVertexType[] vertexData, Vector2 size, float borderRadius, int a, int b, int c, int d, int indexOffset)
    {
        vertexData[indexOffset + a].DistanceFromEdge = new Vector2(borderRadius);
        vertexData[indexOffset + b].DistanceFromEdge = new Vector2(borderRadius - size.X, borderRadius);
        vertexData[indexOffset + c].DistanceFromEdge = new Vector2(borderRadius, borderRadius - size.Y);
        vertexData[indexOffset + d].DistanceFromEdge = new Vector2(borderRadius) - size;
    }

    public static void SetTextureCoordinates(this SDFGraphicsVertexType[] vertexData, Vector2 position, Vector2 size, int indexOffset)
    {
        vertexData[indexOffset].TextureCoordinates = position;
        vertexData[indexOffset + 1].TextureCoordinates = new Vector2(position.X + size.X, position.Y);
        vertexData[indexOffset + 2].TextureCoordinates = new Vector2(position.X, position.Y + size.Y);
        vertexData[indexOffset + 3].TextureCoordinates = position + size;
    }
}