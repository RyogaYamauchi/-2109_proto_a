namespace App.Models
{
    public class GameModel
    {
        public static GameModel Instance { get; } = new GameModel();
        public TsumuRootModel TsumuRootModel { get; }

        public GameModel()
        {
            TsumuRootModel = new TsumuRootModel();
        }
    }
}