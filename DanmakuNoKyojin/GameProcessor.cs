using DanmakuNoKyojin.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ninject;

namespace DanmakuNoKyojin
{
    /// <summary>
    /// Main game executor.
    /// 
    /// Handles game events and distribute them to parts of the game.
    /// Responsibility of the class is similar to XNA Game, but it has been created with testability in mind.
    /// </summary>
    public sealed class GameProcessor
    {

        [Inject] public IUpdatablePart[] UpdatableParts { private get; set; }
        [Inject] public IDrawablePart[] DrawableParts { private get; set; }
        [Inject] public IContentBasedPart[] ContentBasedParts { private get; set; }

        public void LoadContent(IContentLoader provider)
        {
            foreach (var item in ContentBasedParts)
                item.LoadContent(provider);
        }

        public void Update(GameTime gameTime)
        {
            foreach (var item in UpdatableParts)
                item.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            foreach (var item in DrawableParts)
                item.Draw(gameTime, spriteBatch);

            spriteBatch.End();
        }
    }
}
