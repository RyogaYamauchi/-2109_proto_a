using App.Models;

namespace App.Lib
{
    public abstract class RootPresenterBase : PresenterBase
    {
        protected GameModel _gameModel = GameModel.Instance;
    }
}