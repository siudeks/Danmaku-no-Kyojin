using Akka.Actor;
using Akka.TestKit.NUnit3;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Danmaku
{
    [TestFixture]
    public sealed class ShipActorShould : TestKit
    {
        [Test]
        public async Task Move()
        {
            var beaconProps = Props.Create(() => new BeaconActor());
            var beacon = Sys.ActorOf(beaconProps);

            var size = (1, 1);
            var velocity = 1000;
            var actor = ActorOfAsTestActorRef<ShipActor>(Props.Create(() => new ShipActor(new Vector2(1, 2), size, velocity)));

            actor.Tell(new ShipActor.ChangeDirection(3, 4));
            Sys.EventStream.Publish(new UpdateMessage(TimeSpan.FromSeconds(2)));

            var status = await actor.Ask<ShipActor.StatusNotification>(new ShipActor.StatusRequest(), new CancellationTokenSource(100).Token);
            Assert.That(status.PositionX, Is.EqualTo(1 + 3 * 2 * velocity));
            Assert.That(status.PositionY, Is.EqualTo(2 + 4 * 2 * velocity));
        }



        [Test]
        public void IntersectWithOtherShip()
        {
            // we need to create a beacon which needs to be required to support 
            // collision check for the nearest ships.
            var beaconProps = Props.Create(() => new BeaconActor());
            var beacon = Sys.ActorOf(beaconProps);

            var size = (1, 1);
            var actorProps = Props.Create(() => new ShipActor(Vector2.Zero, size, 1000));

            var actor1 = Sys.ActorOf(actorProps);

            // order moving and pass time enough to move actor1 from (0, 0) to (1, 1)
            actor1.Tell(new ShipActor.ChangeDirection(1, 1));
            Sys.EventStream.Publish(new UpdateMessage(TimeSpan.FromSeconds(1)));


            var actor2 = ActorOfAsTestActorRef<ShipActor>(actorProps);

            // the distance between actor1 and actor2 is SQRT(2)
            Assume.That(actor1
                .Ask<ShipActor.StatusNotification>(new ShipActor.StatusRequest())
                .Result
                .IsInvicible, Is.False);

            // let's decrease distance between ships less then 1
            // SQRT(2)-0.5 < 1
            actor2.Tell(new ShipActor.ChangeDirection(0.5f, 0.5f));
            Sys.EventStream.Publish(new UpdateMessage(TimeSpan.FromSeconds(1)));

            Within(TimeSpan.FromSeconds(1), () =>
            {
                AwaitAssert(() =>
                {
                    Assert.True(actor1
                        .Ask<ShipActor.StatusNotification>(new ShipActor.StatusRequest())
                        .Result
                        .IsInvicible);

                    Assert.True(actor2
                        .Ask<ShipActor.StatusNotification>(new ShipActor.StatusRequest())
                        .Result
                        .IsInvicible);
                });
            });
        }
    }
}
