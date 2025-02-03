using ImproveGame.Content.Functions.AutoPiggyBank;
using ImproveGame.Core;
using ImproveGame.UI.OpenBag;
using ImproveGame.UIFramework;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.Graphics2D;
using ImproveGame.UIFramework.SUIElements;
using ReLogic.Graphics;
using System.Diagnostics;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader.IO;
using Terraria.ModLoader.UI;

namespace ImproveGame.Content.NPCs.Dummy;
[AutoSync]
public class RemoveDummyModule : NetModule
{
    int whoami;
    int from;
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
            //Console.WriteLine(whoami);
        }
        //else
        //    Main.NewText(whoami);
    }
}

public class SyncDummyModule : NetModule
{
    Vector2? position;
    int owner;
    DummyConfig config;
    public override void Read(BinaryReader r)
    {
        if (r.ReadBoolean())
            position = r.ReadVector2();
        else position = null;
        owner = r.ReadByte();

        config.LockHP = r.ReadBoolean();
        config.LifeMax = r.ReadInt32();
        config.Defense = r.ReadInt32();
        config.Damage = r.ReadInt32();
        config.ShowBox = r.ReadBoolean();
        config.ShowDamageData = r.ReadBoolean();
        config.ShowNameOnHover = r.ReadBoolean();
        config.Immortal = r.ReadBoolean();
        config.NoGravity = r.ReadBoolean();
        config.NoTileCollide = r.ReadBoolean();
        config.KnockBackResist = r.ReadSingle();
        base.Read(r);
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
        p.Write(config.ShowBox);
        p.Write(config.ShowDamageData);
        p.Write(config.ShowNameOnHover);
        p.Write(config.Immortal);
        p.Write(config.NoGravity);
        p.Write(config.NoTileCollide);
        p.Write(config.KnockBackResist);
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
            foreach (var n in Main.npc)
            {
                if (n.ModNPC is DummyNPC dummy && dummy.Owner == owner)
                {
                    dummy.Config = config;
                    dummy.SetDefaults();
                    n.life = n.lifeMax;
                }
            }
        }
        else
        {
            NPC npc = NPC.NewNPCDirect(null, 0, 0, ModContent.NPCType<DummyNPC>(), target: owner);
            npc.Center = position.Value;
            //var tileCoord = position.Value.ToTileCoordinates();
            //npc.ai[0] = tileCoord.X;
            //npc.ai[1] = tileCoord.Y;
            var dummy = npc.ModNPC as DummyNPC;
            dummy.Owner = owner;
            dummy.Config = config;
            dummy.SetDefaults();
            npc.life = npc.lifeMax = config.LifeMax;
            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Torch, Scale: 2f);
                Main.dust[dust].velocity.Y = -1f;
                Main.dust[dust].noGravity = true;
            }
        }

        if (Main.netMode == NetmodeID.Server)
        {
            Get(position, owner, config).Send(-1, owner);
            //Console.WriteLine(position?.ToString() ?? "Null");

        }
        //else
        //{
        //    Main.NewText(position?.ToString() ?? "Null");
        //}
    }
}
public class DummyNPC : ModNPC
{
    public static DummyConfig LocalConfig = new();
    public DummyConfig Config = new();
    public DummyDPS DummyDPS = new();
    public int Owner;
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
        //npc.aiStyle = (int)Config.AIStyle;
        if (ModContent.TryFind("FargowiltasSouls", "ClippedWingsBuff", out ModBuff buff)) 
        {
            npc.buffImmune[buff.Type] = true;
        }
        DummyDPS.Parent = this;
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
    //private float HitScale { get => NPC.ai[0]; set => NPC.ai[0] = value; }
    private float HitScale { get; set; }

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
    public void Disappear()
    {
        NPC npc = NPC;
        npc.active = false;
        //(npc.ModNPC as DummyNPC).DummyDPS.Reset();

        for (int i = 0; i < 20; i++)
        {
            int dust = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Torch, Scale: 2f);
            Main.dust[dust].velocity.Y = -1f;
            Main.dust[dust].noGravity = true;
        }
    }
    public override void AI()
    {
        Reset();
        if (!Main.player[Owner].active || Vector2.Distance(Main.player[Owner].Center, NPC.Center) > 4096)
            RemoveDummyModule.Get(NPC.whoAmI, Main.myPlayer).Send(runLocally: true);
        NPC npc = NPC;
        npc.dontTakeDamage = CurrentFrameProperties.AnyActiveBoss;
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

        //npc.velocity *= .9f;
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
        //if (Main.netMode == NetmodeID.Server)
        //    Console.WriteLine("服务器端执行受击");
        //else
        //    Main.NewText($"客户端{Main.myPlayer}号执行受击");
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
        /*sb.DrawString(FontAssets.MouseText.Value, $"{npc.whoAmI}\n{Config.LockHP}\n{(npc.lifeMax, Config.LifeMax)}\n{(npc.defense, Config.Defense)}\n" +
            $"{(npc.damage, Config.Damage)}\n{Config.ShowBox}\n{Config.ShowDamageData}\n{(npc.ShowNameOnHover, Config.ShowNameOnHover)}\n{(npc.immortal, Config.Immortal)}\n" +
            $"{(npc.noGravity, Config.NoGravity)}\n{(npc.noTileCollide, Config.NoTileCollide)}\n{(npc.knockBackResist, Config.KnockBackResist)}", npc.Center - Main.screenPosition + new Vector2(0, 80), Color.White);*/
        return false;
    }
}

