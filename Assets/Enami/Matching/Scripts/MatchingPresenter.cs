using Photon.Pun;
using Photon.Realtime;
using UniRx;
using UnityEngine;

namespace App.Matching
{
    public class MatchingPresenter : MonoBehaviourPunCallbacks
    {
        [SerializeField] private MatchingStateView _matchingStateView;

        private void Awake()
        {
            _matchingStateView.InitializeAsyncSubject
                .Subscribe(_ =>
                {
                    PhotonInitialize();
                })
                .AddTo(this);
            
            _matchingStateView.ConnectedToMaster
                .Subscribe(_ =>
                {
                    JoinOrCreateRoom();
                })
                .AddTo(this);
            
            _matchingStateView.JoinedRoom
                .Subscribe(_ =>
                {
                    CreateAvatar();
                })
                .AddTo(this);
        }

        private void PhotonInitialize()
        {
            // PhotonServerSettingsの設定内容を使ってマスターサーバーへ接続する
            PhotonNetwork.ConnectUsingSettings();
        }
        
        private void JoinOrCreateRoom() {
            // "Room"という名前のルームに参加する（ルームが存在しなければ作成して参加する）
            PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions(), TypedLobby.Default);
        }
        
        private  void CreateAvatar() {
            PhotonNetwork.Instantiate("Avatar", Vector3.zero, Quaternion.identity);
        }
    }
}

