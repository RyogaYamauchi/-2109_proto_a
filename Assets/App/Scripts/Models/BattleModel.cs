using UniRx;

namespace App.Models
{
    public class BattleModel
    {
        public bool WinOrLose { get; private set; }

        public void SetWinOrLose(bool winOrLose)
        {
            WinOrLose = winOrLose;
        }
    }
}