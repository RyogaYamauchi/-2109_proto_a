using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace App.Common
{
    public class CommonSceneManager
    {
        private string[] _cantPopSceneNames = {"RootScene"};
        private Stack<string> _sceneStack = new Stack<string>();
        private Scene _currentScene;
        public bool IsStartingFromScript { get; private set; }

        public void SetStartSceneName(string name)
        {
            _sceneStack.Push(name);
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
            _currentScene = scene;
            Debug.Log("画面遷移 : "+_currentScene.name);
        }

        public async UniTask ReplaceSceneAsync(string name)
        {
            var targets = _sceneStack.Where(x => !_cantPopSceneNames.Contains(x));
            var tmp = new Stack<string>(targets.ToArray());
            var top = tmp.Pop();
            _sceneStack = new Stack<string>(tmp.ToArray());
            await PushSceneAsync(name);
            await PopSceneAsync(top);
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