using System;
using System.Collections.Generic;
using System.Linq;
using App.Lib;
using App.Models;
using App.Views;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace App.Presenters
{
    public sealed class TsumuRootPresenter : RootPresenterBase
    {
        private MainRootView _mainRootView;
        private readonly int _maxTsumuCount = 40;
        private readonly int _canSelectDistance = 300;

        private TsumuRootModel _tsumuRootModel => _gameModel.TsumuRootModel;
        private List<TsumuView> _tsumuViewList = new List<TsumuView>();

        public TsumuRootPresenter(MainRootView view)
        {
            _mainRootView = view;
        }

        private void SetEvents()
        {
        }

        public async void Initialize()
        {
            _gameModel.TsumuRootModel.Initialize();
            SetEvents();
            for (var i = 0; i < 10; i++)
            {
                await SpawnTsumuAsync();
            }
        }


        private async UniTask SpawnTsumuAsync()
        {
            var viewModel = _gameModel.TsumuRootModel.SpawnTsumu();
            var tsumuView = await CreateViewAsync<TsumuView>();
            _mainRootView.SetParentTsumu(tsumuView);
            tsumuView.Initialize(viewModel, Vector3.zero);
            tsumuView.OnPointerEnterAsObservable.Subscribe(OnPointerEntertsumu);
            tsumuView.OnPointerDownAsObservable.Subscribe(OnPointerDownTsumu);
            tsumuView.OnPointerUpAsObservable.Subscribe(x => OnPointerUpTsumu(x).Forget());
            _tsumuViewList.Add(tsumuView);
            await UniTask.Delay(500);
        }

        private void SelectTsumu(TsumuView view)
        {
            _gameModel.TsumuRootModel.SelectTsumu(view.Guid);
            view.ChangeColor(true);
        }

        private void UnSelectTsumuAll()
        {
            var selectingTsumuList = _gameModel.TsumuRootModel.GetSelectingTsumuIdList();
            var views = _tsumuViewList.Where(view => selectingTsumuList.Any(guid => view.Guid == guid));
            foreach (var view in views)
            {
                view.ChangeColor(false);
            }

            _gameModel.TsumuRootModel.UnSelectTsumuAll();
        }

        private async UniTask DespawnSelectingTsumusAsync()
        {
            var ids = _tsumuRootModel.GetSelectingTsumuIdList();
            var views = _tsumuViewList.Where(x => ids.Contains(x.Guid)).ToArray();
            foreach (var view in views)
            {
                await DespawnTsumuAsync(view);
            }
        }

        private async UniTask DespawnTsumuAsync(TsumuView tsumuView)
        {
            _tsumuViewList.Remove(tsumuView);
            await tsumuView.CloseAsync();
        }

        private void OnPointerEntertsumu(TsumuView view)
        {
            if (!CanSelect(view))
            {
                return;
            }

            SelectTsumu(view);
        }

        private UniTask OnPointerUpTsumu(TsumuView view)
        {
            if (_tsumuRootModel.GetSelectingTsumuCount() < 3)
            {
                UnSelectTsumuAll();
                return UniTask.CompletedTask;
            }

            DespawnSelectingTsumusAsync().Forget();
            UnSelectTsumuAll();
            return UniTask.CompletedTask;
        }

        private void OnPointerDownTsumu(TsumuView view)
        {
            if (_tsumuRootModel.GetSelectingTsumuCount() != 0)
            {
                return;
            }

            SelectTsumu(view);
        }


        private bool CanSelect(TsumuView view)
        {
            var lastGuid = _gameModel.TsumuRootModel.GetLastTsumuGuid();
            if (lastGuid == Guid.Empty)
            {
                return false;
            }
            var lastView = _tsumuViewList.FirstOrDefault(x => x.Guid == lastGuid);
            var difference = lastView.GetPosition() - view.GetPosition();
            var modelState = _tsumuRootModel.CanSelect(view.Guid);

            if (!modelState)
            {
                return false;
            }

            if (Mathf.Abs(difference.x) < _canSelectDistance && Mathf.Abs(difference.y) < _canSelectDistance)
            {
                return true;
            }

            return false;
        }
    }
}