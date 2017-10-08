using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework
{
    /// <summary>
    /// Allows to expose same functionality as GameComponent without inheritance.
    /// </summary>
    public interface IDrawablePart
    {
        void Draw(GameTime gameTime, SpriteBatch spiteBatch);
    }
}
