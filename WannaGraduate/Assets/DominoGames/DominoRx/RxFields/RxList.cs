using QFSW.QC.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace DominoGames.DominoRx.RxField
{
    [Serializable]
    public class RxList<T> : RxBase, IRxList, IEnumerable<T>
    {
        [SerializeField] private List<T> _list;

        public bool IsFixedSize => false;

        public bool IsReadOnly => false;

        public int Count
        {
            get
            {
                if(_list == null)
                {
                    _list = new();
                }
                return _list.Count;
            }
        }

        public bool IsSynchronized => false;

        public object SyncRoot => null;

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                this[index] = (T)value;
                if(value is RxBase rxBase)
                {
                    rxBase.SetParent(this);
                }
                TriggerModifiedEvents();
            }
        }

        public RxList()
        {
            _list = new();
        }

        public RxList(RxBase parent = null)
        {
            SetParent(parent);
            _list = new();
        }

        public RxList(List<T> val, RxBase parent = null)
        {
            SetParent(parent);
            _list = val;
        }

        public T this[int index]
        {
            get
            {
                return this._list[index];
            }
            set
            {
                this._list[index] = value;
                TriggerModifiedEvents();
            }
        }

        public void Sort(Comparison<T> comparison)
        {
            this._list.Sort(comparison);
            TriggerModifiedEvents();
        }


        public void Add(T item)
        {
            _list.Add(item);
            if(item is RxBase rxBase)
            {
                rxBase.SetParent(this);
            }
            TriggerModifiedEvents();
        }

        public void Remove(T item)
        {
            _list.Remove(item);
            TriggerModifiedEvents();
        }

        public void Clear()
        {
            _list.Clear();
            TriggerModifiedEvents();
        }

        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
            TriggerModifiedEvents();
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public IEnumerator GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public int Add(object value)
        {
            Add((T)value);
            return 1;
        }

        public bool Contains(object value)
        {
            return Contains((T)value);
        }

        public int IndexOf(object value)
        {
            return IndexOf((T)value);
        }

        public void Insert(int index, object value)
        {
            _list.Insert(index, (T)value);
            TriggerModifiedEvents();
        }

        public void Remove(object value)
        {
            Remove((T)value);
        }

        public void CopyTo(Array array, int index)
        {
            return;
        }

        public override object GetValue(object key = null)
        {
            if(key == null)
            {
                return _list;
            }
            else
            {
                return this[(int)key];
            }
        }
        public override T2 GetValue<T2>(object key = null)
        {
            return (T2)System.Convert.ChangeType(this[(int)key], typeof(T2));
        }

        public override void SetValue(object val, object key)
        {
            this[(int)key] = (T)val;
        }

        public void SetList(List<T> targetList)
        {
            this._list = targetList;
            TriggerModifiedEvents();
        }

        public override Type GetDataType()
        {
            return typeof(List<T>);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this._list.GetEnumerator();
        }
    }
}