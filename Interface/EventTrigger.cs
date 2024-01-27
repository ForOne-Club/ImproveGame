using ImproveGame.Common.Animations;
using ImproveGame.Interface.BaseViews;
using Terraria.GameInput;

namespace ImproveGame.Interface;

/// <summary>
/// 事件触发器，用于取代原版的 UserInterface <br/>
/// 支持 Update MouseOut MouseOver 左 & 中 & 右键的 MouseDown MouseUp Click <br/>
/// 鼠标侧键方法 & 双击方法都取消了，因为用不到。 <br/>
/// (全凭感觉设计) <br/>
/// (越写越乱，不确定哪里写的有没有问题，使用遇到问题了随时告诉我) <br/>
/// </summary>
public class EventTrigger
{
    private static bool DisableMouse { get; set; }
    private static EventTrigger CurrentEventTrigger { get; set; }

    private static readonly List<string> LayersPriority;
    protected static readonly Dictionary<string, List<EventTrigger>> LayersDictionary;

    public static int LayerCount => LayersDictionary["Radial Hotbars"].Count;

    static EventTrigger()
    {
        LayersPriority = [];
        LayersDictionary = [];

        DisableMouse = false;
    }

    /// <summary>
    /// 首次调用时 CurrentEventTrigger 已经被赋值了<br/>
    /// 所以不用判断是不是 null
    /// </summary>
    private static void MakePriority()
    {
        string layerName = CurrentEventTrigger._layerName;
        if (LayersDictionary[layerName][0] == CurrentEventTrigger)
        {
            return;
        }

        LayersDictionary[layerName].Remove(CurrentEventTrigger);
        LayersDictionary[layerName].Insert(0, CurrentEventTrigger);
    }

    /// <summary>
    /// 事件处理
    /// </summary>
    /// <param name="gameTime"></param>
    public static void UpdateUI(GameTime gameTime)
    {
        foreach (string layerName in LayersPriority.Where(LayersDictionary.ContainsKey))
        {
            List<EventTrigger> triggers = LayersDictionary[layerName];
            for (int i = 0; i < triggers.Count; i++)
            {
                CurrentEventTrigger = triggers[i];
                CurrentEventTrigger.Update(gameTime);
            }
        }

        DisableMouse = false;
    }

    /// <summary>
    /// 嗯 嗯 啊啊啊啊
    /// </summary>
    public static void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        // 防止进入世界时UI闪一下
        if (!UIPlayer.ShouldShowUI)
            return;

        var layerIndex = new Dictionary<string, int>();

        // 插入到绘制层
        foreach (KeyValuePair<string, List<EventTrigger>> keyValuePair in LayersDictionary)
        {
            layers.FindVanilla(keyValuePair.Key, index =>
            {
                layerIndex.Add(keyValuePair.Key, index);
                foreach (EventTrigger trigger in keyValuePair.Value)
                {
                    layers.Insert(index + 1, new LegacyGameInterfaceLayer($"ImproveGame: {trigger._name}",
                        () => trigger.Draw(), InterfaceScaleType.UI));
                }
            });
        }

