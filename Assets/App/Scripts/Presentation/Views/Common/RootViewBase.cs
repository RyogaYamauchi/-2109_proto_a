using System.Threading;
using App.Presentation;
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
            var presenterBase = _container.Resolve<T>();

            _commonSceneManager.SetStartSceneName(RootSceneName.GetRootSceneName(typeof(T)), presenterBase);
            LoadPresenter(presenterBase).Forget();
        }
        private async UniTask LoadPresenter(RootPresenterBase presenterBase)
        {
            await presenterBase.LoadAsync();
            await presenterBase.DidLoadAsync();
        }
    }
}