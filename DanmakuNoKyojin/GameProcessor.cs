using Microsoft.Xna.Framework;
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
        [Inject]
        public IUpdatablePart[] UpdatableParts { private get; set; }

        public void Update(GameTime gameTime)
        {
            foreach (var item in UpdatableParts)
                item.Update(gameTime);
        }
    }
}
