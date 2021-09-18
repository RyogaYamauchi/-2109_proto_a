using System;
using App.MasterData;
using UnityEngine;

namespace App.ViewModels
{
    public readonly struct TsumuViewModel
    {
        public readonly TsumuData TsumuData;
        public readonly Guid Guid;
        public readonly GameObject ColiderObject;

        public TsumuViewModel(TsumuData tsumuData, Guid guid)
        {
            TsumuData = tsumuData;
            Guid = guid;
            ColiderObject = tsumuData.ColliderObject;
        }
    }
}