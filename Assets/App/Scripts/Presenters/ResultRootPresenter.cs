using App.Lib;
using App.Presenters.Matching;
using App.Views;
using UniRx;
using Cysharp.Threading.Tasks;

namespace App.Presenters
{
    [RootSceneName("Result")]
    public class ResultRootPresenter : RootPresenterBase<ResultRootPresenter>
    {
        public ResultRootPresenter(ResultRootView rootView, bool winOrLose)
        {
            //view.OnClickTitle.Subscribe(x => ChangeScene<Title>);
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