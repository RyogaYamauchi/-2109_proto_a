using UnityEngine;

namespace App.Data
{
    public class TsumuRepository
    {
        public TsumuRepository()
        {
            
        }

        public MasterTsumu[] GetAll()
        {
            return Resources.LoadAll<MasterTsumu>("MasterData/Pazu");
        }
    }
}