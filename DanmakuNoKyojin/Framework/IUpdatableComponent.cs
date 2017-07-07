namespace Microsoft.Xna.Framework
{
    /// <summary>
    /// Allows to expose same functionality as GameComponent without inheritance.
    /// </summary>
    public interface IUpdatableComponent
    {
        void Update(GameTime gameTime);
    }
}
