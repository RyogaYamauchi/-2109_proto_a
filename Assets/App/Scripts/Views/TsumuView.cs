using System;
using App.Lib;
using App.MasterData;
using App.Types;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
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
        private readonly Subject<TsumuView> _onPointerDownSubject = new Subject<TsumuView>();
        private readonly Subject<TsumuView> _onPointerEnterSubject = new Subject<TsumuView>();
        private readonly Subject<TsumuView> _onPointerUpSubject = new Subject<TsumuView>();

        public IObservable<TsumuView> OnPointerUpAsObservable => _onPointerUpSubject.TakeUntilDestroy(this);
        public IObservable<TsumuView> OnPointerEnterAsObservable => _onPointerEnterSubject.TakeUntilDestroy(this);
        public IObservable<TsumuView> OnPointerDownAsObservable => _onPointerDownSubject.TakeUntilDestroy(this);

        public TsumuType TsumuType => _tsumuData.TsumuType;

        
        protected override UniTask OnLoadAsync()
        {
            _button.OnPointerEnterAsObservable().Subscribe(x => _onPointerEnterSubject.OnNext(this)).AddTo(this);
            _button.OnPointerUpAsObservable().Subscribe(x => _onPointerUpSubject.OnNext(this)).AddTo(this);
            _button.OnPointerDownAsObservable().Subscribe(x => _onPointerDownSubject.OnNext(this)).AddTo(this);
            return base.OnLoadAsync();
        }

        public void ChangeColor(bool state)
        {
            _button.image.color = state ? Color.black : TsumuColor.ConvertTsumuColor(TsumuType);
        }

        public void Initialize(TsumuData data, Vector3 position)
        {
            _tsumuData = data;
            _button.image.color = TsumuColor.ConvertTsumuColor(data.TsumuType);
            transform.localPosition = position;
        }

        public UniTask CloseAsync()
        {
            Dispose();
            return UniTask.CompletedTask;
        }

        public Vector3 GetPosition()
        {
            return transform.localPosition;
        }
    }
}