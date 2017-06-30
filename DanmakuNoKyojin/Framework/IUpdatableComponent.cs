namespace Microsoft.Xna.Framework
{
    /// <summary>
    /// Allows to expose same functionality as GameComponent without inheritance.
    /// </summary>
    public interface IDrawableComponent
    {
        void Update(GameTime gameTime);
    }
}
