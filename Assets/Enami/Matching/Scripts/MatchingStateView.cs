using System;
using Photon.Pun;
using Photon.Realtime;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;


namespace  App.Matching
{
    public class MatchingStateView : MonoBehaviourPunCallbacks
    {
        private readonly Subject<Unit> _initializeAsyncSubject = new Subject<Unit>();
        public IObservable<Unit> InitializeAsyncSubject => _initializeAsyncSubject;
        
        private readonly Subject<Unit> _connectedToMaster = new Subject<Unit>();
        public IObservable<Unit> ConnectedToMaster => _connectedToMaster;
        
        private readonly Subject<Unit> _joinedRoom = new Subject<Unit>();
        public IObservable<Unit> JoinedRoom => _joinedRoom;
        void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            _initializeAsyncSubject.OnNext(Unit.Default);
            _initializeAsyncSubject.OnCompleted();
        }
        
        // マスターサーバーへの接続が成功した時に呼ばれるコールバック
        public override void OnConnectedToMaster() {
            _connectedToMaster.OnNext(Unit.Default);
        }

        // ゲームサーバーへの接続が成功した時に呼ばれるコールバック
        public override void OnJoinedRoom() {
            _joinedRoom.OnNext(Unit.Default);
        }
    }
}
