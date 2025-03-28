using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DominoGames.DominoRx.DataModel;
using DominoGames.DominoRx.RxField;

namespace DominoGames.DominoRx.UI
{
    [RequireComponent(typeof(RxDataModelBinder))]
    public class RxGraph : MonoBehaviour, IRxBindData
    {
        [RxBindField] RxProperty<float> targetValue; 

        public void OnDataChanged()
        {
            transform.localScale = new Vector3(targetValue.GetValue<float>(), 1f, 1f);
        }

        public void OnSubscribed()
        {

        }
    }
}