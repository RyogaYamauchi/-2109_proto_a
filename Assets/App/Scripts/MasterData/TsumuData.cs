using App.Types;
using UnityEngine;

namespace App.MasterData
{
    [CreateAssetMenu]
    public class TsumuData : ScriptableObject
    {
        [SerializeField] private TsumuType _tsumuType;
        [SerializeField] private GameObject _colliderGameObject ;
        public TsumuType TsumuType => _tsumuType;
        public GameObject ColliderObject => _colliderGameObject;
    }
}