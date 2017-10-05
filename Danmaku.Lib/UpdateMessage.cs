using System;

namespace Danmaku
{
    public sealed class UpdateMessage
    {
        public TimeSpan ElapsedGameTime { get; private set; }

        public UpdateMessage(TimeSpan elapsedGameTime)
        {
            ElapsedGameTime = elapsedGameTime;
        }

    }
}
