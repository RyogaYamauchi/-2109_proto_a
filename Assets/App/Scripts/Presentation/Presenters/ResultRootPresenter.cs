using System.Threading;
using App.Lib;
using UniRx;
using Cysharp.Threading.Tasks;

namespace App.Presentation
{
    [RootSceneName("Result")]
    public class ResultRootPresenter : RootPresenterBase
    {
        protected override UniTask OnLoadAsync(CancellationToken cancellationToken)
        {
            var rootView = GetRootView<ResultRootView>();
            var param = (ResultRootView.Parameter) _parameter;
            var winOrLose = param.IsWinOrLose;
            var view = GetRootView<ResultRootView>();
            view.Initialize(param.IsWinOrLose);
            rootView.OnClickRetry.Subscribe(x =>
            {
                ChangeScene<MatchingRootPresenter>().Forget();
                rootView.PlayBGM(winOrLose);
            });

            rootView.OnClickTitle.Subscribe(x => { ChangeScene<TitleRootPresenter>().Forget(); });
            return base.OnLoadAsync(cancellationToken);
        }
    }
}