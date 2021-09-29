namespace App.Application
{
    public readonly struct PlayerViewModel
    {
        public readonly int Health;
        public readonly int MaxHealth;

        public PlayerViewModel(int health, int maxHealth)
        {
            Health = health;
            MaxHealth = maxHealth;
        }
    }
}