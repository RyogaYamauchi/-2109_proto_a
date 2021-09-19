using UnityEngine;
using App.Lib;
using App.Models;
using App.Views;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UniRx;

namespace App.Presenters
{
    public class BattlePresenter : PresenterBase
    {
        private readonly PlayerParameter _playerParameter;
        private TsumuRootPresenter _tsumuRootPresenter;
        private BattleView _battleView;
        private TimerView _timerView;
        private readonly OnlineTimer _timer;

        public BattlePresenter(TsumuRootPresenter tsumuRootPresenter, TimerView timerView, BattleView battleView)
        {
            _tsumuRootPresenter = tsumuRootPresenter;

            // scriptableObjectかPrefabとかで設定したい
            _playerParameter = new PlayerParameter(100);
            _timer = new OnlineTimer(5, 60);
            
            _timerView = timerView;
            _battleView = battleView;
        }
        
        public BattlePresenter(TimerView view)
        {
            _playerParameter = new PlayerParameter(100);
            _timer = new OnlineTimer(5, 60);
            
            _timerView = view;
        }

        public void Initialize()
        {
            // タイマーセット
            if (PhotonNetwork.IsMasterClient)
            {
                _timer.OnlineInitialize();
            }
            
            // Timerを動かす
            _timerView.OnStartTimeAsObservable
                .Subscribe(startTime =>
                {
                    _timer.StartTimer(startTime).Forget();
                }).AddTo(_timerView);
            
            // タイマーの値が変わったことを受け取る
            _timer.ObserveEveryValueChanged(x => x.CountDown)
                .Subscribe(_timerView.ChangeCountDown)
                .AddTo(_timerView);
            
            _timer.ObserveEveryValueChanged(x => x.TotalTime)
                .Subscribe(_timerView.ChangeTimer)
                .AddTo(_timerView);
            
            _timer.ObserveEveryValueChanged(x => x.IsStart)
                .Where(x => x)
                .Subscribe( _ => _tsumuRootPresenter.SetEvents())
                .AddTo(_timerView);
            
            _timer.ObserveEveryValueChanged(x => x.IsTimeUp)
                .Where(x => x)
                .Subscribe(_ =>
                {
                    if (!PhotonNetwork.IsMasterClient)
                    {
                        return;
                    }
                    ChangeSceneState(SceneState.SceneStateType.Result);
                })
                .AddTo(_timerView);

            // resultに遷移
            _timerView.ChangeSceneAsObservable
                .Subscribe(_ => ChangeSceneToResult())
                .AddTo(_timerView);


            // ダメージ受ける処理
            // _battleView.OnDamagedAsObservable
            //     .Subscribe(ReceiveDamage).AddTo(_battleView);

            // TODO:ツム消した時の処理
            // ツム消した通知を受け取る
            // ツムのタイプによって攻撃か回復
            // ダメージを計算

            // 回復した値送る


            // ダメージ相手に送る
            // _battleView.SendDamage(CalculateDamage(1));
        }
        

        /// <summary>
        /// ダメージ受けた時の処理
        /// </summary>
        /// <param name="damage"></param>
        private void ReceiveDamage(float damage)
        {
            GameModel.Instance.PlayerParameter.RecieveDamage(damage);
        }

        /// <summary>
        /// 消したツムの数でダメージを計算
        /// </summary>
        /// <param name="tsumuNum"></param>
        /// <returns></returns>
        private float CalculateDamage(int tsumuNum)
        {
            var damage = tsumuNum;

            // コンボ倍率
            // damage *= _playerParameter.Combo;

            return damage;
        }
        
        /// <summary>
        /// sceneStateを変更
        /// </summary>
        /// <param name="sceneState"></param>
        private void ChangeSceneState(SceneState.SceneStateType sceneState)
        {
            var prop = PhotonNetwork.CurrentRoom.CustomProperties;
            
            //キーを持っているか確認した方がよさそう
            if (prop.TryGetValue("scene_state",out var _))
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
            ChangeScene<ResultRootView>().Forget();
        }
    }
}
