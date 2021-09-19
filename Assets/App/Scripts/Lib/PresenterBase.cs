using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace App.Lib
{
    public abstract class PresenterBase
    {
        private RootViewBase _rootViewBase;
        protected async UniTask<T> CreateViewAsync<T>(Transform parent = null) where T : ViewBase
        {
            var path = PrefabPath.GetPrefabPath(typeof(T));
            var obj = await Resources.LoadAsync<T>(path) as T;
            var instance = Object.Instantiate(obj, parent, false);
            instance.SetLoading(true);
            await instance.LoadAsync();
            await instance.DidLoadAsync();
            instance.SetLoading(false);
            instance.SetLoaded(true);
            return instance;
        }

        protected async UniTask<T> ChangeScene<T>(IParameter parameter = null) where T : RootViewBase
        {
            var type = typeof(T);
            var name = RootSceneName.GetRootSceneName(type);
            SceneManager.sceneLoaded += GetRootViewBase<T>;
            await SceneManager.LoadSceneAsync(name);
            SceneManager.sceneLoaded -= GetRootViewBase<T>;
            var rootViewBase = (T) _rootViewBase;
            rootViewBase.SetParameter(parameter);
            rootViewBase.SetLoading(true);
            await _rootViewBase.LoadAsync();
            await rootViewBase.DidLoadAsync();
            rootViewBase.SetLoading(false);
            rootViewBase.SetLoaded(true);
            return (T) _rootViewBase;
        }

        private void GetRootViewBase<T>(Scene scene, LoadSceneMode mode) where T : RootViewBase
        {
            var gameObject = scene.GetRootGameObjects().FirstOrDefault(x => x.GetComponent<T>() != null);
            if (gameObject == null)
            {
                throw new NullReferenceException("ロードしたシーンのルートに指定の型のオブジェクトがありませんでした");
            }

            _rootViewBase = gameObject.GetComponent<T>();
        }
    }
}