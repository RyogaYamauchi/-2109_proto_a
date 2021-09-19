using UniRx;

namespace App.Models
{
    public class PlayerParameter
    {
        private readonly float _defaultHealth = 100;
        private readonly float _defaultMaxHealth = 100;

        private ReactiveProperty<float> _health = new ReactiveProperty<float>();
        private ReactiveProperty<int> _combo = new ReactiveProperty<int>();
        public ReadOnlyReactiveProperty<float> Health => _health.ToReadOnlyReactiveProperty();
        public ReadOnlyReactiveProperty<int> Combo => _combo.ToReadOnlyReactiveProperty();
        public float MaxHealth => _defaultMaxHealth;
        
        
        public PlayerParameter(float? health = null, float? maxHealth = null)
        {
            _health.Value = health ?? _defaultHealth;
            _defaultMaxHealth = maxHealth ?? _defaultMaxHealth;
            _combo.Value = 0;
        }

        public void Clear()
        {
            _health.Value = _defaultHealth;
            _combo.Value = 0;
        }

        public void RecieveDamage(float damage)
        {
            var health = _health;
            var newHealth = health.Value - damage;

            if (newHealth <= 0)
            {
                newHealth = 0;
                
                // 体力が０になったときに通知
            }

            _health.Value = newHealth;
        }
        
        public void Recover(float addHealth)
        {
            var nowHealth = _health.Value;

            var newHealth = nowHealth + addHealth;

            if (newHealth > MaxHealth)
            {
                newHealth = MaxHealth;
            }

            _health.Value = newHealth;
        }
    }
}

