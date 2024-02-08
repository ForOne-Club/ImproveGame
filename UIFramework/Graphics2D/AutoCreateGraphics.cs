namespace ImproveGame.UIFramework.Graphics2D;

/// <summary>
/// 
/// </summary>
public class AutoCreateGraphics
{
    public static EffectPass SpriteEffectPass => Main.spriteBatch.spriteEffectPass;
    public static GraphicsDevice GraphicsDevice => Main.graphics.GraphicsDevice;

    public Effect Effect { get; init; }
    public EffectParameterCollection Parameters { get; init; }

    public void ApplyPass(int passIndex) => Effect.CurrentTechnique.Passes[passIndex].Apply();
    public void ApplyPass(string passName) => Effect.CurrentTechnique.Passes[passName].Apply();
}
