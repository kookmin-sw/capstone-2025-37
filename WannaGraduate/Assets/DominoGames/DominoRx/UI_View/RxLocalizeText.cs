using I2.Loc;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DominoGames.DominoRx.DataModel;
using DominoGames.DominoRx.RxField;

namespace DominoGames.DominoRx.UI
{
    [RequireComponent(typeof(RxDataModelBinder))]
    [RequireComponent(typeof(UI_LocalizeText))]
    public class RxLocalizeText : MonoBehaviour, IRxBindData
    {
        [SerializeField] private string keyString = "";
        [SerializeField] private bool addBindedValueToKey = true;

        [RxBindField] IRxField targetValue;

        public void OnDataChanged()
        {
            string key = keyString;
            if (addBindedValueToKey)
            {
                key += targetValue.GetValue();
            }

            GetComponent<UI_LocalizeText>().SetLocalizeText(key);
        }

        public void OnSubscribed()
        {

        }
    }
}