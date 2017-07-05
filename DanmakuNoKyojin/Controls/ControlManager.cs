using Microsoft.Xna.Framework.Graphics;
using System;

namespace DanmakuNoKyojin.Controls
{
    public class ControlManager
    {
        int selectedControl = 0;
        bool acceptInput = true;

        static SpriteFont spriteFont;

        public static SpriteFont SpriteFont
        {
            get { return spriteFont; }
        }

        public bool AcceptInput
        {
            get { return acceptInput; }
            set { acceptInput = value; }
        }

        public event EventHandler FocusChanged;

        public ControlManager(SpriteFont spriteFont)
            : base()
        {
            ControlManager.spriteFont = spriteFont;
        }
    }
}
