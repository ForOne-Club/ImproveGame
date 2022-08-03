using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace ImproveGame.Common.GlobalNPCs
{
    public class ImproveNPC : GlobalNPC
    {
        public override void SetDefaults(NPC npc)
        {
            npc.value *= MyUtils.Config.NPCCoinDropRate;
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == NPCID.KingSlime)
            {
                int itemType = ModContent.ItemType<Content.Items.SpaceWand>();
                npcLoot.Add(new DropPerPlayerOnThePlayer(itemType, 1, 1, 1, new WandDrop(itemType)));
                itemType = ModContent.ItemType<Content.Items.WallPlace>();
                npcLoot.Add(new DropPerPlayerOnThePlayer(itemType, 1, 1, 1, new WandDrop(itemType)));
            }
        }
    }

    public class WandDrop : IItemDropRuleCondition
    {
        public int itemType;
        public WandDrop(int itemType)
        {
            this.itemType = itemType;
        }

        public bool CanDrop(DropAttemptInfo info)
        {
            if (info.player.HasItem(itemType))
            {
                return false;
            }
            return true;
        }

        public bool CanShowItemDropInUI() => true;

        public string GetConditionDescription() => "玩家拥有当前物品时不会再次掉落";
    }
}
