namespace ImproveGame.Core;

public class RenderTargetPool
{
    /// <summary>
    /// 可使用池
    /// </summary>
    private readonly Dictionary<(int, int), Stack<RenderTarget2D>> AvailablePool;

    /// <summary>
    /// 被借走池
    /// </summary>
    private readonly Dictionary<(int, int), Stack<RenderTarget2D>> BorrowPool;

    /// <summary>
    /// 构造
    /// </summary>
    public RenderTargetPool()
    {
        AvailablePool = new Dictionary<(int, int), Stack<RenderTarget2D>>();
        BorrowPool = new Dictionary<(int, int), Stack<RenderTarget2D>>();
    }

    /// <summary>
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
    }

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

        if (!AvailablePool.ContainsKey(size))
        {
            AvailablePool[size] = new Stack<RenderTarget2D>();
        }

        if (!BorrowPool.ContainsKey(size))
        {
            BorrowPool[size] = new Stack<RenderTarget2D>();
        }

        if (AvailablePool[size].Count > 0)
        {
            AvailablePool[size].TryPop(out rt2d);
            BorrowPool[size].Push(rt2d);
            return true;
        }
        else
        {
            rt2d = Create(width, height);
            BorrowPool[size].Push(rt2d);
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

        if (!AvailablePool.ContainsKey(size) || !BorrowPool.ContainsKey(size) || !BorrowPool[size].Contains(rt2d))
        {
            return false;
        }

        BorrowPool[size].TryPop(out var _);
        AvailablePool[size].Push(rt2d);

        return true;
    }

    public void Dispose()
    {
        foreach ((_, Stack<RenderTarget2D> value) in BorrowPool)
        {
            if (value is null)
                continue;
            foreach (var renderTarget2D in value) {
                renderTarget2D?.Dispose();
            }
        }

        foreach ((_, Stack<RenderTarget2D> value) in AvailablePool)
        {
            if (value is null)
                continue;
            foreach (var renderTarget2D in value) {
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
