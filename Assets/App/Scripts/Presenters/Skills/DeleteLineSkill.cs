using System.Linq;
using App.Models;
using App.Presenters;
using Cysharp.Threading.Tasks;

namespace App.Skills
{
    public class DeleteLineSkill : ISkill
    {
        //画面の真ん中１列のツムを削除する、
        private readonly int YRange = 100;
        private readonly int NeedValue = 30;

        public async UniTask ExecuteAsync(TsumuRootPresenter tsumuRootPresenter)
        {
            var tsumuList = tsumuRootPresenter.GetReadOnLyTsumuList();
            var animatingList = tsumuRootPresenter.GetClosingTsumuList();
            var selectingList = GameModel.Instance.TsumuRootModel.GetSelectingTsumuIdList();
            var targetList = tsumuList
                .Where(x => !animatingList.Contains(x))
                .Where(x => !selectingList.Contains(x.Guid))
                .Where(tsumuView =>
                {
                    var pos = tsumuView.GetLocalPosition();
                    return pos.y < YRange && pos.y - 50 > -YRange;
                }).ToList();
            foreach (var target in targetList)
            {
                //GameModel.Instance.TsumuRootModel.SelectTsumu(target.Guid);
                target.SetInteractive(false);
            }

            await tsumuRootPresenter.DespawnTsumuListAsync(targetList);
            //GameModel.Instance.TsumuRootModel.UnSelectTsumuAll();
            foreach (var target in targetList)
            {
                target.SetInteractive(true);
            }
        }

        public int GetNeedValue()
        {
            return NeedValue;
        }
    }
}