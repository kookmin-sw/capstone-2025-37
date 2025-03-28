using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_DynamicScrollViewItem : UI_Base
{
    [HideInInspector] public UI_DynamicScrollView scrollView;
    public int itemId;

    public virtual void InitItem(int itemId)
    {
        this.itemId = itemId;
    }
}
