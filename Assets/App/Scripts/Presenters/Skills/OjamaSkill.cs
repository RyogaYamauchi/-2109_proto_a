using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using App.Presenters;
using System.Linq;
using System;
using App.ViewModels;
using App.MasterData;


namespace App.Skills
{
    public class OjamaSkill : ISkill
    {
        private List<TsumuData> LoadTsumuData()
        {
            return Resources.LoadAll<TsumuData>("MasterData/").ToList();
        }
        public UniTask ExecuteAsync(TsumuRootPresenter presenter)
        {
            var tsumuList = presenter.GetReadOnLyTsumuList();
            var targetTsumuList = tsumuList.OrderBy(x => Guid.NewGuid()).Take(5).ToList();

            var OajamaData = LoadTsumuData().FirstOrDefault(x => x.TsumuType == Types.TsumuType
            .Ojama);

            foreach(var tsumuView in targetTsumuList)
            {
                var ViewModel = new TsumuViewModel(OajamaData,tsumuView.Guid);
                tsumuView.Initialize(ViewModel,tsumuView.transform.position);
            }
            return UniTask.CompletedTask;
        }
    }
}
