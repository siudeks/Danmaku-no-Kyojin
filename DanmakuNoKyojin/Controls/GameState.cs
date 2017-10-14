using DanmakuNoKyojin.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace DanmakuNoKyojin.Controls
{
    public abstract partial class GameState: IGameComponentPart
    {
        public List<IGameComponentPart> Components { get; private set; } = new List<IGameComponentPart>();
        public GameState Tag { get; private set; }

        protected GameStateManager StateManager;

        public GameState(GameStateManager manager)
        {
            StateManager = manager;

            Tag = this;
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach (var component in Components)
            {
                //if (!component.Enabled) continue;

                component.Update(gameTime);
            }
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var component in Components)
            {
                //if (component is DrawableGameComponent drawable)
                //{
                //    if (!drawable.Visible) continue;

                    component.Draw(gameTime, spriteBatch);
                //}
            }
        }

        protected void StateChange(object sender, EventArgs e)
        {
            if (StateManager.CurrentState == Tag)
                Show();
            else
                Hide();
        }

        protected virtual void Show()
        {
            //Visible = true;
            //Enabled = true;

            foreach (var component in Components)
            {
                //component.Enabled = true;
                if (component is DrawableGameComponent drawable)
                    drawable.Visible = true;
            }
        }

        protected virtual void Hide()
        {
            //Visible = false;
            //Enabled = false;

            //foreach (var component in Components)
            //{
            //    component.Enabled = false;
            //    if (component is DrawableGameComponent drawable)
            //        drawable.Visible = false;
            //}
        }

        public virtual void LoadContent(IContentLoader provider)
        {
        }

        public virtual void Initialize()
        {
        }
    }
}
