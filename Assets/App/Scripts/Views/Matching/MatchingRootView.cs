
using App.Lib;
using App.Presenters.Matching;
using App.View.Matching;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

namespace App.Views
{
    [RootSceneName(("Matching"))]
    public sealed class MatchingRootView : RootViewBase
    {
        [SerializeField] private MatchingStateView matchingStateView;
        [SerializeField] GameObject loadingImage;
        Tweener tweener;
            
        // デバッグ機能、シーン単体で起動できる
        private void Start()
        {
            tweener = loadingImage.transform.DOLocalRotate(new Vector3(0, 0, 180), 1).SetLoops(-1, LoopType.Yoyo);
            Debug.Log("OnPlayDebug");
            var presenter = new MatchingRootPresenter(matchingStateView);
        }
    }
}