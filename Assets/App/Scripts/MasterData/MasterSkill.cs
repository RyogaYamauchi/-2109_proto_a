using App.Types;
using UnityEngine;

namespace App.MasterData
{
    [CreateAssetMenu]
    public class MasterSkill : ScriptableObject
    {
        [SerializeField] private int _id;
        public int Id => _id;
    }
}