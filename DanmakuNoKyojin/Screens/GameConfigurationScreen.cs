using DanmakuNoKyojin.Controls;
using DanmakuNoKyojin.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DanmakuNoKyojin.Screens
{
    public class GameConfigurationScreen : GameScreen
    {
        #region Field region

        private string _message;

        #endregion

        #region Constructor region

        private IViewportProvider viewport;

        public GameConfigurationScreen(IViewportProvider viewport, GameStateManager manager)
            : base(manager)
        {
            this.viewport = viewport;    
        }

        #endregion

        #region XNA Method region

        public override void Update(GameTime gameTime)
        {
            if (InputHandler.PressedCancel())
                StateManager.ChangeState(GameStateManager.State.TitleScreen);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // spriteBatch.Begin();

            spriteBatch.DrawString(ControlManager.SpriteFont, _message,
                new Vector2(
                    viewport.Width / 2f - ControlManager.SpriteFont.MeasureString(_message).X / 2, 
                    viewport.Height / 2f - ControlManager.SpriteFont.MeasureString(_message).Y / 2), 
                Color.White);

            // spriteBatch.End();
        }

        #endregion
    }
}
