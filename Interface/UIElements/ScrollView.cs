namespace ImproveGame.Interface.UIElements
{
    public class ScrollView : UIElement
    {
        public ScrollList ScrollList;
        public ModScrollbar Scrollbar;

        public ScrollView(float Width, float Height, UserInterface userInterface, float Padding = 10f, float HSpacing = 10f, float VSpacing = 10f)
        {
            this.Width.Pixels = Width + 30f;
            this.Height.Pixels = Height;
            SetPadding(Padding);

            OverflowHidden = true;

            Scrollbar = new(userInterface)
            {
                VAlign = 0.5f
            };
            Scrollbar.Height.Pixels = this.HeightInside() - 12f;
            Scrollbar.Left.Pixels = this.WidthInside() - 20f;
            Append(Scrollbar);

            ScrollList = new(HSpacing, VSpacing);
            ScrollList.Width.Pixels = Width - Padding * 2f;
            Append(ScrollList);
        }

        public override void ScrollWheel(UIScrollWheelEvent evt)
        {
            base.ScrollWheel(evt);
            Scrollbar.SetViewPosition(evt.ScrollWheelValue);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (IsMouseHovering)
            {
                Main.LocalPlayer.lastMouseInterface = true;
            }
            ScrollList.Top.Pixels = -Scrollbar.ViewPosition;
            ScrollList.Recalculate();
        }

        public void RemoveAllElement()
        {
            ScrollList.RemoveAllChildren();
        }

        public void AppendElement(UIElement uie)
        {
            ScrollList.AppendElement(uie);
            RefreshScrollbar();
            Recalculate();
        }

        public void RefreshScrollbar()
        {
            Scrollbar.SetView(this.Height() - this.VPadding(), ScrollList.Height());
        }
    }

    public class ScrollList : UIElement
    {
        public float HSpacing;
        public float VSpacing;
        public override bool ContainsPoint(Vector2 point) => Parent.IsMouseHovering;

        public ScrollList(float HSpacing, float VSpacing)
        {
            SetPadding(0);
            this.HSpacing = HSpacing;
            this.VSpacing = VSpacing;
        }


        public void AppendElement(UIElement uie)
        {
            float x = 0f;
            float y = 0f;
            if (Elements.Count > 0)
            {
                UIElement end = Elements[^1];
                if (end.Left() + end.Width() + HSpacing + uie.Width() > this.WidthInside())
                {
                    x = 0;
                    y = end.Top() + end.Height() + VSpacing;
                }
                else
                {
                    x = end.Left() + end.Width() + HSpacing;
                    y = end.Top();
                }
            }
            uie.SetPos(x, y);
            Append(uie);
            Height.Pixels = uie.Top() + uie.Height();
        }

        public void SetWidth(float Width)
        {
            this.Width.Pixels = Width;

            for (int i = 0; i < Elements.Count; i++)
            {
                UIElement uie = Elements[i];
                float x = 0f;
                float y = 0f;
                if (i > 0)
                {
                    UIElement end = Elements[i - 1];
                    if (end.Left() + end.Width() + HSpacing + uie.Width() > this.Width())
                    {
                        x = 0;
                        y = end.Top() + end.Height() + VSpacing;
                    }
                    else
                    {
                        x = end.Left() + end.Width() + HSpacing;
                        y = end.Top();
                    }
                }
                uie.SetPos(x, y);
                Height.Pixels = uie.Top() + VSpacing;
            }
        }
    }
}
