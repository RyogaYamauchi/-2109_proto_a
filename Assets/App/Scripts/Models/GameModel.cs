namespace App.Models
{
    public class GameModel
    {
        public static GameModel Instance { get; } = new GameModel();
        public TsumuRootModel TsumuRootModel { get; }
        public PlayerParameter PlayerParameter { get; }

        public GameModel()
        {
            TsumuRootModel = new TsumuRootModel();
            PlayerParameter = new PlayerParameter();
        }
    }
}