        LayersPriority.Sort(((a, b) => -layerIndex[a].CompareTo(layerIndex[b])));
    }

    private readonly string _name, _layerName;

    public BaseBody ViewBody { get => BaseBody; protected set => BaseBody = value; }
    public BaseBody BaseBody { get; protected set; }
    private Vector2 _mouse;
    private readonly UIElement[] _last;
    private readonly bool[] _pressed;

    public BaseBody Body => BaseBody;

    /// <summary>
    /// 我在这里提醒一下需要为 CanRunFunc 赋值否则 UI 将永不生效<br/>
    /// CanRunFunc 用于判断此 UI 是否执行 Update() 与 Draw() <br/>
    /// 此类构建的方法不需要再与 UISystem.UpdateUI() 中添加代码 <br/>
    /// </summary>
    /// <param name="layerName">114514</param>
    /// <param name="name">114514</param>
    public EventTrigger(string layerName, string name)
    {
        _last = new UIElement[4];
        _pressed = new bool[3];

        _layerName = layerName;
        _name = name;

        if (!LayersPriority.Contains(layerName))
            LayersPriority.Add(layerName);
        if (!LayersDictionary.ContainsKey(layerName))
            LayersDictionary.Add(layerName, new List<EventTrigger>());

        LayersDictionary[layerName].Add(this);
    }

    public void SetBaseBody(BaseBody baseBody)
    {
        if (BaseBody == baseBody)
        {
            return;
        }

        BaseBody = baseBody;

        if (BaseBody != null)
        {
            BaseBody.Activate();
            BaseBody.Recalculate();
        }
    }

    protected virtual void Update(GameTime gameTime)
    {
        if (!BaseBody?.Enabled ?? true)
        {
            return;
        }

        _mouse = new Vector2(Main.mouseX, Main.mouseY);
        bool mouse = DisableMouse;
        bool[] down = [Main.mouseLeft, Main.mouseRight, Main.mouseMiddle];
        // 鼠标目标元素
        UIElement target = mouse ? null : BaseBody.GetElementAt(_mouse);
        var targetMouseEvent = new UIMouseEvent(target, _mouse);

        try
        {
            // 当前目标元素不是上一个目标元素
            if (_last[^1] != target)
            {
                // 鼠标离开元素
                _last[^1]?.MouseOut(new UIMouseEvent(_last[^1], _mouse));
                _last[^1] = target;
                // 鼠标进入元素
                target?.MouseOver(targetMouseEvent);
            }

            // 禁用鼠标
            if (BaseBody.CanDisableMouse(target))
            {
                DisableMouse = true;
            }

            for (int i = 0; i < 3; i++)
            {
                switch (down[i])
                {
                    case true when !_pressed[i]:
                        _last[i] = target;
                        // 按下事件
                        switch (i)
                        {
                            case 0:
                                target?.LeftMouseDown(new UIMouseEvent(target, _mouse));
                                break;
                            case 1:
                                target?.RightMouseDown(new UIMouseEvent(target, _mouse));
                                break;
                            default:
                                target?.MiddleMouseDown(new UIMouseEvent(target, _mouse));
                                break;
                        }

                        // 视图置于顶层
                        if (target != null && BaseBody.CanPriority(target))
                        {
                            MakePriority();
                        }

                        break;
                    case false when _pressed[i] && _last[i] != null:
                        {
                            if (_last[i].ContainsPoint(_mouse))
                            {
                                // 左键点击事件
                                switch (i)
                                {
                                    case 0:
                                        _last[i]?.LeftClick(new UIMouseEvent(target, _mouse));
                                        break;
                                    case 1:
                                        _last[i]?.RightClick(new UIMouseEvent(target, _mouse));
                                        break;
                                    case 2:
                                        _last[i]?.MiddleClick(new UIMouseEvent(target, _mouse));
                                        break;
                                }
                            }

                            // 鼠标离开事件
                            switch (i)
                            {
                                case 0:
                                    _last[i]?.LeftMouseUp(new UIMouseEvent(target, _mouse));
                                    break;
                                case 1:
                                    _last[i]?.RightMouseUp(new UIMouseEvent(target, _mouse));
                                    break;
                                case 2:
                                    _last[i]?.MiddleMouseUp(new UIMouseEvent(target, _mouse));
                                    break;
                            }

                            _last[i] = null;
                            break;
                        }
                }
            }

            if (PlayerInput.ScrollWheelDeltaForUI != 0)
            {
                target?.ScrollWheel(new UIScrollWheelEvent(target, _mouse,
                    PlayerInput.ScrollWheelDeltaForUI));
            }

            BaseBody.Update(gameTime);
        } finally
        {
            _pressed[0] = down[0] && !mouse;
            _pressed[1] = down[1] && !mouse;
            _pressed[2] = down[2] && !mouse;
        }
    }

    protected virtual bool Draw(bool drawToGame = true)
    {
        if (!BaseBody?.Enabled ?? true)
        {
            return true;
        }

        // 类云母，UI本身不透明，背景是游戏画面高斯模糊处理后的结果
        // drawToGame 表示这是拿来绘制到游戏上的，不是拿来造玻璃或者干啥的
        if (drawToGame && GlassVfxType is GlassType.MicaLike)
        {
            var layers = LayersDictionary["Radial Hotbars"];
            int index = layers.IndexOf(this);
            if (index is -1)
            {
                BaseBody?.Draw(Main.spriteBatch);
                return true;
            }

            Main.spriteBatch.ReBegin(null, Matrix.Identity);
            Main.spriteBatch.Draw(GlassmorphismVfx.GlassCovers[index], Vector2.Zero, Color.White);
            Main.spriteBatch.ReBegin(null, Main.UIScaleMatrix);
            BaseBody?.Draw(Main.spriteBatch);
            return true;
        }
        
        BaseBody?.Draw(Main.spriteBatch);

        return true;
    }

    /// <summary>
    /// 绘制所有的 EventTrigger UI，可用于特殊效果
    /// </summary>
    public static void DrawAll()
    {
        // 不包含原版绘制层级的处理，因为目前都是添加到 Radial Hotbars 层的
        foreach ((_, List<EventTrigger> eventTriggers) in LayersDictionary)
        {
            // index 为 0 的应该处于最顶层，所以最后绘制
            for (var i = eventTriggers.Count - 1; i >= 0; i--)
            {
                var trigger = eventTriggers[i];
                trigger?.Draw();
            }
        }
    }

    public static void MakeGlasses(ref RenderTarget2D[] glasses, RenderTarget2D blurredTarget,
        RenderTarget2D uiTarget)
    {
        var shader = ModAsset.Mask.Value;
        var device = Main.instance.GraphicsDevice;
        var batch = Main.spriteBatch;
        var triggers = LayersDictionary["Radial Hotbars"];

        for (var i = 0; i < triggers.Count; i++)
        {
            var trigger = triggers[i];
            var glass = glasses[i];

            device.SetRenderTarget(uiTarget);
            device.Clear(Color.Transparent);
            batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.UIScaleMatrix);
            SDFRectangle.DontDrawShadow = true;
            trigger?.Draw(false);
            SDFRectangle.DontDrawShadow = false;
            batch.End();

            device.SetRenderTarget(glass);
            device.Clear(Color.Transparent);
            batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            shader.CurrentTechnique.Passes["Mask"].Apply();
            device.Textures[1] = blurredTarget;
            device.Textures[2] = uiTarget;
            // 颜色是 Transparent，所以背景图是完全透明
            batch.Draw(uiTarget, Vector2.Zero, Color.White);
            batch.End();
        }

        // 复原
        device.Textures[0] = null;
        device.Textures[1] = null;
        device.Textures[2] = null;
    }
}