using App.Domain;

namespace App.Application
{
    public class BattleUseCase
    {
        private readonly GameModel _gameModel;
        public BattleUseCase(GameModel gameModel)
        {
            _gameModel = gameModel;
        }
        public void LeftPlayer()
        {
           _gameModel.BattleModel.SetWinOrLose(true);
        }

        public bool GetPlayerWinOrLose()
        {
            return _gameModel.BattleModel.WinOrLose;
        }

        public void UpdatePlayerHealth(int health)
        {
            _gameModel.PlayerModel.SetHealth(health);
        }

        public void UpdateEnemyHealth(int health)
        {
            _gameModel.EnemyModel.SetHealth(health);

        }

        public int GetPlayerHealth()
        {
            return _gameModel.PlayerModel.Health;
        }

        public int GetEnemyHealth()
        {
            return _gameModel.EnemyModel.Health;
        }
    }
}