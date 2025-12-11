using ImproveGame.Core;
using ImproveGame.Helpers;
using Terraria.Map;

namespace ImproveGame.Content.NPCs.Dummy;

[AutoSync]
public class RemoveDummyModule : NetModule
{
    private int whoami;
    private int from;

    public static RemoveDummyModule Get(int whoami, int from)
    {
        var packet = NetModuleLoader.Get<RemoveDummyModule>();
        packet.whoami = whoami;
        packet.from = from;
        return packet;
    }

    public override void Receive()
    {
        if (Main.npc[whoami].ModNPC is DummyNPC dummy)
            dummy.Disappear();
        if (Main.netMode == NetmodeID.Server)
        {
            Get(whoami, from).Send(-1, from);
        }
    }
}

public class SyncDummyModule : NetModule
{
    private Vector2? position;
    private int owner;
    private DummyConfig config;

    private const int MaxNPCs = 200;

    public override void Read(BinaryReader r)
    {
        position = r.ReadBoolean() ? r.ReadVector2() : null;
        owner = r.ReadByte();
        config = new DummyConfig
        {
            LockHP = r.ReadBoolean(),
            LifeMax = r.ReadInt32(),
            Defense = r.ReadInt32(),
            Damage = r.ReadInt32(),
            Scale = r.ReadSingle(),
            ShowBox = r.ReadBoolean(),
            ShowDamageData = r.ReadBoolean(),
            ShowNameOnHover = r.ReadBoolean(),
            Immortal = r.ReadBoolean(),
            NoGravity = r.ReadBoolean(),
            NoTileCollide = r.ReadBoolean(),
            KnockBackResist = r.ReadSingle(),
            customAIType = r.ReadByte(),
            ResetTimer = r.ReadInt32(),
        };
    }

    public override void Send(ModPacket p)
    {
        p.Write(position != null);
        if (position.HasValue)
            p.WriteVector2(position.Value);
        p.Write((byte)owner);

        p.Write(config.LockHP);
        p.Write(config.LifeMax);
        p.Write(config.Defense);
        p.Write(config.Damage);
        p.Write(config.Scale);
        p.Write(config.ShowBox);
        p.Write(config.ShowDamageData);
        p.Write(config.ShowNameOnHover);
        p.Write(config.Immortal);
        p.Write(config.NoGravity);
        p.Write(config.NoTileCollide);
        p.Write(config.KnockBackResist);
        p.Write((byte)config.customAIType);
        p.Write(config.ResetTimer);
        base.Send(p);
    }

    public static SyncDummyModule Get(Vector2? position, int owner, DummyConfig dummyConfig)
    {
        var packet = NetModuleLoader.Get<SyncDummyModule>();
        packet.position = position;
        packet.owner = owner;
        packet.config = dummyConfig;
        return packet;
    }

    public override void Receive()
    {
        if (position == null)
        {
            SyncExistingDummies();
        }
        else
        {
            SpawnNewDummy();
        }

        if (Main.netMode == NetmodeID.Server)
        {
            Get(position, owner, config).Send(-1, owner);
        }
    }

    private void SyncExistingDummies()
    {
        foreach (var npc in Main.npc.Where(n => n?.ModNPC is DummyNPC dummy && dummy.Owner == owner))
        {
            if (npc.ModNPC is DummyNPC dummy)
            {
                dummy.Config = config;
                dummy.SetDefaults();
                npc.life = npc.lifeMax;
            }
        }
    }

    private void SpawnNewDummy()
    {
        NPC npc = NPC.NewNPCDirect(null, 0, 0, ModContent.NPCType<DummyNPC>(), target: owner);
        if (npc.whoAmI >= MaxNPCs)
            return;

        if (npc.ModNPC is DummyNPC dummy)
        {
            dummy.Owner = owner;
            dummy.Config = config;
            dummy.SetDefaults();
            npc.life = npc.lifeMax = config.LifeMax;
            npc.Center = position.Value;
            CreateSpawnDust(npc);
        }
    }

