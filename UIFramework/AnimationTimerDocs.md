# AnimationTimer 使用文档

*由 DeepSeek R1 生成。*

## 概述

AnimationTimer 用于管理UI元素的动画状态转换和进度计算，支持线性/缓动两种动画类型，提供丰富的状态检测和插值功能。

## 枚举定义

### AnimationState

```csharp
public enum AnimationState : byte
{
    Opening,  // 正在打开
    Closing,  // 正在关闭
    Opened,   // 完全打开
    Closed    // 完全关闭
}
```

### AnimationType

```csharp
public enum AnimationType : byte
{
    Linear,  // 线性动画
    Easing   // 缓动动画
}
```

## 类初始化

```csharp
public AnimationTimer(
    float speed = 5f,         // 动画速度（缓动系数）
    float timerMax = 100f,    // 最大时间单位
    AnimationType animationType = AnimationType.Easing)
```

## 核心属性

| 属性     | 类型           | 说明                           |
| -------- | -------------- | ------------------------------ |
| Speed    | float          | 动画速度/缓动系数              |
| Timer    | float          | 当前计时器值（范围0-TimerMax） |
| TimerMax | float          | 最大计时器值（默认100）        |
| Schedule | float          | 标准化进度值（0-1）            |
| Type     | AnimationType  | 当前动画类型                   |
| State    | AnimationState | 当前动画状态                   |

## 状态检测属性

```csharp
public bool AnyOpen => 正在打开或已打开
public bool Opening  => 正在打开
public bool Opened   => 已完全打开
public bool AnyClose => 正在关闭或已关闭
public bool Closing  => 正在关闭
public bool Closed   => 已完全关闭
```

## 主要方法

### 状态控制

```csharp
// 启动打开动画
public virtual void Open()

// 重置计时并启动打开动画，也就是先设置到完全关闭状态，再启动打开动画
public virtual void OpenAndResetTimer()

// 启动关闭动画 
public virtual void Close()

// 重置计时并启动关闭动画，也就是先设置到完全打开状态，再启动关闭动画
public virtual void CloseAndResetTimer()

// 立即完成关闭
public void ImmediateClose()

// 立即完成打开
public void ImmediateOpen()
```

### 更新方法

提供了两个更新方法，使用时应该二选一，而不是两个同时调用。一般来说，在绘制循环中调用 UpdateHighFps 能够适配更多情况

```csharp
// 用于适配 HighFpsSupport 模组的高帧率更新
// 可以实现在高帧率下一帧更新一次计时器，实现流畅的缓动效果
// 应在绘制循环调用，如 UI 里的 DrawSelf 方法
public void UpdateHighFps() 

// 常规更新，应在UI里的Update方法调用
public virtual void Update(float speedFactor = 1f)
```

### 插值方法

```csharp
public Color Lerp(Color start, Color end)
public Vector2 Lerp(Vector2 start, Vector2 end)
public float Lerp(float start, float end)
```

### 运算符重载

```csharp
// 向量/数值与进度值的乘法运算
public static Vector2 operator *(AnimationTimer t, Vector2 v)
public static Vector2 operator *(Vector2 v, AnimationTimer t) 
public static float operator *(float n, AnimationTimer t)
public static float operator *(AnimationTimer t, float n)
```

## 使用示例

### 基本使用

```csharp
// 创建缓动动画计时器
var anim = new AnimationTimer(speed: 8f);

// 触发打开动画
anim.Open();

// 在更新循环中（与 UpdateHighFps() 二选一）
anim.Update();

// 在绘制循环中（与 Update() 二选一）
anim.UpdateHighFps();

// 使用插值
var currentColor = anim.Lerp(Color.Transparent, Color.White);
var scaledVector = anim * new Vector2(100f, 50f);
```

### 状态检测

一般来说，实现基本动画功能不需要额外的逻辑代码。

```csharp
if (anim.Opening) {
    // 处理打开中的逻辑
}

if (anim.AnyClose) {
    // 处理关闭相关状态
}
```

## 注意事项

1. `UpdateHighFps`应在绘制循环调用以获得平滑动画
2. Schedule计算规则：
   - Linear：直接时间比例
   - Easing：使用当前Timer的缓动比例
3. 缓动动画公式：
   - Opening：Timer += (TimerMax - Timer) / Speed
   - Closing：Timer -= Timer / Speed
4. 通过OnOpened/OnClosed事件注册状态回调
5. 使用Immediate方法可跳过动画直接设置状态
