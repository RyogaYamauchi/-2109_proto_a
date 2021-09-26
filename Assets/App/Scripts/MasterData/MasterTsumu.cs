using App.Types;
using UnityEngine;

namespace App.MasterData
{
    [CreateAssetMenu]
    public class MasterTsumu : ScriptableObject
    {
        [SerializeField] private TsumuType _tsumuType;
        [SerializeField] private GameObject _colliderGameObject ;
        [SerializeField] private int _attackPoint;
        public TsumuType TsumuType => _tsumuType;
        public GameObject ColliderObject => _colliderGameObject;
        public int AttackPoint => _attackPoint;
    }
}