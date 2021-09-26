using System;
using App.MasterData;
using App.Types;

namespace App.Models
{
    public sealed class TsumuModel
    {
        public Guid Guid { get; }
        public MasterTsumu MasterTsumu { get; }

        public TsumuModel(Guid guid, MasterTsumu masterTsumu)
        {
            Guid = guid;
            MasterTsumu = masterTsumu;
        }
    }
}