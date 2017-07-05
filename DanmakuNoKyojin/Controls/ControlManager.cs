using Microsoft.Xna.Framework.Graphics;

namespace DanmakuNoKyojin.Controls
{
    public class ControlManager
    {
        static SpriteFont spriteFont;

        public static SpriteFont SpriteFont
        {
            get { return spriteFont; }
        }

        public ControlManager(SpriteFont spriteFont)
            : base()
        {
            ControlManager.spriteFont = spriteFont;
        }
    }
}
