using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
using App.Lib;
using System;
using App.Presenters;

namespace App.Views
{
    [RootSceneName("Result")]
    public class ResultView : RootViewBase
    {

        [SerializeField]
        private Button _titleButton;
        [SerializeField]
        private Button _retryButton;
        [SerializeField]
        private Text _resultText;
        [SerializeField]
        private Text _rateltText;
        public IObservable<Unit> OnClickTitle => _titleButton.OnClickAsObservable().TakeUntilDestroy(this);
        public IObservable<Unit> OnClickRetry => _retryButton.OnClickAsObservable().TakeUntilDestroy(this);
        public GameObject loseEnemy;
        public GameObject winEnemy;
        public bool winOrLose = true;


        // Start is called before the first frame update
        void Start()
        {
            var presenter = new ResultRootPresenter(this);
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
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
