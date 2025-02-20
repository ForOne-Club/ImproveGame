using ImproveGame.Common.ModPlayers;
using ImproveGame.Common.ModSystems;
using ImproveGame.Packets.NetAutofisher;
using Terraria.ID;

namespace ImproveGame.Content.Items.Coin;

public class DevMark : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 0;
    }

    public override void SetDefaults()
    {
        Item.width = 24;
        Item.height = 24;
        Item.rare = ItemRarityID.Quest;
        Item.value = 0;
        Item.maxStack = 9999;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.useTime = Item.useAnimation = 30;
    }

    public override void UseAnimation(Player player)
    {
        if (!NetPasswordSystem.Permitted(player.whoAmI))
            return;

        if (AutofishPlayer.TryGet(player, out AutofishPlayer autofishPlayer) && autofishPlayer.Autofisher is not null)
        {
            var fisher = autofishPlayer.Autofisher;
            fisher.fishingPole = new Item(ItemID.GoldenFishingRod);
            fisher.bait = new Item(ItemID.MasterBait, 9999);
            fisher.accessory = new Item(ItemID.LavaproofTackleBag);
            // 不想传输鱼饵，于是只能三次独立发包，用AggregateModule整合
            var packets = AggregateModule.Get(
                [
                    ItemSyncPacket.Get(fisher.ID, ItemSyncPacket.FishingPole),
                    ItemSyncPacket.Get(fisher.ID, ItemSyncPacket.Bait),
                    ItemSyncPacket.Get(fisher.ID, ItemSyncPacket.Accessory)
                ]
            );
            packets.Send();
        }
    }
}

public class Expelliarmus : ModItem
{
    private class ExpelliarmusProj : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Item_{ItemID.RubyStaff}";

        public override bool IsLoadingEnabled(Mod mod) => true;

        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.hide = true;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            Projectile.Kill();
        }

        public override void OnKill(int timeLeft)
        {
            if (!Main.player.IndexInRange(Projectile.owner))
                return;

            var player = Main.player[Projectile.owner];
            if (player is null || !player.active || player.DeadOrGhost)
                return;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                var npc = Main.npc[i];
                if (npc is null || !npc.active || npc.type is NPCID.None || !npc.Hitbox.Intersects(Projectile.Hitbox))
                    continue;

                npc.NPCLoot_DropItems(player);
                npc.NPCLoot_DropMoney(player);
                npc.NPCLoot_DropHeals(player);

                var pos = npc.Center.ToPoint();
                var rect = new Rectangle(pos.X, pos.Y, 16, 16);
                string text = Language.ActiveCulture.Name is "zh-Hans"
                    ? Main.rand.NextBool(2) ? "除你武器！" : "老东西爆金币了"
                    : "Expelliarmus!";
                CombatText.NewText(rect, Color.Pink, text, true);
            }

            var plrCenter = player.MountedCenter;
            var directionVector = plrCenter.DirectionTo(Projectile.Center);
            var dustPosition = plrCenter + directionVector * 56;
            int triedTimes = 0;
            while (dustPosition.DistanceSQ(Projectile.Center) > 10 * 10 && triedTimes < 800)
            {
                dustPosition += directionVector * 4;
                var d = Dust.NewDustPerfect(dustPosition, DustID.GemRuby, Vector2.Zero);
                d.noGravity = true;
                triedTimes++;
            }
        }
    }

    public override string Texture => $"Terraria/Images/Item_{ItemID.RubyStaff}";

    public override bool IsLoadingEnabled(Mod mod) => true;

    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 0;
        Item.staff[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.CloneDefaults(ItemID.RubyStaff);
        Item.shoot = ModContent.ProjectileType<ExpelliarmusProj>();
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type,
        ref int damage, ref float knockback)
    {
        position = Main.MouseWorld;
    }
}

public class ShimmerTech : ModItem
{
    public override string Texture => $"Terraria/Images/Item_{ItemID.GoblinTech}";

    public override bool IsLoadingEnabled(Mod mod) => true;

    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 0;
    }

    public override void SetDefaults()
    {
        Item.width = 24;
        Item.height = 28;
        Item.rare = ItemRarityID.Pink;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.useAnimation = Item.useTime = 45;
        Item.UseSound = SoundID.Item92;
    }

    public override bool AltFunctionUse(Player player) => true;

    public override bool? UseItem(Player player)
    {
        if (player.altFunctionUse == 0)
        {
            player.usedAegisCrystal = false;
            player.usedArcaneCrystal = false;
            player.usedAegisFruit = false;
            player.usedAmbrosia = false;
            player.usedGummyWorm = false;
            player.usedGalaxyPearl = false;
            NetMessage.SendData(MessageID.SyncPlayer, -1, -1, null, player.whoAmI);
        }
        else
        {
            NPC.peddlersSatchelWasUsed = false;
            NPC.combatBookVolumeTwoWasUsed = false;
            NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, -1, -1, null, player.whoAmI, -17f);
            NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, -1, -1, null, player.whoAmI, -18f);
        }

        return null;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        Player player = Main.LocalPlayer;
        for (int i = 0; i < tooltips.Count; i++)
        {
            TooltipLine tip = tooltips[i];

            if (tip.Text == Language.GetTextValue("ItemName.AegisCrystal"))
            {
                if (player.usedAegisCrystal) tip.OverrideColor = Color.Green;
                else tip.OverrideColor = Color.Red;
            }

            if (tip.Text == Language.GetTextValue("ItemName.ArcaneCrystal"))
            {
                if (player.usedArcaneCrystal) tip.OverrideColor = Color.Green;
                else tip.OverrideColor = Color.Red;
            }

            if (tip.Text == Language.GetTextValue("ItemName.AegisFruit"))
            {
                if (player.usedAegisFruit) tip.OverrideColor = Color.Green;
                else tip.OverrideColor = Color.Red;
            }

            if (tip.Text == Language.GetTextValue("ItemName.Ambrosia"))
            {
                if (player.usedAmbrosia) tip.OverrideColor = Color.Green;
                else tip.OverrideColor = Color.Red;
            }

            if (tip.Text == Language.GetTextValue("ItemName.GummyWorm"))
            {
                if (player.usedGummyWorm) tip.OverrideColor = Color.Green;
                else tip.OverrideColor = Color.Red;
            }

            if (tip.Text == Language.GetTextValue("ItemName.GalaxyPearl"))
            {
                if (player.usedGalaxyPearl) tip.OverrideColor = Color.Green;
                else tip.OverrideColor = Color.Red;
            }

            if (tip.Text == Language.GetTextValue("ItemName.CombatBookVolumeTwo"))
            {
                if (NPC.combatBookVolumeTwoWasUsed) tip.OverrideColor = Color.Green;
                else tip.OverrideColor = Color.Red;
            }

            if (tip.Text == Language.GetTextValue("ItemName.PeddlersSatchel"))
            {
                if (NPC.peddlersSatchelWasUsed) tip.OverrideColor = Color.Green;
                else tip.OverrideColor = Color.Red;
            }
        }
    }
}