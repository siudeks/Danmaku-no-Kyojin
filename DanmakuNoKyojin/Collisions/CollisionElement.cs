using System;
using DanmakuNoKyojin.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DanmakuNoKyojin.Collisions
{
    public abstract class CollisionElement
    {
        protected Entity Parent { get; set; }

        public Vector2 RelativePosition { get; set; }

        public abstract bool Intersects(CollisionElement collisionElement);

        public abstract void Draw(SpriteBatch sp);

        public abstract Vector2 GetCenter();

        protected CollisionElement(Entity parent, Vector2 relativePosition)
        {
            Parent = parent;
            RelativePosition = relativePosition;
        }
    }
}
