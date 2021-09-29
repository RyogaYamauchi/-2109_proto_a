using System;
using System.Threading;
using UnityEngine;

namespace App.Lib
{
    public abstract class ViewBase : MonoBehaviour, IDisposable
    {
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        protected CancellationToken _cancellationToken => _cancellationTokenSource.Token;

        public void SetUp()
        {
            OnSetUp();
        }

        protected virtual void OnSetUp()
        {
            
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            Destroy(gameObject);
        }
    }
}