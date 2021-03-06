﻿using Akka.Actor;
using System;

namespace Danmaku
{
    public sealed class BulletActor : ReceiveActor
    {
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }

        private Vector2 direction;

        private float distance;
        private float power;
        private CollisionElements CollisionBoxes = new CollisionElements();
        private Vector2 velocity;
        private bool waveMode { get; set; } = false;

        public BulletActor(Vector2 position, Vector2 direction, Vector2 velocity, float power, float spriteRadius)
        {
            this.Position = position;
            this.direction = direction;
            this.velocity = velocity;
            //this.power = power;

            //Rotation = (float)Math.Atan2(direction.Y, direction.X) - MathHelper.PiOver2;
            //CollisionBoxes.Add(new CollisionCircle(this, new Vector2(), spriteRadius));
            //this.power = power;

            Receive<UpdateMessage>(Update);
        }

        protected override void PreStart()
        {
            base.PreStart();
            Context.System.EventStream.Subscribe(Self, typeof(UpdateMessage));

            // inform a beacon about the bullet.
            var status = new NaviHubActor.BulletStatus(Position.X, Position.Y);
            Context.System.EventStream.Publish(new NaviHubActor.RegisterBulletCommand(status));
        }

        protected override void PostStop()
        {
            Context.System.EventStream.Unsubscribe(Self, typeof(UpdateMessage));
            base.PostStop();
        }


        private bool Update(UpdateMessage gameTime)
        {
            if (waveMode)
            {
                distance += 0.75f;
                direction.X = (float)Math.Cos(distance);
            }

            // experimental feature
            // Rotation = (Rotation + 0.25f) % 360;

            Position += direction * velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            return true;
        }
    }
}
