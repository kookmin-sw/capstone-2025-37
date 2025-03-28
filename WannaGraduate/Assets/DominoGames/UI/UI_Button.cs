using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Button))]
public class UI_Button : UI_Base
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public virtual void OnClick()
    {

    }
}
