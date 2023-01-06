using ImproveGame.Common.Animations;
using ImproveGame.Common.Configs;
using ImproveGame.Interface.Common;
using ImproveGame.Interface.PlayerInfo.UIElements;
using ImproveGame.Interface.SUIElements;
using Terraria.GameInput;

namespace ImproveGame.Interface.PlayerInfo
{
    public class PlayerInfoGUI : UIState
    {
        private static bool visible;
        public static bool Visible
        {
            get => UIConfigs.Instance.PlyInfo switch
            {
                UIConfigs.PlyInfoDisplayMode.AlwaysDisplayed => visible,
                UIConfigs.PlyInfoDisplayMode.WhenOpeningBackpack => Main.playerInventory && visible,
                UIConfigs.PlyInfoDisplayMode.NotDisplayed => false,
                _ => false
            };
            set => visible = true;
        }
        public static bool Expanded { get; set; }
        // new() 缩写，时间久了再看码太容易忘了使用的类型，还是不用缩写好（可以禁用显示虚线）
        // var 也容易忘，为了保证下次阅读代码方便点我还是尽量不用了（我就不用了）
        private static Player player;
        private static Vector2
            BeginPos = new Vector2(480, 0),
            BeginPosInv = new Vector2(590, 0),
            EndPos = new Vector2(480, 20),
            EndPosInv = new Vector2(590, 20);

        public AnimationTimer invTimer, openTimer;
        public SUIPanel mainPanel;
        public View CardPanel;
        public SUITitle title;
        public PlyInfoSwitch Switch;

        public override void OnInitialize()
        {
            if (float.TryParse(PlyInfo("CardWidth"), out float width))
            {
                PlyInfoCard.width = width;
            }
            player = Main.LocalPlayer;
            invTimer = new AnimationTimer();
            openTimer = new AnimationTimer
            {
                State = AnimationState.CloseComplete,
                Timer = 0
            };

            mainPanel = new SUIPanel(UIColor.PanelBorder, UIColor.PanelBg) { Shaded = true };
            mainPanel.SetPadding(0);
            mainPanel.Join(this);

            title = new SUITitle(PlyInfo("Name"), 0.5f)
            {
                background = UIColor.TitleBg2,
                border = UIColor.PanelBorder,
                RoundMode = RoundMode.Round4,
                round4 = new Vector4(10f, 10f, 0, 0),
            };
            title.SetPadding(0);
            title.Height.Pixels = 50f;
            title.Join(mainPanel);

            // 关闭按钮
            Switch = new PlyInfoSwitch(UIColor.TitleBg2, UIColor.PanelBorder, () => Expanded);
            Switch.Join(title);

            // 切换状态的时候保证绘制不越界 OverflowHidden 设置一下。动画结束之后再设置回来。
            Switch.OnMouseDown += (_, _) =>
            {
                Expanded = !Expanded;
                CardPanel.OverflowHidden = true;
            };

            CardPanel = new View()
            {
                Relative = RelativeMode.Vertical,
                OverflowHidden = true
            };
            CardPanel.SetPadding(8);
            CardPanel.SetInnerPixels(PlyInfoCard.TotalSize(3, 7));
            CardPanel.Join(mainPanel);

            title.Width.Pixels = CardPanel.Width.Pixels;

            // 太多了，我在想用 List<T>() 收纳下，然后加一个选择显示哪些的功能？
            CardPanel.Append(new PlyInfoCard(PlyInfo("LifeRegen"), () => $"{player.lifeRegen / 2f}/s", "Life"));
            CardPanel.Append(new PlyInfoCard(PlyInfo("ArmorPenetration"), () => $"{player.GetTotalArmorPenetration(DamageClass.Generic)}", "ArmorPenetration"));

            CardPanel.Append(new PlyInfoCard(PlyInfo("MeleeDamage"), () => $"{GetDamage(DamageClass.Melee)}%", "Melee2"));
            CardPanel.Append(new PlyInfoCard(PlyInfo("MeleeCrit"), () => $"{GetCrit(DamageClass.Melee)}%", "Melee2"));
            CardPanel.Append(new PlyInfoCard(PlyInfo("MeleeSpeed"), () => $"{MathF.Round(player.GetAttackSpeed(DamageClass.Melee) * 100f - 100f)}%", "Melee2"));

            CardPanel.Append(new PlyInfoCard(PlyInfo("RangedDamage"), () => $"{GetDamage(DamageClass.Ranged)}%", "Ranged2"));
            CardPanel.Append(new PlyInfoCard(PlyInfo("RangedCrit"), () => $"{GetCrit(DamageClass.Ranged)}%", "Ranged2"));

            CardPanel.Append(new PlyInfoCard(PlyInfo("ManaRegen"), () => $"{player.manaRegen / 2f}/s", "ManaRegen"));
            CardPanel.Append(new PlyInfoCard(PlyInfo("ManaDamage"), () => $"{GetDamage(DamageClass.Magic)}%", "Magic"));
            CardPanel.Append(new PlyInfoCard(PlyInfo("ManaCrit"), () => $"{GetCrit(DamageClass.Magic)}%", "Magic"));
            CardPanel.Append(new PlyInfoCard(PlyInfo("ManaCost"), () => $"{MathF.Round(player.manaCost * 100f)}%", "Magic"));

            CardPanel.Append(new PlyInfoCard(PlyInfo("SummonDamage"), () => $"{GetDamage(DamageClass.Summon)}%", "Summon"));
            CardPanel.Append(new PlyInfoCard(PlyInfo("MaxMinions"), () => $"{player.slotsMinions}/{player.maxMinions}", "Summon"));
            CardPanel.Append(new PlyInfoCard(PlyInfo("MaxTurrets"), () =>
            {
                int count = 0;
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI && Main.projectile[i].WipableTurret)
                    {
                        count++;
                    }
                }
                return $"{count}/{player.maxTurrets}";
            }, "Summon"));

