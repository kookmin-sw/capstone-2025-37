using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DominoGames.DominoRx.RxField;

namespace DominoGames.DominoRx.RxField
{
    public class RxDispose : IDisposable
    {
        IRxField rxTarget;
        Action<IRxField> targetAction;

        public RxDispose(IRxField rxTarget, Action<IRxField> targetAction)
        {
            this.rxTarget = rxTarget;
            this.targetAction = targetAction;
        }


        public void Dispose()
        {
            this.rxTarget.Dispose(this.targetAction);
        }
    }
}