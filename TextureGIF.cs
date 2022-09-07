namespace ImproveGame
{
    public class TextureGIF
    {
        public int HorizontalFrames { get; private set; }

        public int VerticalFrames { get; private set; }

        public bool IsPaused { get; private set; }

        public bool HasEnded => FrameIndex >= TotalFrames && FrameTick >= TicksPerFrame && !ShouldLoop;

        public bool ShouldLoop { get; set; }

        public int TicksPerFrame { get; set; }

        public int FrameTick { get; private set; }

        public int FrameIndex { get; private set; }

        public Texture2D Texture { get; private set; }

        public int TotalFrames { get; private set; }

        internal TextureGIF(Texture2D tex, int horizontalFrames, int verticalFrames, int totalFrames, int ticksPerFrame)
        {
            HorizontalFrames = horizontalFrames;
            VerticalFrames = verticalFrames;
            TicksPerFrame = ticksPerFrame;
            FrameIndex = 0;
            Texture = tex;
            TotalFrames = totalFrames;
        }

        public Rectangle GetFrameRectangle =>
            Texture.Frame(HorizontalFrames, VerticalFrames, FrameIndex % HorizontalFrames, FrameIndex / HorizontalFrames);

        public int Width => Texture.Width / HorizontalFrames;

        public int Height => Texture.Height / VerticalFrames;

        public void Play()
        {
            FrameTick = 0;
            FrameIndex = 0;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color) =>
            spriteBatch.Draw(Texture, position, GetFrameRectangle, color);

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color,
            float rotation, Vector2 origin, float scale, SpriteEffects effects) =>
            spriteBatch.Draw(Texture, position, GetFrameRectangle, color, rotation, origin, scale, effects, 0f);

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color,
            float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects) =>
            spriteBatch.Draw(Texture, position, GetFrameRectangle, color, rotation, origin, scale, effects, 0f);

        public void UpdateGIF()
        {
            if (!IsPaused && !HasEnded)
                ForwardTicks(1);
        }

        public void ForwardTicks(int tickAmount)
        {
            for (int i = 0; i < tickAmount; i++)
            {
                FrameTick++;

                if (FrameTick < TicksPerFrame)
                    return;

                FrameTick = 0;

                if (FrameIndex < TotalFrames - 1)
                    FrameIndex++;
                else if (ShouldLoop)
                    FrameIndex = 0;
            }
        }
    }
}
