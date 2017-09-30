//using Akka.Actor;
//using System;

//namespace Danmaku
//{

//    /// <summary>
//    /// Single actor responsible to create time updates for game to allow components do logic based on real time.
//    /// </summary>
//    public sealed class GameTimeActor : ReceiveActor
//    {
//        public sealed class HeartbeatMessage
//        {

//        }

//        public GameTimeActor()
//        {
//            Receive<HeartbeatMessage>(OnHeartbeatMessage);
//        }

//        protected override void PreStart()
//        {
//            Context.System.Scheduler.ScheduleTellRepeatedly(TimeSpan.Zero, TimeSpan.FromMilliseconds(100), Self, new HeartbeatMessage(), ActorRefs.NoSender);

//            base.PreStart();
//        }

//        private bool OnHeartbeatMessage(HeartbeatMessage msg)
//        {
//            return true;
//        }
//    }
//}
