using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DominoGames.DominoRx.RxField
{
    public interface IRxField
    {
        public IDisposable Subscribe(Action<IRxField> action);
        public void Dispose(Action<IRxField> action);

        public object GetValue(object key = null);
        public T GetValue<T>(object key = null);
        public void SetValue(object val, object key = null);
        public Type GetDataType();
    }
}