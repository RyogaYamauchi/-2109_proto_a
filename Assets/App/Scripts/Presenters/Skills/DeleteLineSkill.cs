using System.Linq;
using App.Presenters;
using Cysharp.Threading.Tasks;

namespace App.Skills
{
    public class DeleteLineSkill : ISkill
    {
        //画面の真ん中１列のツムを削除する、
        private readonly int YRange = 100;
        public async UniTask ExecuteAsync(TsumuRootPresenter tsumuRootPresenter)
        {
            var tsumuList = tsumuRootPresenter.GetReadOnLyTsumuList();
            var targetTsumuList = tsumuList.Where(tsumuView =>
            {
                var pos = tsumuView.GetPosition();
                return pos.y < YRange && pos.y > -YRange;
            }).ToList();
            await tsumuRootPresenter.DespawnTsumuListAsync(targetTsumuList);
        }
    }
}