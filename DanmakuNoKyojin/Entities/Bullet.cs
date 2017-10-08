using DanmakuNoKyojin.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace DanmakuNoKyojin.Entities
{
    /// <summary>
    /// Represent a bullet visualization
    /// </summary>
    public sealed class BulletView : IEntity
    {
        public bool WaveMode { get; set; }
        public Vector2 Velocity;
        public Vector2 Origin;
        public Vector2 Position;
        public Vector2 Direction;
        public float Rotation;
        public CollisionElements CollisionBoxes { get; } = new CollisionElements();
        private float _distance;
        public float Power;
        public Texture2D Sprite { get; set; }

        float IEntity.Rotation => Rotation;
        Vector2 IEntity.Position => Position;

        Vector2 IEntity.Origin => Origin;

        public BulletView(Texture2D sprite, Vector2 position, Vector2 direction, Vector2 velocity)
        {
            Position = position;
            Sprite = sprite;
            Velocity = velocity;
            Direction = direction;
            Rotation = (float)Math.Atan2(direction.Y, direction.X) - MathHelper.PiOver2;
            _distance = 0;
            WaveMode = false;
            CollisionBoxes.Add(new CollisionCircle(this, Vector2.Zero, sprite.Width / 2f));
            Power = Improvements.ShootPowerData[PlayerData.ShootPowerIndex].Key;
        }

        public void Update(GameTime gameTime)
        {
            if (WaveMode)
            {
                _distance += 0.75f;
                Direction.X = (float)Math.Cos(_distance);
            }

            //Rotation = (Rotation + 0.25f) % 360;

            Position += Direction * Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Sprite, Position, null, Color.White, Rotation, Origin, 1f, SpriteEffects.None, 0f);
        }
    }
}
