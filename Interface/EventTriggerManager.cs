using ImproveGame.Common.Animations;

namespace ImproveGame.Interface;

public static class EventTriggerManager
{
    #region Static Fields and Propertices
    public static Vector2 MouseScreen => new(Main.mouseX, Main.mouseY);
    public static bool DisableMouse { get; set; } = false;
    public static EventTrigger CurrentEventTrigger { get; private set; }

    public static List<string> LayersPriority { get; private set; } = [];
    public static Dictionary<string, List<EventTrigger>> LayersDictionary { get; private set; } = [];

    public static int LayerCount => LayersDictionary["Radial Hotbars"].Count;
    #endregion

    /// <summary>
    /// 首次调用时 CurrentEventTrigger 已经被赋值了 所以不用判断是不是 null
    /// </summary>
    public static void MakePriority()
    {
        string layerName = CurrentEventTrigger.LayerName;
        if (LayersDictionary[layerName][0] == CurrentEventTrigger)
        {
            return;
        }

        LayersDictionary[layerName].Remove(CurrentEventTrigger);
        LayersDictionary[layerName].Insert(0, CurrentEventTrigger);
    }

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
                    layers.Insert(index + 1, new LegacyGameInterfaceLayer($"ImproveGame: {trigger.Name}",
                        () => trigger.Draw(), InterfaceScaleType.UI));
                }
            });
        }

        LayersPriority.Sort(((a, b) => -layerIndex[a].CompareTo(layerIndex[b])));
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
