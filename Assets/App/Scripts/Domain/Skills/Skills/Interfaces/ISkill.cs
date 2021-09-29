using Cysharp.Threading.Tasks;

namespace App.Domain
{
    public interface ISkill
    {
        public void ExecuteAsync();
        int GetNeedValue();
    }
}