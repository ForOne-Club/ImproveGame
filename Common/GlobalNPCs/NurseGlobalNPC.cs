using Terraria.GameContent.Achievements;

namespace ImproveGame.Common.GlobalNPCs
{
    public class NurseGlobalNPC : GlobalNPC
    {
        public override void GetChat(NPC npc, ref string chat)
        {
            if (!Config.QuickNurse || npc.type is not NPCID.Nurse)
                return;

            int health = Main.LocalPlayer.statLifeMax2 - Main.LocalPlayer.statLife;
            int price = health;
            for (int j = 0; j < Player.MaxBuffs; j++)
            {
                int num5 = Main.LocalPlayer.buffType[j];
                if (Main.debuff[num5] && Main.LocalPlayer.buffTime[j] > 60 && (num5 < 0 || !BuffID.Sets.NurseCannotRemoveDebuff[num5]))
                    price += 100;
            }
            if (NPC.downedGolemBoss)
                price *= 200;
            else if (NPC.downedPlantBoss)
                price *= 150;
            else if (NPC.downedMechBossAny)
                price *= 100;
            else if (Main.hardMode)
                price *= 60;
            else if (NPC.downedBoss3 || NPC.downedQueenBee)
                price *= 25;
            else if (NPC.downedBoss2)
                price *= 10;
            else if (NPC.downedBoss1)
                price *= 3;

            if (Main.expertMode)
                price *= 2;

            price = (int)(price * Main.LocalPlayer.currentShoppingSettings.PriceAdjustment);

            int platinum = 0;
            int gold = 0;
            int silver = 0;
            int copper = 0;
            if (price > 0 && price < 1)
                price = 1;

            bool removeDebuffs = true;
            string reason = Language.GetTextValue("tModLoader.DefaultNurseCantHealChat");
            bool canHeal = PlayerLoader.ModifyNurseHeal(Main.LocalPlayer, Main.npc[Main.LocalPlayer.talkNPC], ref health, ref removeDebuffs, ref reason);
            PlayerLoader.ModifyNursePrice(Main.LocalPlayer, Main.npc[Main.LocalPlayer.talkNPC], health, removeDebuffs, ref price);

            if (price > 0 && canHeal)
            {
                if (Main.LocalPlayer.BuyItem(price))
                {
                    var textColor = Color.White;
                    int displayAdjustPrice = price;
                    string displayText = "";
                    if (displayAdjustPrice >= 1000000)
                    {
                        platinum = displayAdjustPrice / 1000000;
                        displayAdjustPrice -= platinum * 1000000;
                    }

                    if (displayAdjustPrice >= 10000)
                    {
                        gold = displayAdjustPrice / 10000;
                        displayAdjustPrice -= gold * 10000;
                    }

                    if (displayAdjustPrice >= 100)
                    {
                        silver = displayAdjustPrice / 100;
                        displayAdjustPrice -= silver * 100;
                    }

                    if (displayAdjustPrice >= 1)
                        copper = displayAdjustPrice;

                    if (platinum > 0)
                        displayText = displayText + platinum + " " + Language.GetTextValue("LegacyInterface.15") + " ";

                    if (gold > 0)
                        displayText = displayText + gold + " " + Language.GetTextValue("LegacyInterface.16") + " ";

                    if (silver > 0)
                        displayText = displayText + silver + " " + Language.GetTextValue("LegacyInterface.17") + " ";

                    if (copper > 0)
                        displayText = displayText + copper + " " + Language.GetTextValue("LegacyInterface.18") + " ";

                    float mouseTextColor = Main.mouseTextColor / 255f;
                    if (platinum > 0)
                        textColor = new Color((byte)(220f * mouseTextColor), (byte)(220f * mouseTextColor), (byte)(198f * mouseTextColor), Main.mouseTextColor);
                    else if (gold > 0)
                        textColor = new Color((byte)(224f * mouseTextColor), (byte)(201f * mouseTextColor), (byte)(92f * mouseTextColor), Main.mouseTextColor);
                    else if (silver > 0)
                        textColor = new Color((byte)(181f * mouseTextColor), (byte)(192f * mouseTextColor), (byte)(193f * mouseTextColor), Main.mouseTextColor);
                    else if (copper > 0)
                        textColor = new Color((byte)(246f * mouseTextColor), (byte)(138f * mouseTextColor), (byte)(96f * mouseTextColor), Main.mouseTextColor);

                    var rect = Main.LocalPlayer.getRect();
                    rect.Y -= 40;
                    rect.Height = 1;
                    CombatText.NewText(rect, textColor, displayText);

                    AchievementsHelper.HandleNurseService(price);
                    SoundEngine.PlaySound(SoundID.Item4);
                    Main.LocalPlayer.HealEffect(health, true);

                    Main.LocalPlayer.statLife += health;

                    if (!removeDebuffs) // no indent for better patching
                        goto SkipDebuffRemoval;

                    for (int l = 0; l < Player.MaxBuffs; l++)
                    {
                        int num24 = Main.LocalPlayer.buffType[l];
                        if (Main.debuff[num24] && Main.LocalPlayer.buffTime[l] > 0 && (num24 < 0 || !BuffID.Sets.NurseCannotRemoveDebuff[num24]))
                        {
                            Main.LocalPlayer.DelBuff(l);
                            l = -1;
                        }
                    }

                    SkipDebuffRemoval:
                    PlayerLoader.PostNurseHeal(Main.LocalPlayer, Main.npc[Main.LocalPlayer.talkNPC], health, removeDebuffs, price);
                }
            }
        }
    }
}
