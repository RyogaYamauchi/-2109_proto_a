using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using App.Lib;
using App.Presenters;
using App.Skills;
using System;

namespace App.Views
{
    [RootSceneName("Title")]
    public class TitleRootView : RootViewBase
    {
        [SerializeField] private Button _OnlineButton;

        [SerializeField] private Button _singlePlayButton;
        public IObservable<Unit> OnClickOnline => _OnlineButton.OnClickAsObservable().TakeUntilDestroy(this);
        public IObservable<Unit> OnClickSingle => _singlePlayButton.OnClickAsObservable().TakeUntilDestroy(this);
        void Start()
        {
            var presenter = new TitleRootPresenter(this);
        }
    }
}
