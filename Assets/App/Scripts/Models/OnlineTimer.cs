using Photon.Pun;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;


namespace App.Models
{
    public class OnlineTimer
    {
        private readonly int _maxCountDown;
        private int _maxTime;
        private bool _isTimeUp;
        private float _countDownStartTime;

        public float TotalTime { get; private set; }

        public int CountDown { get; private set; }
        
        public bool IsStart { get; private set; }

        public bool IsTimeUp { get; private set; }


        private readonly CancellationTokenSource _countDownCts = new CancellationTokenSource();
        private CancellationTokenSource _timeMeasureCts = new CancellationTokenSource();

        public OnlineTimer(int maxCountDown, int maxTime)
        {
            _maxCountDown = maxCountDown;
            _maxTime = maxTime;
            _isTimeUp = false;
            TotalTime = maxTime;
            CountDown = _maxCountDown;
            IsStart = false;
            IsTimeUp = false;
        }

        public void OnlineInitialize()
        {
            // 全体カウントダウンのスタート時間をセット
            ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();
            properties.Add("StartTime", PhotonNetwork.Time);
            PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
        }

        public async UniTask StartTimer(double startTime)
        {
            _countDownStartTime = (float)startTime;
            await InvokeStartCountDown(_countDownCts.Token);
            
            await InvokeTimeMeasure(_timeMeasureCts.Token);
        }
        

        /// <summary>
        /// 時間計測 制限時間
        /// </summary>
        private async UniTask<bool> InvokeTimeMeasure(CancellationToken cancellationToken)
        {
        
            while (true)
            {
                var isCanceled = await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken).SuppressCancellationThrow();
               
                if (isCanceled)
                {
                    break;
                }
                
                TotalTime -= Time.deltaTime;

                if (TotalTime <= 0)
                {
                    IsTimeUp = true;
                    break;
                }
            }
            
            return true;
        }

        /// <summary>
        /// 時間切れ
        /// </summary>
        void TimeUp()
        {
            
        }

        /// <summary>
        /// カウントダウン開始
        /// </summary>
        /// <returns></returns>
        private async UniTask<bool> InvokeStartCountDown(CancellationToken cancellationToken)
        {
        
            while (true)
            {
                var isCanceled = await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken).SuppressCancellationThrow();
               
                if (isCanceled)
                {
                    break;
                }
                
                var elapsedTime = (float)PhotonNetwork.Time - _countDownStartTime;
                CountDown = _maxCountDown- (int)elapsedTime;
                
                if (CountDown <= 0)
                {
                    IsStart = true;
                    break;
                }
            }
            return true;
        }
    }

}
