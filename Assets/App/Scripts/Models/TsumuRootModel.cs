using System;
using System.Collections.Generic;
using System.Linq;
using App.MasterData;
using App.Types;
using App.ViewModels;
using UnityEngine;
using UnityEngine.Assertions;

namespace App.Models
{
    public class TsumuRootModel
    {
        private List<TsumuModel> _tsumuList = new List<TsumuModel>();
        private List<Guid> _selectingTsumuIdList = new List<Guid>();
        private List<MasterTsumu> _tsumuDataList = new List<MasterTsumu>();

        private List<MasterTsumu> _cacheTsumuData = new List<MasterTsumu>();

        private List<MasterTsumu> LoadTsumuData()
        {
            if (_cacheTsumuData.Count == 0)
            {
                _cacheTsumuData = Resources.LoadAll<MasterTsumu>("MasterData/")
                    .Where(x => x.TsumuType != TsumuType.Ojama).ToList();
            }

            return _cacheTsumuData;
        }

        private TsumuModel GetModel(Guid guid)
        {
            return _tsumuList.FirstOrDefault(x => x.Guid == guid);
        }

        public void Initialize()
        {
            _tsumuDataList = LoadTsumuData();
        }


        public TsumuViewModel SpawnTsumu()
        {
            var guid = Guid.NewGuid();
            var data = _tsumuDataList.OrderBy(x => Guid.NewGuid()).First();
            Assert.IsNotNull(data);
            var model = new TsumuModel(guid, data);
            _tsumuList.Add(model);
            return new TsumuViewModel(data, guid);
        }

        public void SelectTsumu(Guid guid)
        {
            _selectingTsumuIdList.Add(guid);
        }

        public void UnSelectTsumuAll()
        {
            _selectingTsumuIdList.Clear();
        }

        public List<Guid> GetSelectingTsumuIdList()
        {
            return _selectingTsumuIdList;
        }

        public bool CanSelect(Guid guid)
        {
            var lastGuid = _selectingTsumuIdList.Last();
            var lastModel = GetModel(lastGuid);
            var targetModel = GetModel(guid);

            if (targetModel.MasterTsumu.TsumuType == TsumuType.Ojama)
            {
                return false;
            }

            if (lastModel.MasterTsumu.TsumuType != targetModel.MasterTsumu.TsumuType)
            {
                return false;
            }

            return true;
        }

        public Guid? GetLastTsumuGuid()
        {
            return _selectingTsumuIdList.LastOrDefault();
        }

        public int GetSelectingTsumuCount()
        {
            return _selectingTsumuIdList.Count;
        }

        public int GetSelectingChainCount()
        {
            return GetSelectingTsumuCount();
        }

        public void ChangeStateTsumuModel(Guid currentGuid, Guid changedGuid, MasterTsumu masterTsumu)
        {
            var currentModel = GetModel(currentGuid);
            _tsumuList.Remove(currentModel);
            var changedModel = new TsumuModel(changedGuid, masterTsumu);
            _tsumuList.Add(changedModel);
        }

        public int GetDamage(TsumuType tsumuType)
        {
            return LoadTsumuData().FirstOrDefault(x => x.TsumuType == tsumuType).AttackPoint;
        }

        public int CalcDamage(int chain, int attackPoint)
        {
            return (int) (attackPoint + (chain * 0.5f));
        }
    }
}