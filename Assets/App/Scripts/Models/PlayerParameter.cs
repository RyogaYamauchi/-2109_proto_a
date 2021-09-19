namespace App.Models
{
    public class PlayerParameter
    {
        public float Health { get; set; }
        public int Combo { get; set; }

        public PlayerParameter(
            float health
        )
        {
            Health = health;
            Combo = 0;
        }
    }
}

