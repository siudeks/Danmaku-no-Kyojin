namespace Microsoft.Xna.Framework
{
    /// <summary>
    /// Allows to expose same functionality as GameComponent without inheritance.
    /// </summary>
    public interface IUpdatablePart
    {
        void Update(GameTime gameTime);
    }
}
