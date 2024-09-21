using ImproveGame.UI.MasterControl;
using ImproveGame.UIFramework;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.SUIElements;
using Terraria.GameInput;

namespace ImproveGame.UI;

[AutoCreateGUI(LayerName.Vanilla.RadialHotbars, "License Panel")]
// 没绑定快捷键的时候弹出
public class LicensePanel : BaseBody
{
    public static LicensePanel Instance { get; private set; }
    public LicensePanel() => Instance = this;
    public override bool Enabled { get => Visible; set => Visible = value; }
    public static bool Visible { get; private set; }

    public override bool CanSetFocusTarget(UIElement target)
        => (target != this && MainPanel.IsMouseHovering) || MainPanel.IsLeftMousePressed;

    // 主面板
    public SUIPanel MainPanel;

    // 标题面板
    private View TitlePanel;

    // 拖动条
    private SUIScrollView2 ScrollView { get;  set; }

    public override void OnInitialize()
    {
        int panelWidth = 900;
        int panelHeight = 600;

        // 主面板
        MainPanel = new SUIPanel(UIStyle.PanelBorder, UIStyle.PanelBg)
        {
            Rounded = new Vector4(10f),
            Shaded = true,
            Draggable = true,
            FinallyDrawBorder = true,
            IsAdaptiveHeight = true
        };
        MainPanel.SetPadding(1.5f);
        MainPanel.SetPosPixels(380, 160)
            .SetSizePixels(panelWidth, 0)
            .JoinParent(this);

        TitlePanel = ViewHelper.CreateHead(Color.Black * 0.25f, 45f, 10f);
        TitlePanel.SetPadding(0f);
        TitlePanel.JoinParent(MainPanel);

        // 标题
        var title = new SUIText
        {
            IsLarge = true,
            UseKey = false,
            TextOrKey = "License",
            TextAlign = new Vector2(0f, 0.5f),
            TextScale = 0.45f,
            Height = StyleDimension.Fill,
            Width = StyleDimension.Fill,
            DragIgnore = true,
            Left = new StyleDimension(16f, 0f)
        };
        title.JoinParent(TitlePanel);

        var cross = new SUICross
        {
            HAlign = 1f,
            Rounded = new Vector4(0f, 10f, 0f, 0f),
            CrossSize = 20f,
            CrossRounded = 4.5f * 0.85f,
            Border = 0f,
            BorderColor = Color.Transparent,
            BgColor = Color.Transparent,
        };
        cross.CrossOffset.X = 1f;
        cross.Width.Pixels = 46f;
        cross.Height.Set(0f, 1f);
        cross.OnUpdate += _ =>
        {
            cross.BgColor = cross.HoverTimer.Lerp(Color.Transparent, Color.Black * 0.25f);
        };
        cross.OnLeftMouseDown += (_, _) => Close();
        cross.JoinParent(TitlePanel);

        var bottomArea = new View
        {
            DragIgnore = true,
            RelativeMode = RelativeMode.Vertical,
            OverflowHidden = true,
            HAlign = 0.5f
        };
        bottomArea.SetPadding(12f);
        bottomArea.SetSize(0f, panelHeight, 1f, 0f);
        bottomArea.JoinParent(MainPanel);

        ScrollView = new SUIScrollView2(Orientation.Vertical)
        {
            RelativeMode = RelativeMode.Vertical,
            Spacing = new Vector2(0)
        };
        ScrollView.SetPadding(0f, 0f);
        ScrollView.SetSize(0f, 0, 1f, 1f);
        ScrollView.JoinParent(bottomArea);

        var text = new SUIText
        {
            IsWrapped = true,
            UseKey = false,
            TextOrKey = LicenseText,
            Width = {Percent = 1f},
            RelativeMode = RelativeMode.Vertical
        };
        text.OnRightMouseDown += (_, _) =>
        {
            TrUtils.OpenToURL("https://github.com/ForOne-Club/ImproveGame");
            SoundEngine.PlaySound(SoundID.MenuOpen);
        };
        text.SetPadding(0f, 0f);
        text.JoinParent(ScrollView.ListView);
        text.RecalculateText();
        text.SetInnerPixels(new Vector2(0f, text.TextSize.Y));

        Recalculate();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (!MainPanel.IsMouseHovering)
            return;

        Main.LocalPlayer.mouseInterface = true;

        if (IsMouseHovering)
            PlayerInput.LockVanillaMouseScroll("ImproveGame: License Panel");
    }

    public void Open()
    {
        SoundEngine.PlaySound(SoundID.MenuOpen);
        Visible = true;
    }

    public void Close()
    {
        SoundEngine.PlaySound(SoundID.MenuClose);
        Visible = false;
    }

    private const string LicenseText =
        """
        This project (ImproveGame) is licensed under the MIT License, open source on GitHub (https://github.com/ForOne-Club/ImproveGame).
        Huge thanks to the following open-source projects and their contributors:

        --- tModLoader ---
        https://github.com/tModLoader/tModLoader
        Copyright 2019 tModLoader Team

        Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

        The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

        THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

        --- Which Mod Is This From ---
        https://github.com/gardenappl/WMITF
        MIT License

        Copyright (c) 2017 Yuriy Grishchenko

        Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

        The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

        THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

        --- Coroutine class by ChevyRay ---
        https://github.com/ChevyRay/Coroutines
        MIT License

        Copyright (c) 2017 Chevy Ray Johnston

        Permission is hereby granted, free of charge, to any person obtaining a copy
        of this software and associated documentation files (the "Software"), to deal
        in the Software without restriction, including without limitation the rights
        to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
        copies of the Software, and to permit persons to whom the Software is
        furnished to do so, subject to the following conditions:

        The above copyright notice and this permission notice shall be included in all
        copies or substantial portions of the Software.

        THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
        IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
        FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
        AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
        LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
        OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
        SOFTWARE.

        --- ProjectStarlight.Interchange ---
        https://github.com/ProjectStarlight/ProjectStarlight.Interchange
        MIT License

        Copyright (c) 2021-2023 Starlight River Dev Team

        Permission is hereby granted, free of charge, to any person obtaining a copy
        of this software and associated documentation files (the "Software"), to deal
        in the Software without restriction, including without limitation the rights
        to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
        copies of the Software, and to permit persons to whom the Software is
        furnished to do so, subject to the following conditions:

        The above copyright notice and this permission notice shall be included in all
        copies or substantial portions of the Software.

        THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
        IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
        FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
        AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
        LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
        OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
        SOFTWARE.

        --- Auto Piggy Bank ---
        https://github.com/diniamo/auto-piggy-bank
        MIT License

        Copyright (c) 2023 diniamo

        Permission is hereby granted, free of charge, to any person obtaining a copy
        of this software and associated documentation files (the "Software"), to deal
        in the Software without restriction, including without limitation the rights
        to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
        copies of the Software, and to permit persons to whom the Software is
        furnished to do so, subject to the following conditions:

        The above copyright notice and this permission notice shall be included in all
        copies or substantial portions of the Software.

        THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
        IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
        FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
        AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
        LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
        OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
        SOFTWARE.
        
        
        
        """;
}