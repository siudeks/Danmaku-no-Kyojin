using System;
using System.Collections.Generic;

namespace Danmaku
{
    public class CollisionCircle : CollisionElement
    {
        public float Radius { get; set; }

        private List<Vector2> _axes;

        public CollisionCircle(BulletActor parent, Vector2 relativePosition, float radius) : base(parent, relativePosition)
        {
            Radius = radius;

            _axes = new List<Vector2>();
        }

        public override bool Intersects(CollisionElement collisionElement)
        {
            if (collisionElement is CollisionCircle)
                return Intersects(collisionElement as CollisionCircle);

            return collisionElement.Intersects(this);
        }

        private bool Intersects(CollisionCircle element)
        {
            float dx = element.GetCenter().X - GetCenter().X;
            float dy = element.GetCenter().Y - GetCenter().Y;
            float radii = Radius + element.Radius;

            if ((dx * dx) + (dy * dy) < radii * radii)
            {
                return true;
            }

            return false;
        }

        public override Vector2 GetCenter()
        {
            return new Vector2(
                Parent.Position.X + RelativePosition.X * (float)(-Math.Sin(Parent.Rotation)),
                Parent.Position.Y + RelativePosition.Y * (float)(Math.Cos(Parent.Rotation))
            );
        }

        public Vector2 Project(Vector2 axis)
        {
            if (!_axes.Contains(axis))
                _axes.Add(axis);

            float a = axis.Y / axis.X;

            float min = Vector2.Dot(new Vector2(GetCenter().X - (float)(Radius * Math.Sin(a)), GetCenter().Y - (float)(Radius * Math.Cos(a))), axis);
            float max = Vector2.Dot(new Vector2(GetCenter().X + (float)(Radius * Math.Sin(a)), GetCenter().Y + (float)(Radius * Math.Cos(a))), axis);

            return new Vector2(min, max);
        }
    }
}
