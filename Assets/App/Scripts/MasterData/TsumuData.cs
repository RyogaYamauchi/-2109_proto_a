using App.Types;
using UnityEngine;

namespace App.MasterData
{
    [CreateAssetMenu]
    public class TsumuData : ScriptableObject
    {
        [SerializeField] private TsumuType _tsumuType;
        public TsumuType TsumuType => _tsumuType;
    }
}