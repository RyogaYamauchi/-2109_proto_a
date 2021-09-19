using UnityEngine;
using UniRx;
using UnityEngine.UI;
using App.Lib;
using System;
using App.Presenters;
using App.Skills;
using Cysharp.Threading.Tasks;

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
        public IObservable<Unit> OnClickTitle => _titleButton.OnClickAsObservable().TakeUntilDestroy(this);
        public IObservable<Unit> OnClickRetry => _retryButton.OnClickAsObservable().TakeUntilDestroy(this);
        public GameObject loseEnemy;
        public GameObject winEnemy;

        public override UniTask OnLoadAsync()
        {
            var param = ((Parameter) GetParameter());
            var presenter = new ResultRootPresenter(this);
            var winOrLose = param.IsWinOrLose;
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
            return base.OnLoadAsync();
        }
        
    }
}
