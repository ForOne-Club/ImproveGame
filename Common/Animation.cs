namespace ImproveGame.Common
{
    public enum AnimationState
    {
        Normal,
        Open,
        Close,
    }
    public enum AnimationMode
    {
        Linear,
        Nonlinear,
    }

    public class Animation
    {
        public Texture2D texture;
        public float scale;
        public float Timer;
        public float TimerMax;
        public AnimationState State;
        public AnimationMode Mode;

        public Animation(Texture2D texture, float TimerMax = 100f, float scale = 100f)
        {
            this.texture = texture;
            this.TimerMax = TimerMax;
            this.scale = scale;
            Open();

            Mode = AnimationMode.Nonlinear;
        }

        public void Open()
        {
            Timer = 0;
            State = AnimationState.Open;
        }

        public void Close()
        {
            Timer = TimerMax;
            State = AnimationState.Close;
        }

        public void Update()
        {
            switch (State)
            {
                case AnimationState.Open:
                    if (Mode == AnimationMode.Linear)
                    {
                        Timer += 2;
                    }
                    else if (Mode == AnimationMode.Nonlinear)
                    {
                        Timer += (TimerMax - Timer) / 10;
                    }
                    if (TimerMax - Timer < 1f)
                    {
                        Close();
                    }
                    break;
                case AnimationState.Close:
                    if (Mode == AnimationMode.Linear)
                    {
                        Timer -= 2;
                    }
                    else if (Mode == AnimationMode.Nonlinear)
                    {
                        Timer -= Timer / 10;
                    }
                    if (Timer < 1f)
                    {
                        Open();
                    }
                    break;
            }
        }

        public float GetSize()
        {
            return (Timer / TimerMax) * scale;
        }

        public void DrawSelf(SpriteBatch sb)
        {
            sb.Draw(texture, TransformToUIPosition(Main.MouseScreen), null, Color.White, 0, texture.Size() / 2f, GetSize(), 0, 1f);
        }

        public void Draw()
        {
            Color color = Color.Lerp(Color.Red, Color.White, Timer / TimerMax);
            SpriteBatch sb = Main.spriteBatch;
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp,
                DepthStencilState.None, sb.GraphicsDevice.RasterizerState, null, Main.UIScaleMatrix);
            ModAssets.ItemEffect.Value.Parameters["border"].SetValue(new Color(70, 73, 129).ToVector4());
            ModAssets.ItemEffect.Value.Parameters["borderSize"].SetValue(3);
            ModAssets.ItemEffect.Value.Parameters["background"].SetValue(new Vector4(0, 0, 0, 0));
            ModAssets.ItemEffect.Value.Parameters["imageSize"].SetValue(new Vector2(GetSize()));
            ModAssets.ItemEffect.Value.CurrentTechnique.Passes["TestColor"].Apply();
            DrawSelf(sb);
            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp,
                DepthStencilState.None, sb.GraphicsDevice.RasterizerState, null, Main.UIScaleMatrix);
        }
    }
}
