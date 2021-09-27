using System.Threading;
using App.Common;
using Cysharp.Threading.Tasks;
using Zenject;

namespace App.Lib
{
    public abstract class RootViewBase : ViewBase
    {
        private DiContainer _container;
        private CommonSceneManager _commonSceneManager;

        [Inject]
        private void Construct(DiContainer container, CommonSceneManager commonSceneManager)
        {
            _container = container;
            _commonSceneManager = commonSceneManager;
        }


        // 全体のRootPresenterを知るRootContextをロードする
        protected async void PlayFromEditor<T>() where T : RootPresenterBase
        {
            if (_commonSceneManager.IsStartingFromScript)
            {
                return;
            }
            await _commonSceneManager.PushSceneAsync("RootScene");
            _commonSceneManager.SetStartSceneName(RootSceneName.GetRootSceneName(typeof(T)));
            LoadPresenter<T>().Forget();
        }
        private async UniTask LoadPresenter<T>() where T : RootPresenterBase
        {
            var presenterBase = _container.Resolve<T>();
            var cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;
            await presenterBase.LoadAsync(token);
            await presenterBase.DidLoadAsync(token);
        }
    }
}