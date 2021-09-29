using App.Domain;

namespace App.Application
{
    public class SkillService
    {
        private readonly ISkill[] _enableSkills = {new DeleteLineSkill()};
        public ISkill[] GetAllSkill()
        {
            return _enableSkills;
        }
    }
}