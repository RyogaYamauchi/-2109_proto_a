using App.Lib;
using App.Models;
using App.Views;
using UnityEngine;

namespace App.Presenters
{
    public sealed class TsumuRootPresenter : RootPresenterBase
    {
        private TsumuRootView _tsumuRootView;
        public TsumuRootPresenter(TsumuRootView view)
        {
            _tsumuRootView = view;
        }

        public void Initialize()
        {
            Debug.Log($"GameModelInstance is {GameModel.Instance != null}");
        }
    }
}