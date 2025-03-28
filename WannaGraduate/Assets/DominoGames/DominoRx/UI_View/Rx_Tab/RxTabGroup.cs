using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DominoGames.DominoRx.DataModel;
using DominoGames.DominoRx.RxField;

namespace DominoGames.DominoRx.UI
{
    [RequireComponent(typeof(RxDataModelBinder))]
    public class RxTabGroup : UI_Base, IRxBindData
    {
        [UIAutoAttachField] public GameObject tabParent, buttonParent;

        [RxBindField] RxProperty<int> tabIndex;


        public void SetTabIndex(int tabIndex)
        {
            this.tabIndex.SetValue(tabIndex);
        }


        public void OnSubscribed()
        {
            for (int i = 0; i < buttonParent.transform.childCount; i++)
            {
                buttonParent.transform.GetChild(i).GetComponent<RxTabButton>().InitButton(this, tabParent.transform.GetChild(i).gameObject, i, tabIndex);
            }
        }

        public void OnDataChanged()
        {

        }
    }
}