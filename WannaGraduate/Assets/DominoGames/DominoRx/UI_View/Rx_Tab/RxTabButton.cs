using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DominoGames.DominoRx.RxField;
using System;

namespace DominoGames.DominoRx.UI
{
    public class RxTabButton : UI_Button
    {
        [HideInInspector] public RxTabGroup tabGroup;

        protected int index = 0;
        protected GameObject targetTab;
        IDisposable dispose;

        public void InitButton(RxTabGroup tabGroup, GameObject targetTab, int index, IRxField rxField)
        {
            this.index = index;
            this.targetTab = targetTab;
            this.tabGroup = tabGroup;

            dispose = rxField.Subscribe(x =>
            {
                OnTabActiveChanged((int)x.GetValue() == index);
            });
        }

        public override void OnClick()
        {
            tabGroup.SetTabIndex(index);
        }

        public virtual void OnTabActiveChanged(bool status)
        {
            targetTab.SetActive(status);
        }

        private void OnDestroy()
        {
            dispose.Dispose();
        }
    }
}