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
            int c = 0;
            foreach (var target in targetList)
            {
                target.SetInteractive(false);
                c++;
            }

            var a = targetList.Select((view, cnt) =>
            {
                return (view, GameModel.Instance.TsumuRootModel.CalcDamage(cnt, (GameModel.Instance.TsumuRootModel.GetDamage(view.TsumuType))));
            });

            await tsumuRootPresenter.DespawnTsumuListAsync(a.ToList());
            tsumuRootPresenter.TakeDamage(a.Sum(x => x.Item2));
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