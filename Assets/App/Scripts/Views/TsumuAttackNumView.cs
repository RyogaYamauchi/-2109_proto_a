using App.Lib;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace App.Views
{
    [PrefabPath("Prefabs/TsumuAttackNumView")]
    public class TsumuAttackNumView : ViewBase
    {
        [SerializeField] private Text _text;
        public void Initialize(int num)
        {
            _text.text = num.ToString();
        }

        public async UniTask MoveToTarget(Vector3 position)
        {
            await transform.DOLocalMoveY(transform.localPosition.y + 20, 1);
            await transform.DOMove(position, 1);
        }
    }
}