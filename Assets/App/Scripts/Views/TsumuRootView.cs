using App.Lib;
using App.Presenters;
using UnityEngine;
using UnityEngine.UI;

namespace App.Views
{
    [RootSceneName(("MainScene"))]
    public sealed class TsumuRootView : RootViewBase
    {
        [SerializeField] private Image _image;

        // Sceneのエントリーポイント
        private void Start()
        {
            var presenter = new TsumuRootPresenter(this);
            presenter.Initialize();
            _image.color = Color.green;
        }
    }
}