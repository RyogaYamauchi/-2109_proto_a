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
        [SerializeField]
        private Button _OnlineButton;
        public IObservable<Unit> OnClickOnline => _OnlineButton.OnClickAsObservable().TakeUntilDestroy(this);
        // Start is called before the first frame update
        void Start()
        {
            var presenter = new TitleRootPresenter(this);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
