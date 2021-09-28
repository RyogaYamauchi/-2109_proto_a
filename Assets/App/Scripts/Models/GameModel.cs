namespace App.Models
{
    public class GameModel
    {
        public static GameModel Instance { get; } = new GameModel();
        public TsumuRootModel TsumuRootModel { get; }
        public PlayerModel PlayerModel { get; }
        public EnemyModel EnemyModel { get; }
        public SkillModel SkillModel { get; }
        public BattleModel BattleModel { get; }

        public GameModel()
        {
            TsumuRootModel = new TsumuRootModel();
            PlayerModel = new PlayerModel();
            SkillModel = new SkillModel();
            BattleModel = new BattleModel();
            EnemyModel = new EnemyModel();
        }
    }
}