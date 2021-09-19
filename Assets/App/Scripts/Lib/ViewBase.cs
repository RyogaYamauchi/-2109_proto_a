using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace App.Lib
{
    public abstract class ViewBase : MonoBehaviour, IDisposable, IViewBase
    {
        protected bool IsLoading;
        protected bool IsLoaded;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        protected CancellationToken _cancellationToken => _cancellationTokenSource.Token;

        public void SetLoading(bool state)
        {
            IsLoading = state;
        }

        public void SetLoaded(bool state)
        {
            IsLoaded = state;
        }

        public virtual UniTask OnLoadAsync()
        {
            return UniTask.CompletedTask;
        }

        public virtual UniTask OnDidLoadAsync()
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
            _cancellationTokenSource.Cancel();
            Destroy(gameObject);
        }
    }
}