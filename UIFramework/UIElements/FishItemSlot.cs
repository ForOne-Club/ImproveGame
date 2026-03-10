using ImproveGame.Common.ModPlayers;
using ImproveGame.Content.Tiles;

namespace ImproveGame.UIFramework.UIElements;

internal class FishItemSlot : ModItemSlot
{
    private TEAutofisher Autofisher => AutofishPlayer.LocalPlayer.Autofisher;
    private AnimationTimer _toggleBanTimer = new(3);
    private int marker;

    public FishItemSlot(int marker) : base(0.85f, null, null)
    {
        this.marker = marker;
    }

    public Action<Item, int, bool> OnFishChange;

    public override void LeftMouseDown(UIMouseEvent evt)
    {
        bool lastFavorited = Item.favorited;
        int lastStack = Item.stack;
        int lastType = Item.type;
        int lastPrefix = Item.prefix;

        base.LeftMouseDown(evt);

        bool itemChangeNotCalled = lastStack == Item.stack && lastType == Item.type && lastPrefix == Item.prefix;
        if (itemChangeNotCalled && lastFavorited != Item.favorited && Item.type is not ItemID.None)
        {
            // 使其发包
            ItemChange();
        }
    }

    public override void ItemChange(bool rightClick = false)
    {
        base.ItemChange(rightClick);
        OnFishChange?.Invoke(Item, marker, rightClick);
    }

    public Action<Item, int, int, bool> OnFishRightClickChange;

    public override void RightClickItemChange(int stackChange, bool typeChange)
    {
        base.RightClickItemChange(stackChange, typeChange);
        OnFishRightClickChange?.Invoke(Item, marker, stackChange, typeChange);
    }

    public override void MiddleClick(UIMouseEvent evt)
    {
        base.MiddleClick(evt);
        if (Autofisher is null)
            return;
        Autofisher.ToggleItem(Item);
        SoundEngine.PlaySound(SoundID.MenuTick);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        if (Autofisher is null)
            return;
        bool disabled = Autofisher.ExcludedItems.Any(i => ItemExtensions.IsSameItem(i.Item, Item));
        if (disabled)
        {
            if (!_toggleBanTimer.AnyOpen)
                _toggleBanTimer.OpenAndResetTimer();
        }
        else
        {
            if (!_toggleBanTimer.AnyClose)
                _toggleBanTimer.CloseAndResetTimer();
        }

        _toggleBanTimer.Update();
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        Vector2 pos = GetDimensions().Position();
        Vector2 size = GetDimensions().Size();

        spriteBatch.Draw(ModAsset.DisabledItem.Value, pos + size * 0.7f, null,
            _toggleBanTimer.Lerp(Color.Transparent, Color.White), 0f,
            ModAsset.DisabledItem.Size() / 2f, _toggleBanTimer.Lerp(0.8f, 1f), 0, 0);
    }
}