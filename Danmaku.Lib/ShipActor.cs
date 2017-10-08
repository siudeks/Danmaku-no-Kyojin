using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Danmaku
{
    public sealed class ShipActor : ReceiveActor
    {

        public float Velocity = 0;
        public Vector2 Direction = Vector2.Zero;
        public float Rotation;
        public Vector2 Position;
        public bool IsInvicible;
        public (int Width, int Height) SpriteSize;
        public List<IActorRef> listeners = new List<IActorRef>();

        [DebuggerDisplay("X:{X},Y:{Y}")]
        public sealed class ChangeDirection
        {
            public readonly float X;
            public readonly float Y;
            public readonly float Rotation;

            public ChangeDirection(float x, float y, float rotation)
            {
                X = x;
                Y = y;
                Rotation = rotation;
            }
        }

        public sealed class StatusRequest
        {
        }

        public sealed class StatusNotification : IEquatable<StatusNotification>
        {
            public readonly float PositionX;
            public readonly float PositionY;
            public readonly float Rotation;
            public readonly bool IsInvicible;

            public StatusNotification(float positionX, float positionY, float rotation,  bool isInvicible)
            {
                PositionX = positionX;
                PositionY = positionY;
                Rotation = rotation;
                IsInvicible = isInvicible;
            }

            public bool Equals(StatusNotification other)
            {
                if (other == null) return false;
                if (PositionX != other.PositionX) return false;
                if (PositionY != other.PositionY) return false;
                if (Rotation != other.Rotation) return false;
                if (IsInvicible != other.IsInvicible) return false;

                return true;
            }
        }

        public sealed class CollisionDetected
        {

        }

        public ShipActor(Vector2 position, (int Width, int Height) spriteSize, float velocity)
        {
            Position = position;
            SpriteSize = spriteSize;
            Velocity = velocity;

            Initialize();
        }

        private void Initialize()
        {
            Receive<BeaconActor.ShipRegistered>(OnShipRegistered);
            Receive<ChangeDirection>(OnChangeDirection);
            Receive<UpdateMessage>(OnUpdateMessage);
            Receive<StatusRequest>(OnStatusRequest);
            Receive<CollisionDetected>(OnCollisionDetected);
        }

        private bool OnShipRegistered(BeaconActor.ShipRegistered obj)
        {
            listeners.Add(Sender);

            return true;
        }

        private bool OnCollisionDetected(CollisionDetected obj)
        {
            IsInvicible = true;

            CalculateStatusAndNotifyListeners();
            return true;
        }

        protected override void PreStart()
        {
            base.PreStart();
            Context.System.EventStream.Subscribe(Self, typeof(UpdateMessage));

            // inform a beacon about the ship.
            var status = new BeaconActor.ShipStatus(Position.X, Position.Y);
            Context.System.EventStream.Publish(new BeaconActor.RegisterShip(status));
        }

        protected override void PostStop()
        {
            Context.System.EventStream.Unsubscribe(Self, typeof(UpdateMessage));
            base.PostStop();
        }

        StatusNotification lastStatusResponse = null;

        private void CalculateStatusAndNotifyListeners()
        {
            var status = new StatusNotification(Position.X, Position.Y, Rotation, IsInvicible);
            if (status.Equals(lastStatusResponse)) return;

            lastStatusResponse = status;

            foreach (var listener in listeners)
                listener.Tell(new BeaconActor.ShipStatus(status.PositionX, status.PositionY));

        }

        private bool OnStatusRequest(StatusRequest req)
        {
            Sender.Tell(new StatusNotification(Position.X, Position.Y, Rotation, IsInvicible));

            return true;
        }

        private bool OnChangeDirection(ChangeDirection cmd)
        {
            Direction = new Vector2(cmd.X, cmd.Y);
            Rotation = cmd.Rotation;
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

            CalculateStatusAndNotifyListeners();

            return true;
        }
    }
}
