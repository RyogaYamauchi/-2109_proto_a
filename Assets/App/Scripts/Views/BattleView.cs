using System;
using Photon.Pun;
using UniRx;
using UnityEngine;

namespace App.Views
{
    public class BattleView : MonoBehaviourPunCallbacks
    {
        private Subject<float> _onDamagedSubject = new Subject<float>();
        public IObservable<float> OnDamagedAsObservable => _onDamagedSubject.TakeUntilDestroy(this);


        public void SendDamage(float damage)
        {
            photonView.RPC(nameof(RPCDamaged), RpcTarget.All, damage);
        }
        
        [PunRPC]
        private void RPCDamaged(float damage) 
        {
            _onDamagedSubject.OnNext(damage);
        }
        
        
    }
}

