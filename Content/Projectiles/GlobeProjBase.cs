using ImproveGame.Content.Items.Globes;
using ImproveGame.Content.Items.Globes.Core;
using ImproveGame.Core;
using Terraria;
using Terraria.ID;

namespace ImproveGame.Content.Projectiles;

public abstract class GlobeProjBase : ModProjectile
{
    public override string Texture => GetModItemDummy().Texture;

    public override LocalizedText DisplayName => GetModItemDummy().DisplayName;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailingMode[Type] = 2;
        ProjectileID.Sets.TrailCacheLength[Type] = 10;
        Globe.GlobeLookup[GetModItemDummy().Type] = Type;
    }

    public override void SetDefaults()
    {
        Projectile.CloneDefaults(ProjectileID.MoonGlobe);
        AIType = ProjectileID.MoonGlobe;
    }

    public override void AI()
    {
        // 弹幕刚出生就进行判断是否会消耗，便于后边整活
        if (Projectile.ai[2] == 0 && Main.netMode != NetmodeID.MultiplayerClient)
        {
            if (RevealOperation(true))
                Projectile.ai[2] = 1;
            else Projectile.ai[2] = -1;
            NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, Projectile.whoAmI);
        }

        // 整活Time！
        if (Projectile.ai[2] < 0)
        {
            Projectile.aiStyle = -1;
            AIType = 0;
            Projectile.ai[2]--;
            if (Projectile.ai[2] >= -180)
            {
                Projectile.velocity.X *= 0.99f;
                Projectile.velocity.Y += 0.13f;
                Projectile.rotation += Projectile.velocity.X / 10f;
            }
            
            if (Projectile.ai[2] == -180)
            {
                Projectile.velocity = Projectile.rotation.ToRotationVector2() * 4;
            }
        }

        if (Projectile.ai[2] < -180)
        {
            Player player = Main.player[Projectile.owner];

            // 穿墙，启动
            Projectile.tileCollide = false;

            // 目标速度
            var targetVel = Vector2.Normalize(player.Center - Projectile.Center) * (7f - Projectile.ai[2] / 60f);
            // 加权平均
            Projectile.velocity = (targetVel + Projectile.velocity * (60 + Projectile.ai[2] / 10f)) / (61f + Projectile.ai[2] / 10f);
            
            Projectile.rotation = Projectile.velocity.ToRotation();

            // 死亡判定与幽默特效
            if (Projectile.owner == Main.myPlayer && Projectile.Center.Distance(player.Center) < 12)
                Projectile.Kill();

            // 幽默特效
            if (Projectile.ai[2] % 12 == 0)
                SoundEngine.PlaySound(SoundID.Item13 with { MaxInstances = 114514}, Projectile.Center);

            if (Main.rand.NextBool(2))
                Dust.NewDustPerfect(Projectile.Center + (Vector2.Normalize(Projectile.velocity) * -16), DustID.Smoke, -Projectile.velocity.RotatedByRandom(0.3f) * 0.5f, 0, default, 2f);

        }
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        // 爆改ai之反弹
        if (Projectile.ai[2] < 0)
        {
            if (Projectile.velocity.X != oldVelocity.X && Math.Abs(oldVelocity.X) > 1f)
            {
                Projectile.velocity.X = oldVelocity.X * -0.85f;
            }
            if (Projectile.velocity.Y != oldVelocity.Y && Math.Abs(oldVelocity.Y) > 1f)
            {
                Projectile.velocity.Y = oldVelocity.Y * -0.85f;
            }

            SoundEngine.PlaySound(Main.rand.Next([SoundID.Item140, SoundID.Item141, SoundID.Item142]), Projectile.Center);
            return false;
        }

        return base.OnTileCollide(oldVelocity);
    }

    public override void OnKill(int timeLeft)
    {
        // 物品只能在服务器/单人掉落
        if (!RevealOperation(false) && Main.netMode is not NetmodeID.MultiplayerClient)
            Main.player[Projectile.owner].QuickSpawnItem(Projectile.GetItemSource_DropAsItem(), GetModItemDummy().Type);

        if (Projectile.ai[2] < 0)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            #region 别展开，里头有答辩
            // 你 好
            for (int i = 0; i < 10; i++)
            {
                int dustIndex = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 2f);
                Main.dust[dustIndex].velocity *= 1.4f;
            }
            for (int i = 0; i < 20; i++)
            {
                int dustIndex = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 3f);
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].velocity *= 5f;
                dustIndex = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 2f);
                Main.dust[dustIndex].velocity *= 3f;
            }
            for (int g = 0; g < 2; g++)
            {
                int goreIndex = Gore.NewGore(Projectile.GetSource_Death(), new Vector2(Projectile.position.X + Projectile.width / 2 - 24f, Projectile.position.Y + Projectile.height / 2 - 24f), default, Main.rand.Next(61, 64), 1f);
                Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X + 1.5f;
                Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y + 1.5f;
                goreIndex = Gore.NewGore(Projectile.GetSource_Death(), new Vector2(Projectile.position.X + Projectile.width / 2 - 24f, Projectile.position.Y + Projectile.height / 2 - 24f), default, Main.rand.Next(61, 64), 1f);
                Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X - 1.5f;
                Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y + 1.5f;
                goreIndex = Gore.NewGore(Projectile.GetSource_Death(), new Vector2(Projectile.position.X + Projectile.width / 2 - 24f, Projectile.position.Y + Projectile.height / 2 - 24f), default, Main.rand.Next(61, 64), 1f);
                Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X + 1.5f;
                Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y - 1.5f;
                goreIndex = Gore.NewGore(Projectile.GetSource_Death(), new Vector2(Projectile.position.X + Projectile.width / 2 - 24f, Projectile.position.Y + Projectile.height / 2 - 24f), default, Main.rand.Next(61, 64), 1f);
                Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X - 1.5f;
                Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y - 1.5f;
            }
            #endregion

            return;
        }

        // 特效弹幕只能在服务器/单人生成
        if (Main.netMode is not NetmodeID.MultiplayerClient)
            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GlobeEffect>(), 0, 0);

        SoundEngine.PlaySound(SoundID.Item107, Projectile.Center);
        for (int i = 0; i < 15; i++)
        {
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Glass, 0f, -2f, 0, default,
                1.2f);
        }
    }

    // 用于视觉特效，没必要同步
    float rand = Main.rand.NextFloat(6.00f, 12.00f);
    Color color = new Color(Main.rand.Next(255), Main.rand.Next(255), Main.rand.Next(255));
    public override bool PreDraw(ref Color lightColor)
    {
        var projTex = TextureAssets.Projectile[Type].Value;
        var tex = TextureAssets.Extra[89].Value;
        var pos = Projectile.Center - Main.screenPosition - (Projectile.rotation + 2.355f).ToRotationVector2() * 8;

        // 顶点拖尾
        List<VertexInfo2> vertices = [];
        for (int i = Projectile.oldPos.Length - 1; i > 0; i--)
        {
            if (Projectile.oldPos[i] != Vector2.Zero)
            {
                Vector2 oldVel;
                if (i > 0)
                    oldVel = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
                else oldVel = Projectile.position - Projectile.oldPos[i];
                Vector2 correct = Vector2.Normalize(oldVel).RotatedBy(1.57f) * (20 - i * 2);
                vertices.Add(new VertexInfo2(Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition + correct, new Vector3(0, 0f, 1 - i / 360f), new Color(255, 255, 255, 0)));
                vertices.Add(new VertexInfo2(Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition - correct, new Vector3(1, 1, 1 - i / 360f), new Color(255, 255, 255, 0)));
            }
        }

        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.GraphicsDevice.RasterizerState, null, Main.GameViewMatrix.TransformationMatrix);

        Main.graphics.GraphicsDevice.Textures[0] = projTex;
        if (vertices.Count >= 3)
        {
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);
        }

        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.GraphicsDevice.RasterizerState, null, Main.GameViewMatrix.TransformationMatrix);

        // 本体绘制
        Main.spriteBatch.Draw(projTex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation + 1.57f, projTex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);

        // 十字高光绘制
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.GraphicsDevice.RasterizerState, null, Main.GameViewMatrix.TransformationMatrix);

        Main.spriteBatch.Draw(tex, pos, null, color, Main.GameUpdateCount / rand + Projectile.rotation, tex.Size() / 2f, 0.8f, SpriteEffects.None, 0);
        Main.spriteBatch.Draw(tex, pos, null, color, Main.GameUpdateCount / rand + 1.57f + Projectile.rotation, tex.Size() / 2f, 0.8f, SpriteEffects.None, 0);

        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.GraphicsDevice.RasterizerState, null, Main.GameViewMatrix.TransformationMatrix);

        return false;
    }

    public abstract ModItem GetModItemDummy();

    public virtual bool RevealOperation(bool onlyJudging)
    {
        var modItem = GetModItemDummy();
        return modItem switch
        {
            OnceForAllGlobe onceForAllGlobe => onceForAllGlobe.RevealOperation(Projectile, onlyJudging),
            Globe basicGlobe => basicGlobe.RevealOperation(Projectile, onlyJudging),
            _ => false
        };
    }
}