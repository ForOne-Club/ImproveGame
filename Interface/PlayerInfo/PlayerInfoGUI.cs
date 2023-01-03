using ImproveGame.Common.Animations;
using ImproveGame.Common.Configs;
using ImproveGame.Interface.BaseUIEs;
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
        public static bool Opened { get; set; }

        // readonly 单纯不想看 VS 一直给我报虚线，真**烦人（我也不想禁用显示）。
        // new() 缩写，时间久了再看码太容易忘了使用的类型，还是不用缩写好（被迫禁用显示虚线）。
        // var 也容易忘，为了保证下次阅读代码方便点我还是尽量不用了（我就不用了）。
        private Player player;
        // 动画用到的位置，BeginPos 是收起状态位置，EndPos 是开启状态位置
        private static Vector2 BeginPos = new Vector2(480, 0);
        private static Vector2 BeginPosInv = new Vector2(590, 0);
        private static Vector2 EndPos = new Vector2(480, 20);
        private static Vector2 EndPosInv = new Vector2(590, 20);

        public AnimationTimer invTimer;
        public AnimationTimer openTimer;
        public SUIPanel mainPanel;
        public View tipPanel;
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

            Append(mainPanel = new SUIPanel(UIColor.PanelBorder, UIColor.PanelBg)
            {
                Shaded = true
            });
            mainPanel.SetPadding(6);

            // 这种写法我也不确定好不好，本身 Append() 会执行一次 Recalculate()
            // 然后 OnInitialize() 之后也会执行一次 Recalculate()，调用实在太频繁了。
            mainPanel.Append(title = new SUITitle(PlyInfo("Name"), 0.5f)
            {
                background = UIColor.TitleBg, border = UIColor.TitleBg
            });

            // RelativeElement 这名字不好听，Android 组件都叫 xxxView，我也想这么叫。
            // 但是格式不统一，全改掉名字又要花时间适应，*****。
            // StyleDimension 这个 struct 真的有点莫名其妙，就俩属性整这么复杂。
            mainPanel.Append(tipPanel = new View()
            {
                Spacing = new Vector2(6),
                Relative = RelativeMode.Vertical,
                OverflowHidden = true
            });
            tipPanel.SetInnerPixels(PlyInfoCard.TotalSize(3, 7));

            title.Width.Pixels = tipPanel.Width.Pixels;

            // 太多了，我在想用 List<T>() 收纳下，然后加一个选择显示哪些的功能？
            tipPanel.Append(new PlyInfoCard(PlyInfo("LifeRegen"), () => $"{player.lifeRegen / 2f}/s", "Life"));
            tipPanel.Append(new PlyInfoCard(PlyInfo("ArmorPenetration"), () => $"{player.GetTotalArmorPenetration(DamageClass.Generic)}", "ArmorPenetration"));

            tipPanel.Append(new PlyInfoCard(PlyInfo("MeleeDamage"), () => $"{GetDamage(DamageClass.Melee)}%", "Melee2"));
            tipPanel.Append(new PlyInfoCard(PlyInfo("MeleeCrit"), () => $"{GetCrit(DamageClass.Melee)}%", "Melee2"));
            tipPanel.Append(new PlyInfoCard(PlyInfo("MeleeSpeed"), () => $"{MathF.Round(player.GetAttackSpeed(DamageClass.Melee) * 100f - 100f)}%", "Melee2"));

            tipPanel.Append(new PlyInfoCard(PlyInfo("RangedDamage"), () => $"{GetDamage(DamageClass.Ranged)}%", "Ranged2"));
            tipPanel.Append(new PlyInfoCard(PlyInfo("RangedCrit"), () => $"{GetCrit(DamageClass.Ranged)}%", "Ranged2"));

            tipPanel.Append(new PlyInfoCard(PlyInfo("ManaRegen"), () => $"{player.manaRegen / 2f}/s", "ManaRegen"));
            tipPanel.Append(new PlyInfoCard(PlyInfo("ManaDamage"), () => $"{GetDamage(DamageClass.Magic)}%", "Magic"));
            tipPanel.Append(new PlyInfoCard(PlyInfo("ManaCrit"), () => $"{GetCrit(DamageClass.Magic)}%", "Magic"));
            tipPanel.Append(new PlyInfoCard(PlyInfo("ManaCost"), () => $"{MathF.Round(player.manaCost * 100f)}%", "Magic"));

            tipPanel.Append(new PlyInfoCard(PlyInfo("SummonDamage"), () => $"{GetDamage(DamageClass.Summon)}%", "Summon"));
            tipPanel.Append(new PlyInfoCard(PlyInfo("MaxMinions"), () => $"{player.slotsMinions}/{player.maxMinions}", "Summon"));
            tipPanel.Append(new PlyInfoCard(PlyInfo("MaxTurrets"), () =>
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

            tipPanel.Append(new PlyInfoCard(PlyInfo("Endurance"), () => $"{MathF.Round(player.endurance * 100f)}%", "Endurance3"));

            tipPanel.Append(new PlyInfoCard(PlyInfo("FishingSkill"), () => $"{player.fishingSkill}", "FishingSkill"));
            tipPanel.Append(new PlyInfoCard(PlyInfo("Luck"), () => $"{player.luck}", "Luck"));
            tipPanel.Append(new PlyInfoCard(PlyInfo("Aggro"), () => $"{player.aggro}", "Aggro"));

            // tipPanel.Append(new PlyInfoCard(PlyInfo("WingTime")}:", () => $"{MathF.Round((player.wingTime + player.rocketTime * 6) / 60f, 2)}s", "Flying"));
            tipPanel.Append(new PlyInfoCard(PlyInfo("WingTimeMax"), () => $"{MathF.Round((player.wingTimeMax + player.rocketTimeMax * 6) / 60f, 2)}s", "Flying"));

            // 关闭按钮
            mainPanel.Append(Switch = new PlyInfoSwitch()
            {
                Spacing = new Vector2(6),
                Width = tipPanel.Width,
                HAlign = 0.5f,
                Opened = () => Opened
            });

            // 切换状态的时候保证绘制不越界 OverflowHidden 设置一下。动画结束之后再设置回来。
            Switch.OnMouseDown += (_, _) =>
            {
                Opened = !Opened;
                tipPanel.OverflowHidden = true;
            };
            openTimer.OnOpenComplete += () => { tipPanel.OverflowHidden = false; };
        }

        public override void Update(GameTime gameTime)
        {
            // 动画开/关
            if (Main.playerInventory)
                invTimer.TryOpen();
            else
                invTimer.TryClose();
            if (Opened)
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

            Vector2 mainSize = new Vector2(
                MathHelper.Lerp(80, tipPanel.Width.Pixels, openTimer.Schedule),
                title.Height.Pixels + 6f + tipPanel.Height.Pixels + 6f + Switch.Height.Pixels
                );

            if (mainPanel.GetSizeInside() != mainSize)
            {
                mainPanel.SetInnerSize(mainSize).Recalculate();
                BeginPos.Y = BeginPosInv.Y = 52 - mainPanel.Height.Pixels;
            }

            Vector2 endPos = Vector2.Lerp(EndPos, EndPosInv, invTimer.Schedule);
            Vector2 startPos = Vector2.Lerp(BeginPos, BeginPosInv, invTimer.Schedule);
            Vector2 Pos = Vector2.Lerp(startPos, endPos, openTimer.Schedule);

            if (Pos != mainPanel.GetPosPixel())
            {
                mainPanel.SetPosPixels(Pos);
                mainPanel.Recalculate();
            }
        }

        public float GetDamage(DamageClass damageClass) => MathF.Round((player.GetTotalDamage(damageClass).Additive - 1) * 100f);
        public float GetCrit(DamageClass damageClass) => player.GetTotalCritChance(damageClass);
        public static string PlyInfo(string str) => GetText($"UI.PlayerInfo.{str}");
    }
}
