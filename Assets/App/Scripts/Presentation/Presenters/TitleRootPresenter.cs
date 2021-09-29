using System.Threading;
using Cysharp.Threading.Tasks;
using App.Lib;
using UniRx;

namespace App.Presentation
{
    [RootSceneName("Title")]
    public class TitleRootPresenter : RootPresenterBase
    {
        protected override UniTask OnLoadAsync(CancellationToken cancellationToken)
        {
            var rootView = GetRootView<TitleRootView>();
            rootView.OnClickOnline.Subscribe(x =>
            {
                ChangeScene<MatchingRootPresenter>().Forget();
            });

            rootView.OnClickSingle.Subscribe(x =>
            {
                ChangeScene<MainRootPresenter>(new MainRootView.Paramater(30 , 120, 500, true)).Forget();
            });
            return base.OnLoadAsync(cancellationToken);
        }
    }
}