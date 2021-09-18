using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using App.Lib;
using App.Views;
using UniRx;
using Cysharp.Threading.Tasks;

namespace App.Presenters
{
    public class ResultRootPresenter : RootPresenterBase
    {
        public ResultRootPresenter(ResultView view)
        {
            //view.OnClickTitle.Subscribe(x => ChangeScene<Title>);
            view.OnClickRetry.Subscribe(x => ChangeScene<MainRootView>().Forget());
        }

    }
}
