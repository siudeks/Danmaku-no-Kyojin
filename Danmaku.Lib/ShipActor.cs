using Akka.Actor;
using System;
using System.Collections.Generic;

namespace Danmaku
{
    public sealed class ShipActor : ReceiveActor
    {
        // ship acceleration.
        // allows to calculate ship velocity when acceleration is enabled. 
        public float Acceleration = 100;

        public float Velocity = 0;
        public Vector2 Direction = Vector2.Zero;
        public bool Forward;
        public double Rotation;
        public bool IsInvicible;
        public (int Width, int Height) SpriteSize;
        public List<IActorRef> listeners = new List<IActorRef>();

        private (float X, float Y) Position;

        public sealed class MoveCommand
        {
            public readonly bool Forward;
            public readonly double Rotation;

            public MoveCommand(bool forward, double rotation)
            {
                Forward = forward;
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
            public readonly double Rotation;
            public readonly bool IsInvicible;

            public StatusNotification(float positionX, float positionY, double rotation,  bool isInvicible)
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

        public ShipActor((float x, float y) position, (int Width, int Height) spriteSize)
        {
            Position = position;
            SpriteSize = spriteSize;

            Initialize();
        }

        private void Initialize()
        {
            Receive<BeaconActor.ShipRegistered>(OnShipRegistered);
            Receive<MoveCommand>(OnMoveCommand);
            Receive<UpdateMessage>(OnUpdateMessage);
            Receive<StatusRequest>(OnStatusRequest);
            Receive<CollisionDetected>(OnCollisionDetected);
        }

        private bool OnMoveCommand(MoveCommand cmd)
        {
            Forward = cmd.Forward;
            Rotation = cmd.Rotation;
            return true;
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

        private bool OnUpdateMessage(UpdateMessage cmd)
        {
            // temporar trick to accelerate th ship
            if (Forward) Velocity = Velocity + (float) (Acceleration * cmd.ElapsedGameTime.TotalMilliseconds / 1000);

            // temporarly we would like to stop the ship if it is not accelerated.
            if (!Forward) Velocity = Velocity - 1;

            if (Velocity > 1000) Velocity = 1000;
            if (Velocity < 0) Velocity = 0;

            var ms = (float)cmd.ElapsedGameTime.TotalMilliseconds;

            var x = (float) (Position.X + (Math.Sin(Rotation) * Velocity * ms) / 1000f / 2);
            var y = (float) (Position.Y + (-Math.Cos(Rotation) * Velocity * ms) / 1000f / 2);

            // do not allow to go outside of game area
            //x = MathHelper.Clamp(x, SpriteSize.Width / 2f, Config.GameAreaX - SpriteSize.Width / 2f);
            //y = MathHelper.Clamp(y, SpriteSize.Height / 2f, Config.GameAreaY - SpriteSize.Height / 2f);

            Position = (x, y);

            CalculateStatusAndNotifyListeners();

            return true;
        }
    }
}
