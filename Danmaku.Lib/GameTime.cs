using System;

namespace Danmaku
{
    public sealed class GameTime
    {
        public  TimeSpan ElapsedGameTime { get; private set; }

        public GameTime(TimeSpan elapsedGameTime)
        {
            ElapsedGameTime = elapsedGameTime;
        }
    }
}
