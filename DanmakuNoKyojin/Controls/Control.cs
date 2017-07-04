using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace DanmakuNoKyojin.Controls
{
    public abstract class Control
    {
        private Vector2 _position;

        public event EventHandler Selected;

        public string Name { get; set; }

        public string Text { get; set; }

        public Vector2 Size { get; set; }

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                _position.Y = (int)_position.Y;
            }
        }

        public object Value { get; set; }

        public virtual bool HasFocus { get; set; }

        public bool Enabled { get; private set; }

        public bool Visible { get; private set; }

        public bool TabStop { get; private set; }

        private SpriteFont SpriteFont { get; set; }

        private Color Color { get; set; }

        public string Type { get; set; }

        protected Control(bool tabStop)
        {
            TabStop = tabStop;
            Color = Color.White;
            Enabled = true;
            Visible = true;
            SpriteFont = ControlManager.SpriteFont;
        }

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch);
        public abstract void HandleInput(PlayerIndex playerIndex);

        protected virtual void OnSelected(EventArgs e)
        {
            Selected?.Invoke(this, e);
        }
    }
}
