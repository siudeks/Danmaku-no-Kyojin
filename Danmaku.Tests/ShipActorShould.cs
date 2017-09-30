using Akka.Actor;
using Akka.TestKit.Xunit2;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Danmaku
{
    public sealed class ShipActorShould : TestKit
    {
        [Fact]
        public async Task Move()
        {
            var size = (1, 1);
            var actor = ActorOfAsTestActorRef<ShipActor>(Props.Create(() => new ShipActor(new Vector2(1,2), size)));

            actor.Tell(new ShipActor.ChangeDirection(3, 4));
            actor.Tell(new ShipActor.UpdateMessage(TimeSpan.FromSeconds(2)));

            var status = await actor.Ask<ShipActor.StatusResponse>(new ShipActor.StatusRequest(), new CancellationTokenSource(100).Token);
            Assert.Equal(status.PositionX, 1 + 3 * 2 * 800);
            Assert.Equal(status.PositionY, 2 + 4 * 2 * 800);
        }
    }
}
