using Microsoft.Xna.Framework;
using Ninject;

namespace DanmakuNoKyojin.Framework
{
    public interface IGameComponentPart : IInitializable, IUpdatablePart, IDrawablePart, IContentBasedPart
    {
    }
}
