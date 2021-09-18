using App.Presenters;
using Cysharp.Threading.Tasks;

namespace App.Skills
{
    public interface ISkill
    {
        public UniTask ExecuteAsync(TsumuRootPresenter presenter);
    }
}