using System;
using App.Data;
using App.Domain;
using UniRx;
using UnityEngine;

namespace App.Application
{
    public readonly struct TsumuViewModel
    {
        public readonly MasterTsumu MasterTsumu;
        public readonly Guid Guid;
        public readonly GameObject ColiderObject;
        public readonly IReadOnlyReactiveProperty<bool> IsSelecting; 

        public TsumuViewModel(TsumuModel tsumuModel, MasterTsumu masterTsumu)
        {
            MasterTsumu = masterTsumu;
            ColiderObject = masterTsumu.ColliderObject;
            Guid = tsumuModel.Guid;
            IsSelecting = tsumuModel.IsSelecting;
        }
    }
}