using System;
using App.Models;
using Photon.Pun;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace App.Views
{
    public class BattleView : MonoBehaviourPunCallbacks
    {
        [SerializeField] private Slider playerHpSlider;
        [SerializeField] private Slider enemyHpSlider;
        [SerializeField] private Image lowHpImage;
        private Color lowHpColor = Color.clear;
        private float lowHpTime;


        private readonly Subject<float> _onDamagedSubject = new Subject<float>();
        public IObservable<float> OnDamagedAsObservable => _onDamagedSubject.TakeUntilDestroy(this);
        
        private readonly Subject<float> _onHealthChangeSubject = new Subject<float>();
        public IObservable<float> OnHealthChangeAsObservable => _onHealthChangeSubject.TakeUntilDestroy(this);
        
        private readonly Subject<float> _onHealthForWinOrLoseSubject = new Subject<float>();
        public IObservable<float> OnHealthForWinOrLoseAsObservable => _onHealthForWinOrLoseSubject.TakeUntilDestroy(this);
        
        private readonly Subject<bool> _onWinOrLoseFlag = new Subject<bool>();
        public IObservable<bool> OnWinOrLoseFlagAsObservable => _onWinOrLoseFlag.TakeUntilDestroy(this);
        
        private readonly Subject<Unit> _arriveWinOrLoseFlag = new Subject<Unit>();
        public IObservable<Unit> ArriveWinOrLoseFlag => _arriveWinOrLoseFlag;


        public void SendDamage(float damage)
        {
            // 敵に通知
            photonView.RPC(nameof(RPCDamaged), RpcTarget.Others, damage);
            
            // 自分の画面に反映
        }
        
        [PunRPC]
        private void RPCDamaged(float damage) 
        {
            _onDamagedSubject.OnNext(damage);
        }
        
        
        public void SendHealthChange(float health)
        {
            photonView.RPC(nameof(RPCHealthChange), RpcTarget.Others, health);
            
            // 自分の体力変える
            SetPlayerHp(health , GameModel.Instance.PlayerParameter.MaxHealth);
        }
        
        [PunRPC]
        private void RPCHealthChange(float health) 
        {
            _onHealthChangeSubject.OnNext(health);
        }

        public void SetPlayerHp(float value, float maxValue)
        {
            playerHpSlider.minValue = 0;
            playerHpSlider.maxValue = maxValue;
            playerHpSlider.value = value;
        }
        
        public void SetEnemyHp(float value, float maxValue)
        {
            enemyHpSlider.minValue = 0;
            enemyHpSlider.maxValue = maxValue;
            enemyHpSlider.value = value;
        }
        
        
        public void SendHealthForWinOrLose(float health)
        {
            photonView.RPC(nameof(RPCHealthForWinOrLose), RpcTarget.Others, health);
        }
        
        [PunRPC]
        private void RPCHealthForWinOrLose(float health) 
        {
            _onHealthForWinOrLoseSubject.OnNext(health);
        }
        
        public void SendWinOrLoseFlag(bool isWin)
        {
            photonView.RPC(nameof(RPCWinOrLoseFlag), RpcTarget.Others, isWin);
        }
        
        [PunRPC]
        private void RPCWinOrLoseFlag(bool isWin) 
        {
            _onWinOrLoseFlag.OnNext(isWin);
        }
        
        public void SendArriveWinOrLoseFlag()
        {
            photonView.RPC(nameof(RPCArriveWinOrLoseFlag), RpcTarget.All);
        }
        
        [PunRPC]
        private void RPCArriveWinOrLoseFlag() 
        {
            _arriveWinOrLoseFlag.OnNext(Unit.Default);
        }

        public void LowHpFlush()
        {
            //画面を赤塗り
            lowHpColor = new Color(0.5f, 0f, 0f, 0.5f);
        }

        public void Update()
        {
            //時間経過ごとに透明化
            lowHpTime += Time.deltaTime;
            if (lowHpTime >= 1.0f)
            {
                lowHpTime = 0f;
            }
            lowHpImage.color = Color.Lerp(lowHpColor, Color.clear, lowHpTime);
        }

        public void LowHpStop()
        {
            lowHpColor = Color.clear;
        }

    }
}