            CardPanel.Append(new PlyInfoCard(PlyInfo("Endurance"), () => $"{MathF.Round(player.endurance * 100f)}%", "Endurance3"));

            CardPanel.Append(new PlyInfoCard(PlyInfo("FishingSkill"), () => $"{player.fishingSkill}", "FishingSkill"));
            CardPanel.Append(new PlyInfoCard(PlyInfo("Luck"), () => $"{player.luck}", "Luck"));
            CardPanel.Append(new PlyInfoCard(PlyInfo("Aggro"), () => $"{player.aggro}", "Aggro"));

            // tipPanel.Append(new PlyInfoCard(PlyInfo("WingTime")}:", () => $"{MathF.Round((player.wingTime + player.rocketTime * 6) / 60f, 2)}s", "Flying"));
            CardPanel.Append(new PlyInfoCard(PlyInfo("WingTimeMax"), () => $"{MathF.Round((player.wingTimeMax + player.rocketTimeMax * 6) / 60f, 2)}s", "Flying"));

            openTimer.OnOpenComplete += () => { CardPanel.OverflowHidden = false; };
        }

        public override void Update(GameTime gameTime)
        {
            bool Recalculate = false;
            // 动画开/关
            if (Main.playerInventory)
                invTimer.TryOpen();
            else
                invTimer.TryClose();
            if (Expanded)
                openTimer.TryOpen();
            else
                openTimer.TryClose();
            invTimer.Update();
            openTimer.Update();
            base.Update(gameTime);

            if (mainPanel.IsMouseHovering)
            {
                PlayerInput.LockVanillaMouseScroll("ImproveGame: PlayerInfo GUI");
                Main.LocalPlayer.mouseInterface = true;
            }

            float cardHeight = MathHelper.Lerp(0, PlyInfoCard.TotalSize(3, 7).Y + CardPanel.VPadding(), openTimer.Schedule);
            if (CardPanel.Height.Pixels != cardHeight)
            {
                CardPanel.Height.Pixels = cardHeight;
                Recalculate = true;
            }

            float round = MathHelper.Lerp(10f, 0, openTimer.Schedule);
            Switch.round4.Z = title.round4.Z = title.round4.W = round;

            Vector2 mainSize = new Vector2(
                MathHelper.Lerp(250, CardPanel.Width.Pixels, openTimer.Schedule),
                title.Height.Pixels + CardPanel.Height.Pixels);

            if (mainPanel.GetInnerSizePixels() != mainSize)
            {
                mainPanel.SetInnerPixels(mainSize);
                Recalculate = true;
                BeginPos.Y = BeginPosInv.Y = 62 - mainPanel.Height.Pixels;
            }

            Vector2 beginPos = Vector2.Lerp(BeginPos, BeginPosInv, invTimer.Schedule);
            Vector2 endPos = Vector2.Lerp(EndPos, EndPosInv, invTimer.Schedule);
            Vector2 Pos = Vector2.Lerp(beginPos, endPos, openTimer.Schedule);

            if (mainPanel.GetPosPixel() != endPos)
            {
                mainPanel.SetPosPixels(endPos);
                Recalculate = true;
            }

            if (Recalculate)
                mainPanel.Recalculate();
        }

        private static float GetDamage(DamageClass damageClass) => MathF.Round((player.GetTotalDamage(damageClass).Additive - 1) * 100f);
        private static float GetCrit(DamageClass damageClass) => player.GetTotalCritChance(damageClass);
        private static string PlyInfo(string str) => GetText($"UI.PlayerInfo.{str}");
    }
}
