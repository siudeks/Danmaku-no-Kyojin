﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Danmaku_no_Kyojin.Sprites
{
    public class BaseSprite
    {
        #region Field Region

        protected Texture2D texture;
        protected Rectangle sourceRectangle;

        protected Vector2 position;
        protected Vector2 velocity;
        protected float speed = 2.0f;

        #endregion

        #region Property Region

        public int Width
        {
            get { return sourceRectangle.Width; }
        }

        public int Height
        {
            get { return sourceRectangle.Height; }
        }

        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle(
                    (int) position.X,
                    (int) position.Y,
                    Width,
                    Height);
            }
        }

        public float Speed
        {
            get { return speed; }
            set { speed = MathHelper.Clamp(value, 1.0f, 16.0f); }
        }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Vector2 Velocity
        {
            get { return velocity; }
            set
            {
                velocity = value;
                if (velocity != Vector2.Zero)
                    velocity.Normalize();
            }
        }

        #endregion

        #region Constructor Region

        public BaseSprite(Texture2D image, Rectangle? sourceRectangle)
        {
            this.texture = image;

            if (sourceRectangle.HasValue)
                this.sourceRectangle = sourceRectangle.Value;
            else
                this.sourceRectangle = new Rectangle(
                    0,
                    0,
                    image.Width,
                    image.Height);

            this.position = Vector2.Zero;
            this.velocity = Vector2.Zero;
        }

        public BaseSprite(Texture2D image, Rectangle? sourceRectangle, Vector2 position)
            : this(image, sourceRectangle)
        {
            this.position = position;
        }

        #endregion

        #region Method Region

        #endregion

        #region Virtual Method region

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                texture,
                Position,
                sourceRectangle,
                Color.White);
        }

        #endregion
    }
}
