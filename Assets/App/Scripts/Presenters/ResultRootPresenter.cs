using App.Lib;
using App.Views;
using UniRx;
using Cysharp.Threading.Tasks;

namespace App.Presenters
{
    [RootSceneName("Result")]
    public class ResultRootPresenter : RootPresenterBase
    {
        public ResultRootPresenter(ResultRootView rootView, bool winOrLose)
        {
            var param = (ResultRootView.Parameter) _parameter;
            var view = GetRootView<ResultRootView>();
            view.Initialize(param.IsWinOrLose);
            rootView.OnClickRetry.Subscribe(x =>
            {
                ChangeScene<MatchingRootPresenter>().Forget();
                rootView.PlayBGM(winOrLose);
            });

            rootView.OnClickTitle.Subscribe(x =>
            {
                ChangeScene<TitleRootPresenter>().Forget();
            });
        }
    }
}