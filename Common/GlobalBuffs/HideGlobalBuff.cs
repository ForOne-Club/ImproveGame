using ImproveGame.Common.Configs;
using ImproveGame.Common.GlobalItems;
using ImproveGame.Common.ModSystems;
using ImproveGame.Content.Functions.PortableBuff;
using ImproveGame.UI;
using ImproveGame.UIFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.UI.Gamepad;

namespace ImproveGame.Common.GlobalBuffs
{
    public class HideGlobalBuff : GlobalBuff
    {
        internal static bool IsDrawingBuffTracker;
        internal static bool UseRegularMethod_NoInventory = false;
        internal static bool UseRegularMethod_Inventory = false;

        // 先用IL，如果IL出错了才在本次加载中启用备用方案
        public override void Load() {
            UseRegularMethod_NoInventory = false;
            UseRegularMethod_Inventory = false;
            IL_Main.DrawInventory += TweakDrawInventoryBuffs;
            IL_Main.DrawInterface_Resources_Buffs += TweakDrawInterfaceBuffs;
        }


        // 源码片段
        /*for (int n = 0; n < Player.MaxBuffs; n++) {
	        if (player[myPlayer].buffType[n] != 0) {
		        int num32 = num28 / num29;
		        int num33 = num28 % num29;
		        Point point = new Point(num23 + num32 * -num31, num24 + num33 * num31);
		        num27 = DrawBuffIcon(num27, n, point.X, point.Y);
		        UILinkPointNavigator.SetPosition(9000 + num28, new Vector2(point.X + 30, point.Y + 30));
		        num28++;
		        if (buffAlpha[n] < 0.65f)
			        buffAlpha[n] = 0.65f;
	        }
        }*/
        // 这里num28是拿来定位的，和索引n分开了，虽然不知道为啥，很显然更容易改了
        private void TweakDrawInventoryBuffs(ILContext il) {
            try {
                ILCursor c = new(il);

                #region 获取索引
                //IL_0B55: ldsfld    class Terraria.Player[] Terraria.Main::player
                //IL_0B5A: ldsfld    int32 Terraria.Main::myPlayer
                //IL_0B5F: ldelem.ref
                //IL_0B60: ldfld     int32[] Terraria.Player::buffType
                //IL_0B65: ldloc.s   n
                // 先获取到索引
                int index = -1;
                if (!c.TryGotoNext(MoveType.After,
                                   i => i.MatchLdsfld(typeof(Main), nameof(Main.player)),
                                   i => i.MatchLdsfld(typeof(Main), nameof(Main.myPlayer)),
                                   i => i.MatchLdelemRef(),
                                   i => i.MatchLdfld(typeof(Player), nameof(Player.buffType)),
                                   i => i.Match(OpCodes.Ldloc_S))) {
                    ErrorHappenedInventory();
                    return;
                }

                // 开一个EmitDelegate来获取索引
                c.EmitDelegate<Func<int, int>>(returnValue => {
                    index = returnValue;
                    return returnValue;
                });
                #endregion

                #region 修改绘制坐标
                if (!c.TryGotoNext(MoveType.Before,
                                   i => i.Match(OpCodes.Ldloc_S),
                                   i => i.Match(OpCodes.Ldloc_S),
                                   i => i.Match(OpCodes.Div),
                                   i => i.Match(OpCodes.Stloc_S))) {
                    ErrorHappenedInventory();
                    return;
                }
                c.Index++;
                c.EmitDelegate<Func<int, int>>(x => {
                    if (!UseRegularMethod_Inventory && UIConfigs.Instance.HideNoConsumeBuffs && HideBuffSystem.BuffTypesShouldHide[Main.LocalPlayer.buffType[index]]) {
                        // x设置成-100000
                        return -100000;
                    }
                    return x;
                });

                if (!c.TryGotoNext(MoveType.Before,
                                   i => i.Match(OpCodes.Ldloc_S),
                                   i => i.Match(OpCodes.Ldloc_S),
                                   i => i.Match(OpCodes.Rem),
                                   i => i.Match(OpCodes.Stloc_S))) {
                    ErrorHappenedInventory();
                    return;
                }
                c.Index++;
                c.EmitDelegate<Func<int, int>>(y => {
                    if (!UseRegularMethod_Inventory && UIConfigs.Instance.HideNoConsumeBuffs && HideBuffSystem.BuffTypesShouldHide[Main.LocalPlayer.buffType[index]]) {
                        // y设置成-100000
                        return -100000;
                    }
                    return y;
                });
                #endregion

                #region 修改位置索引添加

                //add
                //conv.r4
                //newobj instance void [FNA]Microsoft.Xna.Framework.Vector2::.ctor(float32, float32)
                //call      void Terraria.UI.Gamepad.UILinkPointNavigator::SetPosition(int32, valuetype[FNA]Microsoft.Xna.Framework.Vector2)
                //ldloc.s   num28
                //ldc.i4.1
                // 修改

                if (!c.TryGotoNext(MoveType.After,
                                       i => i.Match(OpCodes.Add),
                                       i => i.Match(OpCodes.Conv_R4),
                                       i => i.Match(OpCodes.Newobj),
                                       i => i.MatchCall<UILinkPointNavigator>(nameof(UILinkPointNavigator.SetPosition)),
                                       i => i.Match(OpCodes.Ldloc_S),
                                       i => i.Match(OpCodes.Ldc_I4_1))) {
                    ErrorHappenedInventory();
                    return;
                }

                c.EmitDelegate<Func<int, int>>(add => {
                    if (!UseRegularMethod_Inventory && UIConfigs.Instance.HideNoConsumeBuffs && HideBuffSystem.BuffTypesShouldHide[Main.LocalPlayer.buffType[index]]) {
                        // 不让他+1，让他+0
                        return 0;
                    }
                    return add;
                });
                #endregion

            }
            catch (Exception e) {
                ImproveGame.Instance.Logger.Error(e.Message);
                ErrorHappenedInventory();
            }
        }

