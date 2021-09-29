using System;
using App.Data;
using UniRx;
using UnityEngine;

namespace App.Domain
{
    public sealed class TsumuModel
    {
        private readonly ReactiveProperty<bool> _isSelecting = new ReactiveProperty<bool>();

        public IReadOnlyReactiveProperty<bool> IsSelecting => _isSelecting.SkipLatestValueOnSubscribe().ToReadOnlyReactiveProperty();
        
        public int Id { get; }
        public Guid Guid { get; }
        public MasterTsumu MasterTsumu { get; }

        public TsumuModel( Guid guid, MasterTsumu masterTsumu)
        {
            Id = masterTsumu.Id;
            Guid = guid;
            MasterTsumu = masterTsumu;
        }

        public void UnSelect()
        {
            _isSelecting.Value = false;
        }

        public void Select()
        {
            _isSelecting.Value = true;
        }
    }
}