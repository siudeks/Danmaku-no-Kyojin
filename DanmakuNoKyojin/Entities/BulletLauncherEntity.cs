using System;
using System.Collections.Generic;

namespace DanmakuNoKyojin.Entities
{
    public abstract class BulletLauncherEntity : SpriteEntity
    {
        protected readonly List<BaseBullet> Bullets;
        protected TimeSpan BulletFrequence;

        public List<BaseBullet> GetBullets()
        {
            return Bullets;
        }

        protected BulletLauncherEntity(GameRunner gameRef)
            : base(gameRef)
        {
            Bullets = new List<BaseBullet>();
        }

        protected void AddBullet(BaseBullet bullet)
        {
            Bullets.Add(bullet);
        }
    }
}
