using System.Diagnostics.CodeAnalysis;

namespace ImproveGame.UIFramework;

public readonly struct CanvasSize(int width, int height) : IEquatable<CanvasSize>
{
    public readonly int Width = width;
    public readonly int Height = height;

    public readonly override bool Equals([NotNullWhen(true)] object obj)
    {
        return obj is CanvasSize other && Equals(other);
    }

    public override int GetHashCode() => HashCode.Combine(Width, Height);

    public readonly bool Equals(CanvasSize other) => Width == other.Width && Height == other.Height;

    public static bool operator ==(CanvasSize left, CanvasSize right) => left.Equals(right);
    public static bool operator !=(CanvasSize left, CanvasSize right) => !left.Equals(right);

    public static implicit operator CanvasSize(RenderTarget2D renderTarget) => new(renderTarget.Width, renderTarget.Height);
}

/// <summary>
/// 渲染目标对象池，用于管理和复用 RenderTarget2D 实例
/// </summary>
public sealed class RenderTargetPool : IDisposable
{
    /// <summary>
    /// 获取 RenderTargetPool 的单例实例
    /// </summary>
    public static RenderTargetPool Instance { get; } = new();

    // 图形设备引用
    private readonly GraphicsDevice _graphicsDevice = Main.graphics.GraphicsDevice;

    // 可用渲染目标字典，按尺寸分组存储
    private readonly Dictionary<CanvasSize, HashSet<RenderTarget2D>> _available = [];

    // 已占用渲染目标字典，按尺寸分组存储
    private readonly Dictionary<CanvasSize, HashSet<RenderTarget2D>> _occupied = [];

    /// <summary>
    /// 租借指定尺寸的渲染目标
    /// </summary>
    /// <param name="width">渲染目标宽度</param>
    /// <param name="height">渲染目标高度</param>
    /// <returns>可用的 RenderTarget2D 实例</returns>
    public RenderTarget2D Rent(int width, int height)
    {
        var size = new CanvasSize(width, height);

        if (!_available.TryGetValue(size, out var available))
        {
            available = [];
            _available[size] = available;
        }

        if (!_occupied.TryGetValue(size, out var occupied))
        {
            occupied = [];
            _occupied[size] = occupied;
        }

        if (available.Count > 0)
        {
            var target = available.First(); // 取出任意一个
            available.Remove(target);
            occupied.Add(target);
            return target;
        }

        var newTarget = CreateRenderTarget(_graphicsDevice, width, height);
        occupied.Add(newTarget);
        return newTarget;
    }


    /// <summary>
    /// 归还渲染目标到对象池
    /// </summary>
    /// <param name="renderTarget">要归还的 RenderTarget2D 实例</param>
    /// <exception cref="InvalidOperationException">当尝试归还未从此池租借的渲染目标时抛出</exception>
    public void Return(RenderTarget2D renderTarget)
    {
        var size = (CanvasSize)renderTarget;

        if (!_occupied.TryGetValue(size, out var occupied) || !occupied.Remove(renderTarget))
        {
            throw new InvalidOperationException("RenderTarget was not rented from this pool");
        }

        if (!_available.TryGetValue(size, out var available))
        {
            available = [];
            _available[size] = available;
        }

        available.Add(renderTarget);
    }


    /// <summary>
    /// 创建新的渲染目标
    /// </summary>
    /// <param name="graphicsDevice">图形设备</param>
    /// <param name="width">宽度</param>
    /// <param name="height">高度</param>
    /// <returns>新创建的 RenderTarget2D 实例</returns>
    private static RenderTarget2D CreateRenderTarget(GraphicsDevice graphicsDevice, int width, int height)
    {
        return new RenderTarget2D(
            graphicsDevice,
            width,
            height,
            false,
            graphicsDevice.PresentationParameters.BackBufferFormat,
            DepthFormat.None,
            0,
            RenderTargetUsage.PreserveContents);
    }

    /// <summary>
    /// 释放对象池管理的所有资源
    /// </summary>
    /// <exception cref="InvalidOperationException">当还有未归还的渲染目标时抛出</exception>
    public void Dispose()
    {
        // 释放所有已租借的 RenderTarget2D
        foreach (var occupiedSet in _occupied.Values)
        {
            foreach (var renderTarget in occupiedSet)
            {
                renderTarget?.Dispose();
            }
        }

        // 释放所有可用的 RenderTarget2D
        foreach (var availableSet in _available.Values)
        {
            foreach (var renderTarget in availableSet)
            {
                renderTarget?.Dispose();
            }
        }

        // 清空字典
        _occupied.Clear();
        _available.Clear();
    }

}