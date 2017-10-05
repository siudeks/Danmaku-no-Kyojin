using Akka.Actor;
using System;

namespace Danmaku
{
    public sealed class ShipActor : ReceiveActor
    {

        public float Velocity = 800;
        public Vector2 Direction = Vector2.Zero;
        public Vector2 Position;
        public bool IsInvicible;
        public (int Width, int Height) SpriteSize;
        public IActorRef beacon = ActorRefs.Nobody;

        public sealed class ChangeDirection
        {
            public readonly float X;
            public readonly float Y;

            public ChangeDirection(float x, float y)
            {
                X = x;
                Y = y;
            }
        }

        public sealed class StatusRequest
        {
        }

        public sealed class StatusNotification : IEquatable<StatusNotification>
        {
            public readonly float PositionX;
            public readonly float PositionY;
            public readonly bool IsInvicible;

            public StatusNotification(float positionX, float positionY, bool isInvicible)
            {
                PositionX = positionX;
                PositionY = positionY;
                IsInvicible = isInvicible;
            }

            public bool Equals(StatusNotification other)
            {
                if (other == null) return false;
                if (PositionX != other.PositionX) return false;
                if (PositionY != other.PositionY) return false;
                if (IsInvicible != other.IsInvicible) return false;

                return true;
            }
        }

        public ShipActor(Vector2 position, (int Width, int Height) spriteSize)
        {
            Position = position;
            SpriteSize = spriteSize;

            Initialize();
        }

        private void Initialize()
        {
            Receive<BeaconActor.ShipRegistered>(msg =>
            {
                Become(BeaconFound);
            });

            Receive<ShipActor.FindBeacon>(msg =>
            {
                var currentStatus = new BeaconActor.ShipStatus(Position.X, Position.Y);
                Context.System.EventStream.Publish(new BeaconActor.RegisterShip(currentStatus));

                // just in case beacon is not yet ready - need to try reach it once again
                Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromMilliseconds(10), Self, new FindBeacon(), ActorRefs.Nobody);
                Self.Tell(new FindBeacon());
            });

            Self.Tell(new FindBeacon());
        }

        private void BeaconFound()
        {
            beacon = Sender;

            Receive<ChangeDirection>(OnChangeDirection);
            Receive<UpdateMessage>(OnUpdateMessage);
            Receive<StatusRequest>(OnStatusRequest);
        }

        protected override void PreStart()
        {
            base.PreStart();
            Context.System.EventStream.Subscribe(Self, typeof(UpdateMessage));
        }

        protected override void PostStop()
        {
            Context.System.EventStream.Unsubscribe(Self, typeof(UpdateMessage));
            base.PostStop();
        }

        StatusNotification lastStatusResponse = null;

        private void NotifyStatusListeners()
        {
            var status = new StatusNotification(Position.X, Position.Y, IsInvicible);
            if (status.Equals(lastStatusResponse)) return;

            beacon.Tell(new BeaconActor.ShipStatus(status.PositionX, status.PositionY));
        }

        private bool OnStatusRequest(StatusRequest req)
        {
            Sender.Tell(new StatusNotification(Position.X, Position.Y, IsInvicible));

            return true;
        }

        private bool OnChangeDirection(ChangeDirection cmd)
        {
            Direction = new Vector2(cmd.X, cmd.Y);
            return true;
        }

        private bool OnUpdateMessage(UpdateMessage cmd)
        {
            var dt = (float)cmd.ElapsedGameTime.TotalSeconds;
            var x = Position.X + Direction.X * Velocity * dt;
            var y = Position.Y + Direction.Y * Velocity * dt;

            // do not allow to go outdside of game area
            //x = MathHelper.Clamp(x, SpriteSize.Width / 2f, Config.GameAreaX - SpriteSize.Width / 2f);
            //y = MathHelper.Clamp(y, SpriteSize.Height / 2f, Config.GameAreaY - SpriteSize.Height / 2f);

            Position = new Vector2(x, y);

            return true;
        }

        private class FindBeacon
        {
        }
    }
}
