﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Danmaku_no_Kyojin.Entities
{
    interface ICollidable
    {
        bool Intersects(Entity entity);
    }
}
