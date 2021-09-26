using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using App.Lib;
using App.Presenters.Matching;
using App.Views;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace App.Presenters
{
    [RootSceneName("Title")]
    public class TitleRootPresenter : RootPresenterBase<TitleRootPresenter>
    {
        protected override UniTask OnLoadAsync(CancellationToken cancellationToken)
        {
            var obj = SceneManager.GetActiveScene().GetRootGameObjects();
            var rootView = obj.FirstOrDefault(x => x.GetComponent<TitleRootView>())?.GetComponent<TitleRootView>();
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