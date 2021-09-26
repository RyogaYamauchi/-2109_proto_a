using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace App.Common
{
    public class CommonSceneManager
    {
        private string[] _cantPopSceneNames = {"RootScene"};
        private Stack<string> _sceneStack = new Stack<string>();
        public bool IsStartingFromScript { get; private set; }

        public void SetStartSceneName(string name)
        {
            _sceneStack.Push(name);
        }

        public async UniTask PushSceneAsync(string name)
        {
            IsStartingFromScript = true;
            SceneManager.sceneLoaded += (scene, mode) => { SetActiveScene(scene); };
            await SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
            SceneManager.sceneLoaded -= (scene, mode) => { SetActiveScene(scene); };
            _sceneStack.Push(name);
        }

        private void SetActiveScene(Scene scene)
        {
            if (_cantPopSceneNames.Contains(scene.name))
            {
                return;
            }
            SceneManager.SetActiveScene(scene);
        }

        public async UniTask ReplaceSceneAsync(string name)
        {
            var targets = _sceneStack.Where(x => !_cantPopSceneNames.Contains(x));
            var tmp = new Stack<string>(targets.ToArray());
            var top = tmp.Pop();
            await PushSceneAsync(name);
            await PopSceneAsync(top);
        }

        public async UniTask PopSceneAsync(string name)
        {
            await SceneManager.UnloadSceneAsync(name);
        }
    }
}