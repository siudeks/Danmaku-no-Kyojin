namespace Microsoft.Xna.Framework
{
    public static class GameComponentCollectionExtensions
    {
        public static void Add(this GameComponentCollection collection, IDrawableComponent component, Game game)
        {
            collection.Add(new UpdatableComponentWrapper(component, game));
        }

        private class UpdatableComponentWrapper : GameComponent
        {
            private readonly IDrawableComponent component;
            public UpdatableComponentWrapper(IDrawableComponent component, Game game) : base(game)
            {
                this.component = component;
            }

            public override void Update(GameTime gameTime)
            {
                component.Update(gameTime);
            }
        }
    }
}
