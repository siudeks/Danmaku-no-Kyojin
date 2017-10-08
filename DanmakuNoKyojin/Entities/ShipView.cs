using Akka.Actor;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using DanmakuNoKyojin.Collisions;
using DanmakuNoKyojin.Framework;

namespace DanmakuNoKyojin.Entities
{
    /// <summary>
    /// Visual representation of ShipActor.
    /// </summary>
    public sealed class ShipView : IDisposable, IUpdatablePart, IEntity
    {
        private IActorRef ship;

        // Shield
        private Texture2D _shieldSprite;
        private Vector2 _shieldOrigin;
        private Vector2 origin;
        public float Rotation { get; set; }
        public bool IsAlive { get; set; }
        public bool IsInvincible { get; set; }
        private float _hitboxRadius = (float)Math.PI * 1.5f * 2;

        public CollisionElements CollisionBoxes { get; } = new CollisionElements();

        private Texture2D sprite { get; set; }

        private float rotation;
        private Vector2 position;
        public Vector2 Position { get; set; }


        float IEntity.Rotation => rotation;

        Vector2 IEntity.Position => position;

        Vector2 IEntity.Origin => origin;

        public CollisionCircle _shieldCollisionCircle;

        private readonly ActorSystem system;
        public ShipView(ActorSystem system)
        {
            this.system = system;
        }

        public void LoadContent(IContentLoader contentLoader)
        {
            sprite = contentLoader.Load<Texture2D>("Graphics/Entities/player");
            var size = (sprite.Width, sprite.Height);
            ship = system.ActorOf(Props.Create(() => new Danmaku.ShipActor(new Danmaku.Vector2(position.X, position.Y), size, 1000)));

            _shieldSprite = contentLoader.Load<Texture2D>("Graphics/Entities/shield");
            _shieldOrigin = new Vector2(_shieldSprite.Width / 2f, _shieldSprite.Height / 2f);

            CollisionBoxes.Add(new CollisionCircle(this, new Vector2(sprite.Height / 6f, sprite.Height / 6f), _hitboxRadius / 2f));
            _shieldCollisionCircle = new CollisionCircle(this, Vector2.Zero, _shieldSprite.Width / 2f);
        }

        public void ChangeDirection(Vector2 direction, float rotation)
        {
            ship.Tell(new Danmaku.ShipActor.ChangeDirection(direction.X, direction.Y, rotation));
        }

        public void Update(GameTime gameTime)
        {
            ship.Tell(new Danmaku.UpdateMessage(gameTime.ElapsedGameTime));
        }

        public void Dispose()
        {
            ship.GracefulStop(TimeSpan.FromMilliseconds(100));
        }

        internal void RemoveShield()
        {
            CollisionBoxes.Remove(_shieldCollisionCircle);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // for testing purposes let ask synchronously about ship position
            // later we need to improve it in async way.
            var status = ship
                .Ask<Danmaku.ShipActor.StatusNotification>(new Danmaku.ShipActor.StatusRequest())
                .Result;

            Position = new Vector2(status.PositionX, status.PositionY);
            Rotation = status.Rotation;

            //if (_timeBeforeRespawn.TotalMilliseconds <= 0)
            //{
                spriteBatch.Draw(sprite, Position, null, Color.White, Rotation, origin, 1f, SpriteEffects.None, 0f);

                if (IsInvincible)
                    spriteBatch.Draw(_shieldSprite, Position, null, Color.White, 0f, new Vector2(_shieldSprite.Width / 2f, _shieldSprite.Height / 2f), 1f, SpriteEffects.None, 0f);
            //}

        }
    }
}
