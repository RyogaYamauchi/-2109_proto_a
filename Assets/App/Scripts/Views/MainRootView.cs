using System;
using App.Lib;
using App.Presenters;
using App.Skills;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace App.Views
{
    [RootSceneName(("MainScene"))]
    public sealed class MainRootView : RootViewBase
    {
        public class Paramater : IParameter
        {
            public int MaxTsumuCount;
            public int MaxHp;

            public Paramater(int maxTsumuCount, int maxHp)
            {
                MaxTsumuCount = maxTsumuCount;
                MaxHp = maxHp;
            }
        }
        [SerializeField] private Button _button;
        [SerializeField] private Transform _tsumuSpawnRoot;
        [SerializeField] private Transform _tsumuRoot;
        [SerializeField] private Slider _hpSlider;

        [SerializeField] private TimerView timerView;
        [SerializeField] private BattleView battleView;

        public IObservable<Unit> OnClickSkillAsObservable => _button.OnClickAsObservable().TakeUntilDestroy(this);

        // デバッグ機能、シーン単体で起動できる
        private void Start()
        {
            if (!IsLoading && !IsLoaded)
            {
                // デバッグではDeleteLineSkillを使用
                Debug.Log("OnPlayDebug");
                var presenter = new TsumuRootPresenter(this, new Paramater(30, 300));
                presenter.Initialize();
                
                var battlePresenter = new BattlePresenter(presenter, timerView, battleView);
                battlePresenter.Initialize();
            }
        }

        public override UniTask OnLoadAsync()
        {
            var presenter = new TsumuRootPresenter(this, Parameter);
            presenter.Initialize();
            
            var battlePresenter = new BattlePresenter(presenter, timerView, battleView);
            battlePresenter.Initialize();
            return base.OnLoadAsync();
        }
        
        public void SetParentTsumu(TsumuView tsumuView)
        {
            tsumuView.transform.SetParent(_tsumuRoot, false);
        }

        public Vector2 GetSpawnRootPosition()
        {
            return _tsumuSpawnRoot.position;
        }

        public void SetHp(float value, float maxValue)
        {
            _hpSlider.minValue = 0;
            _hpSlider.maxValue = maxValue;
            _hpSlider.value = value;
        }
    }
}