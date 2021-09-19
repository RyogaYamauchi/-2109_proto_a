using App.Lib;
using App.Skills;
using App.Views;
using UniRx;
using Cysharp.Threading.Tasks;

namespace App.Presenters
{
    public class ResultRootPresenter : RootPresenterBase
    {
        public ResultRootPresenter(ResultRootView rootView,bool winOrLose)
        {
            //view.OnClickTitle.Subscribe(x => ChangeScene<Title>);
            rootView.OnClickRetry.Subscribe(x =>
            {
                ChangeScene<MainRootView>(new MainRootView.Paramater(30, 100)).Forget();
            });
            rootView.PlayBGM(winOrLose);
        }
    }
}