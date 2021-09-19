using System;
using Cysharp.Threading.Tasks;
using Photon.Pun;

namespace App.Lib
{
    public abstract class PhotonViewBase : MonoBehaviourPunCallbacks, IDisposable, IViewBase
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
            Destroy(gameObject);
        }
    }
}