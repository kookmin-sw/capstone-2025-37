using DominoGames.DominoRx.DataModel;
using DominoGames.DominoRx.RxField;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace DominoGames.DominoRx.UI
{
    [RequireComponent(typeof(RxDataModelBinder))]
    public class RxDynamicScrollView : UI_DynamicScrollView, IRxBindData
    {
        [RxBindField] IRxList targetList;

        public virtual void OnDataChanged()
        {
            Init(targetList.Count);
        }

        public virtual void OnSubscribed()
        {
            Init(targetList.Count);
        }

        public object GetListItem(int index)
        {
            return targetList[index];
        }
    }

}