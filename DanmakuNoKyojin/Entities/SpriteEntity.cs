﻿using DanmakuNoKyojin.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DanmakuNoKyojin.Entities
{
    public abstract class SpriteEntity : Entity
    {
        private Texture2D _sprite;

        protected SpriteEntity(GameRunner gameRef) : base(gameRef)
        {
        }

        protected Texture2D Sprite
        {
            get { return _sprite; }
            set
            {
                _sprite = value;
                Size = new Point(value.Width, value.Height);
                Origin = new Vector2(value.Width / 2f, value.Height / 2f);
            }
        }
    }
}
