﻿using ImproveGame.Content.Items;
using ReLogic.Utilities;
using Terraria.GameInput;
using Terraria.ID;

namespace ImproveGame.Content.Projectiles;

public class DetectorDroneProj : ModProjectile
{
    private class PlayerControlLock : ModPlayer
    {
        public override void UpdateEquips()
        {
            // 原版的 isOperatingAnotherEntity 在 UpdateEquips 执行前赋值
            // 所以我们在这写我们的代码
            Player.isOperatingAnotherEntity |=
                Player.ownedProjectileCounts[ModContent.ProjectileType<DetectorDroneProj>()] > 0;
        }
    }

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.DrawScreenCheckFluff[Type] = 960;
        ProjectileID.Sets.TrailCacheLength[Type] = 20;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.CloneDefaults(ProjectileID.JimsDrone);
        Projectile.aiStyle = -1;
        DrawOffsetX = -10;
        DrawOriginOffsetY = -6;
    }

    public override void AI()
    {
        Player player = Main.player[Projectile.owner];
        bool shouldBeKilled = player.dead;

        if (Projectile.owner == Main.myPlayer)
        {
            // 高处爆炸
            if (Projectile.position.Y - Projectile.height <= 16 * Main.offScreenRange / 2)
            {
                Projectile.Kill();
                return;
            }

            if (player.HeldItem.type != ModContent.ItemType<DetectorDrone>())
                shouldBeKilled = true;

            bool gravityFlipped = player.gravDir == -1f;
            var direction = Vector2.Zero;

            if (PlayerInput.UsingGamepad)
            {
                direction = PlayerInput.GamepadThumbstickLeft;
            }
            else
            {
                Player.DirectionalInputSyncCache localInputCache = player.LocalInputCache;
                direction.X -= (localInputCache.controlLeft ^ gravityFlipped).ToInt();
                direction.X += (localInputCache.controlRight ^ gravityFlipped).ToInt();
                direction.Y -= localInputCache.controlUp.ToInt();
                direction.Y += localInputCache.controlDown.ToInt();
                direction = direction.SafeNormalize(Vector2.Zero);
            }

            if (new Vector2(Projectile.ai[0], Projectile.ai[1]) != direction)
            {
                Projectile.ai[0] = direction.X;
                Projectile.ai[1] = direction.Y;
                Projectile.netUpdate = true;
            }

            Main.DroneCameraTracker.Track(Projectile);
        }

        // 八百格开外自动爆炸
        if (player.DistanceSQ(Projectile.Center) > Math.Pow(16 * 600, 2))
        {
            shouldBeKilled = true;
            AddNotification(this.GetLocalizedValue("TooFarAway"));
        }

        if (shouldBeKilled)
        {
            Projectile.Kill();
            return;
        }

        var speedDirection = new Vector2(Projectile.ai[0], Projectile.ai[1]);
        const float speed = 0.12f;
        bool onLand = Projectile.velocity.Y == 0f;
        Projectile.velocity += speedDirection * speed;

        // float gravity = 0.05f; // 重力系数
        // Projectile.velocity.Y += gravity;

        if (Projectile.velocity.Length() > 20f)
            Projectile.velocity *= 20f / Projectile.velocity.Length();

        Projectile.velocity *= 0.98f;

        UpdateSound(onLand, speedDirection);

        Lighting.AddLight(Projectile.Center, new Vector3(59, 217, 180) / 255f * 1.2f);
        Projectile.timeLeft = 2;

        Projectile.rotation = Projectile.velocity.X * 0.1f;

        if (Main.netMode is NetmodeID.Server && Main.player.IndexInRange(Projectile.owner) && player.active)
            RemoteClient.CheckSection(Projectile.owner, Projectile.position);
    }

    public override bool OnTileCollide(Vector2 lastVelocity)
    {
        if (Projectile.velocity.X != lastVelocity.X && Math.Abs(lastVelocity.X) > 1f)
            Projectile.velocity.X = -lastVelocity.X * 0.3f;

        if (Projectile.velocity.Y != lastVelocity.Y && Math.Abs(lastVelocity.Y) > 1f)
            Projectile.velocity.Y = -lastVelocity.Y * 0.3f;

        return false;
    }

    public override bool? CanCutTiles() => false;

    /// <summary>
    /// 声音控制，从原版抄的
    /// </summary>
    private void UpdateSound(bool onLand, Vector2 speedDirection)
    {
        Projectile.localAI[0] += 1f;

        float pitchFactor = 0; // 声音的声调程度
        if (!onLand)
            pitchFactor = 50;

        if (speedDirection.Length() > 0.5f)
            pitchFactor = MathHelper.Lerp(0f, 100f, speedDirection.Length());

        Projectile.localAI[1] = MathHelper.Lerp(Projectile.localAI[1], pitchFactor, 0.2f);
        float num3 = Utils.Remap(Projectile.localAI[0], 0f, 5f, 0f, 1f) *
                     Utils.Remap(Projectile.localAI[0], 5f, 15f, 1f, 0f);
        float volume = Utils.Clamp(MathHelper.Max(Utils.Remap(Projectile.localAI[1], 0f, 100f, 0f, 25f), num3 * 12f),
            0f, 100f) * 2f;
        SoundEngine.TryGetActiveSound(SlotId.FromFloat(Projectile.localAI[2]), out ActiveSound activeSound);
        if (activeSound == null && volume != 0f)
        {
            Projectile.localAI[2] = SoundEngine.PlayTrackedLoopedSound(SoundID.JimsDrone, Projectile.Center,
                new ProjectileAudioTracker(Projectile).IsActiveAndInGame).ToFloat();
            SoundEngine.TryGetActiveSound(SlotId.FromFloat(Projectile.localAI[2]), out activeSound);
        }

        if (activeSound != null)
        {
            activeSound.Volume = volume;
            activeSound.Position = Projectile.Center;
            activeSound.Pitch = Utils.Clamp(Utils.Remap(Projectile.localAI[1], 0f, 100f, -1f, 1f) + num3, -1f, 1f);
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Main.DrawTrail(Projectile, new Vector2(10f, 2f), new Color(59, 217, 180, 100));
        Main.DrawTrail(Projectile, new Vector2(-10f, 2f), new Color(59, 217, 180, 100));
        return base.PreDraw(ref lightColor);
    }

    // 改了DrawOffset和DrawOriginOffset就是这么麻烦
    public override void PostDraw(Color lightColor)
    {
        var tex = ModAsset.DetectorDrone_Glow.Value;

        int offsetX = 0;
        int offsetY = 0;
        float originX = (tex.Width - Projectile.width) * 0.5f + Projectile.width * 0.5f;
        ProjectileLoader.DrawOffset(Projectile, ref offsetX, ref offsetY, ref originX);

        SpriteEffects effects = SpriteEffects.None;
        if (Projectile.spriteDirection < 0) effects = SpriteEffects.FlipHorizontally;

        var origin = new Vector2(originX, Projectile.height / 2 + offsetY);
        var pos = new Vector2(Projectile.position.X - Main.screenPosition.X + originX + offsetX,
            Projectile.position.Y - Main.screenPosition.Y + Projectile.height / 2 + Projectile.gfxOffY);
        pos.X -= 4f;

        var color = Color.White;
        color.A = 0;

        Main.EntitySpriteDraw(tex, pos, null, color, Projectile.rotation, origin, Projectile.scale, effects);
    }

    public override void OnKill(int timeLeft)
    {
        if (SoundEngine.TryGetActiveSound(SlotId.FromFloat(Projectile.localAI[2]), out var sound))
            sound.Stop();
        SoundEngine.PlaySound(SoundID.Item62, Projectile.position);

        Color transparent = Color.Transparent;
        for (int i = 0; i < 15; i++)
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f,
                100,
                transparent, 0.8f);
            d.fadeIn = 0f;
            d.velocity *= 0.5f;
        }

        for (int j = 0; j < 5; j++)
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GoldFlame, 0f,
                0f, 100,
                transparent, 2.5f);
            d.noGravity = true;
            d.velocity *= 2.5f;
            d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GoldFlame, 0f, 0f,
                100,
                transparent, 1.1f);
            d.velocity *= 2f;
            d.noGravity = true;
        }

        for (int k = 0; k < 3; k++)
        {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Electric, 0f,
                0f, 100,
                transparent, 1.1f);
            d.velocity *= 2f;
            d.noGravity = true;
        }

        for (int l = -1; l <= 1; l += 2)
        {
            for (int m = -1; m <= 1; m += 2)
            {
                if (!Main.rand.NextBool(5))
                    continue;

                Gore gore = Gore.NewGoreDirect(Projectile.position, Vector2.Zero, Main.rand.Next(61, 64));
                gore.velocity *= 0.2f;
                gore.scale *= 0.65f;
                gore.velocity += new Vector2(l, m) * 0.5f;
            }
        }
    }
}