using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Chat;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace ImproveGame.Interface.UIElements
{
    internal class FishItemSlot : ModItemSlot
    {
        private int marker;

        public FishItemSlot(int marker) : base(0.85f, null, null) {
            this.marker = marker;
        }

        public Action<Item, int> OnFishChange;
        public override void ItemChange() {
            base.ItemChange();
            if (OnFishChange is not null) {
                OnFishChange.Invoke(Item, marker);
            }
        }
    }
}
