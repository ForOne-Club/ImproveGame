using ImproveGame.Common.Systems;
using Terraria.ID;

namespace ImproveGame.Common.Players
{
    public class NPCMouseTextPlayer : ModPlayer
    {
        public override void PostUpdate()
        {
            if (Main.myPlayer != Player.whoAmI || ModIntegrationsSystem.WMITFLoaded || !Config.ShowModName)
                return;

            var mousePos = Main.MouseWorld;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                var npc = Main.npc[i];
                npc.position += npc.netOffset;

                // 纯纯的原版代码
                Rectangle hoverBoundingBox = new((int)npc.Bottom.X - npc.frame.Width / 2, (int)npc.Bottom.Y - npc.frame.Height, npc.frame.Width, npc.frame.Height);
                if (npc.type >= NPCID.WyvernHead && npc.type <= NPCID.WyvernTail)
                    hoverBoundingBox = new((int)(npc.position.X + npc.width * 0.5 - 32.0), (int)(npc.position.Y + npc.height * 0.5 - 32.0), 64, 64);

                NPCLoader.ModifyHoverBoundingBox(npc, ref hoverBoundingBox);

                if (hoverBoundingBox.Contains(mousePos.ToPoint()))
                {
                    var modNPC = NPCLoader.GetNPC(npc.type);
                    if (modNPC != null && npc.active && !NPCID.Sets.ProjectileNPC[npc.type])
                    {
                        BoxSystem.MouseText = GetTextWith("Tips.FromMod", new { modNPC.Mod.DisplayName });
                        BoxSystem.SecondLine = true;
                        break;
                    }
                }

                npc.position -= npc.netOffset;
            }
        }
    }
}
