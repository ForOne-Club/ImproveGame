using ImproveGame.UIFramework.Graphics2D;
using Terraria.ID;

namespace ImproveGame.Content.NPCs.Dummy;

public class DummyNPC : ModNPC
{
    public static DummyConfig Config = new();
    public static DummyDPS DummyDPS = new();

    public Dictionary<string, object> NPCData = new();

    public override bool CheckDead() => false;
    public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => false;

    public override void SetStaticDefaults()
    {
        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers
        {
            Hide = true
        });
    }

    public override void SetDefaults()
    {
        NPC npc = NPC;

        npc.SetBaseValues(56, 74, Config.LifeMax, false,
            value: 0, damage: Config.Damage, defense: Config.Defense);
        npc.HitSound = SoundID.NPCHit1;
        npc.aiStyle = -1;
        DummyDPS.Parent = this;
    }

    private float HitScale { get => NPC.ai[0]; set => NPC.ai[0] = value; }

    public void Reset()
    {
        NPC npc = NPC;

        npc.immortal = Config.Immortal;
        npc.ShowNameOnHover = Config.ShowNameOnHover;
        npc.damage = Config.Damage;
        npc.lifeMax = Config.LifeMax;
        if (Config.LockHP)
        {
            npc.life = npc.lifeMax;
        }

        npc.defense = Config.Defense;
        npc.noGravity = Config.NoGravity;
        npc.noTileCollide = Config.NoTileCollide;
        npc.knockBackResist = Config.KnockBackResist;

        DummyDPS.Update();
    }

    public override void AI()
    {
        Reset();

        NPC npc = NPC;

        if (npc.HasPlayerTarget)
        {
            Player player = Main.player[npc.target];

            if (player.position.X < npc.position.X)
            {
                npc.spriteDirection = 1;
            }
            else
            {
                npc.spriteDirection = -1;
            }
        }

        UpdateTimer();

        NPC.frame = new Rectangle(0, NPC.frameCounter > 0 ? 76 : 0, 56, 74);
        npc.scale = 1f + HitScale;
        npc.velocity = Vector2.Zero;
    }

    public void ClearBuffs()
    {
        if (!NPC.active) return;

        for (int i = 0; i < NPC.maxBuffs; i++)
        {
            NPC.buffTime[i] = 0;
            NPC.buffType[i] = 0;
        }

        if (Main.netMode == NetmodeID.Server)
            NetMessage.SendData(MessageID.NPCBuffs, -1, -1, null, NPC.whoAmI);
    }

    public void UpdateTimer()
    {
        HitScale -= 0.01f;
        HitScale = Math.Max(0, HitScale);
        NPC.frameCounter -= 1f;
        NPC.frameCounter = Math.Max(0, NPC.frameCounter);
    }

    public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
    {
        DummyDPS.Hurt(damageDone);

        HitScale = 0.1f;
        NPC.frameCounter = 60f;
    }

    public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
    {
        DummyDPS.Hurt(damageDone);

        HitScale = 0.1f;
        NPC.frameCounter = 60f;
    }

    public override bool PreDraw(SpriteBatch sb, Vector2 screenPos, Color drawColor)
    {
        NPC npc = NPC;
        Texture2D texture2D = TextureAssets.Npc[Type].Value;
        Vector2 fSize = npc.frame.Size();

        Vector2 position = npc.position - screenPos;

        if (Config.ShowBox)
        {
            SDFRectangle.HasBorder(position, npc.Size,
                new(0f), Color.Transparent, 2f, Color.White, false);
        }

        sb.Draw(texture2D, position + fSize / 2f,
            npc.frame, drawColor, npc.rotation, fSize / 2f, npc.scale,
            npc.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

        if (Config.ShowDamageData)
        {
            DummyDPS.DrawString(position + new Vector2(fSize.X + 10f, 0f), new Vector2(0f, 0f));
        }

        return false;
    }
}