        private static void ErrorHappenedInventory() {
            Console.WriteLine("Main.DrawInventory IL editing error! Alternative solutions enabled.");
            UseRegularMethod_Inventory = true;
        }


        // 源码片段
        /*
         * for (int i = 0; i < Player.maxBuffs; i++) {
         * 	if (player[myPlayer].buffType[i] > 0) {
         * 		_ = player[myPlayer].buffType[i];
         * 		int x = 32 + i * 38;
         * 		int num3 = 76;
         * 		int num4 = i;
         * 		while (num4 >= num2) {
         * 			num4 -= num2;
         * 			x = 32 + num4 * 38;
         * 			num3 += 50;
         * 		}
         * 		num = DrawBuffIcon(num, i, x, num3);
         * 	}
         * 	else {
         * 		buffAlpha[i] = 0.4f;
         *  }
         * }
         */
        // 在原版代码中，"i"以"ldloc.3"读取，处于一个for循环中，既作为buffType的索引，也用于定位
        // 此处应只修改作为定位的部分，作为索引的部分不修改，不然就乱套了
        private void TweakDrawInterfaceBuffs(ILContext il) {
            try {
                ILCursor c = new(il);

                static int ModifyDrawingIndex(int i, int buffType, bool addCount = false) {
                    if (!UseRegularMethod_NoInventory && UIConfigs.Instance.HideNoConsumeBuffs && HideBuffSystem.BuffTypesShouldHide[buffType]) {
                        // 作为-10000传入
                        if (addCount)
                            HidedBuffCountThisFrame++;
                        return -10000;
                    }
                    return i - HidedBuffCountThisFrame; // 当前的减去需要隐藏的
                }

                // (sbyte)必须强转，不然游戏会以为你传了int，然后崩了

                // 修改第一个
                if (!c.TryGotoNext(MoveType.After,
                                   i => i.Match(OpCodes.Pop),
                                   i => i.Match(OpCodes.Ldc_I4_S, (sbyte)32),
                                   i => i.Match(OpCodes.Ldloc_3))) {
                    ErrorHappenedInterface();
                    return;
                }
                c.EmitDelegate<Func<int, int>>(i => ModifyDrawingIndex(i, Main.LocalPlayer.buffType[i], true));

                // 修改第二个
                if (!c.TryGotoNext(MoveType.After,
                                   i => i.Match(OpCodes.Ldc_I4_S, (sbyte)76),
                                   i => i.Match(OpCodes.Stloc_S),
                                   i => i.Match(OpCodes.Ldloc_3))) {
                    ErrorHappenedInterface();
                    return;
                }
                c.EmitDelegate<Func<int, int>>(i => ModifyDrawingIndex(i, Main.LocalPlayer.buffType[i]));
            }
            catch (Exception e) {
                ImproveGame.Instance.Logger.Error(e.Message);
                ErrorHappenedInterface();
            }
        }

