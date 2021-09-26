using System;
using System.Threading;
using UnityEngine;

namespace App.Lib
{
    public abstract class ViewBase : MonoBehaviour, IDisposable
    {
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        protected CancellationToken _cancellationToken => _cancellationTokenSource.Token;

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            Destroy(gameObject);
        }
    }
}