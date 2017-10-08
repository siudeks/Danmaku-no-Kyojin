﻿using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Danmaku
{
    /// <summary>
    /// Important: Beacon instance needs to be created before any ShipActor instance
    /// because currently ShipActor assumes that request send to BeaconActor will be handled.
    /// 
    /// Purpose of the actor is to act as a clash resolver for nearest ships.
    /// Every ship needs to be connected with only one beacon.
    /// 
    /// Initially, only one beacon will be included in game. In the future we can
    /// imagine more beacons, relocated geografically between ships.
    /// </summary>
    public sealed class BeaconActor : ReceiveActor
    {
        private List<(IActorRef Ship, ShipStatus Status)> knownShips = new List<(IActorRef, ShipStatus)>();

        public sealed class RegisterShip
        {
            public ShipStatus CurrentStatus;

            public RegisterShip(ShipStatus current)
            {
                Contract.Requires(current != null);
                CurrentStatus = current;
            }
        }

        public sealed class ShipRegistered { }

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

        protected override void PreStart()
        {
            base.PreStart();
            Context.System.EventStream.Subscribe(Self, typeof(RegisterShip));
        }

        protected override void PostStop()
        {
            Context.System.EventStream.Unsubscribe(Self, typeof(RegisterShip));
            base.PostStop();
        }

        public BeaconActor()
        {
            Receive<RegisterShip>(OnRegisterShip);
            Receive<BeaconActor.ShipStatus>(OnShipStatusNotification);
        }

        private bool OnShipStatusNotification(BeaconActor.ShipStatus msg)
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

        private bool OnRegisterShip(RegisterShip cmd)
        {
            knownShips.Add((Sender, cmd.CurrentStatus));

            Sender.Tell(new ShipRegistered());
            return true;
        }
    }
}