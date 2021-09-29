using UniRx;
using UnityEngine;

namespace App.Domain
{
    public class EnemyModel
    {
        private readonly int _defaultHealth = 200;
        private readonly int _defaultMaxHealth = 200;

        private ReactiveProperty<int> _combo = new ReactiveProperty<int>();
        public int Health { get;private set; }
        public ReadOnlyReactiveProperty<int> Combo => _combo.SkipLatestValueOnSubscribe().ToReadOnlyReactiveProperty();
        public int MaxHealth => _defaultMaxHealth;
        public int CacheHealth { get; private set; }


        public EnemyModel(int? health = null, int? maxHealth = null)
        {
            Health = health ?? _defaultHealth;
            CacheHealth = Health;
            _defaultMaxHealth = maxHealth ?? _defaultMaxHealth;
            _combo.Value = 0;
        }

        public void Clear()
        {
            Health = _defaultHealth;
            CacheHealth =Health;
            _combo.Value = 0;
        }

        public void RecieveDamage(int damage)
        {
            var health = Health;
            CacheHealth = Health;
            var newHealth = Health - damage;

            if (newHealth <= 0)
            {
                newHealth = 0;
                
                // 体力が０になったときに通知
            }

            Health = newHealth;
        }
        
        public void Recover(int addHealth)
        {
            var nowHealth = Health;
            CacheHealth = Health;

            var newHealth = nowHealth + addHealth;

            if (newHealth > MaxHealth)
            {
                newHealth = MaxHealth;
            }

            Health = newHealth;
        }

        public bool IsDied()
        {
            return Health <= 0;
        }

        public void SetHealth(int health)
        {
            Health = health;
        }
    }
}