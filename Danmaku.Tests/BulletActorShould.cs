using Akka.Actor;
using Akka.TestKit.Xunit2;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Danmaku
{
    public class BulletActorShould : TestKit
    {
        [Fact(Skip ="needs a fix")]
        public async Task Move()
        {
            var props = Props.Create(() => new BulletActor(new Vector2(1, 2), new Vector2(3, 4), new Vector2(5, 6), 0, 0));
            var actorRef = ActorOfAsTestActorRef<BulletActor>(props);

            Sys.EventStream.Publish(new GameTime(TimeSpan.FromSeconds(2)));

            var expectedX = 1 + 3 * 5 * 2;
            var expectedY = 2 + 4 * 6 * 2;
            var expected = new Vector2(expectedX, expectedY);
            Assert.Equal(expected, actorRef.UnderlyingActor.Position);
        }
    }
}
