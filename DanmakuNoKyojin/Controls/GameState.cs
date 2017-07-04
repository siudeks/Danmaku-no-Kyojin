using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace DanmakuNoKyojin.Controls
{
    public abstract partial class GameState : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public List<GameComponent> Components { get; private set; } = new List<GameComponent>();
        public GameState Tag { get; private set; }

        protected GameStateManager StateManager;

        public GameState(Game game, GameStateManager manager)
            : base(game)
        {
            StateManager = manager;

            Tag = this;
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var component in Components)
            {
                if (!component.Enabled) continue;

                component.Update(gameTime);
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var component in Components)
            {
                if (component is DrawableGameComponent drawable)
                {
                    if (!drawable.Visible) continue;

                    drawable.Draw(gameTime);
                }
            }

            base.Draw(gameTime);
        }

        internal protected virtual void StateChange(object sender, EventArgs e)
        {
            if (StateManager.CurrentState == Tag)
                Show();
            else
                Hide();
        }

        protected virtual void Show()
        {
            Visible = true;
            Enabled = true;

            foreach (var component in Components)
            {
                component.Enabled = true;
                if (component is DrawableGameComponent drawable)
                    drawable.Visible = true;
            }
        }

        protected virtual void Hide()
        {
            Visible = false;
            Enabled = false;

            foreach (GameComponent component in Components)
            {
                component.Enabled = false;
                if (component is DrawableGameComponent drawable)
                    drawable.Visible = false;
            }
        }
    }
}
