using System.Collections.Generic;

namespace Danmaku
{
    public class CollisionElements : List<CollisionElement>
    {
        public bool Intersects(CollisionElements collisionElements)
        {
            foreach (var collisionElement in this)
            {
                foreach (var element in collisionElements)
                {
                    if (collisionElement.Intersects(element))
                        return true;
                }
            }

            return false;
        }
    }
}