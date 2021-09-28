using UniRx;
using UnityEngine;

namespace App.Models
{
    public class PlayerModel
    {
        private readonly int _defaultHealth = 200;
        private readonly int _defaultMaxHealth = 200;

        private ReactiveProperty<int> _combo = new ReactiveProperty<int>();
        public int Health { get; private set; }
        public ReadOnlyReactiveProperty<int> Combo => _combo.ToReadOnlyReactiveProperty();
        public int MaxHealth => _defaultMaxHealth;
        
        
        public PlayerModel(int? health = null, int? maxHealth = null)
        {
            Health = health ?? _defaultHealth;
            _defaultMaxHealth = maxHealth ?? _defaultMaxHealth;
            _combo.Value = 0;
        }

        public void Clear()
        {
            Health = _defaultHealth;
            _combo.Value = 0;
        }

        public void RecieveDamage(int damage)
        {
            var health = Health;
            var newHealth = health - damage;

            if (newHealth <= 0)
            {
                newHealth = 0;
            }

            Health = newHealth;
        }
        
        public void Recover(int addHealth)
        {
            var nowHealth = Health;

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

