﻿using System;
using System.ComponentModel.Design.Serialization;
using App.Lib;
using App.Types;
using App.ViewModels;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

namespace App.Views
{
    [PrefabPath("Prefabs/TsumuView")]
    public class TsumuView : ViewBase
    {
        [SerializeField] private Button _button;

        private readonly Subject<TsumuView> _onPointerDownSubject = new Subject<TsumuView>();
        private readonly Subject<TsumuView> _onPointerEnterSubject = new Subject<TsumuView>();
        private readonly Subject<TsumuView> _onPointerUpSubject = new Subject<TsumuView>();

        public IObservable<TsumuView> OnPointerUpAsObservable => _onPointerUpSubject.TakeUntilDestroy(this);
        public IObservable<TsumuView> OnPointerEnterAsObservable => _onPointerEnterSubject.TakeUntilDestroy(this);
        public IObservable<TsumuView> OnPointerDownAsObservable => _onPointerDownSubject.TakeUntilDestroy(this);

        private TsumuViewModel _tsumuViewModel;
        public TsumuType TsumuType => _tsumuViewModel.TsumuData.TsumuType;
        public Guid Guid => _tsumuViewModel.Guid;
        private GameObject _instance;
        

        
        public override UniTask OnLoadAsync()
        {
            _button.OnPointerEnterAsObservable().Subscribe(x => _onPointerEnterSubject.OnNext(this)).AddTo(this);
            _button.OnPointerUpAsObservable().Subscribe(x => _onPointerUpSubject.OnNext(this)).AddTo(this);
            _button.OnPointerDownAsObservable().Subscribe(x => _onPointerDownSubject.OnNext(this)).AddTo(this);
            return base.OnLoadAsync();
        }

        public void ChangeColor(bool state)
        {
            _button.image.color = state ? Color.black : Color.white;
        }

        public void Initialize(TsumuViewModel viewModel,  Vector2 position)
        {
            transform.position = position;
            _tsumuViewModel = viewModel;
            if(_instance != null)
            {
                Destroy(_instance);
            }
            _instance = Instantiate(viewModel.ColiderObject, transform, false);
            _button.image = _instance.GetComponent<Image>();
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