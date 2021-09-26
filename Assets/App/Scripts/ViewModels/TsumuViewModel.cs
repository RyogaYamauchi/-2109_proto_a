using System;
using App.MasterData;
using UnityEngine;

namespace App.ViewModels
{
    public readonly struct TsumuViewModel
    {
        public readonly MasterTsumu MasterTsumu;
        public readonly Guid Guid;
        public readonly GameObject ColiderObject;

        public TsumuViewModel(MasterTsumu masterTsumu, Guid guid)
        {
            MasterTsumu = masterTsumu;
            Guid = guid;
            ColiderObject = masterTsumu.ColliderObject;
        }
    }
}