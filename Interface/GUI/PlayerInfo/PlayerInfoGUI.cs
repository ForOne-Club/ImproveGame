using ImproveGame.Common.Animations;
using ImproveGame.Common.Configs;
using ImproveGame.Interface.Common;
using ImproveGame.Interface.PlayerInfo.Elements;
using ImproveGame.Interface.SUIElements;
using Terraria.GameInput;

namespace ImproveGame.Interface.PlayerInfo
{
    public class PlayerInfoGUI : ViewBody
    {
        private static bool _visible;
        public override bool Display { get => Visible; set => Visible = value; }

        public static bool Visible
        {
            get => UIConfigs.Instance.PlyInfo switch
            {
                UIConfigs.PAPDisplayMode.AlwaysDisplayed => _visible,
                UIConfigs.PAPDisplayMode.WhenOpeningBackpack => Main.playerInventory && _visible,
                UIConfigs.PAPDisplayMode.NotDisplayed => false,
                _ => false
            };
            set => _visible = value;
        }

        private static bool _expanded;
        private static Player _player;

        private static readonly Vector2
            _inventoryOpenPos = new Vector2(480, 25),
            _inventoryClosePos = new Vector2(590, 25);

        private AnimationTimer _invTimer, _openTimer;
        private SUIPanel _mainPanel;
        private View _cardPanel;
        private SUIButton _titleButton;

        public override void OnInitialize()
        {
            if (float.TryParse(GetHJSON("CardWidth"), out float width))
            {
                PlyInfoCard.width = width;
            }

            _player = Main.LocalPlayer;
            _invTimer = new AnimationTimer();
            _openTimer = new AnimationTimer()
            {
                State = AnimationState.CompleteClose,
                Timer = 0
            };

            _mainPanel = new SUIPanel(UIColor.PanelBorder, UIColor.PanelBg)
            {
                Shaded = true,
                ShadowThickness = 20f,
            };
            _mainPanel.Opacity.Type = OpacityType.Self;
            _mainPanel.SetPadding(0);
            _mainPanel.Join(this);

            _titleButton = new SUIButton(GetHJSON("Name"))
            {
                Opacity = { Type = OpacityType.Self },
                DragIgnore = true,
                TextAlign = { X = 0.5f, Y = 0.5f },
                Width = new StyleDimension(0f, 1f),
                Relative = RelativeMode.Horizontal,
                Rounded = new Vector4(10f, 10f, 0f, 0f),
                BeginBgColor = UIColor.TitleBg2 * 0.75f,
                EndBgColor = UIColor.TitleBg2,
            };
            _titleButton.OnLeftMouseDown += (_, _) =>
            {
                _expanded = !_expanded;
                _cardPanel.OverflowHidden = true;
            };
            _titleButton.Join(_mainPanel);

            _cardPanel = new View()
            {
                Relative = RelativeMode.Vertical,
                OverflowHidden = true
            };
            _cardPanel.SetPadding(8);
            _cardPanel.SetInnerPixels(PlyInfoCard.TotalSize(3, 7));
            _cardPanel.Join(_mainPanel);

            new PlyInfoCard(GetHJSON("LifeRegen"),
                () => $"{_player.lifeRegen / 2f}/s", "Life").Join(_cardPanel);

            new PlyInfoCard(GetHJSON("ArmorPenetration"),
                () => $"{_player.GetTotalArmorPenetration(DamageClass.Generic)}", "ArmorPenetration").Join(_cardPanel);

            new PlyInfoCard(GetHJSON("MeleeDamage"),
                () => $"{GetDamage(DamageClass.Melee)}%",
                "Melee2").Join(_cardPanel);

            new PlyInfoCard(GetHJSON("MeleeCrit"),
                () => $"{GetCrit(DamageClass.Melee)}%", "Melee2").Join(_cardPanel);

            new PlyInfoCard(GetHJSON("MeleeSpeed"),
                () => $"{MathF.Round(_player.GetAttackSpeed(DamageClass.Melee) * 100f - 100f)}%", "Melee2").Join(_cardPanel);

            new PlyInfoCard(GetHJSON("RangedDamage"),
                () => $"{GetDamage(DamageClass.Ranged)}%",
                "Ranged2").Join(_cardPanel);

            new PlyInfoCard(GetHJSON("RangedCrit"),
                () => $"{GetCrit(DamageClass.Ranged)}%", "Ranged2").Join(_cardPanel);

            new PlyInfoCard(GetHJSON("ManaRegen"),
                () => $"{_player.manaRegen / 2f}/s", "ManaRegen").Join(_cardPanel);

            new PlyInfoCard(GetHJSON("ManaDamage"),
                () => $"{GetDamage(DamageClass.Magic)}%",
                "Magic").Join(_cardPanel);

            new PlyInfoCard(GetHJSON("ManaCrit"),
                () => $"{GetCrit(DamageClass.Magic)}%", "Magic").Join(_cardPanel);

            new PlyInfoCard(GetHJSON("ManaCost"),
                () => $"{MathF.Round(_player.manaCost * 100f)}%",
                "Magic").Join(_cardPanel);

            new PlyInfoCard(GetHJSON("SummonDamage"),
                () => $"{GetDamage(DamageClass.Summon)}%",
                "Summon").Join(_cardPanel);

            new PlyInfoCard(GetHJSON("MaxMinions"),
                () => $"{_player.slotsMinions}/{_player.maxMinions}", "Summon").Join(_cardPanel);

            new PlyInfoCard(GetHJSON("MaxTurrets"),
                () => $"{Main.projectile.Count(proj => proj.active && proj.owner == _player.whoAmI && proj.WipableTurret)}/{_player.maxTurrets}",
                "Summon").Join(_cardPanel);

            new PlyInfoCard(GetHJSON("Endurance"),
                () => $"{MathF.Round(_player.endurance * 100f)}%",
                "Endurance3").Join(_cardPanel);

            new PlyInfoCard(GetHJSON("FishingSkill"),
                () => $"{_player.fishingSkill}",
                "FishingSkill").Join(_cardPanel);

            new PlyInfoCard(GetHJSON("Luck"),
                () => $"{_player.luck}", "Luck").Join(_cardPanel);

            new PlyInfoCard(GetHJSON("Aggro"),
                () => $"{_player.aggro}", "Aggro").Join(_cardPanel);

            // tipPanel.Append(new PlyInfoCard(PlyInfo("WingTime")}:", () => $"{MathF.Round((player.wingTime + player.rocketTime * 6) / 60f, 2)}s", "Flying").Join(_cardPanel);
            // new PlyInfoCard(GetHJSON("WingTimeMax"),
            //     () => $"{MathF.Round((_player.wingTimeMax + _player.rocketTimeMax * 6) / 60f, 2)}s", "Flying").Join(_cardPanel);

            _openTimer.OnOpened += () => { _cardPanel.OverflowHidden = false; };
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_mainPanel.IsMouseHovering)
            {
                PlayerInput.LockVanillaMouseScroll("ImproveGame: PlayerInfo GUI");
                Main.LocalPlayer.mouseInterface = true;
            }

