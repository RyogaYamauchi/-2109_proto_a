using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace App.Lib
{
    public abstract class ViewBase : MonoBehaviour, IDisposable
    {
        protected bool IsLoading;
        protected bool IsLoaded;

        public void SetLoading(bool state)
        {
            IsLoading = state;
        }

        public void SetLoaded(bool state)
        {
            IsLoaded = state;
        }

        protected virtual UniTask OnLoadAsync()
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask OnDidLoadAsync()
        {
            return UniTask.CompletedTask;
        }

        public async UniTask LoadAsync()
        {
            await OnLoadAsync();
        }

        public async UniTask DidLoadAsync()
        {
            await OnDidLoadAsync();
        }


        public void Dispose()
        {
            Destroy(gameObject);
        }
    }
}