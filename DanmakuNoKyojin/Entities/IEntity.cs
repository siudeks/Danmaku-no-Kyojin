using Microsoft.Xna.Framework;

namespace DanmakuNoKyojin.Entities
{
    public interface IEntity
    {
        float Rotation { get; }

        Vector2 Position { get; }

        Vector2 Origin { get; }
    }
}
