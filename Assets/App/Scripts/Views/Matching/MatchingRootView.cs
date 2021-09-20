
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
            if (!IsLoading && !IsLoaded)
            {
                // デバッグではDeleteLineSkillを使用
                Debug.Log("OnPlayDebug");
                var presenter = new MatchingPresenter(matchingStateView);
            }
        }

        public override UniTask OnLoadAsync()
        {
            var presenter = new MatchingPresenter(matchingStateView);
            return base.OnLoadAsync();
        }

        public void Update()
        {
            //loadingImage.transform.DOLocalRotate(new Vector3(0f, 0f, 360f), 5f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1,LoopType.Restart);
        }
    }
}