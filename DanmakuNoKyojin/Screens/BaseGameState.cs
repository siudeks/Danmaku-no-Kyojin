using DanmakuNoKyojin.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DanmakuNoKyojin.Screens
{
    public abstract partial class BaseGameState : GameState
    {
        #region Fields region

        protected GameProcessor GameRef;

        protected ControlManager ControlManager;

        protected SpriteFont BigFont;

        protected PlayerIndex playerIndexInControl;

        #endregion

        #region Properties region
        #endregion

        #region Constructor Region

        public BaseGameState(Game game, GameStateManager manager)
            : base(game, manager)
        {
            GameRef = (GameProcessor)game;

            playerIndexInControl = PlayerIndex.One;

            var controlFont = GameRef.Content.Load<SpriteFont>("Graphics/Fonts/ControlFont");
            ControlManager = new ControlManager(controlFont);
        }

        #endregion

        #region XNA Method Region

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        #endregion

        #region Method Region
        #endregion
    }
}
