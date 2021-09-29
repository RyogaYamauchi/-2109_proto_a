namespace App.Application
{
    public readonly struct EnemyViewModel
    {
        public readonly int Health;
        public readonly int MaxHealth;

        public EnemyViewModel(int health, int maxHealth)
        {
            Health = health;
            MaxHealth = maxHealth;
        }
    }
}