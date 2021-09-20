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
        public class Paramater : IParameter
        {
            public int MaxTsumuCount;
            public int MaxHp;
            public bool IsSingleMode;

            public Paramater(int maxTsumuCount, int maxHp, bool isSingleMode)
            {
                MaxTsumuCount = maxTsumuCount;
                MaxHp = maxHp;
                IsSingleMode = isSingleMode;
            }
        }
        [SerializeField] private Button _button;
        [SerializeField] private Transform _tsumuSpawnRoot;
        [SerializeField] private Transform _tsumuRoot;
        [SerializeField] private Slider _hpSlider;
        [SerializeField] private GameObject _enemyObject;
        [SerializeField] private GameObject _takeDamageNumRoot;

        [SerializeField] private TimerView timerView;
        [SerializeField] private BattleView battleView;
        [SerializeField] private Slider _skillSlider;
        [SerializeField] private Button _goTitleButton;

        [SerializeField] private Image skillMaxImage;
        private Color skillMaxColor = Color.clear;
        private float skillMaxTime;

        public IObservable<Unit> OnClickSkillAsObservable => _button.OnClickAsObservable().TakeUntilDestroy(this);

        public IObservable<Unit> OnClickGoTitleButtonAsObservable =>
            _goTitleButton.OnClickAsObservable().TakeUntilDestroy(this);

        // デバッグ機能、シーン単体で起動できる
        private void Start()
        {
            if (!IsLoading && !IsLoaded)
            {
                SingleModeSetUp();
            }
        }

        private void SingleModeSetUp()
        {
            Debug.Log("OnPlaySingle");
            var presenter = new TsumuRootPresenter(this, new Paramater(30, 300, true));
            presenter.Initialize();
            presenter.SetEvents();
            _goTitleButton.gameObject.SetActive(true);
        }

        public override UniTask OnLoadAsync()
        {
            _goTitleButton.gameObject.SetActive(false);
            var param = (Paramater)Parameter;
            if (param.IsSingleMode)
            {
                SingleModeSetUp();
                return UniTask.CompletedTask;
            }
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

        public Vector3 GetEnemyPosition()
        {
            return _enemyObject.transform.position;
        }

        public void SetParentTakeDamageNum(TsumuAttackNumView view)
        {
            view.transform.SetParent(_takeDamageNumRoot.transform, false);
        }

        public void SetSkillValue(int value, int max)
        {
            _skillSlider.minValue = 0;
            _skillSlider.maxValue = max;
            _skillSlider.value = value;
        }

        public void SetActiveSkillButton(bool state)
        {
            _button.interactable = state;
            if (state)
            {
                SkillMaxFlush();
            }
            else
            {
                SkillMaxStop();
            }
        }

        public void SkillMaxFlush()
        {
            //画面を赤塗り
            skillMaxColor = new Color(1f, 1f, 1f, 1f);
        }

        public void Update()
        {
            //時間経過ごとに透明化
            skillMaxTime += Time.deltaTime;
            if (skillMaxTime >= 1.0f)
            {
                skillMaxTime = 0f;
            }
            skillMaxImage.color = Color.Lerp(skillMaxColor, Color.clear, skillMaxTime);
        }

        public void SkillMaxStop()
        {
            skillMaxColor = Color.clear;
        }
    }
}