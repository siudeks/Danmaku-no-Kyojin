using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Danmaku
{
    /// <summary>
    /// NaviHub is the Golden Source of data about colission and position of objects. 
    /// If some objects are moving, only NaviHub is able to detect and inform about collision.
    /// 
    /// NaviHub instance needs to be created before any ShipActor instance
    /// because currently ShipActor assumes that request send to BeaconActor will be handled.
    /// 
    /// Purpose of the actor is to act as a clash resolver for objects like ship, bullets etc.
    /// 
    /// Every ship needs to be connected with only one beacon.
    /// 
    /// Initially, only one NaviHub will be included in game. In the future we can
    /// imagine more Hubs, relocated geografically between ships to handle 
    /// big number of ships.
    /// </summary>
    public sealed class NaviHubActor : ReceiveActor
    {
        private List<(IActorRef Ship, ShipStatus Status)> knownShips = new List<(IActorRef, ShipStatus)>();
        private List<(IActorRef Bullet, BulletStatus Status)> knownBullets = new List<(IActorRef Bullet, BulletStatus Status)>();

        /// <summary>
        /// Register a new ship in game loop 
        /// so it can be included in all game processes.
        /// </summary>
        public sealed class RegisterShipCommand
        {
            public ShipStatus CurrentStatus;

            public RegisterShipCommand(ShipStatus current)
            {
                Contract.Requires(current != null);
                CurrentStatus = current;
            }
        }

        /// <summary>
        /// Register a new ship in game loop 
        /// so it can be included in all game processes.
        /// </summary>
        public sealed class RegisterBulletCommand
        {
            public BulletStatus CurrentStatus;

            public RegisterBulletCommand(BulletStatus current)
            {
                Contract.Requires(current != null);
                CurrentStatus = current;
            }
        }

        public sealed class ShipRegisteredEvent { }

        public sealed class BulletRegisteredEvent { }

        public sealed class ShipStatus
        {
            public readonly float positionX;
            public readonly float positionY;

            public ShipStatus(float positionX, float positionY)
            {
                this.positionX = positionX;
                this.positionY = positionY;
            }
        }

        public sealed class BulletStatus
        {
            public readonly float positionX;
            public readonly float positionY;

            public BulletStatus(float positionX, float positionY)
            {
                this.positionX = positionX;
                this.positionY = positionY;
            }
        }

        protected override void PreStart()
        {
            base.PreStart();
            Context.System.EventStream.Subscribe(Self, typeof(RegisterShipCommand));
            Context.System.EventStream.Subscribe(Self, typeof(RegisterBulletCommand));
        }

        protected override void PostStop()
        {
            Context.System.EventStream.Unsubscribe(Self, typeof(RegisterShipCommand));
            Context.System.EventStream.Unsubscribe(Self, typeof(RegisterBulletCommand));
            base.PostStop();
        }

        public NaviHubActor()
        {
            Receive<RegisterShipCommand>(OnRegisterShip);
            Receive<RegisterBulletCommand>(OnRegisterBullet);
            Receive<NaviHubActor.ShipStatus>(OnShipStatusNotification);
        }

        private bool OnShipStatusNotification(NaviHubActor.ShipStatus msg)
        {
            var victims = new List<IActorRef>();

            // if the ship is too close from other ships - all ships needs to be destroyed
            foreach (var item in knownShips)
            {
                if (item.Ship.Equals(Sender)) continue;

                var distanceX = item.Status.positionX - msg.positionX;
                var distanceY = item.Status.positionY - msg.positionY;
                var distance = Math.Sqrt(distanceX * distanceX + distanceY * distanceY);

                // 1000 is not safe distance betwen ships.
                // need to be replaced with real collistion checking
                if ((distance) > 1000) continue;

                victims.Add(item.Ship);
            }

            if (victims.Any()) victims.Add(Sender);
            foreach (var victim in victims)
            {
                victim.Tell(new ShipActor.CollisionDetected());
            }

            return true;
        }

        private bool OnRegisterShip(RegisterShipCommand cmd)
        {
            knownShips.Add((Sender, cmd.CurrentStatus));

            Sender.Tell(new ShipRegisteredEvent());
            return true;
        }

        private bool OnRegisterBullet(RegisterBulletCommand cmd)
        {
            knownBullets.Add((Sender, cmd.CurrentStatus));

            Sender.Tell(new BulletRegisteredEvent());
            return true;
        }

        public static Props Props() => Akka.Actor.Props.Create(() => new NaviHubActor());
    }
}
