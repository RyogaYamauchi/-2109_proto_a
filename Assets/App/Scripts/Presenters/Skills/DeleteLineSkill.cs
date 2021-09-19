using System.Linq;
using App.Presenters;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace App.Skills
{
    public class DeleteLineSkill : ISkill
    {
        //画面の真ん中１列のツムを削除する、
        private readonly int YRange = 100;
        public async UniTask ExecuteAsync(TsumuRootPresenter tsumuRootPresenter)
        {
            Debug.Log("Execute ");
            var tsumuList = tsumuRootPresenter.GetReadOnLyTsumuList();
            var targetTsumuList = tsumuList.Where(tsumuView =>
            {
                var pos = tsumuView.GetLocalPosition();
                return pos.y < YRange && pos.y > -YRange;
            }).ToList();
            await tsumuRootPresenter.DespawnTsumuListAsync(targetTsumuList);
        }
    }
}