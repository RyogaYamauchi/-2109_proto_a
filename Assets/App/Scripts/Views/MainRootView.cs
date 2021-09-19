using System;
using App.Lib;
using App.Presenters;
using App.Skills;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace App.Views
{
    [RootSceneName(("MainScene"))]
    public sealed class MainRootView : RootViewBase
    {
        public class Paramater : IParameter
        {
            public int MaxTsumuCount;
            public ISkill Skill; 

            public Paramater(int maxTsumuCount, ISkill skill)
            {
                MaxTsumuCount = maxTsumuCount;
                Skill = skill;
            }
        }
        [SerializeField] private Button _button;
        [SerializeField] private Transform _tsumuSpawnRoot;
        [SerializeField] private Transform _tsumuRoot;

        public IObservable<Unit> OnClickSkillAsObservable => _button.OnClickAsObservable().TakeUntilDestroy(this);
        
        // デバッグ機能、シーン単体で起動できる
        private void Start()
        {
            if (!IsLoading && !IsLoaded)
            {
                // デバッグではDeleteLineSkillを使用
                Debug.Log("OnPlayDebug");
                var presenter = new TsumuRootPresenter(this, new Paramater(30, new DeleteLineSkill()));
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

        public Vector2 GetSpawnRootPosition()
        {
            return _tsumuSpawnRoot.position;
        }
    }
}