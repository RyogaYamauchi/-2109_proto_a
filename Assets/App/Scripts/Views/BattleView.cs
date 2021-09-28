using System;
using App.Lib;
using Photon.Pun;
using Photon.Realtime;
using UniRx;

namespace App.Views
{
    [PrefabPath("Prefabs/BattleView")]
    public class BattleView : MonoBehaviourPunCallbacks
    {
        private readonly Subject<bool> _onWinOrLoseFlag = new Subject<bool>();
        private readonly Subject<bool> _onDecidedWinOrLose = new Subject<bool>();
        private readonly Subject<Unit> _leftPlayerFlag = new Subject<Unit>();
        private readonly Subject<int> _onChangedEnemyHealth = new Subject<int>();
        private readonly Subject<int> _onChangedPlayerHealth = new Subject<int>();
        
        public IObservable<bool> OnDecidedWinOrLose => _onDecidedWinOrLose;
        public IObservable<Unit> LeftPlayerFlag => _leftPlayerFlag;
        public IObservable<int> OnChangedEnemyHealth => _onChangedEnemyHealth.TakeUntilDestroy(this);
        public IObservable<int> OnChangedPlayerHealth => _onChangedPlayerHealth.TakeUntilDestroy(this);
        
        public void UpdatePlayerHealth(int health)
        {
            photonView.RPC(nameof(RPCUpdatePlayerHealth), RpcTarget.Others, health);
        }

        [PunRPC]
        private void RPCUpdatePlayerHealth(int health)
        {
            _onChangedEnemyHealth.OnNext(health);
        }

        public void UpdateEnemyHealth(int health)
        {
            photonView.RPC(nameof(RPCUpdateEnemyHealth), RpcTarget.Others, health);
        }
        
        [PunRPC]
        private void RPCUpdateEnemyHealth(int health)
        {
            _onChangedPlayerHealth.OnNext(health);
        }
        

        public void SendArriveWinOrLoseFlag(bool state)
        {
            photonView.RPC(nameof(RPCArriveWinOrLoseFlag), RpcTarget.All, state);
        }

        [PunRPC]
        private void RPCArriveWinOrLoseFlag(bool state)
        {
            _onDecidedWinOrLose.OnNext(state);
        }
        
        // プレイヤーが抜けた
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            _leftPlayerFlag.OnNext(Unit.Default);
        }
    }
}