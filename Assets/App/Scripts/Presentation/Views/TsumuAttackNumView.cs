﻿using System.Threading;
using App.Lib;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace App.Presentation
{
    [PrefabPath("Prefabs/TsumuAttackNumView")]
    public class TsumuAttackNumView : ViewBase
    {
        [SerializeField] private Text _text;
        public void Initialize(int num)
        {
            _text.text = num.ToString();
        }

        public async UniTask MoveToTarget(Vector3 position, CancellationToken cancellationToken)
        {
            await transform.DOLocalMoveY(transform.localPosition.y + 20, 1).ToUniTask(cancellationToken:cancellationToken);
            await transform.DOMove(position, 1).ToUniTask(cancellationToken:cancellationToken);
        }
    }
}