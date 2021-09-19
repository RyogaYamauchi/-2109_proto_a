using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Lib;
using App.Models;
using App.Skills;
using App.Views;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace App.Presenters
{
    public sealed class TsumuRootPresenter : RootPresenterBase
    {
        private MainRootView _mainRootView;
        private readonly int _maxTsumuCount;
        private readonly int _canSelectDistance = 300;
        private readonly Vector2[] _spawnPoint =
        {
            new Vector2(-300, 0), new Vector2(-200, 0), new Vector2(-100, 0),
            new Vector2(0, 0), new Vector2(100, 0), new Vector2(200, 0), new Vector2(300, 0)
        };

        private TsumuRootModel _tsumuRootModel => _gameModel.TsumuRootModel;
        private List<TsumuView> _tsumuViewList = new List<TsumuView>();
        private List<Vector2> _canSpawnTsumuPoints = new List<Vector2>();

        private ISkill _skill;

        public TsumuRootPresenter(MainRootView view, IParameter parameter)
        {
            var param = (MainRootView.Paramater) parameter;
            _maxTsumuCount = param.MaxTsumuCount;
            Debug.Log(_maxTsumuCount);
            _mainRootView = view;
        }
        
        public void Initialize()
        {
            _gameModel.TsumuRootModel.Initialize();
            _skill = _gameModel.SkillModel.GetRandomSkill();
            _canSpawnTsumuPoints = new List<Vector2>(_spawnPoint);
            _gameModel.PlayerParameter.Health.Subscribe(x =>
            {
                _mainRootView.SetHp(_gameModel.PlayerParameter.Health.Value, _gameModel.PlayerParameter.MaxHealth);
            }).AddTo(_mainRootView);
            DebugReceiveDamage().Forget();
        }

        private async UniTask DebugReceiveDamage()
        {
            for (var i = 0; i < 20; i++)
            {
                await UniTask.Delay(500); 
                _gameModel.PlayerParameter.RecieveDamage(5);
            }
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
            await skill.ExecuteAsync(this);
        }
        
        private async UniTask SpawnTsumuAsync()
        {
            _canSpawnTsumuPoints = _canSpawnTsumuPoints.OrderBy(x => Guid.NewGuid()).ToList();
            var spawnPoint = _canSpawnTsumuPoints.FirstOrDefault();
            _canSpawnTsumuPoints.Remove(spawnPoint);
            var viewModel = _gameModel.TsumuRootModel.SpawnTsumu();
            var tsumuView = await CreateViewAsync<TsumuView>();
            _mainRootView.SetParentTsumu(tsumuView);
            var spawnRootPosition = _mainRootView.GetSpawnRootPosition();
            tsumuView.Initialize(viewModel,  new Vector2(spawnRootPosition.x + spawnPoint.x, spawnRootPosition.y));
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

        private async UniTask DespawnSelectingTsumusAsync()
        {
            var ids = _tsumuRootModel.GetSelectingTsumuIdList();
            var views = _tsumuViewList.Where(x => ids.Contains(x.Guid)).ToArray();
            foreach (var view in views)
            {
                await DespawnTsumuAsync(view);
            }
        }

        private async UniTask DespawnTsumuAsync(TsumuView tsumuView)
        {
            _tsumuViewList.Remove(tsumuView);
            await tsumuView.CloseAsync();
        }

        private void OnPointerEntertsumu(TsumuView view)
        {
            if (!CanSelect(view))
            {
                return;
            }

            SelectTsumu(view);
        }

        private UniTask OnPointerUpTsumu(TsumuView view)
        {
            if (_tsumuRootModel.GetSelectingTsumuCount() < 3)
            {
                UnSelectTsumuAll();
                return UniTask.CompletedTask;
            }

            DespawnSelectingTsumusAsync().Forget();
            UnSelectTsumuAll();
            return UniTask.CompletedTask;
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
            var lastView = _tsumuViewList.FirstOrDefault(x => x.Guid == lastGuid);
            var difference = lastView.GetPosition() - view.GetPosition();
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

        public async Task DespawnTsumuListAsync(List<TsumuView> targetTsumuList)
        {
            foreach (var view in targetTsumuList)
            {
                await DespawnTsumuAsync(view);
            }
        }
    }
}