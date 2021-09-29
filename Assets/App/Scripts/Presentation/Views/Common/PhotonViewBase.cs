using System;
using Cysharp.Threading.Tasks;
using Photon.Pun;

namespace App.Lib
{
    public abstract class PhotonViewBase : MonoBehaviourPunCallbacks, IDisposable, IViewBase
    {
        public void Dispose()
        {
            Destroy(gameObject);
        }
    }
}