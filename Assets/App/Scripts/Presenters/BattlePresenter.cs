using System;
using System.Threading;
using App.Lib;
using App.Models;
using App.Views;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UniRx;
using UnityEngine;

namespace App.Presenters
{
    public class BattlePresenter : PresenterBase
    {
        private BattleView _battleView;
        private TimerView _timerView;
        private readonly OnlineTimerModel _timerModel;

        private Subject<Unit> _startGameSubject = new Subject<Unit>();
        private Subject<Unit> _onChangedEnemyHealth = new Subject<Unit>();
        private Subject<Unit> _onChangedPlayerHealth = new Subject<Unit>(); 
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();


        public BattlePresenter()
        {
            _timerModel = new OnlineTimerModel(5, 120);
        }

        public IObservable<Unit> StartGameObservable => _startGameSubject.TakeUntilDestroy(_battleView);
        public IObservable<Unit> OnChangedEnemyHealth => _onChangedEnemyHealth.TakeUntilDestroy(_battleView);
        public IObservable<Unit> OnChangedPlayerHealth => _onChangedPlayerHealth.TakeUntilDestroy(_battleView);

        public void Initialize(TimerView timerView, BattleView battleView)
        {
            _battleView = battleView;

            _timerView = timerView;
            StartGame().Forget();
            // タイマーセット
            if (PhotonNetwork.IsMasterClient)
            {
                _timerModel.OnlineInitialize();
            }

            // タイマーの値が変わったことを受け取る
            _timerModel.CountDowntime
                .Subscribe(_timerView.ChangeCountDown)
                .AddTo(_timerView);

            _timerModel.CurrentTime
                .Subscribe(_timerView.ChangeTimer)
                .AddTo(_timerView);

            // resultに遷移
            _timerView.ChangeSceneAsObservable.Subscribe(_ => ChangeSceneToResult()).AddTo(_timerView);

            // 勝敗判定を相手が受け取ったかの通知を購読
            _battleView.OnDecidedWinOrLose
                .Subscribe(_ =>
                {
                    ChangeSceneState(SceneState.SceneStateType.Result);
                })
                .AddTo(_battleView);

            // プレイヤーが抜けた
            _battleView.LeftPlayerFlag
                .Subscribe(_ =>
                {
                    GameModel.Instance.BattleModel.SetWinOrLose(true);
                    FinishGame();
                })
                .AddTo(_battleView);

            battleView.OnChangedEnemyHealth.Subscribe(UpdateEnemyHealth);
            battleView.OnChangedPlayerHealth.Subscribe(UpdatePlayerHealth);
        }

        public void FinishGame()
        {
            Debug.Log("FinishGame!!");
            _battleView.SendDecideWinOrLoseFlag(GameModel.Instance.BattleModel.WinOrLose);
            _cancellationTokenSource.Cancel();

        }

        private void UpdatePlayerHealth(int health)
        {
            GameModel.Instance.PlayerModel.SetHealth(health);
            _onChangedPlayerHealth.OnNext(Unit.Default);
        }

        private void UpdateEnemyHealth(int health)
        {
            GameModel.Instance.EnemyModel.SetHealth(health);
            _onChangedEnemyHealth.OnNext(Unit.Default);
        }

        private void SetUp()
        {
            UpdateEnemyHealth(200);
            UpdatePlayerHealth(200);
        }

        private async UniTask StartGame()
        {
            SetUp();
            // 5秒待つ

            var waitStartTimeResult = await _timerModel.WaitSeconds(5, _cancellationTokenSource.Token).SuppressCancellationThrow();
            
            Debug.Log("開始！");
            if (waitStartTimeResult.IsCanceled)
            {
                // ルームを解除してリザルトに遷移
                ChangeSceneState(SceneState.SceneStateType.Result);
                return;
            }

            _startGameSubject.OnNext(Unit.Default);
            // 120秒待つ
            var waitMainTimeResult = await _timerModel.WaitSeconds(120, _cancellationTokenSource.Token).SuppressCancellationThrow();
            // どちらかのHPが0になったら勝敗
            Debug.Log("終了!");

            if (waitMainTimeResult.IsCanceled)
            {
                // ルームを解除してリザルトに遷移
                ChangeSceneState(SceneState.SceneStateType.Result);
                return;
            }

            // 制限時間が来たら勝敗
            ChangeSceneState(SceneState.SceneStateType.Result);
        }

        /// sceneStateを変更
        /// </summary>
        /// <param name="sceneState"></param>
        private void ChangeSceneState(SceneState.SceneStateType sceneState)
        {
            var prop = PhotonNetwork.CurrentRoom.CustomProperties;

            //キーを持っているか確認した方がよさそう
            if (prop.TryGetValue("scene_state", out var _))
            {
                prop["scene_state"] = sceneState.ToString();
            }
            else
            {
                prop.Add("scene_state", sceneState.ToString());
            }

            //更新したプロパティをセットする
            PhotonNetwork.CurrentRoom.SetCustomProperties(prop);
        }

        /// <summary>
        /// ResultSceneへ行く
        /// </summary>
        private void ChangeSceneToResult()
        {
            ChangeScene<ResultRootPresenter>(
                new ResultRootView.Parameter(GameModel.Instance.BattleModel.WinOrLose)).Forget();
        }


        public void UpdateMySelfHealth()
        {
            var health = GameModel.Instance.PlayerModel.Health;
            _battleView.UpdatePlayerHealth(health);
        }


        public void UpdateEnemyHealth()
        {
            var health = GameModel.Instance.EnemyModel.Health;
            _battleView.UpdateEnemyHealth(health);
        }
    }
}