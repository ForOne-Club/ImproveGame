using ImproveGame.Common.Configs;
using ImproveGame.UIFramework.Common;

namespace ImproveGame.Common.RenderTargetContents;

public class RenderTargetContentSystem : ILoadable
{
    internal static List<ARenderTargetContentByRequest> StackNumberRenderTargets = [];
    internal static ItemSlotContent ItemSlotTarget;

    public void Load(Mod mod)
    {
        for (int i = 0; i <= 9; i++)
        {
            StackNumberRenderTargets.Add(new StackNumberContent(i.ToString()));
        }

        ItemSlotTarget = new ItemSlotContent(UIConfigs.Instance.ThemeType);

        On_TimeLogger.NewDrawFrame += orig =>
        {
            orig.Invoke();

            if (ItemSlotTarget.Theme != UIConfigs.Instance.ThemeType)
            {
                ItemSlotTarget.Reset();
                ItemSlotTarget.Theme = UIConfigs.Instance.ThemeType;
            }

            if (ItemSlotTarget is {IsReady: false })
                ItemSlotTarget.PrepareRenderTarget(Main.instance.GraphicsDevice, Main.spriteBatch);

            foreach (var stackNumberRenderTarget in StackNumberRenderTargets.Where(stackNumberRenderTarget =>
                         stackNumberRenderTarget is {IsReady: false }))
            {
                stackNumberRenderTarget.PrepareRenderTarget(Main.instance.GraphicsDevice, Main.spriteBatch);
            }
        };
    }

    public void Unload()
    {
        Main.QueueMainThreadAction(() =>
        {
            ItemSlotTarget?.Reset();

            foreach (var stackNumberRenderTarget in StackNumberRenderTargets)
            {
                stackNumberRenderTarget?.Reset();
            }

            StackNumberRenderTargets.Clear();
        });
    }
}