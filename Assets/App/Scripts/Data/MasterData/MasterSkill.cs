using UnityEngine;

namespace App.Data
{
    [CreateAssetMenu]
    public class MasterSkill : ScriptableObject
    {
        [SerializeField] private int _id;
        public int Id => _id;
    }
}