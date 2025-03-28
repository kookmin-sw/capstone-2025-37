using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BBB.DataStructure{
    public class DoubleDictionary<T1,T2>
    {
    #region Informations
    /*
        Add 및 Remove가 간편한 중첩 Dictionary 입니다.
    */
    #endregion

    //----------------- Public -----------------
    #region Public Methods
        public int AddItem(T1 targetKey, T2 targetValue){
            int _tempDictionaryId = 0;

            if(this._doubleDictionary.ContainsKey(targetKey)){
                _tempDictionaryId = this._doubleDictionaryId[targetKey];
                this._doubleDictionary[targetKey].Add(_tempDictionaryId, targetValue);
            }
            else{
                this._doubleDictionary.Add(targetKey, new Dictionary<int, T2>());
                this._doubleDictionary[targetKey].Add(0, targetValue);
                this._doubleDictionaryId.Add(targetKey, 0);
            }

            this._doubleDictionaryId[targetKey]++;

            return _tempDictionaryId;
        }

        public T2 RemoveItem(T1 targetKey, int targetDictionaryId, bool isLogError = false){
            T2 _tempResult = default(T2);
            if(this._doubleDictionary.ContainsKey(targetKey)){
                if(this._doubleDictionary[targetKey].ContainsKey(targetDictionaryId)){
                    _tempResult = this._doubleDictionary[targetKey][targetDictionaryId];
                    this._doubleDictionary[targetKey].Remove(targetDictionaryId);
                }
                else{
                    if(isLogError){
                        Debug.LogError(targetDictionaryId.ToString() + " 해당 Dictionary ID가 존재하지 않습니다");
                    }
                }
            }
            else{
                if(isLogError){
                    Debug.LogError(targetKey.ToString() + " 해당 Key가 존재하지 않습니다");
                }
            }

            return _tempResult;
        }

        public void RemoveItemsByTargetKey(T1 targetKey){
            if(this._doubleDictionary.ContainsKey(targetKey)){
                this._doubleDictionary.Remove(targetKey);
                this._doubleDictionaryId.Remove(targetKey);
            }
        }

        public void RemoveAllItems(){
            this._doubleDictionary.Clear();
            this._doubleDictionaryId.Clear();
        }

        public bool ContainsKey(T1 targetKey){
            return this._doubleDictionary.ContainsKey(targetKey);
        }

        public List<T2> GetValues(T1 targetKey, bool isLogError = false){
            if(this._doubleDictionary.ContainsKey(targetKey)){
                return new List<T2>(this._doubleDictionary[targetKey].Values);
            }
            else{
                if(isLogError){
                    Debug.LogError(targetKey.ToString() + " 해당 키가 존재하지 않습니다");
                }
                return new();
            }
        }

        public List<T1> GetKeys(){
            return new List<T1>(this._doubleDictionary.Keys);
        }

        public T2 GetItem(T1 targetKey, int targetDictionaryId, bool isLogError = false){
            if(this._doubleDictionary.ContainsKey(targetKey)){
                if(this._doubleDictionary[targetKey].ContainsKey(targetDictionaryId)){
                    return this._doubleDictionary[targetKey][targetDictionaryId];
                }
                else{
                    if(isLogError){
                        Debug.LogError(targetDictionaryId.ToString() + " 해당 Dictionary ID가 존재하지 않습니다");
                    }
                    return default(T2);
                }
            }
            else{
                if(isLogError){
                    Debug.LogError(targetKey.ToString() + " 해당 키가 존재하지 않습니다");
                }
                return default(T2);
            }
        }

        public int GetItemCount(T1 targetKey){
            if(this._doubleDictionary.ContainsKey(targetKey)){
                return this._doubleDictionary[targetKey].Count;
            }
            else{
                return 0;
            }
        }
    #endregion

    #region Public Properties

    #endregion

    //----------------- Unity -----------------
    #region Unity Methods

    #endregion

    //----------------- Private -----------------
    #region Private Methods
    
    #endregion

    #region Private Properties
        private Dictionary<T1, Dictionary<int, T2>> _doubleDictionary = new Dictionary<T1, Dictionary<int, T2>>();
        private Dictionary<T1, int> _doubleDictionaryId = new Dictionary<T1, int>();
    #endregion
    }
}