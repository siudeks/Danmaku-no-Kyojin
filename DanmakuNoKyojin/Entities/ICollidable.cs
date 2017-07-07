namespace DanmakuNoKyojin.Entities
{
    interface ICollidable
    {
        bool Intersects(Entity entity);
    }
}
