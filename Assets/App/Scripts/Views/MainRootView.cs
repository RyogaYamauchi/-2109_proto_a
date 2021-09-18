using App.Lib;
using App.Presenters;
using UnityEngine;

namespace App.Views
{
    [RootSceneName(("MainScene"))]
    public sealed class MainRootView : RootViewBase
    {
        [SerializeField] private Transform _tsumuRoot;

        // Sceneのエントリーポイント
        private void Start()
        {
            var presenter = new TsumuRootPresenter(this);
            presenter.Initialize();
        }

        public void SetParentTsumu(TsumuView tsumuView)
        {
            tsumuView.transform.SetParent(_tsumuRoot, false);
        }
    }
}