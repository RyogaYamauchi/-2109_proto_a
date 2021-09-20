using System;
using System.Linq;
using App.Skills;
using UniRx;

namespace App.Models
{
    public class SkillModel
    {
        private ISkill[] _enableSkills = { new DeleteLineSkill()};

        private int _maxSkillPoint = 0;
        private ReactiveProperty<int> _skillPoint = new ReactiveProperty<int>(0);
        
        public int MaxSkillPoint => _maxSkillPoint;
        public ReadOnlyReactiveProperty<int> SkillPoint => _skillPoint.ToReadOnlyReactiveProperty();

        public ISkill GetRandomSkill()
        {
            return _enableSkills.OrderBy(x => Guid.NewGuid()).First();
        }

        public void Initialize(int maxValue)
        {
            _maxSkillPoint = maxValue;
            _skillPoint.Value = 0;
        }

        public void AddSkillPoint(int value)
        {
            _skillPoint.Value += value;
        }

        public void ClearSkillPoint()
        {
            _skillPoint.Value = 0;
        }

        public bool CanExecuteSkill(ISkill skill)
        {
            return _skillPoint.Value >= skill.GetNeedValue();
        }
    }
}