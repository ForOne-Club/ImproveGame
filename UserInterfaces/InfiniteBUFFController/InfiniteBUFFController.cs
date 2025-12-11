#if DEBUG && true

using ImproveGame.Common.ModPlayers;
using ImproveGame.Content.Functions.PortableBuff;
using ImproveGame.Packets;
using SilkyUIFramework;
using SilkyUIFramework.Attributes;
using SilkyUIFramework.Elements;
using SilkyUIFramework.Extensions;
using System.Text;
using Terraria.ModLoader.UI;

namespace ImproveGame.UserInterfaces.InfiniteBUFFController;

/// <summary>
/// 设计中不要动
/// </summary>
[RegisterUI]
public partial class InfiniteBUFFController : BaseBody
{
    public UIElementGroup ScrollContainer { get; private set; }

    protected override void OnInitialize()
    {
        BorderColor = SUIColor.Border;
        BackgroundColor = SUIColor.Background * 0.75f;

        InitializeComponent();

        ScrollContainer = ScrollView.Container;

        Header.ControlTarget = this;
        Title.UseDeathText();

        var searchCancel = Main.Assets.Request<Texture2D>("Images/UI/SearchCancel");

        Close.Texture2D = searchCancel;
        Close.LeftMouseDown += delegate { Enabled = false; };
        Close.OnUpdateStatus += delegate
        {
            Close.ImageColor = Color.White * Close.HoverTimer.Lerp(0.5f, 1f);
        };

        FilterBox.Placeholder = "请输入名称";

        ClearEditText.Texture2D = searchCancel;
        ClearEditText.OnUpdateStatus += delegate
        {
            ClearEditText.ImageColor = Color.White * ClearEditText.HoverTimer.Lerp(0.5f, 1f);
        };

        ClearEditText.LeftMouseDown += delegate { FilterBox.Text = ""; };

        // 订阅事件
        if (Main.LocalPlayer.TryGetModPlayer<BattlerPlayer>(out var battlerPlayer))
        {
            Slider.Value = battlerPlayer.SpawnRateSliderValue;

            battlerPlayer.SpawnRateSliderValueChanged += (sender, value) => Slider.Value = value;

            MinButton.LeftMouseDown += (_, _) => battlerPlayer.SpawnRateSliderValue = 0f;

            A2.LeftMouseDown += (_, _) => battlerPlayer.SpawnRateSliderValue = 0.25f;

            DefaultButton.LeftMouseDown += (_, _) => battlerPlayer.SpawnRateSliderValue = 0.5f;

            A4.LeftMouseDown += (_, _) => battlerPlayer.SpawnRateSliderValue = 0.75f;

            MaxButton.LeftMouseDown += (_, _) => battlerPlayer.SpawnRateSliderValue = 1f;
        }

        // 发送数据
        Slider.Drag += (_, value) =>
        {
            SpawnRateSlider.Get(Main.myPlayer, value).Send(runLocally: true);
        };

        foreach (var item in new UITextView[] { MinButton, DefaultButton, MaxButton, A2, A4 })
        {
            item.OnUpdateStatus += (_) =>
            {
                item.TextBorderColor = item.HoverTimer.Lerp(Color.Black, SUIColor.Highlight);
            };
        }
    }

    protected override void UpdateStatus(GameTime gameTime)
    {
        base.UpdateStatus(gameTime);

        if (Slider.Thumb.IsMouseHovering || Slider.Thumb.LeftMousePressed)
        {
            UICommon.TooltipMouseText($"{Slider.Value:0.00}");
        }

        if (ScrollContainer == null) return;
        ScrollContainer.RemoveAllChildren();

        for (int i = 0; i < HideBuffSystem.BuffTypesShouldHide.Length; i++)
        {
            if (!HideBuffSystem.BuffTypesShouldHide[i]) continue;
            //if (!InfBuffPlayer.CheckInfBuffEnable(i)) continue;
            _pool.GetOrAdd(i, key => new SUIBuffItem(key)).Join(ScrollContainer);
        }
    }

    private readonly Dictionary<int, SUIBuffItem> _pool = [];
}

[XmlElementMapping("BuffItem")]
public class SUIBuffItem : UIElementGroup
{
    public int BuffType { get; }

    public SUIImage IconImage { get; }
    public SUIImage BorderImage { get; }
    public SUIBuffItem(int buffType = 0)
    {
        SetSize(36f, 36f);
        BuffType = buffType;

        IconImage = new SUIImage()
        {
            Width = new Dimension(0f, 1f),
            Height = new Dimension(0f, 1f),
            ImageAlign = new Vector2(0.5f),
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
    }

    private bool BuffIsEnabled => InfBuffPlayer.CheckInfBuffEnable(BuffType);

    protected override void UpdateStatus(GameTime gameTime)
    {
        base.UpdateStatus(gameTime);

        IconImage.ImageColor = Color.Lerp(Color.Black, Color.White, (BuffIsEnabled ? 1f : 0.4f));
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
        InfBuffPlayer.Get(Main.LocalPlayer).ToggleInfBuff(BuffType);
    }
}

#endif