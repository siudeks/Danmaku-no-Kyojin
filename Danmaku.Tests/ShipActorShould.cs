using Akka.Actor;
using Akka.TestKit;
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
            var beaconProps = Props.Create(() => new BeaconActor());
            var beacon = Sys.ActorOf(beaconProps);

            var size = (1, 1);
            var actor = ActorOfAsTestActorRef<ShipActor>(Props.Create(() => new ShipActor(new Vector2(1, 2), size)));

            actor.Tell(new ShipActor.ChangeDirection(3, 4));
            Sys.EventStream.Publish(new UpdateMessage(TimeSpan.FromSeconds(2)));

            var status = await actor.Ask<ShipActor.StatusNotification>(new ShipActor.StatusRequest(), new CancellationTokenSource(100).Token);
            Assert.Equal(status.PositionX, 1 + 3 * 2 * 800);
            Assert.Equal(status.PositionY, 2 + 4 * 2 * 800);
        }

        [Fact]
        public async Task IntersectWithOtherShip()
        {
            // we need to create a beacon which needs to be required to support 
            // collision check for the nearest ships.
            // by default one beacon should be enough.
            var beaconProps = Props.Create(() => new BeaconActor())
                .WithDispatcher(CallingThreadDispatcher.Id);

            var beacon = ActorOfAsTestActorRef<BeaconActor>(beaconProps);

            var size = (1, 1);
            var actorProps = Props.Create(() => new ShipActor(Vector2.Zero, size))
                .WithDispatcher(CallingThreadDispatcher.Id);

            var actor1 = ActorOfAsTestActorRef<ShipActor>(actorProps);
            actor1.Tell(new ShipActor.ChangeDirection(1, 1));

            Sys.EventStream.Publish(new UpdateMessage(TimeSpan.FromSeconds(1)));

            var actor2 = ActorOfAsTestActorRef<ShipActor>(actorProps);


            // the distance between actor1 and actor2 is SQRT(2)
            var ctoken = new CancellationTokenSource(100).Token;
            {
                var status = await actor1.Ask<ShipActor.StatusNotification>(new ShipActor.StatusRequest(), ctoken);
                Assert.False(status.IsInvicible);
            }

            // let's decrease distance between ships less then 1
            // SQRT(2)-0.5 < 1
            actor2.Tell(new ShipActor.ChangeDirection(0.5f, 0.5f));

            {
                var status = await actor1.Ask<ShipActor.StatusNotification>(new ShipActor.StatusRequest(), ctoken);
                Assert.True(status.IsInvicible);
            }
        }
    }
}
