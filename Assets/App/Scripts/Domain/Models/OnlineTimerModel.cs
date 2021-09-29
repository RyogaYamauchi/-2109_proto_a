using Photon.Pun;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;


namespace App.Domain
{
    public class OnlineTimerModel
    {
        private readonly int _maxCountDown;
        private bool _isTimeUp;
        private float _countDownStartTime;
        public float TotalTime { get; private set; }
        public bool IsStart { get; private set; }


        private ReactiveProperty<int> _currentTime = new ReactiveProperty<int>();
        private ReactiveProperty<int> _countDownTime = new ReactiveProperty<int>();
        public IReadOnlyReactiveProperty<int> CurrentTime => _currentTime.ToReadOnlyReactiveProperty();
        public IReadOnlyReactiveProperty<int> CountDowntime => _countDownTime.ToReadOnlyReactiveProperty();


        public OnlineTimerModel(int maxCountDown, int maxTime)
        {
            _maxCountDown = maxCountDown;
            _isTimeUp = false;
            TotalTime = maxTime;
            _countDownTime.Value = _maxCountDown;
            IsStart = false;
        }

        public void OnlineInitialize()
        {
            // 全体カウントダウンのスタート時間をセット
            ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();
            properties.Add("StartTime", PhotonNetwork.Time);
            PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
        }

        public async UniTask<bool> WaitSeconds(int time, CancellationToken token)
        {
            _currentTime.Value = time;
            for (var i = 0; i < time; i++)
            {
                var canceled = await UniTask.Delay(1000, cancellationToken: token).SuppressCancellationThrow();
                if (canceled)
                {
                    return false;
                }
                _currentTime.Value--;
            }

            return true;
        }
    }

}
