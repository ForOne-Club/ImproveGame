namespace ImproveGame.UIFramework.Graphics2D;

public class SDFRectangle
{
    /// <summary>
    /// 强制不绘制阴影，用来实现一些“有趣”的效果
    /// </summary>
    internal static bool DontDrawShadow = false;

    public static EffectPass SpriteEffectPass { get; private set; } = Main.spriteBatch.spriteEffectPass;
    public static GraphicsDevice GraphicsDevice { get; private set; } = Main.graphics.GraphicsDevice;

    private static Effect _effect;
    public static void Load() => _effect = ModAsset.SDFRectangle.Value;
    public static void Unload() => _effect = null;

    public static void ApplyPass(int passIndex) => _effect.CurrentTechnique.Passes[passIndex].Apply();
    public static void ApplyPass(string passName) => _effect.CurrentTechnique.Passes[passName].Apply();

    private static void BaseDrawRectangle(Vector2 pos, Vector2 size, Vector4 rounded)
    {
        size /= 2f;

        List<SDFGraphicsVertexType> vertices = [];

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

        SpriteEffectPass.Apply();
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
        const float root2Over2 = 1.414213562373f / 2f;
        parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);
        ApplyPass(0);
        BaseDrawRectangle(pos, size, rounded);
    }

    public static void NoBorder(Vector2 pos, Vector2 size, Vector4 rounded, Color backgroundColor, bool ui = true)
    {
        const float root2Over2 = 1.414213562373f / 2f;
        const float innerShrinkage = 1;
        pos -= new Vector2(innerShrinkage);
        size += new Vector2(innerShrinkage * 2);
        rounded += new Vector4(innerShrinkage);
        _effect.Parameters["uTransform"].SetValue(GetMatrix(ui));
        _effect.Parameters["uBackgroundColor"].SetValue(backgroundColor.ToVector4());
        _effect.Parameters["uInnerShrinkage"].SetValue(innerShrinkage);
        _effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);
        ApplyPass(1);
        BaseDrawRectangle(pos, size, rounded);
    }

    public static void Shadow(Vector2 pos, Vector2 size, Vector4 rounded, Color backgroundColor, float shadow,
        bool ui = true)
    {
        if (DontDrawShadow)
            return;

        const float root2Over2 = 1.414213562373f / 2f;
        _effect.Parameters["uTransform"].SetValue(GetMatrix(ui));
        _effect.Parameters["uBackgroundColor"].SetValue(backgroundColor.ToVector4());
        _effect.Parameters["uShadowSize"].SetValue(shadow);
        _effect.Parameters["uTransition"].SetValue(new Vector2(-root2Over2, root2Over2) / Main.UIScale);
        ApplyPass(2);
        BaseDrawRectangle(pos, size, rounded + new Vector4(shadow));
    }
}