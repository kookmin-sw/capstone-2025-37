using DominoGames.DominoRx.RxField;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RxBase : IRxField
{
    private event Action<IRxField> eventTrigger;
    private RxBase parent;

    protected void TriggerModifiedEvents()
    {
        eventTrigger?.Invoke(this);
        parent?.TriggerModifiedEvents();
    }
    public IDisposable Subscribe(Action<IRxField> action)
    {
        eventTrigger += action;
        action(this);
        return new RxDispose(this, action);
    }
    public void Dispose(Action<IRxField> action)
    {
        eventTrigger -= action;
    }
    public void SetParent(RxBase parent)
    {
        this.parent = parent;
    }




    public virtual Type GetDataType()
    {
        return typeof(RxBase);
    }

    public virtual object GetValue(object key = null)
    {
        return this;
    }

    public virtual T GetValue<T>(object key = null)
    {
        return default(T);
    }

    public virtual void SetValue(object val, object key = null)
    {

    }
}