            // 动画开/关
            if (Main.playerInventory)
                _invTimer.Open();
            else
                _invTimer.Close();
            if (_expanded)
                _openTimer.Open();
            else
                _openTimer.Close();
            _invTimer.Update();
            _openTimer.Update();

            UpdateAnimation();
        }

        private void UpdateAnimation()
        {
            bool recalculate = false;

            if (Opacity.Value != _openTimer.Schedule &&
                _mainPanel.Opacity.SetValue(_openTimer.Schedule))
            {
                recalculate = true;
            }

            _mainPanel.ShadowColor = _mainPanel.BorderColor * _openTimer.Lerp(0.25f, 0.5f);
            _mainPanel.ShadowThickness = 20f; // = _openTimer.Lerp(20f, 40f);
            float cardHeight = _openTimer.Lerp(0, PlyInfoCard.TotalSize(3, 7).Y + _cardPanel.VPadding());
            if (Math.Abs(_cardPanel.Height.Pixels - cardHeight) > 0)
            {
                _cardPanel.Height.Pixels = cardHeight;
                recalculate = true;
            }

            float round = _openTimer.Lerp(10f, 0);
            _titleButton.Rounded.Z = _titleButton.Rounded.W = round;

            Vector2 mainPanelSize = new Vector2(_openTimer.Lerp(_titleButton.HPadding() + _titleButton.TextSize.X, _cardPanel.Width.Pixels), _titleButton.Height.Pixels + _cardPanel.Height.Pixels);
            if (_mainPanel.GetInnerSizePixels() != mainPanelSize)
            {
                _mainPanel.SetInnerPixels(mainPanelSize);
                recalculate = true;
            }

            Vector2 endPos = _invTimer.Lerp(_inventoryOpenPos, _inventoryClosePos);

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
        private static string GetHJSON(string str) => GetText($"UI.PlayerInfo.{str}");

        public override bool CanPriority(UIElement target) => target != this;

        public override bool CanDisableMouse(UIElement target)
        {
            return target != this && _mainPanel.IsMouseHovering || _mainPanel.KeepPressed;
        }
    }
}