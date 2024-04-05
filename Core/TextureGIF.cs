/*
Partly from repository: https://github.com/ProjectStarlight/ProjectStarlight.Interchange
 
This file is part of the library ProjectStarlight.Interchange
which is released under the MIT license.
This is the original LICENSE of that library
 
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
 */

namespace ImproveGame.Core
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
