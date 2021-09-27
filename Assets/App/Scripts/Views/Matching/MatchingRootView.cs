using System;
using App.Lib;
using UnityEngine;
using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;
using UniRx;

namespace App.Views
{
    public sealed class MatchingRootView : PhotonViewBase
    {
        [SerializeField] GameObject loadingImage;
        private readonly Subject<Unit> _connectedToMaster = new Subject<Unit>();
        private readonly Subject<Unit> _joinedRoom = new Subject<Unit>();
        private readonly Subject<Unit> _joinedFailedRoom = new Subject<Unit>();
        private readonly Subject<Unit> _playerJoinedRoom = new Subject<Unit>();
        private readonly Subject<Unit> _changeScene = new Subject<Unit>();

        public IObservable<Unit> ConnectedToMaster => _connectedToMaster;
        public IObservable<Unit> JoinedRoom => _joinedRoom;
        public IObservable<Unit> JoinedFailedRoom => _joinedFailedRoom;
        public IObservable<Unit> PlayerJoinedRoom => _playerJoinedRoom;
        public IObservable<Unit> ChangeScene => _changeScene;

        private bool _isChange = false;

        // デバッグ機能、シーン単体で起動できる
        public void Initialize()
        {
            loadingImage.transform.DOLocalRotate(new Vector3(0, 0, 180), 1).SetLoops(-1, LoopType.Yoyo);
        }


        // マスターサーバーへの接続が成功した時に呼ばれるコールバック
        public override void OnConnectedToMaster()
        {
            _isChange = false;
            _connectedToMaster.OnNext(Unit.Default);
        }

        // ゲームサーバーへの接続が成功した時に呼ばれるコールバック
        public override void OnJoinedRoom()
        {
            _joinedRoom.OnNext(Unit.Default);
        }

        public override void OnPlayerEnteredRoom(Player other)
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
                if ((string) value == SceneState.SceneStateType.Main.ToString())
                {
                    _isChange = true;
                    _changeScene.OnNext(Unit.Default);
                }
            }
        }

        // ランダムで参加できるルームが存在しないなら、新規でルームを作成する
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            _joinedFailedRoom.OnNext(Unit.Default);
        }
    }
}