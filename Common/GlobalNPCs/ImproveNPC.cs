using Terraria.GameContent.ItemDropRules;

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
            if (!info.IsInSimulation)
            {
                return !info.player.HasItem(itemType);
            }
            return false;
        }

        public bool CanShowItemDropInUI() => true;

        public string GetConditionDescription() => GetText("ItemDropRule.WandDrop");
    }
}
