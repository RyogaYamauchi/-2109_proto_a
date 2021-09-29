using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using App.Application;
using App.Lib;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace App.Presentation
{
    [RootSceneName(("MainScene"))]
    public sealed class MainRootPresenter : RootPresenterBase
    {
        private readonly int _canSelectDistance = 250;
        private readonly Vector2[] _spawnPoint =
        {
            new Vector2(-225, 0), new Vector2(-150, 0), new Vector2(-75, 0),
            new Vector2(0, 0), new Vector2(75, 0), new Vector2(150, 0), new Vector2(225, 0)
        };
        private readonly BattlePresenter _battlePresenter;
        private readonly MainRootUseCase _mainRootUseCase;


        private List<TsumuView> _tsumuViewList = new List<TsumuView>();
        private List<Vector2> _canSpawnTsumuPoints = new List<Vector2>();
        private int _maxTsumuCount;
        private MainRootView _mainRootView;

        public MainRootPresenter(BattlePresenter battlePresenter, MainRootUseCase mainRootUseCase)
        {
            _battlePresenter = battlePresenter;
            _mainRootUseCase = mainRootUseCase;
        }
            

        protected override UniTask OnLoadAsync(CancellationToken cancellationToken)
        {
            var param = (MainRootView.Paramater)_parameter;
            _maxTsumuCount = param.MaxTsumuCount;
            _mainRootView = GetRootView<MainRootView>();
            _mainRootUseCase.Initialize();
            _canSpawnTsumuPoints = new List<Vector2>(_spawnPoint);
            _mainRootView.SetActiveSkillButton(false);
            
            if (!param.IsSingleMode)
            {
                _mainRootView.SetActiveGoTitleButton(param.IsSingleMode);
                _battlePresenter.Initialize(_mainRootView.GetTimerView(), _mainRootView.GetBattleView());
                _battlePresenter.StartGameObservable.Subscribe(x => SetEvents());
                _battlePresenter.OnChangedEnemyHealth.Subscribe(x =>
                {
                    _mainRootView.SetEnemyHp(_mainRootUseCase.GetEnemyViewModel());
                });
                _battlePresenter.OnChangedPlayerHealth.Subscribe(x =>
                {
                    _mainRootView.SetHp(_mainRootUseCase.GetPlayerViewModel());
                });
                return UniTask.CompletedTask;
            }

            SetEvents();
            return UniTask.CompletedTask;
        }

        private void SetEvents()
        {
            _mainRootView.Win.Subscribe(x =>
            {
                _mainRootUseCase.DebugWin(true);
                _battlePresenter.FinishGame();
            });
            _mainRootView.OnClickSkillAsObservable.Subscribe(x =>
            {
                UseSkillAsync().Forget();
            });
            _mainRootView.UpdateAsObservable().Subscribe(x =>
            {
                RefillTsumus();
            }).AddTo(_mainRootView);
            _mainRootView.OnClickGoTitleButtonAsObservable.Subscribe(x =>
            {
                ChangeScene<TitleRootPresenter>().Forget();
            });
            _mainRootUseCase.OnChangedSkillPointAsObservable.Subscribe(x =>
            {
                var skillPointViewModel = _mainRootUseCase.GetSkillPointViewModel();
                _mainRootView.SetSkillValue(skillPointViewModel);
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

        private async UniTask UseSkillAsync()
        {
            _mainRootUseCase.UseSkill();
            _mainRootView.SetActiveSkillButton(false);
        }
        
        private async UniTask SpawnTsumuAsync()
        {
            var spawnPointY = Screen.height / 2;
            _canSpawnTsumuPoints = _canSpawnTsumuPoints.OrderBy(x => Guid.NewGuid()).ToList();
            var spawnPoint = _canSpawnTsumuPoints.FirstOrDefault();
            _canSpawnTsumuPoints.Remove(spawnPoint);
            var viewModel = _mainRootUseCase.SpawnTsumu();
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
            _mainRootUseCase.SelectTsumu(view.Guid);
        }

        private async UniTask DespawnTsumuAsync(Guid guid, int damage)
        {
            var tsumuView = _tsumuViewList.FirstOrDefault(x => x.Guid == guid);
            _tsumuViewList.Remove(tsumuView);
            if (tsumuView == null)
            {
                return;
            }
            var pos = tsumuView.transform.position;
            PlayDamageView(pos, tsumuView, damage, _mainRootView.GetCancellationTokenOnDestroy()).Forget();
            await tsumuView.CloseAsync();
        }

        private async UniTask PlayDamageView(Vector3 pos, TsumuView tsumuView, int damage, CancellationToken cancellationToken)
        {
            var damageView = await CreateViewAsync<TsumuAttackNumView>();
            _mainRootView.SetParentTakeDamageNum(damageView);
            damageView.transform.position = pos;
            damageView.Initialize(damage);
            await damageView.MoveToTarget(_mainRootView.GetEnemyPosition(), cancellationToken);
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
            if (_mainRootUseCase.CanUnSelect())
            {
                _mainRootUseCase.UnSelectTsumuAll();
                return;
            }

            var chain = _mainRootUseCase.GetSelectingCount();
            var playerHealth = _mainRootUseCase.GetPlayerViewModel().Health;
            var enemyHealth = _mainRootUseCase.GetEnemyViewModel().Health;
            var sumDamage = 0;

            await _mainRootUseCase.DeleteSelectingTsumu(async (guid, damage) =>
            {
                sumDamage += damage;
                await DespawnTsumuAsync(guid, damage);
                _mainRootUseCase.AddSkillPoint();

                if (_mainRootUseCase.CanExecuteSkill())
                {
                    _mainRootView.SetActiveSkillButton(true);
                }
            });

            _mainRootUseCase.ResolveDamage(chain, sumDamage);

            var afterPlayerViewModel = _mainRootUseCase.GetPlayerViewModel();
            var afterEnemyViewModel = _mainRootUseCase.GetEnemyViewModel();
            if (afterPlayerViewModel.Health != playerHealth)
            {
                _mainRootView.SetHp(afterPlayerViewModel);
                _battlePresenter.UpdateMySelfHealth();
            }
            if (afterEnemyViewModel.Health != enemyHealth)
            {
                _mainRootView.SetEnemyHp(afterEnemyViewModel);
                _battlePresenter.UpdateEnemyHealth();
            }

            if (_mainRootUseCase.IsEnemyDied())
            {
                _battlePresenter.FinishGame();
            }
        }

        private void OnPointerDownTsumu(TsumuView view)
        {
            if (_mainRootUseCase.GetSelectingCount() != 0)
            {
                return;
            }

            SelectTsumu(view);
        }


        private bool CanSelect(TsumuView view)
        {
            var lastTsumuGuid = _mainRootUseCase.GetLastTsumuGuid();
            if (lastTsumuGuid == null)
            {
                return true;
            }
            
            if (!_mainRootUseCase.CanSelect(view.Guid))
            {
                return false;
            }

            var lastView = _tsumuViewList.FirstOrDefault(x => x.Guid == lastTsumuGuid);
            var difference = lastView.GetLocalPosition() - view.GetLocalPosition();


            if (Mathf.Abs(difference.x) < _canSelectDistance && Mathf.Abs(difference.y) < _canSelectDistance)
            {
                return true;
            }

            return false;
        }
        
    }
}