        private static void ErrorHappenedInterface() {
            Console.WriteLine("Main.DrawInterface_Resources_Buffs IL editing error! Alternative solutions enabled.");
            UseRegularMethod_NoInventory = true;
        }

        /// <summary>
        /// 本帧被隐藏的Buff数量，便于后面的Buff重设绘制坐标
        /// </summary>
        internal static int HidedBuffCountThisFrame;

        public override void ModifyBuffText(int type, ref string buffName, ref string tip, ref int rare) {
            if (TryGetKeybindString(KeybindSystem.BuffTrackerKeybind, out _) || IsDrawingBuffTracker)
                return;

            tip += $"\n{GetText($"Tips.BuffTracker{(BuffTrackerGUI.Visible ? "Off" : "On")}")}";
            if (Main.mouseLeft && Main.mouseLeftRelease) {
                if (BuffTrackerGUI.Visible) {
                    UISystem.Instance.BuffTrackerGUI.Close();
                }
                else {
                    UISystem.Instance.BuffTrackerGUI.Open();
                }
            }

            if (!UIConfigs.Instance.HideNoConsumeBuffs)
                tip += $"\n{GetText("Tips.HideMyBuffs")}";
        }

        public override bool PreDraw(SpriteBatch spriteBatch, int type, int buffIndex, ref BuffDrawParams drawParams) {
            if (HideBuffSystem.BuffTypesShouldHide[type]) {
                // 不管咋样都不显示文本
                drawParams.TextPosition = new Vector2(-114514f);
                if (UIConfigs.Instance.HideNoConsumeBuffs)
                {
                    // 干掉指针显示
                    drawParams.MouseRectangle = Rectangle.Empty;
                    if (UseRegularMethod_NoInventory || Main.playerInventory)
                    {
                        HidedBuffCountThisFrame++;
                    }
                    return false;
                }
            }
            if (HidedBuffCountThisFrame > 0) {
                int i = buffIndex - HidedBuffCountThisFrame;
                if (UseRegularMethod_NoInventory) {
                    int x = 32 + i * 38;
                    int y = 76;
                    if (i >= 11) { // 一行
                        x = 32 + Math.Abs(i % 11) * 38;
                        y += 50 * (i / 11);
                    }
                    // 重设各种参数
                    drawParams.Position = new Vector2(x, y);
                    int width = drawParams.Texture.Width;
                    int height = drawParams.Texture.Height;
                    drawParams.TextPosition = new Vector2(x, y + height);
                    drawParams.MouseRectangle = new Rectangle(x, y, width, height);
                }
                // 装备栏下方绘制
                if (Main.playerInventory && UseRegularMethod_Inventory) {
                    int mH = 0;
                    if (Main.mapEnabled && !Main.mapFullscreen && Main.mapStyle == 1) {
                        mH = 256;
                    }
                    if (mH + Main.instance.RecommendedEquipmentAreaPushUp > Main.screenHeight)
                        mH = Main.screenHeight - Main.instance.RecommendedEquipmentAreaPushUp;
                    int num23 = Main.screenWidth - 92;
                    int num24 = mH + 174;
                    num24 += 247;
                    num23 += 8;
                    int num29 = 3;
                    int num30 = 260;
                    if (Main.screenHeight > 630 + num30 * (Main.mapStyle == 1).ToInt())
                        num29++;

                    if (Main.screenHeight > 680 + num30 * (Main.mapStyle == 1).ToInt())
                        num29++;

                    if (Main.screenHeight > 730 + num30 * (Main.mapStyle == 1).ToInt())
                        num29++;

                    int num31 = 46;

                    int num32 = i / num29;
                    int num33 = i % num29;
                    int x = num23 + num32 * -num31;
                    int y = num24 + num33 * num31;
                    // 重设各种参数
                    drawParams.Position = new Vector2(x, y);
                    int width = drawParams.Texture.Width;
                    int height = drawParams.Texture.Height;
                    drawParams.TextPosition = new Vector2(x, y + height);
                    drawParams.MouseRectangle = new Rectangle(x, y, width, height);
                }
            }
            return base.PreDraw(spriteBatch, type, buffIndex, ref drawParams);
        }
    }
}
