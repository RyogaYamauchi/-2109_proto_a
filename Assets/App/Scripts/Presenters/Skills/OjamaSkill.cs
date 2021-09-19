using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using App.Presenters;
using System.Linq;
using System;
using App.ViewModels;
using App.MasterData;
using App.Models;
using App.Types;


namespace App.Skills
{
    public class OjamaSkill : ISkill
    {
        private readonly int NeedValue = 30;
        private List<TsumuData> LoadTsumuData()
        {
            return Resources.LoadAll<TsumuData>("MasterData/").ToList();
        }

        public async UniTask ExecuteAsync(TsumuRootPresenter presenter)
        {
            var tsumuList = presenter.GetReadOnLyTsumuList();
            var maxOjamaTsumu = 5;
            var canChangeTsumuCount = tsumuList.Count(x => x.TsumuType != TsumuType.Ojama);
            var changeTsumuCount = maxOjamaTsumu <= canChangeTsumuCount ? maxOjamaTsumu : canChangeTsumuCount;
            var targetTsumuList = tsumuList.OrderBy(x => Guid.NewGuid()).Where(x => x.TsumuType != TsumuType.Ojama).Take(changeTsumuCount).ToList();

            var tsumuDataList = LoadTsumuData();

            var oajamaData = tsumuDataList.FirstOrDefault(x => x.TsumuType == Types.TsumuType.Ojama);
            var pairs = targetTsumuList.Select(view => (view, view.TsumuType)).ToArray();

            foreach (var tsumuView in targetTsumuList)
            {
                GameModel.Instance.TsumuRootModel.ChangeStateTsumuModel(tsumuView.Guid, tsumuView.Guid, oajamaData);
                var viewModel = new TsumuViewModel(oajamaData, tsumuView.Guid);
                tsumuView.Initialize(viewModel, tsumuView.transform.position);
            }

            await UniTask.Delay(5000); // 5秒後に戻す

            foreach (var pair in pairs)
            {
                var tsumuData = tsumuDataList.FirstOrDefault(x => x.TsumuType == pair.TsumuType);
                GameModel.Instance.TsumuRootModel.ChangeStateTsumuModel(pair.view.Guid, pair.view.Guid, tsumuData);
                var viewModel = new TsumuViewModel(tsumuData, pair.view.Guid);
                pair.view.Initialize(viewModel, pair.view.transform.position);
            }
        }

        public int GetNeedValue()
        {
            return NeedValue;
        }
    }
}