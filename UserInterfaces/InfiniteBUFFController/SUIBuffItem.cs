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
/// Buff 项 UI 组件
/// </summary>
public class SUIBuffItem : UIElementGroup
{
    public int BuffType { get; }

    // BUFF 图标
    public SUIImage IconImage { get; }
    // 选中边框
    public SUIImage BorderImage { get; }
    // 置顶 星标 (收藏)
    public SUIImage StarImage { get; }

    public SUIBuffItem(int buffType = 0)
    {
        SetSize(36f, 36f);
        BuffType = buffType;

        IconImage = new SUIImage()
        {
            Width = new Dimension(0f, 1f),
            Height = new Dimension(0f, 1f),
            ImageAlign = new Vector2(0.5f),
            // Buff
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
            // 光标
            Texture2D = TextureAssets.Cursors[3],
            Width = new Dimension(0f, 1f),
            Height = new Dimension(0f, 1f),
        }.Join(this);
    }

    private bool BuffIsEnabled => InfBuffPlayer.CheckInfBuffEnable(BuffType);

    protected override void UpdateStatus(GameTime gameTime)
    {
        base.UpdateStatus(gameTime);

        IconImage.ImageColor = Color.Lerp(Color.Black, Color.White, BuffIsEnabled ? 1f : 0.4f);
        BorderImage.ImageColor = HoverTimer.Lerp(Color.Transparent, Color.White);

        if (IsMouseHovering)
        {
            string buffName = Lang.GetBuffName(BuffType);
            string buffTooltip = Main.GetBuffTooltip(Main.LocalPlayer, BuffType);

            var sb = new StringBuilder($"{buffName}\n{buffTooltip}\n");

            sb.AppendLine(InfiniteBuffHelper.GetLeftClickString(BuffIsEnabled));

            UICommon.TooltipMouseText(sb.ToString());
        }
    }

    public override void OnLeftMouseDown(SilkyUIFramework.UIMouseEvent evt)
    {
        base.OnLeftMouseDown(evt);
        // 切换无限 BUFF 状态
        InfBuffPlayer.Get(Main.LocalPlayer).ToggleInfBuff(BuffType);
    }
}

#endif