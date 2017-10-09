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
        [OneTimeSetUp]
        public void Initialize()
        {
            // we need to create a beacon which needs to be required to support 
            // collision check for the nearest ships.
            var beaconProps = Props.Create(() => new BeaconActor());
            var beacon = Sys.ActorOf(beaconProps);
        }

        [Test]
        public async Task Move()
        {
            var size = (1, 1);
            var actor = ActorOfAsTestActorRef<ShipActor>(Props.Create(() => new ShipActor(new Vector2(1, 2), size)));

            Sys.EventStream.Publish(new UpdateMessage(TimeSpan.FromSeconds(2)));

            var status = await actor.Ask<ShipActor.StatusNotification>(new ShipActor.StatusRequest(), new CancellationTokenSource(100).Token);
            Assert.That(status.PositionX, Is.EqualTo(1 + 3 * 2));
            Assert.That(status.PositionY, Is.EqualTo(2 + 4 * 2));
        }

        [Test]
        public void Rotate()
        {
            var size = (1, 1);
            var actor = ActorOfAsTestActorRef<ShipActor>(Props.Create(() => new ShipActor(new Vector2(1, 2), size)));

            Assume.That(
                actor
                .Ask<ShipActor.StatusNotification>(new ShipActor.StatusRequest(), CancellationToken.None)
                .Result.Rotation, Is.EqualTo(0));

            Assert.That(actor
                .Ask<ShipActor.StatusNotification>(new ShipActor.StatusRequest(), CancellationToken.None)
                .Result.Rotation, Is.EqualTo(1));
        }

        [Test]
        public void IntersectWithOtherShip()
        {
            var size = (1, 1);
            var actorProps = Props.Create(() => new ShipActor(Vector2.Zero, size));

            var actor1 = Sys.ActorOf(actorProps);

            // order moving and pass time enough to move actor1 from (0, 0) to (1, 1)
            Sys.EventStream.Publish(new UpdateMessage(TimeSpan.FromSeconds(1)));


            var actor2 = ActorOfAsTestActorRef<ShipActor>(actorProps);

            // the distance between actor1 and actor2 is SQRT(2)
            Assume.That(actor1
                .Ask<ShipActor.StatusNotification>(new ShipActor.StatusRequest())
                .Result
                .IsInvicible, Is.False);

            // let's decrease distance between ships less then 1
            // SQRT(2)-0.5 < 1
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
