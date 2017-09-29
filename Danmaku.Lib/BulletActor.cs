using Akka.Actor;
using System;

namespace Danmaku
{
    public sealed class BulletActor : ReceiveActor
    {
        public bool WaveMode { get; set; }

        private float rotation;
        private float distance;
        private float power;
        private CollisionElements CollisionBoxes;
        private Vector2 direction;
        private Vector2 velocity;
        public Vector2 position;

        public BulletActor(Vector2 position, Vector2 direction, Vector2 velocity, float power, float spriteRadius)
        {
            rotation = (float)Math.Atan2(direction.Y, direction.X) - MathHelper.PiOver2;
            WaveMode = false;
            CollisionBoxes.Add(new CollisionCircle(this, new Vector2(), spriteRadius));
            this.power = power;

            Receive<GameTime>(Update);
        }

        private bool Update(GameTime gameTime)
        {
            if (WaveMode)
            {
                distance += 0.75f;
                direction.X = (float)Math.Cos(distance);
            }

            // interesting feature
            // will test and eihher use or remove later on
            //Rotation = (Rotation + 0.25f) % 360;

            position += direction * velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            return true;
        }
    }
}
