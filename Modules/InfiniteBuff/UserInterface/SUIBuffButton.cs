using SilkyUIFramework;
using SilkyUIFramework.Elements;
using SilkyUIFramework.Extensions;
using System.Text;
using Terraria.ModLoader.UI;

namespace ImproveGame.Modules.InfiniteBuff.UserInterface;

/// <summary>
/// 可点击的 Buff 项视图，负责展示图标、悬停高亮与收藏标识。
/// </summary>
public class SUIBuffButton : UIElementGroup
{
    /// <summary>
    /// 当前项绑定的 Buff 类型 ID。
    /// </summary>
    public required int BuffType
    {
        get; init
        {
            field = value;
            IconImage.Texture2D = TextureAssets.Buff[field];
        }
    }

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
    public SUIImage FavoriteImage { get; }

    /// <summary>
    /// 创建一个 Buff 项视图。
    /// </summary>
    /// <param name="buffType">要绑定的 Buff 类型 ID。</param>
    public SUIBuffButton()
    {
        SetSize(36f, 36f);
        IconImage = new SUIImage()
        {
            Width = new Dimension(0f, 1f),
            Height = new Dimension(0f, 1f),
            ImageAlign = new Vector2(0.5f),
        }.Join(this);

        BorderImage = new SUIImage()
        {
            ZIndex = -1,
            Positioning = Positioning.Absolute,
            Texture2D = ModAsset.Buff_HoverBorder,
            Width = new Dimension(0f, 1f),
            Height = new Dimension(0f, 1f),
        }.Join(this);

        FavoriteImage = new SUIImage()
        {
            ZIndex = 1,
            Positioning = Positioning.Absolute,
            // 光标贴图索引 3 作为星标图案，与本界面其他标识保持一致。
            Texture2D = TextureAssets.Cursors[3],
            Width = new Dimension(0f, 1f),
            Height = new Dimension(0f, 1f),
        }.Join(this);
    }

    private bool Blacklisted
    {
        get
        {
            if (!Main.LocalPlayer.TryGetModPlayer<InfiniteBuffPlayer>(out var infinitePlayer)) return false;
            return infinitePlayer.Blacklist.ContainsByType(BuffType);
        }
    }

    private bool Favorited
    {
        get
        {
            if (!Main.LocalPlayer.TryGetModPlayer<InfiniteBuffPlayer>(out var infinitePlayer)) return false;
            return infinitePlayer.Favorites.ContainsByType(BuffType);
        }
    }

    /// <summary>
    /// 每帧刷新显示状态：根据启用状态调整图标亮度，悬停时显示说明与操作提示。
    /// </summary>
    /// <param name="gameTime">当前帧时间信息。</param>
    protected override void UpdateStatus(GameTime gameTime)
    {
        base.UpdateStatus(gameTime);

        IconImage.ImageColor = Blacklisted ? Color.Lerp(Color.Black, Color.White, 0.4f) : Color.White;
        FavoriteImage.ImageColor = Favorited ? Color.White : Color.Transparent;

        BorderImage.ImageColor = HoverTimer.Lerp(Color.Transparent, Color.White);

        if (!IsMouseHovering) return;

        UICommon.TooltipMouseText(GetTooltipText().ToString());
    }

    /// <summary>
    /// 鼠标悬浮提示文本
    /// </summary>
    private StringBuilder GetTooltipText()
    {
        var name = Lang.GetBuffName(BuffType);
        var tooltip = Main.GetBuffTooltip(Main.LocalPlayer, BuffType);

        return new StringBuilder($"{name}\n{tooltip}\n")
             .AppendLine(InfiniteBuffHelper.GetLeftClickString(Blacklisted))
             .AppendLine(InfiniteBuffHelper.GetRightClickString(Favorited));
    }

    public override void OnLeftMouseDown(SilkyUIFramework.UIMouseEvent evt)
    {
        base.OnLeftMouseDown(evt);

        // 黑名单
        if (!Main.LocalPlayer.TryGetModPlayer<InfiniteBuffPlayer>(out var infinitePlayer)) return;
        infinitePlayer.Blacklist.ToggleByType(BuffType);
    }

    public override void OnRightMouseDown(SilkyUIFramework.UIMouseEvent evt)
    {
        base.OnRightMouseDown(evt);

        // 收藏
        if (!Main.LocalPlayer.TryGetModPlayer<InfiniteBuffPlayer>(out var infinitePlayer)) return;
        infinitePlayer.Favorites.ToggleByType(BuffType);
    }
}