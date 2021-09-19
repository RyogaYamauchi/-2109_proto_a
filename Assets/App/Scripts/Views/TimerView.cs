using UnityEngine;
using App.Presenters;
using Photon.Pun;
using UnityEngine.UI;
using UniRx;
using System;
using App.Lib;

namespace App.Views
{
    public class TimerView : PhotonViewBase
    {
        [SerializeField] private Text countText;
        [SerializeField] private Text timerText;
        
        private Subject<double> _onStartTimeSubject = new Subject<double>();
        public IObservable<double> OnStartTimeAsObservable => _onStartTimeSubject.TakeUntilDestroy(this);
        
        private readonly Subject<Unit> _changeSceneSubject = new Subject<Unit>();
        public IObservable<Unit> ChangeSceneAsObservable => _changeSceneSubject;
        
        private bool _isChange = false;
        // void Start()
        // {
        //     var battlePresenter = new BattlePresenter(this);
        //     battlePresenter.Initialize();
        // }

        public void ChangeCountDown(int countDown)
        {
            countText.text = countDown.ToString();
        }

        public void ChangeTimer(float timer)
        {
            var intTimer = (int) timer;
            timerText.text = intTimer.ToString();
        }
        
        //ルームプロパティが変更されたときに呼ばれる
        public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        {
            //変更のあったプロパティに"Time"が含まれているならtimeを更新
            if (propertiesThatChanged.TryGetValue("StartTime", out var value))
            {
                _onStartTimeSubject.OnNext((double)value);
            }
            
            if (propertiesThatChanged.TryGetValue("scene_state", out var temp))
            {
                if (_isChange) return;
                if ((string)temp == SceneState.SceneStateType.Result.ToString())
                {
                    _isChange = true;
                    _changeSceneSubject.OnNext(Unit.Default);
                }
            }
        }

    }
}

