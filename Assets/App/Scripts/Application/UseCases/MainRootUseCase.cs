using System;
using System.Linq;
using App.Data;
using App.Domain;
using Cysharp.Threading.Tasks;

namespace App.Application
{
    public class MainRootUseCase
    {
        private readonly SkillService _skillService;
        private readonly GameModel _gameModel;
        private readonly TsumuRepository _tsumuRepository;

        public MainRootUseCase(SkillService skillService, GameModel gameModel, TsumuRepository tsumuRepository)
        {
            _skillService = skillService;
            _gameModel = gameModel;
            _tsumuRepository = tsumuRepository;
        }

        public IObservable<int> OnChangedSkillPointAsObservable => _gameModel.SkillModel.SkillPoint;

        public ISkill GetSkill()
        {
            return _skillService.GetAllSkill().OrderBy(x => Guid.NewGuid()).First();
        }

        public TsumuViewModel SpawnTsumu()
        {
            var model = _gameModel.TsumuRootModel.SpawnTsumu();
            var tsumuData = _tsumuRepository.GetAll();
            var data = tsumuData.FirstOrDefault(x => x.Id == model.Id);
            return new TsumuViewModel(model, data);
        }

        public void Initialize()
        {
            _gameModel.TsumuRootModel.Initialize();
            _gameModel.PlayerModel.Clear();
            _gameModel.EnemyModel.Clear();
            _gameModel.SkillModel.Initialize(GetSkill());
        }

        public EnemyViewModel GetEnemyViewModel()
        {
            return new EnemyViewModel(_gameModel.EnemyModel.Health, _gameModel.EnemyModel.MaxHealth);
        }

        public PlayerViewModel GetPlayerViewModel()
        {
            return new PlayerViewModel(_gameModel.PlayerModel.Health, _gameModel.PlayerModel.MaxHealth);
        }

        public void UseSkill()
        {
            var skill = _gameModel.SkillModel.GetInstance();
            skill.ExecuteAsync();
            _gameModel.SkillModel.ClearSkillPoint();
        }

        public void DebugWin(bool state)
        {
            _gameModel.BattleModel.SetWinOrLose(state);
        }

        public SkillPointViewModel GetSkillPointViewModel()
        {
            return new SkillPointViewModel(_gameModel.SkillModel.SkillPoint.Value, _gameModel.SkillModel.MaxSkillPoint);
        }

        public void SelectTsumu(Guid guid)
        {
            _gameModel.TsumuRootModel.SelectTsumu(guid);
        }

        public void UnSelectTsumuAll()
        {
            _gameModel.TsumuRootModel.UnSelectTsumuAll();
        }

        public bool CanUnSelect()
        {
            return _gameModel.TsumuRootModel.CanUnSelect();
        }

        public async UniTask DeleteSelectingTsumu(Func<Guid, int, UniTask> callback)
        {
            await _gameModel.TsumuRootModel.DeleteSelectingTsumu(callback);
        }

        public void AddSkillPoint()
        {
            _gameModel.SkillModel.AddSkillPoint(1);
        }

        public bool CanExecuteSkill()
        {
            return _gameModel.SkillModel.CanExecuteSkill();
        }

        public void ResolveDamage(int sumDamage)
        {
            _gameModel.EnemyModel.RecieveDamage(sumDamage);
            if (_gameModel.EnemyModel.IsDied())
            {
                _gameModel.BattleModel.SetWinOrLose(true);
            }
        }

        public void ResolveRecover(int recover)
        {
            _gameModel.PlayerModel.Recover(recover);
        }

        public int GetSumDamage()
        {
            var selectingTsumuModelList = _gameModel.TsumuRootModel.GetSelectingTsumuList();
            if (selectingTsumuModelList.Any(x => x.MasterTsumu.TsumuType != TsumuType.Heal))
            {
                return _gameModel.TsumuRootModel.GetSelectingDamage();
            }

            return 0;
        }

        public int GetRecoverDamage()
        {
            var selectingTsumuModelList = _gameModel.TsumuRootModel.GetSelectingTsumuList();
            if (selectingTsumuModelList.All(x => x.MasterTsumu.TsumuType == TsumuType.Heal))
            {
                return _gameModel.TsumuRootModel.GetSelectingDamage();
            }

            return 0;
        }

        public int GetSelectingCount()
        {
            return _gameModel.TsumuRootModel.GetSelectingTsumuCount();
        }

        public bool IsEnemyDied()
        {
            return _gameModel.EnemyModel.IsDied();
        }

        public bool CanSelect(Guid guid)
        {
            return _gameModel.TsumuRootModel.CanSelect(guid);
        }

        public Guid? GetLastTsumuGuid()
        {
            return _gameModel.TsumuRootModel.GetLastTsumuGuid();
        }
    }
}