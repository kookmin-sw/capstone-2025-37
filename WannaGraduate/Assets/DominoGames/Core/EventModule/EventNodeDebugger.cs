using DominoGames.Core.DataStructure;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DominoGames.Core.EventSystem.Test
{
    public class EventNodeDebugger : EventNode
    {
        public override void OnEventInvoke(bool args)
        {
            base.OnEventInvoke(args);
        }


        [Button]
        public void Test()
        {
            DominoEventSystem.Pub(EEventTypes.None, true);
        }


    }
}
