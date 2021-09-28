using UnityEngine;
using UniRx;
using UnityEngine.UI;
using App.Lib;
using System;
using App.Skills;
using Photon.Pun;

namespace App.Views
{
    public class ResultRootView : RootViewBase
    {
        public class Parameter : IParameter
        {
            public bool IsWinOrLose;

            public Parameter(bool isWinorLose)
            {
                IsWinOrLose = isWinorLose;
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

        public void Initialize(bool winOrLose)
        {
            _resultText.text = winOrLose ? "You win" : "You lose";
            winEnemy.SetActive(winOrLose);
            loseEnemy.SetActive(!winOrLose);
            if (PhotonNetwork.IsMasterClient) 
            {
                var myRoom = PhotonNetwork.CurrentRoom;
                myRoom.IsOpen = false;            // 部屋を閉じる
                myRoom.IsVisible = false;         // ロビーから見えなくする
            }

            SetRate(winOrLose);
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
