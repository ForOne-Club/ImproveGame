namespace ImproveGame.Core;

public class RenderTargetPool
{
    /// <summary>
    /// 缓存
    /// </summary>
    private readonly Dictionary<(int, int), Stack<RenderTarget2D>> _cached = [];

    /// <summary>
    /// 被占用
    /// </summary>
    private readonly Dictionary<(int, int), Stack<RenderTarget2D>> _occupied = [];

    /*/// <summary>
    /// 借走 RenderTarget2D （记得还）
    /// </summary>
    /// <param name="size">大小</param>
    /// <param name="rt2d">返回值</param>
    /// <returns> （记得还）</returns>
    public bool TryBorrow(Vector2 size, out RenderTarget2D rt2d)
    {
        return TryBorrow((int)Math.Ceiling(size.X), (int)Math.Ceiling(size.Y), out rt2d);
    }

    /// <summary>
    /// 借走 RenderTarget2D （记得还）
    /// </summary>
    /// <param name="width">宽度</param>
    /// <param name="height">高度</param>
    /// <param name="rt2d">返回值</param>
    /// <returns> （记得还）</returns>
    public bool TryBorrow(float width, float height, out RenderTarget2D rt2d)
    {
        return TryBorrow((int)Math.Ceiling(width), (int)Math.Ceiling(height), out rt2d);
    }*/

    /// <summary>
    /// 借走 RenderTarget2D （记得还）
    /// </summary>
    /// <param name="width">宽度</param>
    /// <param name="height">高度</param>
    /// <param name="rt2d">返回值</param>
    /// <returns> （记得还）</returns>
    public bool TryBorrow(int width, int height, out RenderTarget2D rt2d)
    {
        if (width <= 0 || height <= 0)
        {
            rt2d = null;
            return false;
        }

        (int width, int height) size = (width, height);

        if (!_cached.ContainsKey(size))
        {
            _cached[size] = new Stack<RenderTarget2D>();
        }

        if (!_occupied.ContainsKey(size))
        {
            _occupied[size] = new Stack<RenderTarget2D>();
        }

        if (_cached[size].Count > 0)
        {
            _cached[size].TryPop(out rt2d);
            _occupied[size].Push(rt2d);
            return true;
        }
        else
        {
            rt2d = Create(width, height);
            _occupied[size].Push(rt2d);
            return true;
        }
    }

    /// <summary>
    /// 归还 RenderTarget2D
    /// </summary>
    /// <param name="rt2d">要归还的 RenderTarget2D</param>
    /// <returns>归还是否成功 false 意味着并不记录在案</returns>
    public bool TryReturn(RenderTarget2D rt2d)
    {
        (int width, int height) size = (rt2d.Width, rt2d.Height);

        if (!_cached.ContainsKey(size) || !_occupied.ContainsKey(size) || !_occupied[size].Contains(rt2d))
        {
            return false;
        }

        _occupied[size].TryPop(out var _);
        _cached[size].Push(rt2d);

        return true;
    }

    public void Dispose()
    {
        foreach ((_, Stack<RenderTarget2D> value) in _occupied)
        {
            if (value is null)
                continue;
            foreach (var renderTarget2D in value)
            {
                renderTarget2D?.Dispose();
            }
        }

        foreach ((_, Stack<RenderTarget2D> value) in _cached)
        {
            if (value is null)
                continue;
            foreach (var renderTarget2D in value)
            {
                renderTarget2D?.Dispose();
            }
        }
    }

    /// <summary>
    /// 创建 RenderTarget2D
    /// </summary>
    /// <param name="width">宽度</param>
    /// <param name="height">高度</param>
    /// <returns>创建的对象</returns>
    public static RenderTarget2D Create(int width, int height)
    {
        // Usage 设置为 PreserveContents 在每次替换的时候可以不清除内容
        return new RenderTarget2D(Main.graphics.GraphicsDevice, width, height,
                false, Main.graphics.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
    }
}
