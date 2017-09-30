using Akka.Actor;
using System;
using System.Diagnostics;

namespace Danmaku
{
    public sealed class ShipActor : ReceiveActor
    {

        public float Velocity = 800;
        public Vector2 Direction = Vector2.Zero;
        public Vector2 Position;
        public (int Width, int Height) SpriteSize;


        public sealed class UpdateMessage
        {
            public TimeSpan ElapsedGameTime { get; private set; }

            public UpdateMessage(TimeSpan elapsedGameTime)
            {
                ElapsedGameTime = elapsedGameTime;
            }

        }

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

        public sealed class StatusResponse
        {
            public readonly float PositionX;
            public readonly float PositionY;

            public StatusResponse(float positionX, float positionY)
            {
                PositionX = positionX;
                PositionY = positionY;
            }
        }

        public ShipActor(Vector2 position, (int Width, int Height) spriteSize)
        {
            Position = position;
            SpriteSize = spriteSize;

            Receive<ChangeDirection>(OnChangeDirection);
            Receive<UpdateMessage>(OnUpdateMessage);
            Receive<StatusRequest>(OnStatusRequest);
        }

        private bool OnStatusRequest(StatusRequest req)
        {
            Sender.Tell(new StatusResponse(Position.X, Position.Y));

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

    }
}
