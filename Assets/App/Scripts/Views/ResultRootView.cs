using UnityEngine;
using UniRx;
using UnityEngine.UI;
using App.Lib;
using System;
using App.Presenters;
using App.Skills;
using Cysharp.Threading.Tasks;
using Photon.Pun;

namespace App.Views
{
    [RootSceneName("Result")]
    public class ResultRootView : RootViewBase
    {
        public class Parameter : IParameter
        {
            public bool IsWinOrLose;
            public ISkill Skill;

            public Parameter(bool isWinorLose, ISkill skill)
            {
                IsWinOrLose = isWinorLose;
                Skill = skill;
            }
        }
        [SerializeField]
        private Button _titleButton;
        [SerializeField]
        private Button _retryButton;
        [SerializeField]
        private Text _resultText;
        [SerializeField]
        private Text _rateltText;
        [SerializeField]
        private AudioClip LoseSound;
        [SerializeField]
        private AudioClip WinSound;
        [SerializeField]
        private AudioSource audioSource;
        public IObservable<Unit> OnClickTitle => _titleButton.OnClickAsObservable().TakeUntilDestroy(this);
        public IObservable<Unit> OnClickRetry => _retryButton.OnClickAsObservable().TakeUntilDestroy(this);
        public GameObject loseEnemy;
        public GameObject winEnemy;

        public override UniTask OnLoadAsync()
        {
            var param = ((Parameter) GetParameter());
            var winOrLose = param.IsWinOrLose;
            var presenter = new ResultRootPresenter(this,winOrLose);
            Debug.Log(winOrLose);
            //_rateltText.text = "1300";
            if (winOrLose == true)
            {
                _resultText.text = "You win";
                winEnemy.SetActive(true);
                loseEnemy.SetActive(false);
            }
            else if (winOrLose == false)
            {
                _resultText.text = "You lose";
                winEnemy.SetActive(false);
                loseEnemy.SetActive(true);
            }
            if (PhotonNetwork.IsMasterClient) 
            {
                var myRoom = PhotonNetwork.CurrentRoom;
                myRoom.IsOpen = false;            // 部屋を閉じる
                myRoom.IsVisible = false;         // ロビーから見えなくする
            }

            SetRate(winOrLose);
            
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
            
            return base.OnLoadAsync();
        }
        public void PlayBGM(bool winOrLose)
        {
            if (winOrLose)
            {
                audioSource.PlayOneShot(WinSound);

            }
            else
            {
                audioSource.PlayOneShot(LoseSound);
            }

        }

        private void SetRate(bool isWin)
        {
            var range = 50;
            var rate = PlayerPrefs.HasKey("rate") ? PlayerPrefs.GetInt("rate") : 1000;
            rate += isWin ? range : -range;
            PlayerPrefs.SetInt("rate", rate);
            var plusOrMinul = isWin ? "+" : "-";
            _rateltText.text = $"rate : {rate}\n{plusOrMinul}{range}...";
        }
        
    }
}
