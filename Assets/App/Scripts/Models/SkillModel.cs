using System;
using System.Linq;
using App.Skills;

namespace App.Models
{
    public class SkillModel
    {
        private ISkill[] _enableSkills = {new OjamaSkill(), new DeleteLineSkill()};
        public ISkill GetRandomSkill()
        {
            return _enableSkills.OrderBy(x => Guid.NewGuid()).First();
        }
    }
}