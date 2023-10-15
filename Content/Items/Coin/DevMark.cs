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

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type,ref int damage, ref float knockback)
    {
        position = Main.MouseWorld;
    }
}