using System.Linq;
using App.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace App.Lib
{
    public abstract class RootPresenterBase<P> : PresenterBase
    {
        protected GameModel _gameModel = GameModel.Instance;
        protected IParameter _parameter;


        protected T GetRootView<T>() where T : RootViewBase
        {
            var scene = _commonSceneManager.GetCurrentScene();
            var obj = scene.GetRootGameObjects();
            return obj.FirstOrDefault(x => x.GetComponent<T>())?.GetComponent<T>();
        }

        public void SetParameter(IParameter parameter)
        {
            _parameter = parameter;
        }
    }
}