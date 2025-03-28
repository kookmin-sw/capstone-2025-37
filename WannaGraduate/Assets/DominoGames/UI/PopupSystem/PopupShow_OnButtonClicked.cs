using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(UI_Button))]
public class PopupShow_OnButtonClicked : PopupCaller
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            ShowPopup();
        });
    }
}
