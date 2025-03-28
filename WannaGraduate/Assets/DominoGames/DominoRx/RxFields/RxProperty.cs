using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DominoGames.DominoRx.RxField
{
    [Serializable]
    public class RxProperty<T> : RxBase
    {
        [SerializeField] private T _value;

        public T Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                if(_value is RxBase rxBase)
                {
                    rxBase.SetParent(this);
                }
                TriggerModifiedEvents();
            }
        }

        public override Type GetDataType()
        {
            return typeof(T);
        }

        public override object GetValue(object key = null)
        {
            return _value;
        }
        public override T2 GetValue<T2>(object key = null)
        {
            return (T2)System.Convert.ChangeType(_value, typeof(T2));
        }
        public override void SetValue(object val, object key = null)
        {
            this.Value = (T)val;
        }

        public RxProperty(RxBase parent = null)
        {
            this.SetParent(parent);
            this._value = default(T);
        }
        public RxProperty(T val, RxBase parent = null)
        {
            this.SetParent(parent);
            this._value = val;
        }
    }
}