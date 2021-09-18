using System;
using App.Lib;
using App.Presenters;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace App.Views
{
    [RootSceneName(("MainScene"))]
    public sealed class MainRootView : RootViewBase
    {
        [SerializeField] private Button _button;
        public class Paramater : IParameter
        {
            public int MaxTsumuCount;

            public Paramater(int maxTsumuCount)
            {
                MaxTsumuCount = maxTsumuCount;
            }
        }
        [SerializeField] private Transform _tsumuRoot;

        public IObservable<Unit> OnClickGoResultAsObservable => _button.OnClickAsObservable().TakeUntilDestroy(this);
        
        // デバッグ機能、シーン単体で起動できる
        private void Start()
        {
            if (!IsLoading || !IsLoaded)
            {
                var presenter = new TsumuRootPresenter(this, new Paramater(30));
                presenter.Initialize();
            }
        }

        protected override UniTask OnLoadAsync()
        {
            var presenter = new TsumuRootPresenter(this, Parameter);
            presenter.Initialize();
            return base.OnLoadAsync();
        }
        
        public void SetParentTsumu(TsumuView tsumuView)
        {
            tsumuView.transform.SetParent(_tsumuRoot, false);
        }
    }
}