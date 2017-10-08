using Akka.Actor;
using Akka.TestKit;
using Akka.TestKit.NUnit3;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Danmaku
{
    public class BulletActorShould : TestKit
    {
        [Test]
        public void Move()
        {
            var props = Props
                .Create(() => new BulletActor(new Vector2(1, 2), new Vector2(3, 4), new Vector2(5, 6), 0, 0))
                .WithDispatcher(CallingThreadDispatcher.Id);
            var actorRef = ActorOfAsTestActorRef<BulletActor>(props);

            var expectedX = 1 + 3 * 5 * 2;
            var expectedY = 2 + 4 * 6 * 2;
            var expected = new Vector2(expectedX, expectedY);

            Sys.EventStream.Publish(new UpdateMessage(TimeSpan.FromSeconds(2)));
            Assert.That(expected, Is.EqualTo(actorRef.UnderlyingActor.Position));
        }
    }
}
