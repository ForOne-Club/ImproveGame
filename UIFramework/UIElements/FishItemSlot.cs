namespace ImproveGame.UIFramework.UIElements
{
    internal class FishItemSlot : ModItemSlot
    {
        private int marker;

        public FishItemSlot(int marker) : base(0.85f, null, null) {
            this.marker = marker;
        }

        public Action<Item, int, bool> OnFishChange;
        public override void ItemChange(bool rightClick = false) {
            base.ItemChange(rightClick);
            if (OnFishChange is not null) {
                OnFishChange.Invoke(Item, marker, rightClick);
            }
        }

        public Action<Item, int, int, bool> OnFishRightClickChange;
        public override void RightClickItemChange(int stackChange, bool typeChange) {
            base.RightClickItemChange(stackChange, typeChange);
            if (OnFishRightClickChange is not null) {
                OnFishRightClickChange.Invoke(Item, marker, stackChange, typeChange);
            }
        }
    }
}
