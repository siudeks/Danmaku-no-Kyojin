using Akka.Actor;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DanmakuNoKyojin.Collisions;
using DanmakuNoKyojin.Framework;
using System.Diagnostics;

namespace DanmakuNoKyojin.Entities
{
    /// <summary>
    /// Visual representation of ShipActor.
    /// </summary>
    public sealed class ShipView : IDisposable, IUpdatablePart, IEntity
    {
        // remote reference to ship actor.
        // ship actor is the remote part of loginc, where in the cloud is able
        // to watch and resolve interaction between objects in the game.
        private IActorRef ship;

        // Shield
        private Texture2D _shieldSprite;
        private Vector2 _shieldOrigin;
        private Vector2 origin;
        private float rotation;
        private bool isInvincible;
        private float _hitboxRadius = (float)Math.PI * 1.5f * 2;

        private CollisionElements CollisionBoxes { get; } = new CollisionElements();

        private Texture2D sprite;
        private Vector2 position;


        public Vector2 Position => position;
        public float Rotation => (float) rotation;
        float IEntity.Rotation => (float) rotation;
        Vector2 IEntity.Position => position;
        Vector2 IEntity.Origin => origin;

        private CollisionCircle _shieldCollisionCircle;

        private readonly ActorSystem system;
        public ShipView(ActorSystem system)
        {
            this.system = system;
        }

        public void LoadContent(IContentLoader contentLoader)
        {
            sprite = contentLoader.Load<Texture2D>("Graphics/Entities/player");
            var size = (sprite.Width, sprite.Height);
            var pos = (position.X, position.Y);
            ship = system.ActorOf(Props.Create(() => new Danmaku.ShipActor(pos, size)));

            _shieldSprite = contentLoader.Load<Texture2D>("Graphics/Entities/shield");
            _shieldOrigin = new Vector2(_shieldSprite.Width / 2f, _shieldSprite.Height / 2f);

            CollisionBoxes.Add(new CollisionCircle(this, new Vector2(sprite.Height / 6f, sprite.Height / 6f), _hitboxRadius / 2f));
            _shieldCollisionCircle = new CollisionCircle(this, Vector2.Zero, _shieldSprite.Width / 2f);
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
            isInvincible = false;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // for testing purposes let ask synchronously about ship position
            // later we need to improve it in async way.
            var status = ship
                .Ask<Danmaku.ShipActor.StatusNotification>(new Danmaku.ShipActor.StatusRequest())
                .Result;

            position = new Vector2(status.PositionX, status.PositionY);
            rotation = (float) status.Rotation;

            spriteBatch.Draw(sprite, position, null, Color.White, rotation, _shieldOrigin, 1f, SpriteEffects.None, 0f);

            if (isInvincible)
                spriteBatch.Draw(_shieldSprite, position, null, Color.White, 0f, new Vector2(_shieldSprite.Width / 2f, _shieldSprite.Height / 2f), 1f, SpriteEffects.None, 0f);
        }

        internal void EnableShield()
        {
            isInvincible = true;
            CollisionBoxes.Add(_shieldCollisionCircle);
        }

        internal bool Intersects(CollisionElements collisionBoxes)
        {
            return CollisionBoxes.Intersects(collisionBoxes);
        }

        internal void MoveCommand(bool forward, float rotation)
        {
            ship.Tell(new Danmaku.ShipActor.MoveCommand(forward, rotation));
        }
    }
}
