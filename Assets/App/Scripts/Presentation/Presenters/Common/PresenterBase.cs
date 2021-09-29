using System;
using System.Threading;
using App.Presentation;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace App.Lib
{
    public abstract class PresenterBase : IDisposable
    {
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        protected CancellationToken _cancellationToken => _cancellationTokenSource.Token;
        protected DiContainer _container;
        protected CommonSceneManager _commonSceneManager;

        [Inject]
        public void Construct(DiContainer container, CommonSceneManager commonSceneManager)
        {
            _container = container;
            _commonSceneManager = commonSceneManager;
        }

        public Action OnClosed;

       
        public async UniTask LoadAsync()
        {
            await OnLoadAsync(_cancellationToken);
        }

        public async UniTask DidLoadAsync()
        {
            await OnDidLoadAsync(_cancellationToken);
        }

        protected virtual UniTask OnLoadAsync(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask OnDidLoadAsync(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        protected async UniTask<T> CreateViewAsync<T>(Transform parent = null) where T : ViewBase
        {
            var path = PrefabPath.GetPrefabPath(typeof(T));
            var obj = await Resources.LoadAsync<T>(path) as T;
            var instance = Object.Instantiate(obj, parent, false);
            instance.SetUp();
            return instance;
        }

        protected async UniTask<T> ChangeScene<T>(IParameter parameter = null) where  T : RootPresenterBase
        {
            return await _commonSceneManager.ReplaceSceneAsync<T>(parameter);
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            OnClosed?.Invoke();
        }
    }
}