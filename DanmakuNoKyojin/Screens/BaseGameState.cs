﻿using DanmakuNoKyojin.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DanmakuNoKyojin.Screens
{
    public abstract partial class BaseGameState : GameState
    {
        protected GameProcessor GameRef;

        protected ControlManager ControlManager;

        public BaseGameState(Game game, GameStateManager manager)
            : base(game, manager)
        {
            GameRef = (GameProcessor)game;

            var controlFont = GameRef.Content.Load<SpriteFont>("Graphics/Fonts/ControlFont");
            ControlManager = new ControlManager(controlFont);
        }
    }
}
