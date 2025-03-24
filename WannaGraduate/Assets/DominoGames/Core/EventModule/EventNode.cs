using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DominoGames.Core.EventSystem
{
    public class EventNode : MonoBehaviour
    {
        [SerializeField] protected EEventTypes eventId = EEventTypes.None;
        [SerializeField] protected bool subscriptionStatus = true;

        public virtual EEventTypes GetEventId()
        {
            return eventId;
        }

        public virtual void SetEventId(EEventTypes eventId)
        {
            this.eventId = eventId;
        }


        public virtual bool GetSubStatus()
        {
            return subscriptionStatus;
        }
        public virtual void SetSubStatus(bool status)
        {
            this.subscriptionStatus = status;
        }







        public virtual void OnEventInvoke(bool args)
        {

        }

        private void Awake()
        {
            DominoEventSystem.Sub<bool>(eventId, OnEventInvoke);
        }
    }
}