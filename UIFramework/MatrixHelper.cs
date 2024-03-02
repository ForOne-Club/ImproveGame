namespace ImproveGame.UIFramework;

public class MatrixHelper
{
    public static Matrix CreateScreenOrthographicOffCenter()
    {
        float width, height;

        GraphicsDevice device = Main.graphics.GraphicsDevice;
        RenderTargetBinding[] renderTargetBinding = device.GetRenderTargets();

        if (renderTargetBinding.Length > 0 && renderTargetBinding[0].RenderTarget is Texture2D texture2D)
        {
            width = texture2D.Width;
            height = texture2D.Height;
        }
        else
        {
            width = device.PresentationParameters.BackBufferWidth;
            height = device.PresentationParameters.BackBufferHeight;
        }

        return Matrix.CreateOrthographicOffCenter(0, width / Main.UIScale, height / Main.UIScale, 0, 0, 1);
    }
}
