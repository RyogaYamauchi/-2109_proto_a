
using App.Lib;
using App.Presenters.Matching;
using App.View.Matching;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace App.Views
{
    [RootSceneName(("Matching"))]
    public sealed class MatchingRootView : RootViewBase
    {
        [SerializeField] private MatchingStateView matchingStateView;
        // デバッグ機能、シーン単体で起動できる
        private void Start()
        {
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
    }
}