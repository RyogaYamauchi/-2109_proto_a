using App.Skills;
using Photon.Pun;
using Photon.Realtime;
using UniRx;
using UnityEngine;
using App.Lib;
using App.View.Matching;
using App.Views;
using Cysharp.Threading.Tasks;

namespace App.Presenters.Matching
{
    public class MatchingPresenter : PresenterBase
    {
        private readonly MatchingStateView _matchingStateView;
        // ハッシュテーブルを宣言
        private ExitGames.Client.Photon.Hashtable _roomHash;

        public MatchingPresenter(MatchingStateView stateView)
        {
            _matchingStateView = stateView;
            Init();
        }
        
        private void Init()
        {
            PhotonInitialize();
            
            _matchingStateView.ConnectedToMaster
                .Subscribe(_ =>
                {
                    JoinOrCreateRoom();
                })
                .AddTo(_matchingStateView);
            
            _matchingStateView.JoinedRoom
                .Subscribe(_ =>
                {
                    CreateAvatar();
                })
                .AddTo(_matchingStateView);
            
            _matchingStateView.PlayerJoinedRoom
                .Subscribe(_ =>
                {
                    if (!PhotonNetwork.IsMasterClient)
                    {
                        return;
                    }

                    if (PhotonNetwork.PlayerList.Length == 2)
                    {
                        ChangeSceneState(SceneState.SceneStateType.Main);
                    }
                    
                })
                .AddTo(_matchingStateView);
            
            _matchingStateView.ChangeScene
                .Subscribe(_ =>
                {
                    ChangeSceneToMain();
                })
                .AddTo(_matchingStateView);

            if (PhotonNetwork.IsMasterClient)
            {
                ChangeSceneState(SceneState.SceneStateType.Matching);
            }
        }

        /// <summary>
        /// PhotonServerSettingsの設定内容を使ってマスターサーバーへ接続する
        /// </summary>
        private void PhotonInitialize()
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        
        /// <summary>
        /// Roomがあれば入る、なければ作る
        /// </summary>
        private void JoinOrCreateRoom() 
        {
            PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions(), TypedLobby.Default);
        }
        
        private  void CreateAvatar() 
        {
           //  PhotonNetwork.Instantiate("Avatar", Vector3.zero, Quaternion.identity);
           Debug.Log("create");
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
        /// MainSceneへ行く
        /// </summary>
        private  void ChangeSceneToMain()
        {
            Debug.Log("main");
            ChangeScene<MainRootView>(new MainRootView.Paramater(30, new DeleteLineSkill())).Forget();
        }

    }
}

