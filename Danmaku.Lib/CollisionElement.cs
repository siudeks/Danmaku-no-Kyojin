namespace Danmaku
{
    public abstract class CollisionElement
    {
        protected BulletActor Parent { get; set; }

        public Vector2 RelativePosition { get; set; }

        public abstract bool Intersects(CollisionElement collisionElement);

        public abstract Vector2 GetCenter();

        protected CollisionElement(BulletActor parent, Vector2 relativePosition)
        {
            Parent = parent;
            RelativePosition = relativePosition;
        }
    }
}