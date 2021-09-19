using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace App.Lib
{
    public interface IViewBase
    {
        
        public void SetLoading(bool state);

        public void SetLoaded(bool state);

        public UniTask OnLoadAsync();
        
        public UniTask OnDidLoadAsync();

        public UniTask LoadAsync();

        public UniTask DidLoadAsync();
        
    }
}

