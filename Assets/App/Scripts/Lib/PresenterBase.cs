using System;
using System.Threading;
using App.Common;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace App.Lib
{
    public abstract class PresenterBase : IDisposable
    {
        protected DiContainer _container;
        protected CommonSceneManager _commonSceneManager;

        [Inject]
        public void Construct(DiContainer container, CommonSceneManager commonSceneManager)
        {
            _container = container;
            _commonSceneManager = commonSceneManager;
        }

        public Action OnClosed;

       
        public async UniTask LoadAsync(CancellationToken cancellationToken)
        {
            await OnLoadAsync(cancellationToken);
        }

        public async UniTask DidLoadAsync(CancellationToken cancellationToken)
        {
            await OnDidLoadAsync(cancellationToken);
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

        protected async UniTask<P> ChangeScene<P>(IParameter parameter = null) where  P : RootPresenterBase<P>
        {
            var type = typeof(P);
            var name = RootSceneName.GetRootSceneName(type);
            await _commonSceneManager.ReplaceSceneAsync(name);
            var rootPresenter = _container.Resolve<P>();
            var cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;
            rootPresenter.OnClosed += () => cancellationTokenSource.Cancel();
            rootPresenter.SetParameter(parameter);
            await rootPresenter.LoadAsync(token);
            await rootPresenter.DidLoadAsync(token);
            return rootPresenter;
        }

        public void Dispose()
        {
            OnClosed.Invoke();
        }
    }
}