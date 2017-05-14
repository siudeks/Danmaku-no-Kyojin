﻿using System;
using Danmaku_no_Kyojin.BulletEngine;
using Danmaku_no_Kyojin.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Danmaku_no_Kyojin.Entities
{
    class Turret : SpriteEntity
    {
        private MoverManager _moverManager;

        private Vector2 _initialPosition;
        private Entity _parent;
        private Entity _target;
        private BulletPattern _pattern;
        private TimeSpan _timer;
        private Color _color;

        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }

        public Vector2 InitialPosition
        {
            get { return _initialPosition; }
        }

        public Turret(
            DnK gameRef, 
            Entity parent, 
            Entity target,
            MoverManager moverManager,
            Vector2 initialPosition, 
            BulletPattern pattern, 
            Color color)
            : base(gameRef)
        {
            _initialPosition = initialPosition;
            X = InitialPosition.X;
            Y = InitialPosition.Y;
            _parent = parent;
            _target = target;
            _moverManager = moverManager;
            _pattern = pattern;
            _timer = new TimeSpan(0, 0, (int)(GameRef.Rand.NextDouble() * 30));
            _color = color;
            Scale = new Vector2(0.5f, 0.5f);
        }

        protected override void LoadContent()
        {
            Sprite = GameRef.Content.Load<Texture2D>("Graphics/Entities/base_turret");
            CollisionBoxes.Add(new CollisionCircle(this, Vector2.Zero, Sprite.Width / 2f));
        }

        public override void Update(GameTime gameTime)
        {
            // Update position according to parent
            UpdatePosition();

            _timer -= gameTime.ElapsedGameTime;

            if (_timer.Milliseconds <= 0)
            {
                var mover = (Mover)_moverManager.CreateBullet();
                mover.X = Position.X;
                mover.Y = Position.Y;
                mover.SetBullet(_pattern.RootNode);

                _timer = new TimeSpan(0, 0, (int)(GameRef.Rand.NextDouble() * 30));
            }

            // Rotate to target
            Rotation = (float)Math.Atan2(Position.Y - _target.Position.Y, Position.X - _target.Position.X) - MathHelper.PiOver2;
        }

        private void UpdatePosition()
        {
            var position = new Vector2(InitialPosition.X, InitialPosition.Y);

            var angleCos = (float)Math.Cos(_parent.Rotation);
            var angleSin = (float)Math.Sin(_parent.Rotation);

            // Translate point back to origin  
            position.X -= _parent.Origin.X;
            position.Y -= _parent.Origin.Y;

            // Rotate point
            var newX = position.X * angleCos - position.Y * angleSin;
            var newY = position.X * angleSin + position.Y * angleCos;

            // Translate point back
            position.X = newX + _parent.Position.X;
            position.Y = newY + _parent.Position.Y;

            Position = position;
        }

        public override void Draw(GameTime gameTime)
        {
            GameRef.SpriteBatch.Draw(Sprite, Position, null, _color, Rotation, Origin, Scale, SpriteEffects.None, 0f);

            base.Draw(gameTime);
        }
    }
}
