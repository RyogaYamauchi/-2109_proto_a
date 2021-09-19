using System;
using Photon.Pun;
using Photon.Realtime;
using UniRx;
using App.Presenters.Matching;
using App.Lib;
using Cysharp.Threading.Tasks;


namespace App.View.Matching
{
    public class MatchingStateView : PhotonViewBase
    {
        private readonly Subject<Unit> _initializeAsyncSubject = new Subject<Unit>();
        public IObservable<Unit> InitializeAsyncSubject => _initializeAsyncSubject;
        
        private readonly Subject<Unit> _connectedToMaster = new Subject<Unit>();
        public IObservable<Unit> ConnectedToMaster => _connectedToMaster;
        
        private readonly Subject<Unit> _joinedRoom = new Subject<Unit>();
        public IObservable<Unit> JoinedRoom => _joinedRoom;
        
        private readonly Subject<Unit> _playerJoinedRoom = new Subject<Unit>();
        public IObservable<Unit> PlayerJoinedRoom => _playerJoinedRoom;
        
        private readonly Subject<Unit> _changeScene = new Subject<Unit>();
        public IObservable<Unit> ChangeScene => _changeScene;

        private MatchingPresenter _matchingPresenter;

        private bool _isChange = false;
        // public override UniTask OnLoadAsync()
        // {
        //     Initialize();
        //     return base.OnLoadAsync();
        // }

        // private void Initialize()
        // {
        //     _matchingPresenter = new MatchingPresenter(this);
        //     _initializeAsyncSubject.OnNext(Unit.Default);
        //     _initializeAsyncSubject.OnCompleted();
        // }
        
        // マスターサーバーへの接続が成功した時に呼ばれるコールバック
        public override void OnConnectedToMaster() {
            _connectedToMaster.OnNext(Unit.Default);
        }

        // ゲームサーバーへの接続が成功した時に呼ばれるコールバック
        public override void OnJoinedRoom() {
            _joinedRoom.OnNext(Unit.Default);
        }
        
        public override void  OnPlayerEnteredRoom(Player other)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                _playerJoinedRoom.OnNext(Unit.Default);
            }
        }

        public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        {
            if (propertiesThatChanged.TryGetValue("scene_state", out var value))
            {
                if (_isChange) return;
                if ((string)value == SceneState.SceneStateType.Main.ToString())
                {
                    _isChange = true;
                    _changeScene.OnNext(Unit.Default);
                }
            }
        }
    }
}
