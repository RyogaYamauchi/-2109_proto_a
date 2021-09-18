using System;
using App.Lib;
using App.MasterData;
using App.Types;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

namespace App.Views
{
    [PrefabPath("Prefabs/TsumuView")]
    public class TsumuView : ViewBase
    {
        [SerializeField] private Button _button;

        private TsumuData _tsumuData;

        public IObservable<Unit> OnClickAsObservable => _button.OnClickAsObservable().TakeUntilDestroy(this);

        protected override UniTask OnLoadAsync()
        {
            
            return base.OnLoadAsync();
        }

        public void ChangeColor(Color color)
        {
            _button.image.color = color;
        }

        public void Initialize(TsumuData data, Vector3 position)
        {
            _tsumuData = data;
            _button.image.color = TsumuColor.ConvertTsumuColor(data.TsumuType);
            transform.localPosition = position;
        }
    }
}