    private static void CreateSpawnDust(NPC npc)
    {
        const int dustCount = 20;
        for (int i = 0; i < dustCount; i++)
        {
            int dust = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Torch, Scale: 2f);
            Main.dust[dust].velocity.Y = -1f;
            Main.dust[dust].noGravity = true;
        }
    }
}

public class DummyMapLayer : ModMapLayer
{
    public override void Draw(ref MapOverlayDrawContext context, ref string text)
    {
        foreach (var n in Main.npc)
        {
            if (n.ModNPC is DummyNPC && n.active)
            {
                var pos = n.Center / 16f;
                context.Draw(ModAsset.DummyNPC_Head.Value, pos, Alignment.Center);
            }
        }
    }
}

public class DummyNPC : ModNPC
{
    private const float HitScaleDecayRate = 0.01f;
    private const float MaxDistance = 4096f;
    private const int HitFrameDuration = 60;

    public static DummyConfig LocalConfig = new();
    public DummyConfig Config = new();
    public DummyDPS DummyDPS = new();
    public int Owner;

    public override bool PreKill()
    {
        return true;
    }

    public override bool CheckDead()
    {
        if (Config.LockHP)
        {
            NPC.life = NPC.lifeMax;
            return false;
        }

        return true;
    }

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
        float scaledWidth = 56 * Config.Scale;
        float scaledHeight = 74 * Config.Scale;
        NPC.SetBaseValues(
            width: (int)scaledWidth,
            height: (int)scaledHeight,
            lifeMax: Config.LifeMax,
            friendly: false,
            value: 0,
            damage: Config.Damage,
            defense: Config.Defense
        );
        NPC.HitSound = SoundID.NPCHit1;
        ResetAI();
        DummyDPS.Parent = this;
    }

    private void ResetAI()
    {
        if (Config.AIStyle == DummyConfig.AIType.SelfDefine)
        {
            if (MyUtils.Config.DummyCustomAIStyleAllowed || Main.netMode == NetmodeID.SinglePlayer)
            {
                AIType = Config.customAIType;
                var mimicNpc = new NPC();
                mimicNpc.SetDefaults(Config.customAIType);
                NPC.aiStyle = mimicNpc.aiStyle;
            }
            else
            {
                Config.AIStyle = DummyConfig.AIType.Default;
                NPC.aiStyle = -1;
                AIType = 0;
            }
        }
        else
        {
            AIType = (int)Config.AIStyle;
            if (Config.AIStyle is DummyConfig.AIType.Default)
            {
                NPC.aiStyle = -1;
            }
            else
            {
                var mimicNpc = new NPC();
                mimicNpc.SetDefaults((int)Config.AIStyle);
                NPC.aiStyle = mimicNpc.aiStyle;
            }
        }
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.Write(HitScale);
        base.SendExtraAI(writer);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        HitScale = reader.ReadSingle();
        base.ReceiveExtraAI(reader);
    }

    private float HitScale { get; set; }

    public void Reset()
    {
        NPC npc = NPC;

        npc.immortal = Config.Immortal;
        npc.ShowNameOnHover = Config.ShowNameOnHover;
        npc.damage = Config.Damage;
        npc.lifeMax = Config.LifeMax;
        npc.scale = Config.Scale;
        npc.width = (int)(56 * npc.scale);
        npc.height = (int)(74 * npc.scale);
        if (Config.LockHP)
        {
            npc.life = npc.lifeMax;
        }

        npc.defense = Config.Defense;
        npc.noGravity = Config.NoGravity;
        npc.noTileCollide = Config.NoTileCollide;
        npc.knockBackResist = Config.KnockBackResist;

        ResetAI();
        DummyDPS.ResetInterval = Config.ResetTimer;
        DummyDPS.Update();
    }

    public void Disappear()
    {
        NPC npc = NPC;
        npc.active = false;

        for (int i = 0; i < 20; i++)
        {
            int dust = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Torch, Scale: 2f);
            Main.dust[dust].velocity.Y = -1f;
            Main.dust[dust].noGravity = true;
        }
    }

    public override void AI()
    {
        if (!IsPlayerInRange())
        {
            RemoveDummyModule.Get(NPC.whoAmI, Main.myPlayer).Send(runLocally: true);
            return;
        }

        Reset();
        UpdateBossInteraction();
        UpdateDirection();
        UpdateHitEffects();
        UpdateFrame();

        if (NPC.aiStyle == -1)
            NPC.velocity = Vector2.Zero;
    }

    /// <summary>
    /// 召唤 NPC 的玩家存在并且在指定范围内
    /// </summary>
    /// <returns>true 在范围内</returns>
    private bool IsPlayerInRange()
    {
        return Main.player[Owner].active &&
               Vector2.Distance(Main.player[Owner].Center, NPC.Center) <= MaxDistance;
    }

    private void UpdateBossInteraction()
    {
        NPC.dontTakeDamage = CurrentFrameProperties.AnyActiveBoss;
        NPC.friendly = CurrentFrameProperties.AnyActiveBoss;
    }

    private void UpdateDirection()
    {
        if (NPC.HasPlayerTarget)
        {
            NPC.spriteDirection = Main.player[NPC.target].position.X < NPC.position.X ? 1 : -1;
        }
    }

    private void UpdateFrame()
    {
        NPC.frame = new Rectangle(0, NPC.frameCounter > 0 ? 76 : 0, 56, 74);
        NPC.scale = Config.Scale + HitScale;
    }

    private void UpdateHitEffects()
    {
        HitScale = Math.Max(0, HitScale - HitScaleDecayRate);
        NPC.frameCounter = Math.Max(0, NPC.frameCounter - 1f);
    }

    /// <summary>
    /// 清除所有 BUFF
    /// </summary>
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

    public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
    {
        DummyDPS.Hurt(damageDone);

        HitScale = 0.1f;
        NPC.frameCounter = HitFrameDuration;
    }

    public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
    {
        DummyDPS.Hurt(damageDone);

        HitScale = 0.1f;
        NPC.frameCounter = HitFrameDuration;
    }

    public override bool PreDraw(SpriteBatch sb, Vector2 screenPos, Color drawColor)
    {
        NPC npc = NPC;
        Texture2D texture2D = TextureAssets.Npc[Type].Value;
        Vector2 fSize = npc.frame.Size();

        Vector2 position = npc.position - screenPos;
        Vector2 center = npc.Center - screenPos;

        if (Config.ShowBox)
        {
            sb.Draw(TextureAssets.MagicPixel.Value, npc.position + npc.Size * new Vector2(.5f, 0) - Main.screenPosition,
                new Rectangle(0, 0, 1, 1), Color.White, 0, new Vector2(.5f), new Vector2(npc.Size.X, 2f), 0, 0);
            sb.Draw(TextureAssets.MagicPixel.Value, npc.position + npc.Size * new Vector2(.5f, 1) - Main.screenPosition,
                new Rectangle(0, 0, 1, 1), Color.White, 0, new Vector2(.5f), new Vector2(npc.Size.X, 2f), 0, 0);
            sb.Draw(TextureAssets.MagicPixel.Value, npc.position + npc.Size * new Vector2(0, .5f) - Main.screenPosition,
                new Rectangle(0, 0, 1, 1), Color.White, 0, new Vector2(.5f), new Vector2(2f, npc.Size.Y), 0, 0);
            sb.Draw(TextureAssets.MagicPixel.Value, npc.position + npc.Size * new Vector2(1, .5f) - Main.screenPosition,
                new Rectangle(0, 0, 1, 1), Color.White, 0, new Vector2(.5f), new Vector2(2f, npc.Size.Y), 0, 0);
        }

        sb.Draw(texture2D, center,
            npc.frame, drawColor, npc.rotation, fSize / 2f, npc.scale,
            npc.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

        if (Config.ShowDamageData)
        {
            DummyDPS.DrawString(position + new Vector2(fSize.X * npc.scale + 10f, 0f), new Vector2(0f, 0f));
        }

        return false;
    }
}