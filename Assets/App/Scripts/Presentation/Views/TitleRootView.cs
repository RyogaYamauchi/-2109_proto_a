using UnityEngine;
using UnityEngine.UI;
using UniRx;
using App.Lib;
using System;

namespace App.Presentation
{
    public class TitleRootView : RootViewBase
    {
        [SerializeField] private Button _OnlineButton;

        [SerializeField] private Button _singlePlayButton;
        public IObservable<Unit> OnClickOnline => _OnlineButton.OnClickAsObservable().TakeUntilDestroy(this);
        public IObservable<Unit> OnClickSingle => _singlePlayButton.OnClickAsObservable().TakeUntilDestroy(this);

        private void Awake()
        {
            PlayFromEditor<TitleRootPresenter>();
        }
    }
}
