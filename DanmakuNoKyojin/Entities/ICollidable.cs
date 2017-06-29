using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DanmakuNoKyojin.Entities
{
    interface ICollidable
    {
        bool Intersects(Entity entity);
    }
}
