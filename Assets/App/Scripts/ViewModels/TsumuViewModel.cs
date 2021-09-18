using System;
using App.MasterData;

namespace App.ViewModels
{
    public readonly struct TsumuViewModel
    {
        public readonly TsumuData TsumuData;
        public readonly Guid Guid;

        public TsumuViewModel(TsumuData tsumuData, Guid guid)
        {
            TsumuData = tsumuData;
            Guid = guid;
        }
    }
}