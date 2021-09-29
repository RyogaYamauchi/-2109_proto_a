using System;
using UniRx;

namespace App.Domain
{
    public class SkillModel
    {
        private int _maxSkillPoint = 0;
        private ReactiveProperty<int> _skillPoint = new ReactiveProperty<int>(0);
        private ISkill _skill;
        
        public int MaxSkillPoint => _maxSkillPoint;
        public ReadOnlyReactiveProperty<int> SkillPoint => _skillPoint.ToReadOnlyReactiveProperty();

        public void Initialize(ISkill skill)
        {
            _skill = skill;
            _maxSkillPoint = skill.GetNeedValue();
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

        public bool CanExecuteSkill()
        {
            return _skillPoint.Value >= _skill.GetNeedValue();
        }

        public ISkill GetInstance()
        {
            return _skill;
        }
    }
}