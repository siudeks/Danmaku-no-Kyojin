using DanmakuNoKyojin.Controls;
using DanmakuNoKyojin.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace DanmakuNoKyojin.Screens
{
    public class KeyboardInputsScreen : GameScreen
    {
        private readonly IViewportProvider viewport;
        private string _title;

        private Texture2D _background;
        private SpriteFont _titleFont;

        private int _menuIndex;

        public KeyboardInputsScreen(IViewportProvider viewport, GameStateManager manager)
            : base(manager)
        {
            this.viewport = viewport;
            _title = "Keyboard";
        }

        #region XNA Method region

        public override void Initialize()
        {
            _menuIndex = 0;

            base.Initialize();
        }

        public override void LoadContent(IContentLoader loader)
        {
            _background = loader.Load<Texture2D>("Graphics/Pictures/background");
            _titleFont = loader.Load<SpriteFont>("Graphics/Fonts/TitleFont");

            base.LoadContent(loader);
        }

        public override void Update(GameTime gameTime)
        {
            if (InputHandler.PressedCancel())
                StateManager.ChangeState(GameStateManager.State.OptionsScreen);

            base.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // GameRef.SpriteBatch.Begin();

            spriteBatch.Draw(_background, new Rectangle(0, 0, Config.Resolution.X, Config.Resolution.Y), Color.Yellow);

            spriteBatch.DrawString(_titleFont, _title,
                new Vector2(
                    viewport.Width / 2f - _titleFont.MeasureString(_title).X / 2 + 5,
                    viewport.Height / 2f - (_titleFont.MeasureString(_title).Y * 2) + 5),
                Color.Black);
            spriteBatch.DrawString(_titleFont, _title,
                new Vector2(
                    viewport.Width / 2f - _titleFont.MeasureString(_title).X / 2,
                    viewport.Height / 2f - (_titleFont.MeasureString(_title).Y * 2)),
                Color.White);

            int i = 0;
            foreach (KeyValuePair<string, Keys> pair in Config.PlayerKeyboardInputs)
            {
                Color textColor = Color.White;

                if (i == _menuIndex)
                    textColor = Color.OrangeRed;

                string text = pair.Key + ": " + pair.Value;

                spriteBatch.DrawString(ControlManager.SpriteFont, text,
                    new Vector2(
                        viewport.Width / 2f - ControlManager.SpriteFont.MeasureString(text).X / 2,
                        viewport.Height / 2f - ControlManager.SpriteFont.MeasureString(text).Y / 2 + 50 * i - 50),
                    textColor);

                i++;
            }

            // spriteBatch.End();

            base.Draw(gameTime, spriteBatch);
        }

        #endregion
    }
}
