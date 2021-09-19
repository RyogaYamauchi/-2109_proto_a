using System;
using App.MasterData;
using App.Types;

namespace App.Models
{
    public sealed class TsumuModel
    {
        public Guid Guid { get; }
        public TsumuData TsumuData { get; }

        public TsumuModel(Guid guid, TsumuData tsumuData)
        {
            Guid = guid;
            TsumuData = tsumuData;
        }
    }
}