﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using App.Lib;
using App.Models;
using App.Skills;
using App.Types;
using App.Views;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace App.Presenters
{
    [RootSceneName(("MainScene"))]
    public sealed class MainRootPresenter : RootPresenterBase
    {
        private MainRootView _mainRootView;
        private readonly int _canSelectDistance = 250;
        private readonly Vector2[] _spawnPoint =
        {
            new Vector2(-225, 0), new Vector2(-150, 0), new Vector2(-75, 0),
            new Vector2(0, 0), new Vector2(75, 0), new Vector2(150, 0), new Vector2(225, 0)
        };

        private TsumuRootModel _tsumuRootModel => _gameModel.TsumuRootModel;
        private List<TsumuView> _tsumuViewList = new List<TsumuView>();
        private List<Vector2> _canSpawnTsumuPoints = new List<Vector2>();
        private List<TsumuView> _closingViewList = new List<TsumuView>();
        private int _maxTsumuCount;

        private ISkill _skill;
        
        // 攻撃ツム消した数
        private readonly ReactiveProperty<int> _attackDamageReactiveProperty = new ReactiveProperty<int>(0);
        public IObservable<int> AttackDamageObservable => _attackDamageReactiveProperty;
        
        // 回復ツム消した数
        private readonly ReactiveProperty<int> _healTsumuNumReactiveProperty = new ReactiveProperty<int>(0);
        public IObservable<int> HealTsumuNumObservable => _healTsumuNumReactiveProperty;

        protected override UniTask OnLoadAsync(CancellationToken cancellationToken)
        {
            var param = (MainRootView.Paramater)_parameter;
            _maxTsumuCount = param.MaxTsumuCount;

            _mainRootView = GetRootView<MainRootView>();
            
            _mainRootView.Initialize(param.IsSingleMode);

            _gameModel.TsumuRootModel.Initialize();
            _gameModel.PlayerParameter.Clear();
            _skill = _gameModel.SkillModel.GetRandomSkill();
            _gameModel.SkillModel.Initialize(_skill.GetNeedValue());
            _canSpawnTsumuPoints = new List<Vector2>(_spawnPoint);

            _gameModel.SkillModel.SkillPoint.Subscribe(x =>
            {
                _mainRootView.SetSkillValue(_gameModel.SkillModel.SkillPoint.Value, _gameModel.SkillModel.MaxSkillPoint);
            }).AddTo(_mainRootView);
            _mainRootView.SetActiveSkillButton(false);
            SetEvents();
            return UniTask.CompletedTask;
        }

        public void SetEvents()
        {
            _mainRootView.OnClickSkillAsObservable.Subscribe(x =>
            {
                UseSkillAsync(_skill).Forget();
            });
            _mainRootView.UpdateAsObservable().Subscribe(x =>
            {
                RefillTsumus();
            }).AddTo(_mainRootView);
            _mainRootView.OnClickGoTitleButtonAsObservable.Subscribe(x =>
            {
                ChangeScene<TitleRootPresenter>().Forget();
            });
        }
        
        private void RefillTsumus()
        {
            if (_canSpawnTsumuPoints.Count == 0)
            {
                return;
            }

            if (_tsumuViewList.Count > _maxTsumuCount)
            {
                return;
            }

            SpawnTsumuAsync().Forget();
        }

        private async UniTask UseSkillAsync(ISkill skill)
        {
            _gameModel.SkillModel.ClearSkillPoint();
            _mainRootView.SetActiveSkillButton(false);
            if (skill is IApplyMySelfSkill)
            {
                await skill.ExecuteAsync(this);
            }
            else if (skill is IApplyEnemySkill)
            {
                // 相手の盤面に出したいawait skill
            }
        }
        
        private async UniTask SpawnTsumuAsync()
        {
            var spawnPointY = Screen.height / 2;
            _canSpawnTsumuPoints = _canSpawnTsumuPoints.OrderBy(x => Guid.NewGuid()).ToList();
            var spawnPoint = _canSpawnTsumuPoints.FirstOrDefault();
            _canSpawnTsumuPoints.Remove(spawnPoint);
            var viewModel = _gameModel.TsumuRootModel.SpawnTsumu();
            var tsumuView = await CreateViewAsync<TsumuView>();
            _mainRootView.SetParentTsumu(tsumuView);
            var spawnRootPosition = _mainRootView.GetSpawnRootPosition();
            tsumuView.Initialize(viewModel,  new Vector2(spawnRootPosition.x + spawnPoint.x, spawnPointY));
            tsumuView.OnPointerEnterAsObservable.Subscribe(OnPointerEntertsumu);
            tsumuView.OnPointerDownAsObservable.Subscribe(OnPointerDownTsumu);
            tsumuView.OnPointerUpAsObservable.Subscribe(x => OnPointerUpTsumu(x).Forget());
            _tsumuViewList.Add(tsumuView);
            await UniTask.Delay(500);
            _canSpawnTsumuPoints.Add(spawnPoint);
        }

        private void SelectTsumu(TsumuView view)
        {
            _gameModel.TsumuRootModel.SelectTsumu(view.Guid);
            view.ChangeColor(true);
        }

        private void UnSelectTsumuAll()
        {
            var selectingTsumuList = _gameModel.TsumuRootModel.GetSelectingTsumuIdList();
            var views = _tsumuViewList.Where(view => selectingTsumuList.Any(guid => view.Guid == guid));
            foreach (var view in views)
            {
                view.ChangeColor(false);
            }

            _gameModel.TsumuRootModel.UnSelectTsumuAll();
        }
        
        private async UniTask DespawnTsumuAsync(TsumuView tsumuView, int damage)
        {
            _tsumuViewList.Remove(tsumuView);
            if (tsumuView == null)
            {
                return;
            }
            var pos = tsumuView.transform.position;
            PlayDamageView(pos, tsumuView, damage).Forget();
            await tsumuView.CloseAsync();
        }

        private async UniTask PlayDamageView(Vector3 pos, TsumuView tsumuView, int damage)
        {
            var damageView = await CreateViewAsync<TsumuAttackNumView>();
            _mainRootView.SetParentTakeDamageNum(damageView);
            damageView.transform.position = pos;
            damageView.Initialize(damage);
            await damageView.MoveToTarget(_mainRootView.GetEnemyPosition());
            if (damageView != null)
            {
                damageView.Dispose();
            }
        }

        private void OnPointerEntertsumu(TsumuView view)
        {
            if (!CanSelect(view))
            {
                return;
            }

            SelectTsumu(view);
        }

        private async UniTask OnPointerUpTsumu(TsumuView view)
        {
            if (_tsumuRootModel.GetSelectingTsumuCount()< 3 || _closingViewList.Contains(view))
            {
                UnSelectTsumuAll();
                return;
            }
            
            var ids = _tsumuRootModel.GetSelectingTsumuIdList();
            var chain = ids.Count;
            var views = _tsumuViewList.Where(x => ids.Contains(x.Guid)).ToArray();

            var deleteTsumuList = _gameModel.TsumuRootModel.GetSelectingTsumuIdList()
                .Select(id => _tsumuViewList.FirstOrDefault(x =>id == x.Guid)).ToArray();
            _gameModel.TsumuRootModel.UnSelectTsumuAll();
            _closingViewList.AddRange(deleteTsumuList);
            var sumDamage = 0;
            var c = 0;
            foreach (var tsumu in deleteTsumuList)
            {
                _gameModel.SkillModel.AddSkillPoint(1);
                var damage = _tsumuRootModel.CalcDamage(c, _tsumuRootModel.GetDamage(tsumu.TsumuType));
                sumDamage += damage;
                await DespawnTsumuAsync(tsumu,damage);
                if (_gameModel.SkillModel.CanExecuteSkill(_skill))
                {
                    _mainRootView.SetActiveSkillButton(true);
                }
                _closingViewList.Remove(tsumu);
                c++;
            }
            
            if (views.Any(x => x.TsumuType == TsumuType.Heal))
            {
                _healTsumuNumReactiveProperty.Value = chain;
            }
            else
            {
                _attackDamageReactiveProperty.Value = sumDamage;
            }
        }

        private void OnPointerDownTsumu(TsumuView view)
        {
            if (_tsumuRootModel.GetSelectingTsumuCount() != 0)
            {
                return;
            }

            SelectTsumu(view);
        }


        private bool CanSelect(TsumuView view)
        {
            var lastGuid = _gameModel.TsumuRootModel.GetLastTsumuGuid();
            if (lastGuid == Guid.Empty)
            {
                return false;
            }

            if (_closingViewList.Any(x => x == view))
            {
                return false;
            }
            
            var lastView = _tsumuViewList.FirstOrDefault(x => x.Guid == lastGuid);
            if (lastView == null)
            {
                return false;
            }
            
            var difference = lastView.GetLocalPosition() - view.GetLocalPosition();
            var modelState = _tsumuRootModel.CanSelect(view.Guid);

            if (!modelState)
            {
                return false;
            }

            if (Mathf.Abs(difference.x) < _canSelectDistance && Mathf.Abs(difference.y) < _canSelectDistance)
            {
                return true;
            }

            return false;
        }

        public IReadOnlyList<TsumuView> GetReadOnLyTsumuList()
        {
            return _tsumuViewList.AsReadOnly();
        }

        public async UniTask DespawnTsumuListAsync(List<(TsumuView, int)> targetTsumuList)
        {
            foreach (var view in targetTsumuList)
            {
                DespawnTsumuAsync(view.Item1,view.Item2 ).Forget();
            }
        }

        public void TakeDamage(int damage)
        {
            _attackDamageReactiveProperty.Value = damage;
        }

        public IReadOnlyList<TsumuView> GetClosingTsumuList()
        {
            return _closingViewList.AsReadOnly();
        }
    }
}