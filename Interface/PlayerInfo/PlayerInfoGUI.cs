using ImproveGame.Common.Animations;
using ImproveGame.Interface.BaseUIEs;
using ImproveGame.Interface.Common;
using ImproveGame.Interface.PlayerInfo.UIElements;
using ImproveGame.Interface.SUIElements;
using Terraria.GameInput;

namespace ImproveGame.Interface.PlayerInfo
{
    public class PlayerInfoGUI : UIState
    {
        public static bool Visible;
        public static Player player => Main.LocalPlayer;
        public bool Opened;
        public static Vector2 startPos1 = new Vector2(480, -100);
        public static Vector2 startPos2 = new Vector2(600, -100);
        public static Vector2 endPos1 = new Vector2(480, 20);
        public static Vector2 endPos2 = new Vector2(620, 20);
        public SUIPanel mainPanel;
        public RelativeElement list;
        public SUITitle title;
        public AnimationTimer invTimer;
        public AnimationTimer openTimer;
        public PlyInfoSwitch Switch;
        public override void OnInitialize()
        {
            invTimer = new AnimationTimer();
            openTimer = new AnimationTimer();

            Append(mainPanel = new SUIPanel(UIColor.PanelBorder, UIColor.PanelBackground)
            {
                Shaded = true
            });

            mainPanel.Append(title = new SUITitle("角色属性面板", 0.5f, 1)
            {
                background = new Color(0, 0, 0, 0.5f),
                border = new Color(0, 0, 0, 0.5f)
            });

            int w = 3;
            int h = 7;
            mainPanel.Append(list = new RelativeElement()
            {
                Width = new StyleDimension(PlyTip.w * w + 10f * (w - 1), 0),
                Height = new StyleDimension(PlyTip.h * h + 10f * (h - 1), 0),
                Relative = true,
                Interval = new Vector2(10),
                Layout = RelativeElement.RelativeMode.Vertical,
                OverflowHidden = true
            });

            title.Width.Pixels = list.Width.Pixels;

            list.Append(new PlyTip("生命回复:", () => $"{player.lifeRegen / 2f} / s", "Life"));
            list.Append(new PlyTip("全能破甲:", () => $"{player.GetTotalArmorPenetration(DamageClass.Generic)}", "ArmorPenetration"));

            list.Append(new PlyTip("近战伤害:", () => $"{GetDamage(DamageClass.Melee)}%", "Melee2"));
            list.Append(new PlyTip("近战暴击:", () => $"{GetCrit(DamageClass.Melee)}%", "Melee2"));
            list.Append(new PlyTip("近战速度:", () => $"{MathF.Round(player.GetAttackSpeed(DamageClass.Melee) * 100f - 100f)}%", "Melee2"));

            list.Append(new PlyTip("远程伤害:", () => $"{GetDamage(DamageClass.Ranged)}%", "Ranged2"));
            list.Append(new PlyTip("远程暴击:", () => $"{GetCrit(DamageClass.Ranged)}%", "Ranged2"));

            list.Append(new PlyTip("法力回复:", () => $"{player.manaRegen / 2f} / s", "ManaRegen"));
            list.Append(new PlyTip("魔法伤害:", () => $"{GetDamage(DamageClass.Magic)}%", "Magic"));
            list.Append(new PlyTip("魔法暴击:", () => $"{GetCrit(DamageClass.Magic)}%", "Magic"));
            list.Append(new PlyTip("魔力消耗:", () => $"{MathF.Round(player.manaCost * 100f)}%", "Magic"));

            list.Append(new PlyTip("召唤伤害:", () => $"{GetDamage(DamageClass.Summon)}%", "Summon"));
            list.Append(new PlyTip("召唤数量:", () => $"{player.slotsMinions} / {player.maxMinions}", "Summon"));
            list.Append(new PlyTip("哨兵栏:", () =>
            {
                int count = 0;
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI && Main.projectile[i].WipableTurret)
                    {
                        count++;
                    }
                }
                return $"{count} / {player.maxTurrets}";
            }, "Summon"));

            list.Append(new PlyTip("伤害免疫:", () => $"{MathF.Round(player.endurance * 100f)}%", "Endurance3"));

            list.Append(new PlyTip("渔力:", () => $"{player.fishingSkill}", "FishingSkill"));
            list.Append(new PlyTip("幸运值:", () => $"{player.luck}", "Luck"));
            list.Append(new PlyTip("仇恨值:", () => $"{player.aggro}", "Aggro"));

            list.Append(new PlyTip("飞行时间:", () => $"{MathF.Round((player.wingTime + player.rocketTime * 6) / 60f, 2)} s", "Flying"));
            list.Append(new PlyTip("飞行上限:", () => $"{MathF.Round((player.wingTimeMax + player.rocketTimeMax * 6) / 60f, 2)} s", "Flying"));

            mainPanel.Append(Switch = new PlyInfoSwitch(new Color(0, 0, 0, 0.5f))
            {
                Width = list.Width,
                HAlign = 0.5f,
                round = 10f,
                Opened = () => Opened
            });

            Switch.OnMouseDown += (_, _) =>
            {
                Opened = !Opened;
                list.OverflowHidden = true;
            };

            openTimer.OnOpenComplete += () => { list.OverflowHidden = false; };
            openTimer.OnCloseComplete += () => { list.OverflowHidden = false; };
        }

        /// <summary>
        /// 伤害值
        /// </summary>
        /// <param name="damageClass"></param>
        /// <returns></returns>
        public static float GetDamage(DamageClass damageClass)
        {
            return MathF.Round((player.GetTotalDamage(damageClass).Additive - 1) * 100f);
        }

        /// <summary>
        /// 暴击率
        /// </summary>
        /// <param name="damageClass"></param>
        /// <returns></returns>
        public static float GetCrit(DamageClass damageClass)
        {
            return player.GetTotalCritChance(damageClass);
        }

        public override void Update(GameTime gameTime)
        {
            if (Main.playerInventory)
            {
                invTimer.TryOpen();
            }
            else
            {
                invTimer.TryClose();
            }

            if (Opened)
            {
                openTimer.TryOpen();
            }
            else
            {
                openTimer.TryClose();
            }
            invTimer.Update();
            openTimer.Update();
            base.Update(gameTime);

            if (mainPanel.IsMouseHovering)
            {
                PlayerInput.LockVanillaMouseScroll("ImproveGame: PlayerInfo GUI");
                Main.LocalPlayer.mouseInterface = true;
            }

            Vector2 mainSize = new Vector2(MathHelper.Lerp(50, list.Width.Pixels, openTimer.Schedule), title.Height.Pixels + 10f + list.Height.Pixels + 10f + Switch.Height.Pixels);

            if (mainPanel.GetSizeInside() != mainSize)
            {
                mainPanel.SetInnerSize(mainSize).Recalculate();
                startPos1.Y = 60 - mainPanel.Height.Pixels;
                startPos2.Y = 60 - mainPanel.Height.Pixels;
            }

            Vector2 endPos = Vector2.Lerp(endPos1, endPos2, invTimer.Schedule);
            Vector2 startPos = Vector2.Lerp(startPos1, startPos2, invTimer.Schedule);
            Vector2 Pos = Vector2.Lerp(startPos, endPos, openTimer.Schedule);

            if (Pos != mainPanel.GetPPos())
            {
                mainPanel.SetPos(Pos).Recalculate();
            }
        }
    }
}
