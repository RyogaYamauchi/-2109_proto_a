using System;
using System.Collections.Generic;
using System.Linq;
using App.Lib;
using App.MasterData;
using App.Views;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;

namespace App.Presenters
{
    public sealed class TsumuRootPresenter : RootPresenterBase
    {
        private MainRootView _mainRootView;
        private readonly int _maxTsumuCount = 40;
        private readonly int _canSelectDistance = 300;

        private List<TsumuData> _tsumuDataList = new List<TsumuData>();
        private List<TsumuView> _selectingTsumuViewList = new List<TsumuView>();
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
            SetEvents();
            _tsumuDataList = LoadTsumuData();
            for (var i = 0; i < 10; i++)
            {
                await SpawnTsumuAsync();
            }
        }

        private List<TsumuData> LoadTsumuData()
        {
            return Resources.LoadAll<TsumuData>("MasterData/").ToList();
        }

        private async UniTask SpawnTsumuAsync()
        {
            var data = _tsumuDataList.OrderBy(x => Guid.NewGuid()).First();
            Assert.IsNotNull(data);
            var tsumuView = await CreateViewAsync<TsumuView>();
            _mainRootView.SetParentTsumu(tsumuView);
            tsumuView.Initialize(data, new Vector3(0, 0, 0));
            tsumuView.OnPointerEnterAsObservable.Subscribe(OnPointerEntertsumu);
            tsumuView.OnPointerDownAsObservable.Subscribe(OnPointerDownTsumu);
            tsumuView.OnPointerUpAsObservable.Subscribe(x => OnPointerUpTsumu(x).Forget());
            
            await UniTask.Delay(500);
        }

        private void SelectTsumu(TsumuView view)
        {
            _selectingTsumuViewList.Add(view);
            view.ChangeColor(true);
        }


        private void UnSelectTsumuAll()
        {
            _selectingTsumuViewList.ForEach(x => x.ChangeColor(false));
            _selectingTsumuViewList.Clear();
        }

        private void OnPointerDownTsumu(TsumuView view)
        {
            if (_selectingTsumuViewList.Count > 0)
            {
                return;
            }

            SelectTsumu(view);
        }

        private UniTask OnPointerUpTsumu(TsumuView view)
        {
            if (_selectingTsumuViewList.Count < 3)
            {
                UnSelectTsumuAll();
                return UniTask.CompletedTask;
            }

            DespawnSelectingTsumusAsync().Forget();
            UnSelectTsumuAll();
            return UniTask.CompletedTask;
        }

        private async UniTask DespawnSelectingTsumusAsync()
        {
            var chain = _selectingTsumuViewList.Count;
            var animatingTsumuViews = _selectingTsumuViewList.ToArray();
            foreach (var view in animatingTsumuViews)
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

        private bool CanSelect(TsumuView view)
        {
            if (_selectingTsumuViewList.Count == 0)
            {
                return false;
            }

            var lastView = _selectingTsumuViewList.Last();

            if (lastView.TsumuType != view.TsumuType)
            {
                return false;
            }

            var difference = lastView.GetPosition() - view.GetPosition();
            Debug.Log(difference);
            if (Mathf.Abs(difference.x) < _canSelectDistance && Mathf.Abs(difference.y) < _canSelectDistance)
            {
                return true;
            }

            return false;
        }
    }
}