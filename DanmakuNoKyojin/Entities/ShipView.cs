using Akka.Actor;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace DanmakuNoKyojin.Entities
{
    /// <summary>
    /// Visual representation of ShipActor.
    /// </summary>
    public sealed class ShipView : IDisposable, IUpdatablePart
    {
        private readonly IActorRef ship;
        private Vector2 position;

        public ShipView(ActorSystem system, Texture2D sprite)
        {
            var size = (sprite.Width, sprite.Height);
            ship = system.ActorOf(Props.Create(() => new Danmaku.ShipActor(new Danmaku.Vector2(position.X, position.Y), size, 1000)));
        }

        public void ChangeDirection(Vector2 direction)
        {
            ship.Tell(new Danmaku.ShipActor.ChangeDirection(direction.X, direction.Y));
        }

        public void Update(GameTime gameTime)
        {
            ship.Tell(new Danmaku.UpdateMessage(gameTime.ElapsedGameTime));
        }

        public void Dispose()
        {
            ship.GracefulStop(TimeSpan.FromMilliseconds(100));
        }

        public Danmaku.ShipActor.StatusNotification GetStatus()
        {
            var result = ship
                .Ask<Danmaku.ShipActor.StatusNotification>(new Danmaku.ShipActor.StatusRequest())
                .Result;

            Debug.WriteLine($"Position: X:Y={result.PositionX}:{result.PositionY}");
            return result;


        }
    }
}
