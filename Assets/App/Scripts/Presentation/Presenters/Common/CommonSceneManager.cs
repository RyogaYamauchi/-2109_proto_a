using System.Collections.Generic;
using System.Linq;
using System.Threading;
using App.Lib;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace App.Presentation
{
    public class CommonSceneManager
    {
        private string[] _cantPopSceneNames = {"RootScene"};
        private Stack<string> _sceneStack = new Stack<string>();
        private DiContainer _container;
        private RootPresenterBase _currentPresenterBase;

        public CommonSceneManager(DiContainer container)
        {
            _container = container;
        }

        public bool IsStartingFromScript { get; private set; }

        public void SetStartSceneName(string name, RootPresenterBase rootPresenterBase)
        {
            _sceneStack.Push(name);
            _currentPresenterBase = rootPresenterBase;
        }

        public async UniTask PushSceneAsync(string name)
        {
            IsStartingFromScript = true;
            SceneManager.sceneLoaded += SetActiveScene;
            await SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
            SceneManager.sceneLoaded -= SetActiveScene;
            _sceneStack.Push(name);
        }

        private void SetActiveScene(Scene scene, LoadSceneMode mode)
        {
            if (_cantPopSceneNames.Contains(scene.name))
            {
                return;
            }

            SceneManager.SetActiveScene(scene);
            Debug.Log("画面遷移 : " + scene.name);
        }

        public async UniTask<T> ReplaceSceneAsync<T>(IParameter parameter) where T : RootPresenterBase
        {
            _currentPresenterBase.Dispose();

            var type = typeof(T);
            var name = RootSceneName.GetRootSceneName(type);

            var targets = _sceneStack.Where(x => !_cantPopSceneNames.Contains(x));
            var tmp = new Stack<string>(targets.ToArray());
            var top = tmp.Pop();
            _sceneStack = new Stack<string>(tmp.ToArray());
            await PushSceneAsync(name);
            await PopSceneAsync(top);

            _currentPresenterBase = _container.Resolve<T>();

            _currentPresenterBase.SetParameter(parameter);
            await _currentPresenterBase.LoadAsync();
            await _currentPresenterBase.DidLoadAsync();
            return (T) _currentPresenterBase;
        }

        public async UniTask PopSceneAsync(string name)
        {
            await SceneManager.UnloadSceneAsync(name);
        }

        public Scene GetCurrentScene()
        {
            return SceneManager.GetActiveScene();
        }
    }
}