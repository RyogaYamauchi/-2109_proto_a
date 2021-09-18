using App.Models;

namespace App.Lib
{
    public abstract class RootPresenterBase : PresenterBase
    {
        GameModel _gameModel = GameModel.Instance;
    }
}