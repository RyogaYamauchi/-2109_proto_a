using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using App.Lib;
using App.Skills;
using App.View.Matching;
using App.Views;

namespace App.Presenters
{
    public class TitleRootPresenter : RootPresenterBase
    {
        public TitleRootPresenter(TitleRootView rootView)
        {
            rootView.OnClickOnline.Subscribe(x =>
            {
                ChangeScene<MatchingRootView>().Forget();
            });
        }
    }
}