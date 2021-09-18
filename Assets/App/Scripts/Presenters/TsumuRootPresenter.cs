using System;
using System.Collections.Generic;
using System.Linq;
using App.Lib;
using App.MasterData;
using App.Types;
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

        private List<TsumuData> _tsumuDatas = new List<TsumuData>();

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
            _tsumuDatas = LoadTsumuData();
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
            var data = _tsumuDatas.OrderBy(x => Guid.NewGuid()).First();
            Assert.IsNotNull(data);
            var tsumuView = await CreateViewAsync<TsumuView>();
            _mainRootView.SetParentTsumu(tsumuView);
            tsumuView.Initialize(data, new Vector3(0, 0, 0));
            tsumuView.OnClickAsObservable.Subscribe(x => tsumuView.ChangeColor(Color.red));
            await UniTask.Delay(500);
        }
    }
}