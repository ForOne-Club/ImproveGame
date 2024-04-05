using ImproveGame.Common.Configs;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.UIStructs;

namespace ImproveGame.UIFramework.ModifyVanillaUI;

// 原版的 GetClippingRectangle 方法有问题，这里修复
// 当然，只对我们的UI，别的UI还是不管了，免得出问题
public class ClippingRectangleFix : ILoadable
{
    public void Load(Mod mod)
    {
        // 如果是服务器或者配置了重置原版UI，就不加载
        // 打开了重设配置的话，对所有UI都会用新的裁切
        if (Main.dedServ || UIConfigs.Instance.ResetNativeUI)
            return;

        // 修改原版 UI 裁切算法
        On_UIElement.GetClippingRectangle += (orig, self, batch) => self is not View
            ? orig.Invoke(self, batch)
            : FixedGet(self, batch);
    }

    public void Unload()
    {
    }

    public static Rectangle FixedGet(UIElement self, SpriteBatch spriteBatch)
    {
        var innerRectangle = new RectangleFloat(self._innerDimensions.X, self._innerDimensions.Y,
            self._innerDimensions.Width, self._innerDimensions.Height);

        Rectangle rectangle = RectangleFloat.Transform(innerRectangle, Main.UIScaleMatrix).CeilingSize().ToRectangle();
        Rectangle scissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;

        int left = Utils.Clamp(rectangle.Left, scissorRectangle.Left, scissorRectangle.Right);
        int top = Utils.Clamp(rectangle.Top, scissorRectangle.Top, scissorRectangle.Bottom);
        int right = Utils.Clamp(rectangle.Right, scissorRectangle.Left, scissorRectangle.Right);
        int bottom = Utils.Clamp(rectangle.Bottom, scissorRectangle.Top, scissorRectangle.Bottom);

        return new Rectangle(left, top, right - left, bottom - top);
    }
}