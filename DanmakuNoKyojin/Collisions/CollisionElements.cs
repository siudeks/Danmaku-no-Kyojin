using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace DanmakuNoKyojin.Collisions
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
        public void Draw(SpriteBatch sp)
        {
            foreach (var collisionElement in this)
                collisionElement.Draw(sp);
        }
    }

}
