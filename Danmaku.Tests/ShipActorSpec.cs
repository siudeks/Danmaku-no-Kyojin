using Akka.Actor;
using Akka.Event;
using Akka.TestKit.NUnit3;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Danmaku
{
    [TestFixture]
    public sealed class ShipActorSpec : TestKit
    {
        [SetUp]
        public void Initialize()
        {
            // we need to create a beacon which needs to be required to support 
            // collision check for the nearest ships.
            // (nearest from the beacon perspective)
            Sys.ActorOf(NaviHubActor.Props());
        }

        [Test]
        public async Task Move()
        {
            var size = (1, 1);
            var position = (1f, 2f);
            var actor = ActorOfAsTestActorRef<ShipActor>(Props.Create(() => new ShipActor(position, size)));

            actor.Tell(new ShipActor.MoveCommand(true, rotation: Math.PI / 2));
            var time = TimeSpan.FromSeconds(2);
            Sys.EventStream.Publish(new UpdateMessage(time));

            var status = await actor.Ask<ShipActor.StatusNotification>(new ShipActor.StatusRequest(), new CancellationTokenSource(100).Token);


            var a = 500f; // wellKnownShipAcceleration
            var t = time.TotalSeconds;

            Assert.That(status.PositionX, Is.EqualTo(1 + a * t * t / 2));
            Assert.That(status.PositionY, Is.EqualTo(2));
        }

        [Test]
        public void Rotate()
        {
            var size = (1, 1);
            var position = (1f, 2f);
            var actor = ActorOfAsTestActorRef<ShipActor>(Props.Create(() => new ShipActor(position, size)));

            Assume.That(
                actor
                .Ask<ShipActor.StatusNotification>(new ShipActor.StatusRequest(), CancellationToken.None)
                .Result.Rotation, Is.EqualTo(0));

            actor.Tell(new ShipActor.MoveCommand(false, 1));

            Assert.That(actor
                .Ask<ShipActor.StatusNotification>(new ShipActor.StatusRequest(), CancellationToken.None)
                .Result.Rotation, Is.EqualTo(1));
        }

        [Test]
        public void IntersectWithOtherShip()
        {
            var size = (1, 1);
            var position = (0f, 0f);
            var actorProps = Props.Create(() => new ShipActor(position, size));

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

        /// <summary>
        /// Scenario
        ///   Destroy a ship with bullet
        /// When
        ///   Aggressor is directed to a Victim
        ///   and Aggressor shoots
        /// Then
        ///   Victim is destroyed
        /// </summary>
        [Test]
        public void DestroyEnemyShip()
        {
            var size = (1, 1);

            var aggressor = Sys.ActorOf(Props.Create(() => new ShipActor(ValueTuple.Create(0f, 0f), size)));
            var victim = Sys.ActorOf(Props.Create(() => new ShipActor(ValueTuple.Create(0f, 10f), size)));

            // use Watch to be notified about the vistim's dead
            Watch(victim);

            // direct the aggressor to the victim and shoot
            aggressor.Tell(new ShipActor.MoveCommand(false, Math.PI / 2));
            aggressor.Tell(new ShipActor.ShootBulletCommand());

            // because the distance between ships is smaller then 14.14 (10 * SQRT(2))
            // hence 15 secs are enough to find the target
            // assumptions:
            // * the bullet's velocity is 1 
            Sys.EventStream.Publish(new UpdateMessage(TimeSpan.FromSeconds(15)));

            var msg = ExpectMsg<Terminated>();
            Assert.That(msg.ActorRef, Is.EqualTo(victim));
        }
    }
}
