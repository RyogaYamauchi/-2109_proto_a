using System;
using App.Application;
using App.Lib;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace App.Presentation
{
    public sealed class MainRootView : RootViewBase
    {
        public class Paramater : IParameter
        {
            public int MaxTsumuCount;
            public int MaxHp;
            public int MaxTime;
            public bool IsSingleMode;

            public Paramater(int maxTsumuCount, int maxTime, int maxHp, bool isSingleMode)
            {
                MaxTsumuCount = maxTsumuCount;
                MaxHp = maxHp;
                maxTime = maxTime;
                IsSingleMode = isSingleMode;
            }
        }

        [SerializeField] private Button _button;
        [SerializeField] private Transform _tsumuSpawnRoot;
        [SerializeField] private Transform _tsumuRoot;
        [SerializeField] private Slider _hpSlider;
        [SerializeField] private Slider _enemyHpslider;
        [SerializeField] private GameObject _enemyObject;
        [SerializeField] private GameObject _takeDamageNumRoot;
        [SerializeField] private BattleView _battleView;
        [SerializeField] private TimerView _timerView;
        [SerializeField] private Slider _skillSlider;
        [SerializeField] private Button _goTitleButton;
        [SerializeField] private Image skillMaxImage;
        [SerializeField] private Image _lowHpImage;
        [SerializeField] private Button _winButton;
        private Color skillMaxColor = Color.clear;
        private float skillMaxTime;

        public IObservable<Unit> OnClickSkillAsObservable => _button.OnClickAsObservable().TakeUntilDestroy(this);
        public IObservable<Unit> Win => _winButton.OnClickAsObservable().TakeUntilDestroy(this);

        public IObservable<Unit> OnClickGoTitleButtonAsObservable =>
            _goTitleButton.OnClickAsObservable().TakeUntilDestroy(this);
        
        public void Initialize(bool paramIsSingleMode)
        {
            _goTitleButton.gameObject.SetActive(paramIsSingleMode);
        }

        public void SetActiveGoTitleButton(bool state)
        {
            _goTitleButton.gameObject.SetActive(state);
        }

        public void SetParentTsumu(TsumuView tsumuView)
        {
            tsumuView.transform.SetParent(_tsumuRoot, false);
        }

        public Vector2 GetSpawnRootPosition()
        {
            return _tsumuSpawnRoot.position;
        }

        public void SetHp(PlayerViewModel viewModel)
        {
            _hpSlider.minValue = 0;
            _hpSlider.maxValue = viewModel.MaxHealth;
            _hpSlider.value = viewModel.Health;
        }

        public Vector3 GetEnemyPosition()
        {
            return _enemyObject.transform.position;
        }

        public void SetParentTakeDamageNum(TsumuAttackNumView view)
        {
            view.transform.SetParent(_takeDamageNumRoot.transform, false);
        }

        public void SetSkillValue(SkillPointViewModel viewModel)
        {
            _skillSlider.minValue = 0;
            _skillSlider.maxValue = viewModel.MaxValue;
            _skillSlider.value = viewModel.Value;
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
        
        public TimerView GetTimerView()
        {
            return _timerView;
        }
        
        public void SetEnemyHp(EnemyViewModel viewModel)
        {
            _enemyHpslider.minValue = 0;
            _enemyHpslider.maxValue = viewModel.MaxHealth;
            _enemyHpslider.value = viewModel.Health;
        }
        
        public void SetHpState(bool flush)
        {
            //画面を赤塗り
            _lowHpImage.color = flush ? new Color(0.5f, 0f, 0f, 0.5f) : Color.clear;
            _lowHpImage.DOFade(0, 1).Loops();
        }

        public BattleView GetBattleView()
        {
            return _battleView;
        }
    }
}