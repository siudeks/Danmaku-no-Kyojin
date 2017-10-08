using DanmakuNoKyojin.Framework;

namespace Microsoft.Xna.Framework
{
    /// <summary>
    /// Allows to expose same functionality as GameComponent without inheritance.
    /// </summary>
    public interface IContentBasedPart
    {
        void LoadContent(IContentLoader provider);
    }
}
