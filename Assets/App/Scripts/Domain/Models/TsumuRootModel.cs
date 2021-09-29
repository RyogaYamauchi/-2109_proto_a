using System;
using System.Collections.Generic;
using System.Linq;
using App.Data;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace App.Domain
{
    public class TsumuRootModel
    {
        private List<TsumuModel> _tsumuList = new List<TsumuModel>();
        private List<Guid> _selectingTsumuIdList = new List<Guid>();
        private List<MasterTsumu> _tsumuDataList = new List<MasterTsumu>();
        private List<Guid> _closingTsumuIdList = new List<Guid>();

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


        public TsumuModel SpawnTsumu()
        {
            var guid = Guid.NewGuid();
            var data = _tsumuDataList.OrderBy(x => Guid.NewGuid()).First();
            Assert.IsNotNull(data);
            var model = new TsumuModel(guid, data);
            _tsumuList.Add(model);
            return model;
        }

        public void SelectTsumu(Guid guid)
        {
            _selectingTsumuIdList.Add(guid);
            var model = _tsumuList.FirstOrDefault(x => x.Guid == guid);
            model.Select();
        }

        public void UnSelectTsumuAll()
        {
            var selectingTsumuList = GetSelectingTsumuIdList();
            var models = _tsumuList.Where(view => selectingTsumuList.Any(guid => view.Guid == guid));
            foreach (var model in models)
            {
                model.UnSelect();
            }
            _selectingTsumuIdList.Clear();
        }

        private List<Guid> GetSelectingTsumuIdList()
        {
            return _selectingTsumuIdList;
        }

        public bool CanSelect(Guid guid)
        {
            var lastGuid = _selectingTsumuIdList.LastOrDefault();
            if (lastGuid == Guid.Empty)
            {
                return false;
            }

            if (_closingTsumuIdList.Any(x => x == guid))
            {
                return false;
            }
       
            
            var lastModel = GetModel(lastGuid);
            if (lastModel == null)
            {
                return true;
            }
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

        public bool CanUnSelect()
        {
            return GetSelectingTsumuCount() < 3;
        }

        public async UniTask DeleteSelectingTsumu(Func<Guid, int, UniTask> callBack)
        {
            var ids = GetSelectingTsumuIdList();
            var chain = ids.Count;
            var models = _tsumuList.Where(x => ids.Contains(x.Guid)).ToArray();


            var deleteTsumuList = ids.Select(id => models.FirstOrDefault(x => id == x.Guid)).ToArray();
            // UnSelectする
            _selectingTsumuIdList.Clear();

            _closingTsumuIdList.AddRange(deleteTsumuList.Select(x => x.Guid));
            var sumDamage = 0;
            var c = 0;
            foreach (var tsumu in deleteTsumuList)
            {
                var damage = CalcDamage(c, GetDamage(tsumu.MasterTsumu.TsumuType));
                sumDamage += damage;
                await callBack.Invoke(tsumu.Guid, damage);
                _closingTsumuIdList.Remove(tsumu.Guid);
                c++;
            }
        }

        public void ResolveDamage()
        {
           
        }

        public IReadOnlyList<TsumuModel> GetSelectingTsumuList()
        {
            return _selectingTsumuIdList.Select(x => _tsumuList.FirstOrDefault(model => model.Guid == x)).ToList();
        }
    }
}