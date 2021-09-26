using System.Threading;
using Cysharp.Threading.Tasks;
using App.Lib;
using App.Presenters.Matching;
using App.Views;
using UniRx;

namespace App.Presenters
{
    [RootSceneName("Title")]
    public class TitleRootPresenter : RootPresenterBase<TitleRootPresenter>
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
                ChangeScene<MainRootPresenter>(new MainRootView.Paramater(30 , 500, true)).Forget();
            });
            return base.OnLoadAsync(cancellationToken);
        }
    }
}