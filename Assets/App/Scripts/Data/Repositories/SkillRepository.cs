using UnityEngine;

namespace App.Data
{
    public class SkillRepository
    {
        public MasterSkill[] GetAllMasterSkill()
        {
            return Resources.LoadAll<MasterSkill>("MasterData/Skill");
        }
    }
}