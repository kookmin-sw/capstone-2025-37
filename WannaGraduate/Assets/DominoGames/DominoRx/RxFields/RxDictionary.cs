using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace DominoGames.DominoRx.RxField
{
    [Serializable]
    public class RxDictionary<T1, T2> : RxBase, IRxDictionary, IEnumerable<KeyValuePair<T1,T2>>
    {
        [SerializeField] private Dictionary<T1, T2> dictionary = new();

        public RxDictionary(RxBase parent = null)
        {
            SetParent(parent);
        }

        public RxDictionary(Dictionary<T1, T2> dict, RxBase parent = null)
        {
            SetParent(parent);
            this.dictionary = dict;
        }

        public List<T1> Keys => dictionary.Keys.ToList();
        public List<T2> Values => dictionary.Values.ToList();
        public int Count => dictionary.Count;

        ICollection IDictionary.Keys => dictionary.Keys;

        ICollection IDictionary.Values => dictionary.Values;

        public bool IsReadOnly => false;

        public bool IsFixedSize => false;

        public bool IsSynchronized => false;

        public object SyncRoot => false;

        public T2 this[T1 key]
        {
            get
            {
                return dictionary[key];
            }
            set
            {
                dictionary[key] = value;
                TriggerModifiedEvents();
            }
        }

        public object this[object key]
        {
            get
            {
                return dictionary[(T1)key];
            }
            set
            {
                dictionary[(T1)key] = (T2)value;

                if(value is RxBase rxBase)
                {
                    rxBase.SetParent(this);
                }

                TriggerModifiedEvents();
            }
        }


        public void Clear()
        {
            dictionary.Clear();
            TriggerModifiedEvents();
        }

        public void Remove(object key)
        {
            dictionary.Remove((T1)key);
            TriggerModifiedEvents();
        }

        public bool TryGetValue(T1 key, out T2 value)
        {
            return dictionary.TryGetValue(key, out value);
        }

        public void Add(KeyValuePair<T1, T2> item)
        {
            dictionary.Add(item.Key, item.Value);

            if (item.Value is RxBase rxBase)
            {
                rxBase.SetParent(this);
            }

            TriggerModifiedEvents();
        }
        public void Add(object key, object value)
        {
            dictionary.Add((T1)key, (T2)value);

            if (value is RxBase rxBase)
            {
                rxBase.SetParent(this);
            }

            TriggerModifiedEvents();
        }
        public override void SetValue(object val, object key)
        {
            this[(T1)key] = (T2)val;
        }


        public bool Contains(KeyValuePair<T1, T2> item)
        {
            return dictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<T1, T2>[] array, int arrayIndex)
        {
            return;
        }

        public bool Remove(KeyValuePair<T1, T2> item)
        {
            var result = dictionary.Remove(item.Key);
            TriggerModifiedEvents();
            return result;
        }

        public IEnumerator<KeyValuePair<T1, T2>> GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }








        public bool ContainsKey(T1 key)
        {
            return dictionary.ContainsKey(key);
        }

        public override object GetValue(object key)
        {
            return this[(T1)key];
        }

        public override T3 GetValue<T3>(object key = null)
        {
            return (T3)System.Convert.ChangeType(this[(T1)key], typeof(T3));
        }


        public void SetDictionary(Dictionary<T1, T2> targetDictionary)
        {
            this.dictionary = targetDictionary;
            TriggerModifiedEvents();
        }

        public override Type GetDataType()
        {
            return typeof(Dictionary<T1, T2>);
        }

        public bool Contains(object key)
        {
            return ContainsKey((T1)key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }
    }
}