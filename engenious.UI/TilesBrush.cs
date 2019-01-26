﻿using engenious.Graphics;

namespace engenious.UI
{
    public sealed class TilesBrush : Brush
    {
        public Point Offset { get; set; }

        public Texture2D TileTexture { get; set; }

        public override void Draw(SpriteBatch batch, Rectangle area, float alpha)
        {
            Rectangle source = new Rectangle(Offset, area.Size);
            batch.Draw(TileTexture, area, source, Color.White * alpha);
        }
    }
}
