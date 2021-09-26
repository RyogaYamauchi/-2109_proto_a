using App.Models;
using UnityEngine.SceneManagement;

namespace App.Lib
{
    public abstract class RootPresenterBase<P> : PresenterBase
    {
        protected GameModel _gameModel = GameModel.Instance;
        protected IParameter _parameter;

        public void SetParameter(IParameter parameter)
        {
            _parameter = parameter;
        }

    }
}