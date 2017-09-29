namespace Danmaku
{
    public abstract class CollisionElement
    {
        protected object Parent { get; set; }

        public Vector2 RelativePosition { get; set; }

        public abstract bool Intersects(CollisionElement collisionElement);

        public abstract Vector2 GetCenter();

        protected CollisionElement(object parent, Vector2 relativePosition)
        {
            Parent = parent;
            RelativePosition = relativePosition;
        }
    }
}