namespace ImproveGame.Common.Utils;

public static class NPCHelper
{
    public static void SetBaseValues(this NPC npc, int width, int height, int lifeMax, bool friendly, int value, int damage = 0, int defense = 0, float knockBackResist = 0f)
    {
        npc.width = width;
        npc.height = height;
        npc.lifeMax = lifeMax;
        npc.friendly = friendly;
        npc.value = value;
        npc.damage = damage;
        npc.defense = defense;
        npc.knockBackResist = knockBackResist;
    }
}
