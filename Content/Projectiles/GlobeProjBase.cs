using ImproveGame.Common.Configs;
using ImproveGame.Content.Items.Globes.Core;
using ImproveGame.Core;

namespace ImproveGame.Content.Projectiles;

public abstract class GlobeProjBase(Color mainColor) : ModProjectile
{
    public override string Texture => GetModItemDummy().Texture;

    public override LocalizedText DisplayName => GetModItemDummy().DisplayName;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailingMode[Type] = 2;
        ProjectileID.Sets.TrailCacheLength[Type] = 30;
        Globe.GlobeLookup[GetModItemDummy().Type] = Type;
    }

    public override void SetDefaults()
    {
        Projectile.CloneDefaults(ProjectileID.MoonGlobe);
        Projectile.width = 32;
        Projectile.height = 32;
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

        // ai进入二阶段
        if (Projectile.ai[2] < -180 && Projectile.ai[1] >= 0)
        {
            Player player = Main.player[Projectile.owner];

            // 穿墙，启动
            Projectile.tileCollide = false;

            // 目标速度
            var targetVel = Vector2.Normalize(player.Center - Projectile.Center) * (7f - Projectile.ai[2] / 60f);
            // 加权平均
            Projectile.velocity = (targetVel + Projectile.velocity * (60 + Projectile.ai[2] / 10f)) /
                                  (61f + Projectile.ai[2] / 10f);

            Projectile.rotation = Projectile.velocity.ToRotation();

            // 你给我死
            if (Projectile.owner == Main.myPlayer && Projectile.Center.Distance(player.Center) < 12)
            {
                Projectile.velocity *= 0.000000001f;
                Projectile.ai[1] = -1;
                NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, Projectile.whoAmI);
            }

            // 幽默特效
            if (Projectile.ai[2] % 12 == 0)
                SoundEngine.PlaySound(SoundID.Item13 with { MaxInstances = 114514 }, Projectile.Center);

            if (Main.rand.NextBool(2))
                Dust.NewDustPerfect(Projectile.Center + (Vector2.Normalize(Projectile.velocity) * -16), DustID.Smoke,
                    -Projectile.velocity.RotatedByRandom(0.3f) * 0.5f, 0, default, 2f);
        }

        // 死亡缓至——无法预料，无法逃避
        if (Projectile.ai[1] == -1)
        {
            Projectile.ai[1]--;

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
                    int dustIndex = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y),
                        Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 2f);
                    Main.dust[dustIndex].velocity *= 1.4f;
                }

                for (int i = 0; i < 20; i++)
                {
                    int dustIndex = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y),
                        Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 3f);
                    Main.dust[dustIndex].noGravity = true;
                    Main.dust[dustIndex].velocity *= 5f;
                    dustIndex = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width,
                        Projectile.height, DustID.Torch, 0f, 0f, 100, default, 2f);
                    Main.dust[dustIndex].velocity *= 3f;
                }

                for (int g = 0; g < 2; g++)
                {
                    int goreIndex = Gore.NewGore(Projectile.GetSource_Death(),
                        new Vector2(Projectile.position.X + Projectile.width / 2 - 24f,
                            Projectile.position.Y + Projectile.height / 2 - 24f), default, Main.rand.Next(61, 64), 1f);
                    Main.gore[goreIndex].velocity.X += 1.5f;
                    Main.gore[goreIndex].velocity.Y += 1.5f;
                    goreIndex = Gore.NewGore(Projectile.GetSource_Death(),
                        new Vector2(Projectile.position.X + Projectile.width / 2 - 24f,
                            Projectile.position.Y + Projectile.height / 2 - 24f), default, Main.rand.Next(61, 64), 1f);
                    Main.gore[goreIndex].velocity.X -= 1.5f;
                    Main.gore[goreIndex].velocity.Y += 1.5f;
                    goreIndex = Gore.NewGore(Projectile.GetSource_Death(),
                        new Vector2(Projectile.position.X + Projectile.width / 2 - 24f,
                            Projectile.position.Y + Projectile.height / 2 - 24f), default, Main.rand.Next(61, 64), 1f);
                    Main.gore[goreIndex].velocity.X += 1.5f;
                    Main.gore[goreIndex].velocity.Y -= 1.5f;
                    goreIndex = Gore.NewGore(Projectile.GetSource_Death(),
                        new Vector2(Projectile.position.X + Projectile.width / 2 - 24f,
                            Projectile.position.Y + Projectile.height / 2 - 24f), default, Main.rand.Next(61, 64), 1f);
                    Main.gore[goreIndex].velocity.X -= 1.5f;
                    Main.gore[goreIndex].velocity.Y -= 1.5f;
                }

                #endregion

                return;
            }

            // 特效弹幕只能在服务器/单人生成
            //var effectColor = GetEffectColor();
            if (Main.netMode is not NetmodeID.MultiplayerClient)
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero,
                    ModContent.ProjectileType<GlobeEffect>(), 0, 0, ai0: Type);

            SoundEngine.PlaySound(SoundID.Item107, Projectile.Center);
            for (int i = 0; i < 15; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Glass, 0f, -2f, 0, default,
                    1.2f);
            }
        }

        if (Projectile.ai[1] < 0)
        {
            Projectile.ai[1]--;

            // 这才是真正的死亡
            if (Projectile.ai[1] < -30)
                Projectile.Kill();
        }
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        // 爆改ai之反弹
        if (Projectile.ai[2] < 0)
        {
            if (Projectile.velocity.X != oldVelocity.X && Math.Abs(oldVelocity.X) > 0.4f)
            {
                Projectile.velocity.X = oldVelocity.X * -0.9f;
            }

            if (Projectile.velocity.Y != oldVelocity.Y && Math.Abs(oldVelocity.Y) > 1f)
            {
                Projectile.velocity.Y = oldVelocity.Y * -0.85f;
            }

            SoundEngine.PlaySound(Main.rand.Next([SoundID.Item140, SoundID.Item141, SoundID.Item142]),
                Projectile.Center);
            return false;
        }

        // 进死门（
        if (Projectile.ai[1] >= 0 && Projectile.owner == Main.myPlayer)
        {
            Projectile.velocity *= 0.000000001f;
            Projectile.tileCollide = false;
            Projectile.ai[1] = -1;
            Projectile.aiStyle = -1;
            AIType = 0;
            NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, Projectile.whoAmI);
        }

        return false;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (!UIConfigs.Instance.GlobeEffect)
            return base.PreDraw(ref lightColor);

        var projTex = TextureAssets.Projectile[Type].Value;
        var cross = ModAsset.CrazyGlowCross.Value;
        var roundGlow = ModAsset.GlowOrb.Value;
        var center = Projectile.Center - Main.screenPosition;
        var glowCrossColor = RealColor;
        glowCrossColor.A = 0;

        // 顶点拖尾
        var triangleList = PrepareTriangleList();
        if (triangleList.Count >= 3)
            DrawTrail(triangleList);

        // 进入死亡阶段后不要再绘制下面了
        if (Projectile.ai[1] >= 0)
        {
            // 背后发光大圆球
            Main.spriteBatch.Draw(roundGlow, center, null, glowCrossColor * 0.7f,
                1.57f, roundGlow.Size() / 2f, 0.45f, SpriteEffects.None, 0);

            // 本体绘制
            Main.spriteBatch.Draw(projTex, center, null, Color.White,
                Projectile.rotation + 1.57f, projTex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);

            // 十字高光绘制
            Main.spriteBatch.Draw(cross, center, null, glowCrossColor,
                0f, cross.Size() / 2f, 1f, SpriteEffects.None, 0);
        }

        Main.spriteBatch.ReBegin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

        return false;
    }

    private void DrawTrail(List<VertexInfo2> triangleList)
    {
        var projTex = TextureAssets.Projectile[Type].Value;

        Main.spriteBatch.ReBegin(SpriteSortMode.Deferred, BlendState.Additive);

        // 干掉注释就可以显示三角形栅格
        // RasterizerState rasterizerState = new RasterizerState();
        // rasterizerState.CullMode = CullMode.None;
        // rasterizerState.FillMode = FillMode.WireFrame;
        // Main.graphics.GraphicsDevice.RasterizerState = rasterizerState;

        var screenCenter = Main.screenPosition + Main.ScreenSize.ToVector2() / 2f;
        var screenSize = Main.ScreenSize.ToVector2() / Main.GameViewMatrix.Zoom;
        if (Main.LocalPlayer.gravDir == -1)
        {
            screenSize.Y = -screenSize.Y;
        }

        var screenPos = screenCenter - screenSize / 2f;

        var projection = Matrix.CreateOrthographicOffCenter(0, screenSize.X, screenSize.Y, 0, 0, 1);
        var model = Matrix.CreateTranslation(new Vector3(-screenPos.X, -screenPos.Y, 0));

        float uTime = (Main.GameUpdateCount % 30) / 30f;

        ModAsset.GlobeTrail.Value.Parameters["uTransform"].SetValue(model * projection);
        ModAsset.GlobeTrail.Value.Parameters["pixelSize"].SetValue(new Vector2(1f) / projTex.Size() * 6f);
        ModAsset.GlobeTrail.Value.Parameters["uTime"].SetValue(uTime);

        Main.graphics.GraphicsDevice.Textures[0] = projTex;
        Main.graphics.GraphicsDevice.Textures[1] = ModAsset.TrailStyle2.Value;

        ModAsset.GlobeTrail.Value.CurrentTechnique.Passes[0].Apply();

        Main.instance.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0,
            triangleList.Count / 3);

        Main.spriteBatch.ReBegin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
    }

    private List<VertexInfo2> PrepareTriangleList()
    {
        List<VertexInfo2> vertices = [];
        List<VertexInfo2> triangleList = [];
        for (var i = 1; i < Projectile.oldPos.Length; i++)
        {
            float actualLength = Projectile.oldPos.Count(vec => vec != Vector2.Zero);
            if (Projectile.oldPos[i] == Vector2.Zero)
                continue;

            Vector2 oldVel = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
            var factor = i / actualLength; // 遍历时以0-1递增
            var factorSquare = (float)Math.Pow(factor, 2);
            var color = RealColor;
            color.A = (byte)MathHelper.Lerp(150, 0, factorSquare);

            Vector2 correct = Vector2.Normalize(oldVel).RotatedBy(1.57f) * MathHelper.SmoothStep(20, 0, factorSquare);
            vertices.Add(new VertexInfo2(Projectile.oldPos[i] + Projectile.Size / 2f + correct,
                new Vector3(factor, 0f, 0f), color));
            vertices.Add(new VertexInfo2(Projectile.oldPos[i] + Projectile.Size / 2f - correct,
                new Vector3(factor, 1f, 0f), color));
        }

        if (vertices.Count > 2)
        {
            // 按照顺序连接三角形
            for (int i = 0; i < vertices.Count - 2; i += 2)
            {
                // 这是一个四边形 [i] [i+2] [i+1] [i+3]
                triangleList.Add(vertices[i]);
                triangleList.Add(vertices[i + 2]);
                triangleList.Add(vertices[i + 1]);

                triangleList.Add(vertices[i + 1]);
                triangleList.Add(vertices[i + 2]);
                triangleList.Add(vertices[i + 3]);
            }
        }

        return triangleList;
    }

    public abstract Globe GetModItemDummy();

    public virtual Color? GetEffectColor() => null;

    protected GlobeProjBase() : this(Color.White)
    {

    }

    public Color mainColor = mainColor;

    public Color RealColor => GetEffectColor() ?? mainColor;

    public abstract bool RevealOperation(bool onlyJudging);
}
public abstract class GlobeProjBase<T>(Color mainColor) : GlobeProjBase(mainColor) where T : Globe
{
    public override Globe GetModItemDummy() => ModContent.GetInstance<T>();
}