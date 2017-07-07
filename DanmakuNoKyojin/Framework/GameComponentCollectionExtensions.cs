namespace Microsoft.Xna.Framework
{
    public static class GameComponentCollectionExtensions
    {
        public static void Add(this GameComponentCollection collection, IUpdatableComponent component, Game game)
        {
            collection.Add(new UpdatableComponentWrapper(component, game));
        }

        private class UpdatableComponentWrapper : GameComponent
        {
            private readonly IUpdatableComponent component;
            public UpdatableComponentWrapper(IUpdatableComponent component, Game game) : base(game)
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
