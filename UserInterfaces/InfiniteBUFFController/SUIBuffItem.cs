#if DEBUG && true

using ImproveGame.Common.ModPlayers;
using SilkyUIFramework;
using SilkyUIFramework.Attributes;
using SilkyUIFramework.Elements;
using SilkyUIFramework.Extensions;
using System.Text;
using Terraria.ModLoader.UI;

namespace ImproveGame.UserInterfaces.InfiniteBUFFController;

/// <summary>
/// 可点击的 Buff 项视图，负责展示图标、悬停高亮与收藏标识。
/// </summary>
public class SUIBuffItem : UIElementGroup
{
    /// <summary>
    /// 当前项绑定的 Buff 类型 ID。
    /// </summary>
    public int BuffType { get; }

    /// <summary>
    /// Buff 主图标；启用状态会影响其显示亮度。
    /// </summary>
    public SUIImage IconImage { get; }

    /// <summary>
    /// 悬停边框层，用于提供鼠标反馈，不参与点击判定。
    /// </summary>
    public SUIImage BorderImage { get; }

    /// <summary>
    /// 收藏星标层，由父级控制显隐。
    /// </summary>
    public SUIImage StarImage { get; }

    /// <summary>
    /// 创建一个 Buff 项视图。
    /// </summary>
    /// <param name="buffType">要绑定的 Buff 类型 ID。</param>
    public SUIBuffItem(int buffType = 0)
    {
        SetSize(36f, 36f);
        BuffType = buffType;

        IconImage = new SUIImage()
        {
            Width = new Dimension(0f, 1f),
            Height = new Dimension(0f, 1f),
            ImageAlign = new Vector2(0.5f),
            // 直接按 Buff 类型索引贴图表，避免额外映射成本。
            Texture2D = TextureAssets.Buff[buffType],
        }.Join(this);

        BorderImage = new SUIImage()
        {
            ZIndex = -1,
            Positioning = Positioning.Absolute,
            Texture2D = ModAsset.Buff_HoverBorder,
            Width = new Dimension(0f, 1f),
            Height = new Dimension(0f, 1f),
        }.Join(this);

        StarImage = new SUIImage()
        {
            ZIndex = 1,
            Positioning = Positioning.Absolute,
            // 光标贴图索引 3 作为星标图案，与本界面其他标识保持一致。
            Texture2D = TextureAssets.Cursors[3],
            Width = new Dimension(0f, 1f),
            Height = new Dimension(0f, 1f),
        }.Join(this);
    }

    /// <summary>
    /// 当前玩家是否已启用该 Buff 的无限效果。
    /// </summary>
    private bool BuffIsEnabled => InfBuffPlayer.CheckInfiniteBuffEnable(BuffType);

    /// <summary>
    /// 每帧刷新显示状态：根据启用状态调整图标亮度，悬停时显示说明与操作提示。
    /// </summary>
    /// <param name="gameTime">当前帧时间信息。</param>
    protected override void UpdateStatus(GameTime gameTime)
    {
        base.UpdateStatus(gameTime);

        IconImage.ImageColor = Color.Lerp(Color.Black, Color.White, BuffIsEnabled ? 1f : 0.4f);
        BorderImage.ImageColor = HoverTimer.Lerp(Color.Transparent, Color.White);

        if (IsMouseHovering)
        {
            string buffName = Lang.GetBuffName(BuffType);
            string buffTooltip = Main.GetBuffTooltip(Main.LocalPlayer, BuffType);

            // 统一拼接 Tooltip，避免多次调用覆盖已有提示内容。
            var sb = new StringBuilder($"{buffName}\n{buffTooltip}\n");

            sb.AppendLine(InfiniteBuffHelper.GetLeftClickString(BuffIsEnabled));

            UICommon.TooltipMouseText(sb.ToString());
        }
    }

    /// <summary>
    /// 左键切换当前 Buff 的无限状态。
    /// </summary>
    /// <param name="evt">鼠标事件参数。</param>
    public override void OnLeftMouseDown(SilkyUIFramework.UIMouseEvent evt)
    {
        base.OnLeftMouseDown(evt);
        InfBuffPlayer.Get(Main.LocalPlayer).ToggleInfiniteBuff(BuffType);
    }

    public override void OnRightMouseDown(SilkyUIFramework.UIMouseEvent evt)
    {
        base.OnRightMouseDown(evt);
    }
}

#endif
