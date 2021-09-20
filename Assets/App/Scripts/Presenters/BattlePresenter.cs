using UnityEngine;
using App.Lib;
using App.Models;
using App.Views;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UniRx;
using UnityEngine.UI;

namespace App.Presenters
{
    public class BattlePresenter : PresenterBase
    {
        private readonly PlayerParameter _playerParameter;
        private TsumuRootPresenter _tsumuRootPresenter;
        private BattleView _battleView;
        private TimerView _timerView;
        private readonly OnlineTimer _timer;
        private bool _isWin;

        public BattlePresenter(TsumuRootPresenter tsumuRootPresenter, TimerView timerView, BattleView battleView)
        {
            _tsumuRootPresenter = tsumuRootPresenter;

            // scriptableObjectかPrefabとかで設定したい
            _playerParameter = new PlayerParameter(100);
            _timer = new OnlineTimer(5, 60);
            
            _timerView = timerView;
            _battleView = battleView;
            _isWin = false;
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
                .Skip(1)
                .Subscribe(_timerView.ChangeTimer)
                .AddTo(_timerView);
            
            _timer.ObserveEveryValueChanged(x => x.IsStart)
                .Where(x => x)
                .Subscribe( _ => _tsumuRootPresenter.SetEvents())
                .AddTo(_timerView);

            // resultに遷移
            _timerView.ChangeSceneAsObservable
                .Subscribe(_ => ChangeSceneToResult())
                .AddTo(_timerView);


            // ダメージ受ける処理
            _battleView.OnDamagedAsObservable
                .Subscribe(ReceiveDamage).AddTo(_battleView);

            
            // ツム消した通知を受け取る
            // 攻撃
            _tsumuRootPresenter.AttackDamageObservable
                .Skip(1)
                .Subscribe(damage =>
                {
                    _battleView.SendDamage(damage);
                })
                .AddTo(_battleView);

            // 回復
            _tsumuRootPresenter.HealTsumuNumObservable
                .Skip(1)
                .Subscribe(num =>
                {
                    var addHealth = CalculateHeal(num);
                    GameModel.Instance.PlayerParameter.Recover(addHealth);
                })
                .AddTo(_battleView);
            
            // 体力が変わったことを相手に通知
            GameModel.Instance.PlayerParameter.Health
                .Subscribe(health =>
                {
                    Debug.Log(health);
                    _battleView.SendHealthChange(health);
                    if (health <= 0)
                    {
                        _battleView.SendHealthForWinOrLose(health);
                        // 負けResultに飛びたい
                        _isWin = false;
                        _battleView.SendWinOrLoseFlag(!_isWin);
                    }else if(health <= 20)
                    {
                        _battleView.LowHpFlush();
                    }else if(health > 20)
                    {
                        _battleView.LowHpStop();
                    }
                })
                .AddTo(_battleView);
            
            // 敵の体力が変わった通知を受けとる
            _battleView.OnHealthChangeAsObservable
                .Subscribe(health =>
                {
                    _battleView.SetEnemyHp(health , GameModel.Instance.PlayerParameter.MaxHealth);
                })
                .AddTo(_battleView);
            
            // TimeUpになったらシーンステートを変更
            _timer.ObserveEveryValueChanged(x => x.IsTimeUp)
                .Where(x => x)
                .Subscribe(_ =>
                {
                    if (!PhotonNetwork.IsMasterClient)
                    {
                        _battleView.SendHealthForWinOrLose(GameModel.Instance.PlayerParameter.Health.Value);
                    }
                })
                .AddTo(_timerView);

            // マスターしか受け取らない
            _battleView.OnHealthForWinOrLoseAsObservable
                .Subscribe(enemyHealth =>
                {
                    _isWin = Judge(enemyHealth);
                    // 勝敗結果を送る
                    _battleView.SendWinOrLoseFlag(!_isWin);
                });

            // 勝敗判定を受け取る
            _battleView.OnWinOrLoseFlagAsObservable
                .Subscribe(isWin =>
                {
                    _isWin = isWin;
                    _battleView.SendArriveWinOrLoseFlag();
                })
                .AddTo(_battleView);
            
            // 勝敗判定を相手が受け取ったかの通知を購読
            _battleView.ArriveWinOrLoseFlag
                .Subscribe(_ =>
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        ChangeSceneState(SceneState.SceneStateType.Result);
                    }
                })
                .AddTo(_battleView);
            
            // プレイヤーが抜けた
            _battleView.LeftPlayerFlag
                .Subscribe(_ =>
                {
                    _isWin = true;
                    ChangeSceneState(SceneState.SceneStateType.Result);
                })
                .AddTo(_battleView);
        }

        private bool Judge(float enemyHealth)
        {
            if (enemyHealth <= 0)
            {
                return true;
            }
            
            return GameModel.Instance.PlayerParameter.Health.Value > enemyHealth;
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
        /// 消したツムの数で回復量を計算
        /// </summary>
        /// <param name="tsumuNum"></param>
        /// <returns></returns>
        private float CalculateHeal(int tsumuNum)
        {
            var addHealth = tsumuNum;

            // コンボ倍率
            // damage *= _playerParameter.Combo;

            return addHealth;
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
            ChangeScene<ResultRootView>(new ResultRootView.Parameter(_isWin, null)).Forget();
        }
    }
}
