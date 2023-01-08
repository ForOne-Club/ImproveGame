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
        private static bool _visible;

        public static bool Visible
        {
            get => UIConfigs.Instance.PlyInfo switch
            {
                UIConfigs.PlyInfoDisplayMode.AlwaysDisplayed => _visible,
                UIConfigs.PlyInfoDisplayMode.WhenOpeningBackpack => Main.playerInventory && _visible,
                UIConfigs.PlyInfoDisplayMode.NotDisplayed => false,
                _ => false
            };
            set => _visible = value;
        }

        private static bool Expanded { get; set; }

        // new() 缩写，时间久了再看码太容易忘了使用的类型，还是不用缩写好（可以禁用显示虚线）
        // var 也容易忘，为了保证下次阅读代码方便点我还是尽量不用了
        private static Player _player;

        private static readonly Vector2
            InvPosOff = new Vector2(480, 20),
            InvPosOn = new Vector2(590, 20);

        private AnimationTimer _invTimer, _openTimer;
        private SUIPanel _mainPanel;
        private View _cardPanel;
        private SUITitle _title;
        private PlyInfoSwitch _switch;

        public override void OnInitialize()
        {
            if (float.TryParse(PlyInfo("CardWidth"), out float width))
            {
                PlyInfoCard.width = width;
            }

            _player = Main.LocalPlayer;
            _invTimer = new AnimationTimer();
            _openTimer = new AnimationTimer
            {
                State = AnimationState.CloseComplete,
                Timer = 0
            };

            _mainPanel = new SUIPanel(UIColor.PanelBorder, UIColor.PanelBg) { Shaded = true };
            _mainPanel.SetPadding(0);
            _mainPanel.Join(this);

            _title = new SUITitle(PlyInfo("Name"), 0.5f)
            {
                background = UIColor.TitleBg2,
                border = UIColor.PanelBorder,
                RoundMode = RoundMode.Round4,
                round4 = new Vector4(10f, 10f, 0, 0),
            };
            _title.SetPadding(0);
            _title.Height.Pixels = 50f;
            _title.Join(_mainPanel);

            // 关闭按钮
            _switch = new PlyInfoSwitch(UIColor.TitleBg2, UIColor.PanelBorder, () => Expanded);
            _switch.Join(_title);

            // 切换状态的时候保证绘制不越界 OverflowHidden 设置一下。动画结束之后再设置回来。
            _switch.OnMouseDown += (_, _) =>
            {
                Expanded = !Expanded;
                _cardPanel.OverflowHidden = true;
            };

            _cardPanel = new View()
            {
                Relative = RelativeMode.Vertical,
                OverflowHidden = true
            };
            _cardPanel.SetPadding(8);
            _cardPanel.SetInnerPixels(PlyInfoCard.TotalSize(3, 7));
            _cardPanel.Join(_mainPanel);

            _title.Width.Pixels = _cardPanel.Width.Pixels;

            // 太多了，我在想用 List<T>() 收纳下，然后加一个选择显示哪些的功能？
            _cardPanel.Append(new PlyInfoCard(PlyInfo("LifeRegen"), () => $"{_player.lifeRegen / 2f}/s", "Life"));
            _cardPanel.Append(new PlyInfoCard(PlyInfo("ArmorPenetration"),
                () => $"{_player.GetTotalArmorPenetration(DamageClass.Generic)}", "ArmorPenetration"));

            _cardPanel.Append(new PlyInfoCard(PlyInfo("MeleeDamage"), () => $"{GetDamage(DamageClass.Melee)}%",
                "Melee2"));
            _cardPanel.Append(new PlyInfoCard(PlyInfo("MeleeCrit"), () => $"{GetCrit(DamageClass.Melee)}%", "Melee2"));
            _cardPanel.Append(new PlyInfoCard(PlyInfo("MeleeSpeed"),
                () => $"{MathF.Round(_player.GetAttackSpeed(DamageClass.Melee) * 100f - 100f)}%", "Melee2"));

            _cardPanel.Append(new PlyInfoCard(PlyInfo("RangedDamage"), () => $"{GetDamage(DamageClass.Ranged)}%",
                "Ranged2"));
            _cardPanel.Append(
                new PlyInfoCard(PlyInfo("RangedCrit"), () => $"{GetCrit(DamageClass.Ranged)}%", "Ranged2"));

            _cardPanel.Append(new PlyInfoCard(PlyInfo("ManaRegen"), () => $"{_player.manaRegen / 2f}/s", "ManaRegen"));
            _cardPanel.Append(new PlyInfoCard(PlyInfo("ManaDamage"), () => $"{GetDamage(DamageClass.Magic)}%",
                "Magic"));
            _cardPanel.Append(new PlyInfoCard(PlyInfo("ManaCrit"), () => $"{GetCrit(DamageClass.Magic)}%", "Magic"));
            _cardPanel.Append(new PlyInfoCard(PlyInfo("ManaCost"), () => $"{MathF.Round(_player.manaCost * 100f)}%",
                "Magic"));

            _cardPanel.Append(new PlyInfoCard(PlyInfo("SummonDamage"), () => $"{GetDamage(DamageClass.Summon)}%",
                "Summon"));
            _cardPanel.Append(new PlyInfoCard(PlyInfo("MaxMinions"),
                () => $"{_player.slotsMinions}/{_player.maxMinions}", "Summon"));
            _cardPanel.Append(new PlyInfoCard(PlyInfo("MaxTurrets"),
                () =>
                    $"{Main.projectile.Count(t => t.active && t.owner == _player.whoAmI && t.WipableTurret)}/{_player.maxTurrets}",
                "Summon"));

            _cardPanel.Append(new PlyInfoCard(PlyInfo("Endurance"), () => $"{MathF.Round(_player.endurance * 100f)}%",
                "Endurance3"));

            _cardPanel.Append(new PlyInfoCard(PlyInfo("FishingSkill"), () => $"{_player.fishingSkill}",
                "FishingSkill"));
            _cardPanel.Append(new PlyInfoCard(PlyInfo("Luck"), () => $"{_player.luck}", "Luck"));
            _cardPanel.Append(new PlyInfoCard(PlyInfo("Aggro"), () => $"{_player.aggro}", "Aggro"));

            // tipPanel.Append(new PlyInfoCard(PlyInfo("WingTime")}:", () => $"{MathF.Round((player.wingTime + player.rocketTime * 6) / 60f, 2)}s", "Flying"));
            _cardPanel.Append(new PlyInfoCard(PlyInfo("WingTimeMax"),
                () => $"{MathF.Round((_player.wingTimeMax + _player.rocketTimeMax * 6) / 60f, 2)}s", "Flying"));

            _openTimer.OnOpenComplete += () => { _cardPanel.OverflowHidden = false; };
        }

        public override void Update(GameTime gameTime)
        {
            bool recalculate = false;
            // 动画开/关
            if (Main.playerInventory)
                _invTimer.TryOpen();
            else
                _invTimer.TryClose();
            if (Expanded)
                _openTimer.TryOpen();
            else
                _openTimer.TryClose();
            _invTimer.Update();
            _openTimer.Update();
            base.Update(gameTime);

            if (_mainPanel.IsMouseHovering)
            {
                PlayerInput.LockVanillaMouseScroll("ImproveGame: PlayerInfo GUI");
                Main.LocalPlayer.mouseInterface = true;
            }

            float cardHeight = MathHelper.Lerp(0, PlyInfoCard.TotalSize(3, 7).Y + _cardPanel.VPadding(),
                _openTimer.Schedule);
            if (Math.Abs(_cardPanel.Height.Pixels - cardHeight) > 0)
            {
                _cardPanel.Height.Pixels = cardHeight;
                recalculate = true;
            }

            float round = MathHelper.Lerp(10f, 0, _openTimer.Schedule);
            _switch.round4.Z = _title.round4.Z = _title.round4.W = round;

            Vector2 mainSize = new Vector2(
                MathHelper.Lerp(250, _cardPanel.Width.Pixels, _openTimer.Schedule),
                _title.Height.Pixels + _cardPanel.Height.Pixels);

            if (_mainPanel.GetInnerSizePixels() != mainSize)
            {
                _mainPanel.SetInnerPixels(mainSize);
                recalculate = true;
            }

            Vector2 endPos = Vector2.Lerp(InvPosOff, InvPosOn, _invTimer.Schedule);

            if (_mainPanel.GetPosPixel() != endPos)
            {
                _mainPanel.SetPosPixels(endPos);
                recalculate = true;
            }

            if (recalculate)
                _mainPanel.Recalculate();
        }

        private static float GetDamage(DamageClass damageClass) =>
            MathF.Round((_player.GetTotalDamage(damageClass).Additive - 1) * 100f);

        private static float GetCrit(DamageClass damageClass) => _player.GetTotalCritChance(damageClass);
        private static string PlyInfo(string str) => GetText($"UI.PlayerInfo.{str}");
    }
}