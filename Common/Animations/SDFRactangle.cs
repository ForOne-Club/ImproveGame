namespace ImproveGame.Common.Animations;

public class SDFRectangle
{
    /// <summary>
    /// 强制不绘制阴影，用来实现一些“有趣”的效果
    /// </summary>
    public static bool DontDrawShadow = false;

    private static readonly GraphicsDevice GraphicsDevice = Main.graphics.GraphicsDevice;

    private static Effect _effect;

    public static void Load() => _effect = ModAsset.RoundRectangle.Value;

    public static void Unload() => _effect = null;

    private static void BaseDrawRectangle(Vector2 pos, Vector2 size, Vector4 rounded)
    {
        size /= 2f;

        List<VertexPositionCoordRounded> vertices = new List<VertexPositionCoordRounded>();

        Vector2 coordQ1 = new Vector2(rounded.X) - size;
        Vector2 coordQ2 = new Vector2(rounded.X);
        vertices.AddRectangle(pos, size, coordQ2, coordQ1, rounded.X);

        coordQ1 = new Vector2(rounded.Y) - size;
        coordQ2 = new Vector2(rounded.Y);
        vertices.AddRectangle(pos + new Vector2(size.X, 0f), size, new Vector2(coordQ1.X, coordQ2.Y),
            new Vector2(coordQ2.X, coordQ1.Y), rounded.Y);

        coordQ1 = new Vector2(rounded.Z) - size;
        coordQ2 = new Vector2(rounded.Z);
        vertices.AddRectangle(pos + new Vector2(0f, size.Y), size, new Vector2(coordQ2.X, coordQ1.Y),
            new Vector2(coordQ1.X, coordQ2.Y), rounded.Z);


        coordQ1 = new Vector2(rounded.W) - size;
        coordQ2 = new Vector2(rounded.W);
        vertices.AddRectangle(pos + size, size, coordQ1, coordQ2, rounded.W);

        GraphicsDevice.DrawUserPrimitives(0, vertices.ToArray(), 0, vertices.Count / 3);

        Main.spriteBatch.spriteEffectPass.Apply();
    }

    public static void HasBorder(Vector2 pos, Vector2 size, Vector4 rounded, Color backgroundColor, float border,
        Color borderColor, bool ui = true)
    {
        const float innerShrinkage = 1;
        pos -= new Vector2(innerShrinkage);
        size += new Vector2(innerShrinkage * 2);
        rounded += new Vector4(innerShrinkage);
        EffectParameterCollection parameters = _effect.Parameters;
        parameters["uTransform"].SetValue(GetMatrix(ui));
        parameters["uBackgroundColor"].SetValue(backgroundColor.ToVector4());
        parameters["uBorder"].SetValue(border);
        parameters["uBorderColor"].SetValue(borderColor.ToVector4());
        parameters["uInnerShrinkage"].SetValue(innerShrinkage);
        _effect.CurrentTechnique.Passes[0].Apply();
        BaseDrawRectangle(pos, size, rounded);
    }

    public static void NoBorder(Vector2 pos, Vector2 size, Vector4 rounded, Color backgroundColor, bool ui = true)
    {
        const float innerShrinkage = 1;
        pos -= new Vector2(innerShrinkage);
        size += new Vector2(innerShrinkage * 2);
        rounded += new Vector4(innerShrinkage);
        _effect.Parameters["uTransform"].SetValue(GetMatrix(ui));
        _effect.Parameters["uBackgroundColor"].SetValue(backgroundColor.ToVector4());
        _effect.Parameters["uInnerShrinkage"].SetValue(innerShrinkage);
        _effect.CurrentTechnique.Passes[1].Apply();
        BaseDrawRectangle(pos, size, rounded);
    }

    public static void Shadow(Vector2 pos, Vector2 size, Vector4 rounded, Color backgroundColor, float shadow,
        bool ui = true)
    {
        if (DontDrawShadow)
            return;

        _effect.Parameters["uTransform"].SetValue(GetMatrix(ui));
        _effect.Parameters["uBackgroundColor"].SetValue(backgroundColor.ToVector4());
        _effect.Parameters["uShadowSize"].SetValue(shadow);
        _effect.CurrentTechnique.Passes[2].Apply();
        BaseDrawRectangle(pos, size, rounded + new Vector4(shadow));
    }
}