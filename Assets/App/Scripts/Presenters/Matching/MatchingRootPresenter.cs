using System.Linq;
using System.Threading;
using Photon.Pun;
using Photon.Realtime;
using UniRx;
using UnityEngine;
using App.Lib;
using App.Views;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace App.Presenters
{
    [RootSceneName(("Matching"))]
    public class MatchingRootPresenter : RootPresenterBase
    {
        private MatchingRootView _matchingRootView;

        // ハッシュテーブルを宣言
        private ExitGames.Client.Photon.Hashtable _roomHash;

        protected override UniTask OnLoadAsync(CancellationToken cancellationToken)
        {
            _matchingRootView = SceneManager.GetActiveScene().GetRootGameObjects()
                .FirstOrDefault(x => x.GetComponent<MatchingRootView>())
                ?.GetComponent<MatchingRootView>();
            _matchingRootView.Initialize();
            Init();
            return base.OnLoadAsync(cancellationToken);
        }

        private void Init()
        {
            PhotonInitialize();

            _matchingRootView.ConnectedToMaster
                .Subscribe(_ =>
                {
                    // ランダムなルームに参加する
                    PhotonNetwork.JoinRandomRoom();
                })
                .AddTo(_matchingRootView);

            _matchingRootView.JoinedRoom
                .Subscribe(_ =>
                {
                    //CreateAvatar();
                })
                .AddTo(_matchingRootView);

            _matchingRootView.JoinedFailedRoom
                .Subscribe(_ => { CreateRoom(); })
                .AddTo(_matchingRootView);

            _matchingRootView.PlayerJoinedRoom
                .Subscribe(_ =>
                {
                    if (!PhotonNetwork.IsMasterClient)
                    {
                        return;
                    }

                    if (PhotonNetwork.PlayerList.Length == 2)
                    {
                        PhotonNetwork.CurrentRoom.IsOpen = false;
                        ChangeSceneState(SceneState.SceneStateType.Main);
                    }
                })
                .AddTo(_matchingRootView);

            _matchingRootView.ChangeScene
                .Subscribe(_ => { ChangeSceneToMain(); })
                .AddTo(_matchingRootView);

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

        private void CreateRoom()
        {
            // ルームの参加人数を2人に設定する
            var roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 2;

            PhotonNetwork.CreateRoom(null, roomOptions);
        }

        /// <summary>
        /// sceneStateを変更
        /// </summary>
        /// <param name="sceneState"></param>
        private void ChangeSceneState(SceneState.SceneStateType sceneState)
        {
            var prop = PhotonNetwork.CurrentRoom.CustomProperties;

            //キーを持っているか確認した方がよさそう
            if (prop.TryGetValue("scene_state", out var _))
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
        private void ChangeSceneToMain()
        {
            Debug.Log("main");
            ChangeScene<MainRootPresenter>(new MainRootView.Paramater(30, 120, 300, false)).Forget();
        }
    }
}