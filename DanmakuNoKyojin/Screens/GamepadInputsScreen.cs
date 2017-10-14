using DanmakuNoKyojin.Controls;
using DanmakuNoKyojin.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DanmakuNoKyojin.Screens
{
    public class GamepadInputsScreen : GameScreen
    {
        #region Field region

        private string _title;
        private string[] _messages;

        private Texture2D _background;
        private SpriteFont _titleFont;

        #endregion

        private readonly IViewportProvider viewport;

        public GamepadInputsScreen(IViewportProvider viewport, GameStateManager manager)
            : base(manager)
        {
            this.viewport = viewport;

            _title = "Gamepad";
            _messages = new string[]
                {
                    "This functionnality is not implemented yet !",
                    "[Press Escape to go back to the title screen]"
                };
        }

        #region XNA Method region

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

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //GameRef.SpriteBatch.Begin();

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

            for (int i = 0; i < _messages.Length; i++)
            {
                spriteBatch.DrawString(ControlManager.SpriteFont, _messages[i],
                new Vector2(
                    viewport.Width / 2f - ControlManager.SpriteFont.MeasureString(_messages[i]).X / 2,
                    viewport.Height / 2f - ControlManager.SpriteFont.MeasureString(_messages[i]).Y / 2 + 20 * i),
                Color.White);
            }

            //GameRef.SpriteBatch.End();

            base.Draw(gameTime, spriteBatch);
        }

        #endregion
    }
}
